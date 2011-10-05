using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;

namespace DS3dbugger
{
	public partial class MainForm : Form
	{
		static void Main(string[] args)
		{
			new DS3dbugger.MainForm().ShowDialog();
		}

		ConfigStruct Config = new ConfigStruct();

		string fnPrefs;
		public MainForm()
		{
			InitializeComponent();
			fnPrefs = Path.Combine(Path.GetTempPath(), "DS3dbugger.txt");
			Config = ConfigService.Load<ConfigStruct>(fnPrefs);
			txtHost.Text = Config.host;
		}

		protected override void OnClosed(EventArgs e)
		{
			Config.host = txtHost.Text;
			ConfigService.Save(fnPrefs, Config);
		}

		uint[] testData = new uint[] {
		0x00000010, 0x00000002,
0x00000012, 0x00000000,
0x00000010, 0x00000002,
0x00000015, 0x00000000,
0x00000010, 0x00000000,
0x00000015, 0x00000000,
0x00000010, 0x00000003,
0x00000015, 0x00000000,
0x00000050, 0x00000000,
0x0000002A, 0x00000000,
0x00000029, 0x00000000,
0x00000010, 0x00000000,
0x00000015, 0x00000000,
0x00000010, 0x00000002,
0x00000015, 0x00000000,
0x00000010, 0x00000003,
0x00000015, 0x00000000,
0x00000060, 0xBFFF0000,
0x00000010, 0x00000000,
0x00000015, 0x00000000,
0x00000018, 0x00001127,
0x00000018, 0x00000000,
0x00000018, 0x00000000,
0x00000018, 0x00000000,
0x00000018, 0x00000000,
0x00000018, 0x000016E1,
0x00000018, 0x00000000,
0x00000018, 0x00000000,
0x00000018, 0xFFFFFFFB,
0x00000018, 0x00000000,
0x00000018, 0xFFFFEFF8,
0x00000018, 0xFFFFF000,
0x00000018, 0x00000000,
0x00000018, 0x00000000,
0x00000018, 0xFFFFFCCE,
0x00000018, 0x00000000,
0x00000020, 0x00007FFF,
0x00000029, 0x001F00C0,
0x00000010, 0x00000002,
0x00000011, 0x00000000,
0x00000015, 0x00000000,
0x0000001C, 0xFFFFE800,
0x0000001C, 0x00000000,
0x0000001C, 0xFFFFA000,
0x00000040, 0x00000000,
0x00000023, 0x10000000,
0x00000023, 0x00000000,
0x00000023, 0xF000F000,
0x00000023, 0x00000000,
0x00000023, 0xF0001000,
0x00000023, 0x00000000,
0x00000041, 0x00000000,
0x0000001C, 0x00003000,
0x0000001C, 0x00000000,
0x0000001C, 0x00000000,
0x00000040, 0x00000001,
0x00000023, 0x1000F000,
0x00000023, 0x00000000,
0x00000023, 0x10001000,
0x00000023, 0x00000000,
0x00000023, 0xF0001000,
0x00000023, 0x00000000,
0x00000023, 0xF000F000,
0x00000023, 0x00000000,
0x00000041, 0x00000000,
0x00000012, 0x00000001,
0x00000050, 0x00000000,
		};

		TcpClient tcpc;
		NetworkStream ns;
		private void btnConnect_Click(object sender, EventArgs e)
		{
			tcpc = new TcpClient(txtHost.Text, 9393);
			tcpc.NoDelay = true;
			ns = tcpc.GetStream();

			Message msg = new Message();
			msg.Recv(ns);

			msg.type = MessageType.Message_Ack;
			msg.Send(ns);

			btnConnect.Enabled = false;
			btnConnect.Text = "connected";

			msg.type = MessageType.Message_DisplayList;
			msg.displist_size = testData.Length * 4 + 4;
			msg.Send(ns);

			BinaryWriter bw = new BinaryWriter(ns);
			bw.Write(testData.Length);
			for (int i = 0; i < testData.Length;i++ )
			{
				bw.Write(testData[i]);
			}
			bw.Flush();
			ns.Flush();
		}
	}

	class ConfigStruct
	{
		public string host;
	}

}
