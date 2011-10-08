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
		0x15101210,
                    0x00000002, // MTX_MODE Position & Vector Simultaneous Set mode
                    0x00000000, // MTX_POP 0
                    0x00000002, // MTX_MODE Position & Vector Simultaneous Set mode
                                // MTX_IDENTITY
        0x15101510,
                    0x00000000, // MTX_MODE Projection
                                // MTX_IDENTITY
                    0x00000003, // MTX_MODE Texture
                                // MTX_IDENTITY
        0x10292A50,
                    0x00000000, // SWAP_BUFFERS Auto-sort, Z-value
                    0x00000000, // TEXIMAGE_PARAM
                    0x00000000, // POLYGON_ATTR
                    0x00000000, // MTX_MODE Projection
        0x10151015,
                                // MTX_IDENTITY
                    0x00000002, // MTX_MODE Position & Vector Simultaneous Set mode
                                // MTX_IDENTITY
                    0x00000003, // MTX_MODE Texture
        0x15106015,
                                // MTX_IDENTITY
                    0xBFFF0000, // VIEWPORT (0, 0, 255, 191)
                    0x00000000, // MTX_MODE Projection
                                // MTX_IDENTITY
        0x29201800,
                                // NOP
                    0x00001127, // MTX_MULT_4x4 1.072021484
                    0x00000000, //              0.0
                    0x00000000, //              0.0
                    0x00000000, //              0.0
                    0x00000000, //              0.0
                    0x000016E1, //              1.429931641
                    0x00000000, //              0.0
                    0x00000000, //              0.0
                    0xFFFFFFFB, //             -0.001220703
                    0x00000000, //              0.0
                    0xFFFFEFF8, //             -1.001953125
                    0xFFFFF000, //             -1.0
                    0x00000000, //              0.0
                    0x00000000, //              0.0
                    0xFFFFFCCE, //             -0.199707031
                    0x00000000, //              0.0
                    0x0000001F, // COLOR RGB15(31, 0, 0)
                    0x001F00C0, // POLYGON_ATTR Back Surface Render, Front Surface Render, Alpha 31
        0x1C151110,
                    0x00000002, // MTX_MODE Position & Vector Simultaneous Set mode
                                // MTX_PUSH
                                // MTX_IDENTITY
                    0xFFFFE800, // MTX_TRANS X = -1.5
                    0x00000000, //           Y =  0.0
                    0xFFFFA000, //           Z = -6.0
        0x23232340,
                    0x00000000, // BEGIN_VTXS Triangles
                    0x10000000, // VTX_16 X =  1.0, Y =  0.0
                    0x00000000, //        Z =  0.0
                    0xF000F000, // VTX_16 X = -1.0, Y = -1.0
                    0x00000000, //        Z =  0.0
                    0xF0001000, // VTX_16 X = -1.0, Y =  1.0
                    0x00000000, //        Z =  0.0
        0x40201C41,
                                // END_VTXS
                    0x00003000, // MTX_TRANS X = 3.0
                    0x00000000, //           Y = 0.0
                    0x00000000, //           Z = 0.0
                    0x000003E0, // COLOR RGB15(0, 31, 0)
                    0x00000001, // BEGIN_VTXS Quads
        0x23232323,
                    0x1000F000, // VTX_16 X =  1.0, Y = -1.0
                    0x00000000, //        Z =  0.0
                    0x10001000, // VTX_16 X =  1.0, Y =  1.0
                    0x00000000, //        Z =  0.0
                    0xF0001000, // VTX_16 X = -1.0, Y =  1.0
                    0x00000000, //        Z =  0.0
                    0xF000F000, // VTX_16 X = -1.0, Y = -1.0
                    0x00000000, //        Z =  0.0
        0x00501241,
                                // END_VTXS
                    0x00000001, // MTX_POP 1
                    0x00000000, // SWAP_BUFFERS Auto-sort, Z-value
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
            txtHost.ReadOnly = true;

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

        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "All files (*.*)|*.*";
            dialog.Title = "Select a file";
            if (dialog.ShowDialog() == DialogResult.OK)
                txtFilename.Text = dialog.FileName;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (txtFilename.Text != "")
            {
                int length = (int)(new FileInfo(txtFilename.Text)).Length;
                byte[] buffer = new byte[length];

                BinaryReader fr = new BinaryReader(File.Open(txtFilename.Text, FileMode.Open));
                fr.Read(buffer, 0, length);
                fr.Close();

                BinaryWriter bw = new BinaryWriter(ns);
                Message msg = new Message();
                msg.type = MessageType.Message_DisplayList;
                msg.displist_size = length + 4;
                msg.Send(ns);

                bw.Write(length);
                bw.Write(buffer, 0, length);

                bw.Flush();
                ns.Flush();
            }
        }
	}

	class ConfigStruct
	{
		public string host;
	}

}
