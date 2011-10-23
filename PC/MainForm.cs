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

		Project CurrProject = new Project();

		ConfigStruct Config = new ConfigStruct();

		RadioButton[] rbTexFormats;
		string fnPrefs;
		unsafe public MainForm()
		{
			InitializeComponent();

			rbTexFmt1_A3I5.Tag = TextureFormat.Format1_A3I5;
			rbTexFmt2_I2.Tag = TextureFormat.Format2_I2;
			rbTexFmt3_I4.Tag = TextureFormat.Format3_I4;
			rbTexFmt4_I8.Tag = TextureFormat.Format4_I8;
			rbTexFmt5_4x4.Tag = TextureFormat.Format5_4x4;
			rbTexFmt6_A5I3.Tag = TextureFormat.Format6_A5I3;
			rbTexFmt7_16bpp.Tag = TextureFormat.Format7_16bpp;
			rbTexFormats = new[] { rbTexFmt1_A3I5, rbTexFmt2_I2, rbTexFmt3_I4, rbTexFmt4_I8, rbTexFmt5_4x4, rbTexFmt6_A5I3, rbTexFmt7_16bpp };

			fnPrefs = Path.Combine(Path.GetTempPath(), "DS3dbugger.txt");
			Config = ConfigService.Load<ConfigStruct>(fnPrefs);
			txtHost.Text = Config.host;

			lastScreen = new System.Drawing.Bitmap(256, 192, PixelFormat.Format32bppArgb);
			using (var g = Graphics.FromImage(lastScreen))
				g.Clear(Color.Black);
			SetViewport(lastScreen);

			PathManager.CurrProjectPath = Path.Combine(PathManager.CurrDirectory, "untitled.3dproj");
			SyncTitle();
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
			68,         // Length
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

			byte[] toSend = Util.Compress(testData.Select(x => BitConverter.GetBytes(x)).SelectMany(x => x).ToArray());

			msg.type = MessageType.Message_DisplayList;
			msg.displist_size = (int)toSend.Length;
			msg.Send(ns);

			ns.Write(toSend, 0, toSend.Length);
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

				byte[] toSend = Util.Compress(buffer);

				Message msg = new Message();
				msg.type = MessageType.Message_DisplayList;
				msg.displist_size = buffer.Length;
				msg.Send(ns);

				ns.Write(buffer, 0, buffer.Length);
				ns.Flush();
			}
		}

		private void lvTextures_DragDrop(object sender, DragEventArgs e)
		{
			foreach (var fn in e.Data.GetData("FileDrop") as string[])
			{
				string relative = PathManager.Relativeize(fn);
				if (relative == null)
				{
					MessageBox.Show("Texture wasn't in a subdirectory relative to the project file");
					return;
				}
				CurrProject.DefineTexture(relative);
			}
			SyncTextures();
		}

		void SyncTextures()
		{
			lvTextures.Clear();
			lvTextures.LargeImageList = new ImageList();
			lvTextures.LargeImageList.ImageSize = new Size(32, 32);
			foreach (var tex in CurrProject.TextureReferences)
			{
				var lvi = new ListViewItem();
				lvi.Text = tex.Name;
				lvi.ImageIndex = lvTextures.LargeImageList.Images.Count;
				lvi.Tag = tex;
				lvTextures.LargeImageList.Images.Add(tex.Bmp);
				lvTextures.Items.Add(lvi);
			}
		}

		private void lvTextures_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Link;
		}

		private void lvTextures_DragOver(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Link;
		}

		private void btnNewProject_Click(object sender, EventArgs e)
		{
			var sfd = new System.Windows.Forms.SaveFileDialog();
			sfd.DefaultExt = "3dproj";
			sfd.Filter = "DS3dbugger project files (*.3dproj)|*.3dproj";
			sfd.InitialDirectory = PathManager.CurrDirectory;
			sfd.RestoreDirectory = true;
			//sfd.FileOk += new CancelEventHandler(ParticleFileValidate);
			sfd.OverwritePrompt = true;
			if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				PathManager.CurrProjectPath = sfd.FileName;
				PathManager.CurrDirectory = Path.GetDirectoryName(sfd.FileName);
				SaveProject();
				SyncTitle();
			}
		}

		void SyncTitle()
		{
			Text = string.Format("DS3dbugger - {0}", Path.GetFileName(PathManager.CurrProjectPath));
		}

		void SaveProject()
		{
			File.WriteAllText(PathManager.CurrProjectPath, "");
		}

		private void btnOpenProject_Click(object sender, EventArgs e)
		{
			var ofd = new System.Windows.Forms.OpenFileDialog();
			ofd.DefaultExt = "3dproj";
			ofd.Filter = "DS3dbugger project files (*.3dproj)|*.3dproj";
			ofd.InitialDirectory = PathManager.CurrDirectory;
			ofd.RestoreDirectory = true;
			//sfd.FileOk += new CancelEventHandler(ParticleFileValidate);
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				PathManager.CurrProjectPath = ofd.FileName;
				PathManager.CurrDirectory = Path.GetDirectoryName(ofd.FileName);
				LoadProject(PathManager.CurrProjectPath);
				SyncTitle();
			}
		}

		void LoadProject(string path)
		{
			//tbd
		}

		private void btnSaveProject_Click(object sender, EventArgs e)
		{
			SaveProject();
		}

		int GetSelectedTexture()
		{
			int newIndex = -1;
			if (lvTextures.SelectedIndices.Count == 0) newIndex = -1;
			else newIndex = lvTextures.SelectedIndices[0];
			return newIndex;
		}

		private void lvTextures_SelectedIndexChanged(object sender, EventArgs e)
		{
			SyncTexInfo();
		}

		void SyncTexInfo()
		{
			int index = GetSelectedTexture();
			if (index == -1)
			{
				vpTexLarge.SetBitmap(null);
				grpTexInfo.Enabled = false;
				return;
			}

			grpTexInfo.Enabled = true;
			var tex = lvTextures.Items[index].Tag as Project.TextureReference;
			SetSelectedTextureFormat(tex.Format);
			txtTexInfoName.Text = tex.Name;
			txtTexInfoDims.Text = string.Format("{0}, {1}", tex.Width, tex.Height);
			txtTexInfoSize.Text = (tex.TexImage.DSTexData.Length).ToFileSize(2);
			if (tex.TexImage.DSPalData.Length != 0)
				txtTexInfoSize.Text += " + " + (tex.TexImage.DSPalData.Length * 2) + "B";
			vpTexLarge.SetBitmap(new Bitmap(tex.TexImage.Preview));
		}

		void SetSelectedTextureFormat(TextureFormat format)
		{
			foreach (var rb in rbTexFormats)
				if ((TextureFormat)rb.Tag == format)
				{
					rb.Checked = true;
					break;
				}
		}

		TextureFormat GetSelectedTextureFormat()
		{
			foreach (var rb in rbTexFormats)
				if (rb.Checked) return (TextureFormat)rb.Tag;
			return TextureFormat.Format0_None;
		}

		private void rbTexFmt_CheckedChanged(object sender, EventArgs e)
		{
			var tex = lvTextures.Items[GetSelectedTexture()].Tag as Project.TextureReference;
			tex.SetFormat(GetSelectedTextureFormat());
			SyncTexInfo();
		}
	}

	class ConfigStruct
	{
		public string host;
	}

}
