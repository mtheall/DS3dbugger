using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
		[STAThread]
		static void Main(string[] args)
		{
			new DS3dbugger.MainForm().ShowDialog();
		}

		ConfigStruct Config = new ConfigStruct();

		string fnPrefs;
		unsafe public MainForm()
		{
			InitializeComponent();
			fnPrefs = Path.Combine(Path.GetTempPath(), "DS3dbugger.txt");
			Config = ConfigService.Load<ConfigStruct>(fnPrefs);
			txtHost.Text = Config.host;

			lastScreen = new System.Drawing.Bitmap(256, 192, PixelFormat.Format32bppArgb);
			using (var g = Graphics.FromImage(lastScreen))
				g.Clear(Color.Black);
			SetViewport(lastScreen);
		}

		void SetViewport(Bitmap bmp)
		{
			var bmpLarge = new Bitmap(512, 384, PixelFormat.Format32bppArgb);
			using (var g = Graphics.FromImage(bmpLarge))
			{
				g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
				g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
				g.DrawImage(bmp, 0, 0, 512, 384);
			}

			vpDSScreen.SetBitmap(bmpLarge);
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
0x00000015,
0x00000010, 0x00000000,
0x00000015,
0x00000010, 0x00000003,
0x00000015,
0x00000050, 0x00000000,
0x0000002A, 0x00000000,
0x00000029, 0x00000000,
0x00000010, 0x00000000,
0x00000015,
0x00000010, 0x00000002,
0x00000015,
0x00000010, 0x00000003,
0x00000015,
0x00000060, 0xBFFF0000,
0x00000010, 0x00000000,
0x00000015, 0x00000000,
0x00000018, 0x00001127,
0x00000000,
0x00000000,
0x00000000,
0x00000000,
0x000016E1,
0x00000000,
0x00000000,
0xFFFFFFFB,
0x00000000,
0xFFFFEFF8,
0xFFFFF000,
0x00000000,
0x00000000,
0xFFFFFCCE,
0x00000000,
0x00000020, 0x0000001F,
0x00000029, 0x001F00C0,
0x00000010, 0x00000002,
0x00000011,
0x00000015,
0x0000001C, 0xFFFFE800,
0x00000000,
0xFFFFA000,
0x00000040, 0x00000000,
0x00000023, 0x10000000,
0x00000000,
0x00000023, 0xF000F000,
0x00000000,
0x00000023, 0xF0001000,
0x00000000,
0x00000041,
0x0000001C, 0x00003000,
0x00000000,
0x00000000,
0x00000020, 0x000003E0,
0x00000040, 0x00000001,
0x00000023, 0x1000F000,
0x00000000,
0x00000023, 0x10001000,
0x00000000,
0x00000023, 0xF0001000,
0x00000000,
0x00000023, 0xF000F000,
0x00000000,
0x00000041,
0x00000012, 0x00000001,
0x00000050, 0x00000000,
		};

		Bitmap lastScreen;
		unsafe void AcquireScreen()
		{
			Message msg = new Message();
			msg.type = MessageType.Message_DisplayCapture;
			msg.Send(ns);

			msg.Recv(ns);
			int zipped_size = msg.dispcap_size;
			BinaryReader br = new BinaryReader(ns);
			byte[] zipbuf = new byte[zipped_size];
			for (int i = 0; i < zipped_size; i++)
				zipbuf[i] = br.ReadByte();

			var inf = new ICSharpCode.SharpZipLib.Zip.Compression.Inflater();
			inf.SetInput(zipbuf);
			byte[] bscreen = new byte[256 * 192 * 2];
			inf.Inflate(bscreen);

			int zzz = 9;

			var bmp = new Bitmap(256, 192, PixelFormat.Format32bppArgb);
			BitmapData bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, 256, 192), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			byte* bp = (byte*)bmpdata.Scan0.ToPointer();
			for (int i = 0; i < 256 * 192; i++)
			{
			    int r = bscreen[i * 2] & 0x1F;
			    int g = ((bscreen[i * 2] & 0xE0)>>5) | ((bscreen[i*2+1] & 3)<<3);
				int b = (bscreen[i * 2 + 1] >> 2) & 0x1F;
				int a = bscreen[i * 2 + 1] >> 7;

				//todo - use same color conversion as desmume (whatever that is)
				r <<= 3;
				g <<= 3;
				b <<= 3;

				bp[i * 4 + 0] = (byte)b;
				bp[i * 4 + 1] = (byte)g;
				bp[i * 4 + 2] = (byte)r;
				bp[i * 4 + 3] = 255;
			}
			bmp.UnlockBits(bmpdata);

			if (lastScreen != null) lastScreen.Dispose();
			lastScreen = bmp;
			SetViewport(lastScreen);
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

			AcquireScreen();
		}

		private void btnAcquireScreen_Click(object sender, EventArgs e)
		{
			AcquireScreen();
		}

		private void btnCopyScreen_Click(object sender, EventArgs e)
		{
			System.Windows.Forms.Clipboard.SetImage(lastScreen);
		}
	}

	class ConfigStruct
	{
		public string host;
	}

}
