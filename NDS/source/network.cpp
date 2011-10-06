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
static const int on              = 1;

/* Message handlers */
void handleSyn(SOCKET connection, Message &msg);
void handleAck(SOCKET connection, Message &msg);
void handleTexture(SOCKET connection, Message &msg);
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

  iprintf("\nPress B to exit");
  waitForKey(KEY_B);

  exit(0);
}

static inline int recvall(int s, char *buf, size_t len, int flags) {
  int rc;
  size_t recvd = 0;

  do {
    rc = recv(s, &(buf[recvd]), len-recvd, flags);
    if(rc == -1) {
      if(errno != EWOULDBLOCK)
        quit("recv: %s\n", strerror(errno));
      else if(recvd == 0)
        return -1;
    }
    else
      recvd += rc;
  } while(recvd < len);

  return recvd;
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
    iprintf("OK!\n");
  else if(rc == ASSOCSTATUS_CANNOTCONNECT)
    quit("Failed\n");

  iprintf("OK!\n");

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
      quit("accept: %d", strerror(errno));
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
  iprintf("Sending Syn message\n");
  memset(&msg, 0, sizeof(msg));
  msg.type      = Message_Syn;
  msg.syn.magic = 0xDEADBEEF;

  rc = send(connection, &msg, sizeof(msg), 0);
  if(rc != sizeof(msg)) {
    if(rc == -1)
      quit("send: %s\n", strerror(errno));
    else
      quit("Only sent %d/%d bytes\n", rc, sizeof(msg));
  }
  iprintf("Sent %d bytes\n", sizeof(msg));

  /* receive Acknowledge message */
  iprintf("Listening for Ack message\n");
  memset(&msg, 0, sizeof(msg));
  do {
    rc = recvall(connection, (char*)&msg, sizeof(msg), 0);
  } while(rc == -1);
  printf("Received %d bytes\n", rc);

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
    rc = recvall(connection, (char*)&msg, sizeof(msg), 0);
    if(rc == sizeof(msg))
      handle[msg.type](connection, msg);
  } while(rc == sizeof(msg));
}

void handleSyn(SOCKET connection, Message &msg) {
  int rc;
  if(msg.type != Message_Syn)
    quit("Syn: Message type mismatch");

  if(msg.syn.magic != 0xDEADBEEF)
    quit("Syn: Bad magic\n");

  iprintf("Got Syn message\n");

  msg.type = Message_Ack;
  rc = send(connection, &msg, sizeof(msg), 0);

  if(rc != sizeof(msg)) {
    if(rc == -1)
      quit("send: %s\n", strerror(errno));
    else
      quit("Only sent %d/%d bytes\n", rc, sizeof(msg));
  }
}

void handleAck(SOCKET connection, Message &msg) {
  if(msg.type != Message_Ack)
    quit("Ack: Message type mismatch");
  if(msg.ack.magic != 0xDEADBEEF)
    quit("Ack: Bad magic");

  iprintf("Got Ack message\n");
}

void handleTexture(SOCKET connection, Message &msg) {
  int rc;
  void *buffer;

  if(msg.type != Message_Texture)
    quit("Texture: Message type mismatch");

  iprintf("Got Texture message\n");
  iprintf("  Address: %08X\n", (u32)msg.tex.address);
  iprintf("  Size:    %8d\n", msg.tex.size);

  buffer = malloc(msg.tex.size);
  if(buffer == NULL)
     quit("Failed to malloc buffer");

  do {
    rc = recvall(connection, (char*)buffer, msg.tex.size, 0);
  } while (rc == -1);

  DC_FlushRange(buffer, msg.tex.size);
  dmaCopy(buffer, msg.tex.address, msg.tex.size);
  free(buffer);
}

void handleRegister16(SOCKET connection, Message &msg) {
  if(msg.type != Message_Register16)
    quit("Register16: Message type mismatch");

  iprintf("Got Register16 message\n");
  iprintf("  Address: %08X\n", (u32)msg.register16.address);
  iprintf("  Value:   %8d\n", msg.register16.value);

  *(vu16*)msg.register16.address = msg.register16.value;
}

