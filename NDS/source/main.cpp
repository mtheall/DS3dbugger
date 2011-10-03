#include <nds.h>
#include "network.h"

int main(int argc, char *argv[]) {
  NetManager net;

  videoSetMode(MODE_0_3D);
  videoSetModeSub(MODE_0_2D);
  vramSetPrimaryBanks(VRAM_A_LCD, VRAM_B_LCD, VRAM_C_LCD, VRAM_D_LCD);
  vramSetBankH(VRAM_H_SUB_BG);
  consoleInit(0, 0, BgType_Text4bpp, BgSize_T_256x256, 2, 0, false, true);

  net.connect();

  do {
    net.update();
    swiWaitForVBlank();
    scanKeys();
  } while(!(keysDown() & KEY_B));

  return 0;
}

