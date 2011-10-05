#include <nds.h>
#include <dswifi9.h>
#include <netinet/in.h>
#include <stdio.h>
#include <errno.h>
#include <string.h>
#include <stdarg.h>
#include "network.h"

static char      spinner[] = { '/', '-', '\\', '|' };
static const int on              = 1;
static void     *dispList        = NULL;
static int       dispListSize    = 0;
static int       dispListMaxSize = 0;

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
bool NetManager::handshake() {
  connection = accept(listener, (struct sockaddr *)&addr, &addr_len);
  if(connection < 0) {
    if(errno == EWOULDBLOCK)
      return true;
    else
      return false;
  }

  /* close the listening socket (we don't want more connections) */
  closesocket(listener);
  listener = -1;

  iprintf("Accepted connection from %s\n", inet_ntoa(addr.sin_addr));

  return true;
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
  while(!handshake() && connection < 0) {
    swiWaitForVBlank();
    scanKeys();

    /* give up */
    if(keysDown() & KEY_B) {
      quit("Quit\n");
    }
  }

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
  rc = recv(connection, &msg, sizeof(msg), MSG_WAITALL);
  if(rc != sizeof(msg)) {
    if(rc == -1)
      quit("recv: %s\n", strerror(errno));
    else
      quit("Only recv %d/%d bytes\n", rc, sizeof(msg));
  }
  printf("Received %d bytes\n", sizeof(msg));

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
    rc = recv(connection, &msg, sizeof(msg), 0);
    if(rc == sizeof(msg))
      handle[msg.type](connection, msg);
  } while(rc == sizeof(msg));

  if(rc == -1) {
    if(errno == EWOULDBLOCK)
      return;
    else
      quit("recv: %s", strerror(errno));
  }
  else
    quit("Only recv %d/%d bytes\n", rc, sizeof(msg));
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

  rc = recv(connection, buffer, msg.tex.size, MSG_WAITALL);

  if(rc != msg.tex.size) {
    if(rc == -1)
      quit("recv: %s\n", strerror(errno));
    else
      quit("Only recv %d/%d bytes\n", rc, msg.tex.size);
  }

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
  int rc;

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
  dispListSize = msg.displist.size;

  rc = recv(connection, dispList, msg.displist.size, MSG_WAITALL);

  if(rc != msg.displist.size) {
    if(rc == -1)
      quit("recv: %s\n", strerror(errno));
    else
      quit("Only recv %d/%d bytes\n", rc, msg.displist.size);
  }

  DC_FlushRange(dispList, msg.displist.size);

  glCallList((u32*)dispList);
}

void handleDisplayCapture(SOCKET connection, Message &msg) {
  if(msg.type != Message_DisplayCapture)
    quit("DisplayCapture: Message type mismatch");

  iprintf("Got DisplayCapture message\n");
}

