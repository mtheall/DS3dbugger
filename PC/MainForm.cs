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
		}
	}

	class ConfigStruct
	{
		public string host;
	}

}
