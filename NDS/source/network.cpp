#include <nds.h>
#include <dswifi9.h>
#include <netinet/in.h>
#include <stdio.h>
#include <errno.h>
#include <string.h>
#include <stdarg.h>
#include "network.h"
#include "zlib.h"

static char      spinner[] = { '/', '-', '\\', '|' };
static const int on        = 1;
static u8        zscratch[1048576];

/* Message handlers */
void handleSyn(SOCKET connection, Message &msg);
void handleAck(SOCKET connection, Message &msg);
void handleTexture(SOCKET connection, Message &msg);
void handleRegister8(SOCKET connection, Message &msg);
void handleRegister16(SOCKET connection, Message &msg);
void handleRegister32(SOCKET connection, Message &msg);
void handleDisplayList(SOCKET connection, Message &msg);
void handleDisplayCapture(SOCKET connection, Message &msg);

/* Message handlers lookup table */
void (*handle[])(SOCKET connection, Message &msg) =
{
  handleSyn,
  handleAck,
  handleTexture,
  handleRegister8,
  handleRegister16,
  handleRegister32,
  handleDisplayList,
  handleDisplayCapture,
};

/* Wait for a key to be pressed */
static inline void waitForKey(int key) {
  int down = 0;

  while(!(down & key)) {
    swiWaitForVBlank();
    scanKeys();
    down = keysDown();
  }
}

static inline void quit(const char *format, ...) {
  va_list args;

  va_start(args, format);
  viprintf(format, args);
  va_end(args);

  Wifi_DisconnectAP();
  Wifi_DisableWifi();

  iprintf("Press B to exit\n");
  waitForKey(KEY_B);

  exit(0);
}

static inline int RECV(int s, char *buf, size_t len, int flags) {
  int    rc;
  size_t recvd = 0;

  do {
    int toRecv = len-recvd < 1024 ? len-recvd : 1024;
    rc = recv(s, &(buf[recvd]), toRecv, flags);
    if(rc == -1) {
      if(errno != EWOULDBLOCK)
        quit("recv: %s\n", strerror(errno));
      else if(recvd == 0)
        return -1;
    }
    else
      recvd += rc;
    if(toRecv == 1024)
      swiWaitForVBlank();
  } while(recvd < len);

  return recvd;
}

static inline int SEND(int s, char *buf, size_t len, int flags) {
  int    rc;
  size_t sent = 0;

  do {
    int toSend = len-sent < 1024 ? len-sent : 1024;
    rc = send(s, &(buf[sent]), toSend, flags);
    if(rc == -1) {
      if(errno != EWOULDBLOCK)
        quit("send: %s\n", strerror(errno));
      else if(sent == 0)
        return -1;
    }
    else
      sent += rc;
    if(toSend == 1024)
      swiWaitForVBlank();
  } while(sent < len);

  return sent;
}

/* Constructor */
NetManager::NetManager() {
  listener   = -1;
  connection = -1;
}

/* Destructor */
NetManager::~NetManager() {
  shutdown();
}

/* Connect to Wifi */
void NetManager::connectWifi() {
  int rc;
  int spin = 0;

  /* Initialize wifi */
  iprintf("Initializing wifi...");
  if(!Wifi_InitDefault(INIT_ONLY))
    quit("Failed\n");
  iprintf("OK!\n");

  /* Connect to AP */
  iprintf("Connecting to AP.../");
  Wifi_AutoConnect();

  while((rc = Wifi_AssocStatus()) != ASSOCSTATUS_ASSOCIATED
     && rc != ASSOCSTATUS_CANNOTCONNECT) {
    swiWaitForVBlank();
    iprintf("\x08");
    scanKeys();

    if(keysDown() & KEY_B) {
      quit("Quit\n");
    }
    spin = (spin+1)%(4*5);
    iprintf("%c", spinner[spin/5]);
  }
  if(rc == ASSOCSTATUS_ASSOCIATED)
    iprintf("\x08OK!\n");
  else if(rc == ASSOCSTATUS_CANNOTCONNECT)
    quit("\x08""Failed\n");

  /* Get IP */
  ip = Wifi_GetIPInfo(NULL, NULL, NULL, NULL);
}

