using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;


namespace DS3dbugger
{
	public static class ExtensionMethods
	{

	}

	public static class Util
	{
		//extensions
		public static string ToFileSize(this int n, int decimals)
		{
			return String.Format(new FileSizeFormatProvider(), "{0:fs" + decimals + "}", n);
		}

		//---------

		public static byte[] Compress(byte[] input)
		{
			System.IO.MemoryStream ms = new MemoryStream();
			var def = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(ms, new ICSharpCode.SharpZipLib.Zip.Compression.Deflater(9));
			def.Write(input, 0, input.Length);
			def.Flush();
			def.Finish();
			return ms.ToArray();
		}

		private class FileSizeFormatProvider : IFormatProvider, ICustomFormatter
		{
			public object GetFormat(Type formatType)
			{
				if (formatType == typeof(ICustomFormatter)) return this;
				return null;
			}

			private const string fileSizeFormat = "fs";
			private const Decimal OneKiloByte = 1024M;
			private const Decimal OneMegaByte = OneKiloByte * 1024M;
			private const Decimal OneGigaByte = OneMegaByte * 1024M;

			public string Format(string format, object arg, IFormatProvider formatProvider)
			{
				if (format == null || !format.StartsWith(fileSizeFormat))
				{
					return defaultFormat(format, arg, formatProvider);
				}

				if (arg is string)
				{
					return defaultFormat(format, arg, formatProvider);
				}

				Decimal size;

				try
				{
					size = Convert.ToDecimal(arg);
				}
				catch (InvalidCastException)
				{
					return defaultFormat(format, arg, formatProvider);
				}

				string suffix;
				if (size > OneGigaByte)
				{
					size /= OneGigaByte;
					suffix = "GB";
				}
				else if (size > OneMegaByte)
				{
					size /= OneMegaByte;
					suffix = "MB";
				}
				else if (size > OneKiloByte)
				{
					size /= OneKiloByte;
					suffix = "KB";
				}
				else
				{
					suffix = "B";
				}

				string precision = format.Substring(2);
				if (String.IsNullOrEmpty(precision)) precision = "2";
				return String.Format("{0:N" + precision + "}{1}", size, suffix);

			}

			private static string defaultFormat(string format, object arg, IFormatProvider formatProvider)
			{
				IFormattable formattableArg = arg as IFormattable;
				if (formattableArg != null)
				{
					return formattableArg.ToString(format, formatProvider);
				}
				return arg.ToString();
			}

		}

		[Obsolete]
		public static int SizeForTexture(int width, int height, TextureFormat format)
		{
			int pixels = width * height;
			switch (format)
			{
				case TextureFormat.Format0_None: return 0;
				case TextureFormat.Format1_A3I5: return pixels;
				case TextureFormat.Format2_I2: return pixels / 4;
				case TextureFormat.Format3_I4: return pixels / 2;
				case TextureFormat.Format4_I8: return pixels;
				case TextureFormat.Format5_4x4: return 0;
				case TextureFormat.Format6_A5I3: return pixels;
				case TextureFormat.Format7_16bpp: return pixels * 2;
				default: throw new InvalidOperationException();
			}
		}

		public class TempFile : IDisposable
		{
			public TempFile(string ext)
			{
				Extension = "." + ext;
			}
			string BasePath = System.IO.Path.GetTempFileName();
			string Extension = "";
			public string Path { get { return System.IO.Path.ChangeExtension(BasePath, Extension); } }
			public void Dispose()
			{
				File.Delete(BasePath);
				File.Delete(Path);
			}
			public override string ToString()
			{
				return Path;
			}
		}

		public static unsafe Bitmap BitmapFromCorImage(Corona.Image img)
		{
			Corona.Image temp = img.Convert(Corona.PixelFormat.R8G8B8A8);
			byte[] pixels = temp.GrabPixels();

			var bmp = new System.Drawing.Bitmap(temp.Width, temp.Height, PixelFormat.Format32bppArgb);
			var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			fixed (byte* _srcp = &pixels[0])
			{
				byte* srcp = (byte*)_srcp;
				byte* destp = (byte*)bmpdata.Scan0.ToPointer();
				for (int y = 0; y < temp.Height; y++)
				{
					for (int x = 0; x < temp.Width; x++)
					{
						destp[0] = srcp[2];
						destp[1] = srcp[1];
						destp[2] = srcp[0];
						destp[3] = srcp[3];
						destp += 4;
						srcp += 4;
					}
					destp += bmpdata.Stride - temp.Width * 4;
				}
			}

			bmp.UnlockBits(bmpdata);
			temp.Dispose();
			return bmp;
		}

	}
}