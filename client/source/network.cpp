#include <nds.h>
#include <dswifi9.h>
#include <netinet/in.h>
#include <stdio.h>
#include <errno.h>
#include <string.h>
#include "network.h"

static char spinner[] = { '/', '-', '\\', '|' };
static const int on  = 1;

static inline void waitForKey(int key) {
  int down = 0;

  while(!(down & key)) {
    swiWaitForVBlank();
    scanKeys();
    down = keysDown();
  }
}

NetManager::NetManager() {
  listener   = -1;
  connection = -1;
}

NetManager::~NetManager() {
  shutdown();
}

bool NetManager::connectWifi() {
  int rc;
  int spin = 0;

  /* Initialize wifi */
  iprintf("Initializing wifi...");
  if(!Wifi_InitDefault(INIT_ONLY)) {
    iprintf("Failed\n");
    iprintf("Press B to exit\n");

    waitForKey(KEY_B);
    return false;
  }
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
      return false;
    }
    spin = (spin+1)%(4*15);
    iprintf("%c", spinner[spin/15]);
  }
  if(rc == ASSOCSTATUS_ASSOCIATED)
    iprintf("OK!\n");
  else if(rc == ASSOCSTATUS_CANNOTCONNECT) {
    iprintf("Failed\n");
    iprintf("Press B to exit\n");

    waitForKey(KEY_B);
    return false;
  }
  iprintf("OK!\n");

  ip = Wifi_GetIPInfo(NULL, NULL, NULL, NULL);

  return true;
}

bool NetManager::initSockets() {
  int rc;

  listener = socket(AF_INET, SOCK_STREAM, 0);
  if(listener < 0)
    return false;

  rc = setsockopt(listener, SOL_SOCKET, SO_REUSEADDR, (char*)&on, sizeof(on));
  if(rc < 0)
    return false;

  rc = ioctl(listener, FIONBIO, (char*)&on);
  if(rc < 0)
    return false;

  addr.sin_family      = AF_INET;
  addr.sin_port        = htons(PORTNUM);
  addr.sin_addr.s_addr = htonl(INADDR_ANY);

  rc = bind(listener, (struct sockaddr *)&addr, sizeof(addr));
  if(rc == -1)
    return false;

  rc = listen(listener, 1);
  if(rc < 0)
    return false;

  return 1;
}

bool NetManager::handshake() {
  connection = accept(listener, (struct sockaddr *)&addr, &addr_len);
  if(connection < 0) {
    if(errno == EWOULDBLOCK)
      return true;
    else
      return false;
  }

  return true;
}
bool NetManager::connect() {
  int rc;

  if(!connectWifi()) {
    shutdown();
    return false;
  }

  if(!initSockets()) {
    shutdown();
    return false;
  }

  while(!handshake() && connection < 0) {
    swiWaitForVBlank();
    scanKeys();
    if(keysDown() & KEY_B) {
      shutdown();
      return false;
    }
  }

  memset(&msg, 0, sizeof(msg));
  msg.type      = Message_Syn;
  msg.syn.magic = 0xDEADBEEF;
  if(send(connection, &msg, sizeof(msg), 0) != sizeof(msg))
    return false;

  memset(&msg, 0, sizeof(msg));
  while((rc = recv(connection, &msg, sizeof(msg), 0)) != sizeof(msg)) {
    if(rc < 0 && errno != EWOULDBLOCK)
      return false;
    else if(rc >= 0)
      return false;
  }

  if(msg.type != Message_Ack && msg.ack.magic != 0xDEADBEEF)
    return false;

  return true;
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

void NetManager::printIP() {
  iprintf("IP: %s\n", inet_ntoa(ip));
}

