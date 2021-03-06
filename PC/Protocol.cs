﻿using System;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace DS3dbugger
{
	enum MessageType : uint
	{
		Message_Syn,            //ds <-> pc
		Message_Ack,            //ds <-> pc
		Message_Texture,        //pc --> ds
		Message_Register8,     //pc --> ds
		Message_Register16,     //pc --> ds
		Message_Register32,     //pc --> ds
		Message_DisplayList,    //pc --> ds
		Message_DisplayCapture, //ds --> pc
	} ;

	[StructLayout(LayoutKind.Explicit)]
	unsafe struct Message
	{
		[FieldOffset(0)]
		public MessageType type;

		[FieldOffset(4)]
		public uint syn_magic;

		[FieldOffset(4)]
		public uint ack_magic;

		[FieldOffset(4)]
		public uint tex_addr;
		[FieldOffset(8)]
		public uint tex_size;


		[FieldOffset(4)]
		public uint register8_address;
		[FieldOffset(8)]
		public byte register8_value;

		[FieldOffset(4)]
		public uint register16_address;
		[FieldOffset(8)]
		public ushort register16_value;

		[FieldOffset(4)]
		public uint register32_address;
		[FieldOffset(8)]
		public uint register32_value;

		[FieldOffset(4)]
		public int displist_size;

		[FieldOffset(4)]
		public int dispcap_size;

		public void Send(Stream s)
		{
			int len = sizeof(Message);
			IntPtr ptr = Marshal.AllocHGlobal(len);
			Marshal.StructureToPtr(this, ptr, false);
			byte[] buf = new byte[len];
			Marshal.Copy(ptr, buf, 0, len);
			s.Write(buf, 0, len);
			Marshal.FreeHGlobal(ptr);
		}

		public void Recv(Stream s)
		{
			int len = sizeof(Message);
			byte[] buf = new byte[len];
			int n = 0;
			while (n != len)
			{
				n += s.Read(buf, n, len - n);
			}

			IntPtr ptr = Marshal.AllocHGlobal(len);
			Marshal.Copy(buf, 0, ptr, len);
			this = (Message)Marshal.PtrToStructure(ptr, GetType());
			Marshal.FreeHGlobal(ptr);
		}
		}

}