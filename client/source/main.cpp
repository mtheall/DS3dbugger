#include <nds.h>
#include "network.h"

int main(int argc, char *argv[]) {
  NetManager net;

  consoleDemoInit();

  if(net.connect())
    net.printIP();

  do {
    swiWaitForVBlank();
    scanKeys();
  } while(!(keysDown() & KEY_B));

  return 0;
}

