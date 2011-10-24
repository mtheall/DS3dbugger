using System;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace DS3dbugger
{
	/// <summary>
	/// the idea behind this was to make friendly nds register setters but thats sort of too much work right now
	/// we'll keep some hacky register stuff in here for now though
	/// </summary>
	public class NDS
	{
		NetworkStream ns;
		public NDS(NetworkStream ns)
		{
			this.ns = ns;
		}

		void WriteReg8(int addr, int val)
		{
			Message msg = new Message();
			msg.type = MessageType.Message_Register8;
			msg.register8_address = (uint)addr;
			msg.register8_value = (byte)val;
			msg.Send(ns);
		}

		void WriteReg32(int addr, uint val)
		{
			Message msg = new Message();
			msg.type = MessageType.Message_Register32;
			msg.register32_address = (uint)addr;
			msg.register32_value = val;
			msg.Send(ns);
		}

		void Set_VRAM_ABCD(uint val) { WriteReg32(0x04000240, val); }
		void Set_VRAM_A(int val) { WriteReg8(0x04000240, val); }
		void Set_VRAM_B(int val) { WriteReg8(0x04000241, val); }
		void Set_VRAM_C(int val) { WriteReg8(0x04000242, val); }
		void Set_VRAM_D(int val) { WriteReg8(0x04000243, val); }

		public void MapLCDC()
		{
			Set_VRAM_ABCD(0x80808080);
		}

		public void MapNormal()
		{
			//A: 1 xx 00 x11 = 83
			//B: 1 xx 01 x11 = 8B
			//C: 1 xx 10 011 = 93
			//D: 1 xx 11 011 = 9B
			Set_VRAM_ABCD(0x9B938B83);
		}

		public void BasicTextured3DCnt()
		{
			WriteReg32(0x04000060, 1); //enable textures
		}
	}
}