/* Initialize sockets */
void NetManager::initSockets() {
  int rc;

  /* listens for incoming connections */
  listener = socket(AF_INET, SOCK_STREAM, 0);
  if(listener < 0)
    quit("socket: %s\n", strerror(errno));

  /* let address be reusable */
  rc = setsockopt(listener, SOL_SOCKET, SO_REUSEADDR, (char*)&on, sizeof(on));
  if(rc < 0)
    quit("setsockopt: %s\n", strerror(errno));

  /* set non-blocking (connections will inherit this) */
  rc = ioctl(listener, FIONBIO, (char*)&on);
  if(rc < 0)
    quit("ioctl: %s\n", strerror(errno));

  /* set up bind address */
  addr.sin_family      = AF_INET;
  addr.sin_port        = htons(PORTNUM);
  addr.sin_addr.s_addr = htonl(INADDR_ANY);

  /* bind to a port */
  rc = bind(listener, (struct sockaddr *)&addr, sizeof(addr));
  if(rc == -1)
    quit("bind: %s\n", strerror(errno));

  /* listen for connections */
  rc = listen(listener, 1);
  if(rc < 0)
    quit("listen: %s\n", strerror(errno));
}

/* Get connection */
void NetManager::handshake() {
  connection = accept(listener, (struct sockaddr *)&addr, &addr_len);
  if(connection == -1) {
    if(errno != EWOULDBLOCK)
      quit("accept: %d\n", strerror(errno));
  }
  else {
    /* close the listening socket (we don't want more connections) */
    closesocket(listener);
    listener = -1;

    iprintf("Accepted connection from %s\n", inet_ntoa(addr.sin_addr));
  }
}

/* Do everything necessary to talk to PC */
void NetManager::connect() {
  int rc;

  /* connect to Wifi */
  connectWifi();

  /* initialize the sockets */
  initSockets();

  iprintf("IP: %s\n", inet_ntoa(ip));

  /* wait for PC to connect to us */
  iprintf("Waiting for connection\n");
  do {
    handshake();
    swiWaitForVBlank();
    scanKeys();

    /* give up */
    if(keysDown() & KEY_B) {
      quit("Quit\n");
    }
  } while(connection == -1);

  /* send Sync message */
  memset(&msg, 0, sizeof(msg));
  msg.type      = Message_Syn;
  msg.syn.magic = 0xDEADBEEF;

  do {
    rc = SEND(connection, (char*)&msg, sizeof(msg), 0);
  } while(rc == -1);

  /* receive Acknowledge message */
  memset(&msg, 0, sizeof(msg));
  do {
    rc = RECV(connection, (char*)&msg, sizeof(msg), 0);
  } while(rc == -1);

  handleAck(connection, msg);
}

void NetManager::shutdown() {
  if(listener >= 0) {
    closesocket(listener);
    listener = -1;
  }
  if(connection >= 0) {
    closesocket(connection);
    listener = -1;
  }

  Wifi_DisconnectAP();
  Wifi_DisableWifi();
}

void NetManager::update() {
  int rc;
  Message msg;

  do {
    rc = RECV(connection, (char*)&msg, sizeof(msg), 0);
    if(rc == sizeof(msg) && msg.type >= Message_Syn && msg.type <= Message_DisplayCapture)
      handle[msg.type](connection, msg);
    else if(rc == sizeof(msg))
      quit("Invalid message type: %d\n", msg.type);
  } while(rc == sizeof(msg));
}

void handleSyn(SOCKET connection, Message &msg) {
  int rc;
  if(msg.type != Message_Syn)
    quit("Syn: Message type mismatch\n");

  if(msg.syn.magic != 0xDEADBEEF)
    quit("Syn: Bad magic\n");

  iprintf("Got Syn message\n");

  msg.type = Message_Ack;

  do {
    rc = SEND(connection, (char*)&msg, sizeof(msg), 0);
  } while(rc == -1);
}

void handleAck(SOCKET connection, Message &msg) {
  if(msg.type != Message_Ack)
    quit("Ack: Message type mismatch\n");
  if(msg.ack.magic != 0xDEADBEEF)
    quit("Ack: Bad magic\n");

  iprintf("Got Ack message\n");
}

void handleTexture(SOCKET connection, Message &msg) {
  int            rc;
  size_t         size;
  static u8     *buffer        = NULL;
  static size_t  bufferMaxSize = 0;

  if(msg.type != Message_Texture)
    quit("Texture: Message type mismatch\n");

  iprintf("Got Texture message\n");
  iprintf("  Address: %08X\n", (u32)msg.tex.address);
  iprintf("  Size:    %8d\n", msg.tex.size);

  if(msg.tex.size > bufferMaxSize) {
    buffer = (u8*)realloc(buffer, msg.tex.size);
    if(buffer == NULL)
       quit("Failed to realloc buffer\n");
    bufferMaxSize = msg.tex.size;
  }

  do {
    rc = RECV(connection, (char*)buffer, msg.tex.size, 0);
  } while (rc == -1);

  size = sizeof(zscratch);
  if(uncompress(zscratch, (uLongf*)&size, buffer, msg.tex.size) != Z_OK)
    quit("uncompress: failure\n");

  DC_FlushRange(zscratch, size);

  swiWaitForVBlank();
  dmaCopy(zscratch, msg.tex.address, size);

  iprintf("Done receiving texture\n");
}

