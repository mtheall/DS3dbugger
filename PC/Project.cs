using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace DS3dbugger
{
	public class Project
	{
		public class TexImage : IDisposable
		{
			public TexImage() { }
			public void Load(string path)
			{
				RawImage = Corona.Image.Open(new MemoryStream(File.ReadAllBytes(path)), Corona.PixelFormat.DontCare, Corona.FileFormat.Auto);

				//convert the raw image to one of two formats, so we don't go crazy with lots of options
				if (RawImage.PixelFormat == Corona.PixelFormat.I8) { } //no problem, its fine
				else
				{
					var temp = RawImage.Convert(Corona.PixelFormat.R8G8B8A8);
					RawImage.Dispose();
					RawImage = temp;
				}

				Preview = Util.BitmapFromCorImage(RawImage);
			}

			public int Width { get { return RawImage.Width; } }
			public int Height { get { return RawImage.Height; } }

			Corona.Image RawImage;
			Corona.Image ConvImage;

			public Bitmap Preview;

			public byte[] DSTexData = new byte[0];
			public short[] DSPalData = new short[0];

			public void ConvertFormat(TextureFormat format)
			{
				if (ConvImage != null) ConvImage.Dispose();
				ConvImage = null;

				switch (format)
				{
					case TextureFormat.Format2_I2:
					case TextureFormat.Format3_I4:
					case TextureFormat.Format4_I8:
						Convert_Paletted(format); 
						break;
					case TextureFormat.Format7_16bpp:
						Convert_16bpp();
						break;
				}

				Preview = Util.BitmapFromCorImage(ConvImage);

				//grab the DS bytes
				DSPalData = new short[0];

				switch (format)
				{
					case TextureFormat.Format2_I2: 
						Grab_I2();
						Grab_Palette(4);
						break;
					case TextureFormat.Format3_I4:
						Grab_I4();
						Grab_Palette(16);
						break;
					case TextureFormat.Format4_I8:
						Grab_I8();
						Grab_Palette(256);
						break;
					case TextureFormat.Format7_16bpp:
						Grab_16bpp();
						break;
				}
			}

			void Grab_16bpp()
			{
				int todo = Width * Height;
				DSTexData = new byte[todo * 2];

				byte[] data = ConvImage.GrabPixels();
				for (int i = 0; i < todo; i++)
				{
					int r = data[i * 4 + 0];
					int g = data[i * 4 + 1];
					int b = data[i * 4 + 2];
					r >>= 3;
					g >>= 3;
					b >>= 3;
					int c = r | (g << 5) | (b << 10) | 0x8000;
					DSTexData[i * 2 + 0] = (byte)(c & 0xFF);
					DSTexData[i * 2 + 0] = (byte)((c >> 8) & 0xFF);
				}
			}

			void Grab_I2()
			{
				int todo = Width * Height;
				int bytes = todo / 4;
				DSTexData = new byte[bytes];
				byte[] pixels = ConvImage.GrabPixels();
				int dptr=0;
				for (int i = 0; i < todo;)
				{
					int b = 0;
					for (int j = 0; j < 4; j++,i++)
					{
						b <<= 2;
						b |= pixels[i];
					}
					DSTexData[dptr++] = (byte)b;
					b = 0;
				}
			}

			void Grab_I4()
			{
				int todo = Width * Height;
				int bytes = todo / 2;
				DSTexData = new byte[bytes];
				byte[] pixels = ConvImage.GrabPixels();
				int dptr = 0;
				for (int i = 0; i < todo; )
				{
					int b = 0;
					for (int j = 0; j < 2; j++, i++)
					{
						b <<= 4;
						b |= pixels[i];
					}
					DSTexData[dptr++] = (byte)b;
					b = 0;
				}
			}

			void Grab_I8()
			{
				int todo = Width * Height;
				DSTexData = ConvImage.GrabPixels();
			}

			void Grab_Palette(int colors)
			{
				DSPalData = new short[colors * 2];
				byte[] palette = ConvImage.GrabPalette();
				for (int i = 0; i < colors; i++)
				{
					int r = palette[i * 4 + 0];
					int g = palette[i * 4 + 1];
					int b = palette[i * 4 + 2];
					r >>= 3;
					g >>= 3;
					b >>= 3;
					int c = r | (g << 5) | (b << 10) | 0x8000;
					DSPalData[i] = (short)c;
				}
			}

			void Convert_16bpp()
			{
				ConvImage = new ImageMagick().ConvertDown(RawImage, TextureFormat.Format7_16bpp);
			}

			void Convert_Paletted(TextureFormat format)
			{
				ConvImage = new ImageMagick().ConvertDown(RawImage, format);
			}

			public void Dispose()
			{
				if (Preview != null) Preview.Dispose();
				Preview = null;

				if (RawImage != null) RawImage.Dispose();
				RawImage = null;
			}
		}

		public class TextureReference : IDisposable
		{
			public string Path;
			public string Name;
			public TextureFormat Format = TextureFormat.Format0_None;

			public int Width { get { return TexImage.Width; } }
			public int Height { get { return TexImage.Height; } }

			public TexImage TexImage;

			public void Dispose()
			{
				if (TexImage != null) TexImage.Dispose();
				TexImage = null;
			}

			void PurgeBMP()
			{
				if (TexImage != null) TexImage.Dispose();
				TexImage = null;
			}

			public void SetFormat(TextureFormat newFormat)
			{
				if (newFormat == Format) return;
				Format = newFormat;
				TexImage.ConvertFormat(newFormat);
			}

			public Bitmap Bmp { get { return TexImage.Preview; } }

			public void PopulateBMP()
			{
				PurgeBMP();
				TexImage = new TexImage();
				TexImage.Load(PathManager.FullyQualify(Path));
				
				//TODO - try to autodetect a format (corona may need to manage palette size a little better)
				SetFormat(TextureFormat.Format4_I8);
			}
		}

		public List<TextureReference> TextureReferences = new List<TextureReference>();

		public void LoadFrom(string path)
		{
			//todo
		}

		public void DefineTexture(string path)
		{
			//make sure its not already in there
			int index = TextureReferences.FindIndex((x) => x.Path.ToUpper() == x.Path);
			if (index != -1) return;

			var pathFull = PathManager.FullyQualify(path);

			TextureReference tr = new TextureReference();

			//try loading it as tga
			tr.Name = Path.GetFileNameWithoutExtension(path);
			tr.Path = path;

			tr.PopulateBMP();
			tr.SetFormat(TextureFormat.Format4_I8);

			TextureReferences.Add(tr);
		}

	}
}