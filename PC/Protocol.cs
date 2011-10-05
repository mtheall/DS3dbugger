using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;

class Protocol
{
	static void Main(string[] args)
	{
		//testing DS connectivity
		var tcpc = new TcpClient("192.168.1.103", 9393);
		tcpc.NoDelay = true;
		var ns = tcpc.GetStream();
		var bw = new BinaryWriter(ns);
		var br = new BinaryReader(ns);

		byte syn = br.ReadByte();
		uint magic = br.ReadUInt32();

		int zzz = 9;

		//sanity check:

		//var tcpc = new TcpClient("www.google.com", 80);
		//tcpc.NoDelay = true;
		//var ns = tcpc.GetStream();
		//var bw = new BinaryWriter(ns);
		//var br = new BinaryReader(ns);

		//var sw = new StreamWriter(ns);
		//sw.WriteLine("GET /\n\n");
		//sw.Flush();

		//for (; ; )
		//{
		//    Console.Write(br.ReadChar());
		//}

		//int zzz = 9;
	}
}