void handleRegister32(SOCKET connection, Message &msg) {
  if(msg.type != Message_Register32)
    quit("Register32: Message type mismatch");

  iprintf("Got Register32 message\n");
  iprintf("  Address: %08X\n", (u32)msg.register32.address);
  iprintf("  Value:   %8d\n", msg.register32.value);

  *(vu16*)msg.register32.address = msg.register32.value;
}

void handleDisplayList(SOCKET connection, Message &msg) {
  int            rc;
  static void   *dispList        = NULL;
  static size_t  dispListMaxSize = 0;

  if(msg.type != Message_DisplayList)
    quit("DisplayList: Message type mismatch");

  iprintf("Got DisplayList message\n");
  iprintf("  Size:    %8d\n", msg.displist.size);

  if(msg.displist.size > dispListMaxSize) {
    dispList = realloc(dispList, msg.displist.size);
    if(dispList == NULL)
      quit("No memory for dispList");
    dispListMaxSize = msg.displist.size;
  }

  do {
    rc = recvall(connection, (char*)dispList, msg.displist.size, 0);
  } while(rc == -1);
  iprintf("Received %d bytes\n", rc);

  DC_FlushRange(dispList, msg.displist.size);

  glCallList((u32*)dispList);
}

void handleDisplayCapture(SOCKET connection, Message &msg) {
  int       rc;
  size_t    sent = 0;
  z_stream  strm;
  static u8 cap      [256*192*2]; /* 256x192 16bpp buffer =  96KB */
  static u8 zcap     [128*1024];  /* Ample space for zcap = 128KB */
  static u8 vram_temp[128*1024];  /* Copy VRAM D          = 128KB */

  if(msg.type != Message_DisplayCapture)
    quit("DisplayCapture: Message type mismatch");

  iprintf("Got DisplayCapture message\n");

  /* change VRAM D to LCD mode */
  u8 vram_cr_temp = VRAM_D_CR;
  VRAM_D_CR = VRAM_D_LCD;

  /* keep old copy of VRAM D */
  dmaCopy(VRAM_D, vram_temp, 128*1024);
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
  DC_InvalidateAll();

  /* move old copy back into VRAM D */
  dmaCopy(vram_temp, VRAM_D, 128*1024);

  VRAM_D_CR = vram_cr_temp;

  /* initialize compression stream */
  iprintf("Compressing display capture\n");
  strm.zalloc = NULL;
  strm.zfree  = NULL;
  strm.opaque = NULL;
  rc = deflateInit(&strm, 9);
  if(rc != Z_OK)
    quit("Failed to init deflate stream\n");

  /* fill zcap buffer with compressed data */
  strm.avail_in  = sizeof(cap);
  strm.next_in   = cap;
  strm.avail_out = sizeof(zcap);
  strm.next_out  = zcap;
  rc = deflate(&strm, Z_FINISH);
  if(rc == Z_STREAM_ERROR)
    quit("Z_STREAM_ERROR\n");
  if(rc != Z_STREAM_END)
    quit("Failed to complete compression\n"
         "Used %d/%d bytes of zcap\n", strm.avail_out, sizeof(zcap));
  msg.dispcap.size = strm.avail_out;

  /* send compressed data */
  do {
    rc = send(connection, &msg, sizeof(msg), 0);
    if(rc == -1 && errno != EWOULDBLOCK)
      quit("send: %s\n", strerror(errno));
    else if(rc != sizeof(msg))
      quit("Only sent %d/%d bytes\n", rc, sizeof(msg));
  } while(rc == -1);

  iprintf("Sent %d/%d bytes", sent, msg.dispcap.size);
  do {
    /* only send 1KB at a time */
    int toSend = msg.dispcap.size-sent < 1024 ? msg.dispcap.size-sent : 1024;
    rc = send(connection, &zcap[sent], toSend, 0);
    if(rc == -1) {
      if(errno != EWOULDBLOCK)
        quit("\nsend: %s\n", strerror(errno));
    }
    else
      sent += rc;
    swiWaitForVBlank(); /* don't try to send too fast */
    iprintf("\x1b[28DSent %d/%d bytes", sent, msg.dispcap.size);
  } while(sent < msg.dispcap.size);
  iprintf("\n");

  /* clean up compression stream */
  deflateEnd(&strm);
}