void handleRegister8(SOCKET connection, Message &msg) {
  if(msg.type != Message_Register8)
    quit("Register8: Message type mismatch\n");

  iprintf("Got Register8 message\n");
  iprintf("  Address: %08X\n", (u32)msg.register8.address);
  iprintf("  Value:   %8d\n", msg.register8.value);

  *(vu8*)msg.register8.address = msg.register8.value;
}

void handleRegister16(SOCKET connection, Message &msg) {
  if(msg.type != Message_Register16)
    quit("Register16: Message type mismatch\n");

  iprintf("Got Register16 message\n");
  iprintf("  Address: %08X\n", (u32)msg.register16.address);
  iprintf("  Value:   %8d\n", msg.register16.value);

  *(vu16*)msg.register16.address = msg.register16.value;
}

void handleRegister32(SOCKET connection, Message &msg) {
  if(msg.type != Message_Register32)
    quit("Register32: Message type mismatch\n");

  iprintf("Got Register32 message\n");
  iprintf("  Address: %08X\n", (u32)msg.register32.address);
  iprintf("  Value:   %8d\n", msg.register32.value);

  *(vu16*)msg.register32.address = msg.register32.value;
}

void handleDisplayList(SOCKET connection, Message &msg) {
  int            rc;
  size_t         size;
  static u8     *dispList        = NULL;
  static size_t  dispListMaxSize = 0;

  if(msg.type != Message_DisplayList)
    quit("DisplayList: Message type mismatch\n");

  iprintf("Got DisplayList message\n");
  iprintf("  Size:    %8d\n", msg.displist.size);

  if(msg.displist.size > dispListMaxSize) {
    dispList = (u8*)realloc(dispList, msg.displist.size);
    if(dispList == NULL)
      quit("Failed to realloc dispList\n");
    dispListMaxSize = msg.displist.size;
  }

  do {
    rc = RECV(connection, (char*)dispList, msg.displist.size, 0);
  } while(rc == -1);

  size = sizeof(zscratch);
  if(uncompress(zscratch, (uLongf*)&size, dispList, msg.displist.size) != Z_OK)
    quit("uncompress: failure\n");

  DC_FlushRange(zscratch, size);

  swiWaitForVBlank();
  glCallList((u32*)zscratch);
}

void handleDisplayCapture(SOCKET connection, Message &msg) {
  int       rc;
  static u8 cap      [256*192*2]; /* 256x192 16bpp buffer =  96KB */
  static u8 zcap     [128*1024];  /* Ample space for zcap = 128KB */
  static u8 vram_temp[128*1024];  /* Copy VRAM D          = 128KB */

  if(msg.type != Message_DisplayCapture)
    quit("DisplayCapture: Message type mismatch\n");

  iprintf("Got DisplayCapture message\n");

  /* change VRAM D to LCD mode */
  u8 vram_cr_temp = VRAM_D_CR;
  VRAM_D_CR = VRAM_D_LCD | VRAM_ENABLE;

  /* keep old copy of VRAM D */
  dmaCopy(VRAM_D, vram_temp, 128*1024);

  /* wait two frames to make sure the scene actually rendered */
  swiWaitForVBlank();
  swiWaitForVBlank();

  /* start capture */
  REG_DISPCAPCNT = DCAP_ENABLE
                 | DCAP_MODE(DCAP_MODE_A)
                 | DCAP_SRC_A(DCAP_SRC_A_3DONLY)
                 | DCAP_SIZE(DCAP_SIZE_256x192)
  /*             | DCAP_BANK(DCAP_BANK_VRAM_D) wrong value? */
                 | DCAP_BANK(3);
  while(REG_DISPCAPCNT & DCAP_ENABLE);

  /* copy capture into buffer */
  dmaCopy(VRAM_D, cap, 256*192*2);
  DC_InvalidateRange(cap, sizeof(cap));

  /* move old copy back into VRAM D */
  dmaCopy(vram_temp, VRAM_D, 128*1024);

  VRAM_D_CR = vram_cr_temp;

  /* fill zcap buffer with compressed data */
  msg.dispcap.size = sizeof(zcap);
  rc = compress2(zcap, (uLongf*)&msg.dispcap.size, cap, sizeof(cap), Z_BEST_COMPRESSION);
  if(rc != Z_OK)
    quit("compress2: failure\n");
  msg.type = Message_DisplayCapture;

  /* send compressed data */
  do {
    rc = SEND(connection, (char*)&msg, sizeof(msg), 0);
  } while(rc == -1);

  do {
    rc = SEND(connection, (char*)&zcap, msg.dispcap.size, 0);
  } while(rc == -1);
  iprintf("Sent %d bytes\n", rc);
}

