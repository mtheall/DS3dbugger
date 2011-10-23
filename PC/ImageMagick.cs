using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

namespace DS3dbugger
{
	class ImageMagick
	{
		static string[] Escape(object[] args)
		{
			return args.Select(s => {
				var tmp = s.ToString();
				return tmp.Contains(" ") ? string.Format("\"{0}\"", tmp) : tmp;
			}).ToArray();
		}

		public Corona.Image ConvertDown(Corona.Image sourceImage, TextureFormat toFormat)
		{
			int bits = -1;
			if (toFormat == TextureFormat.Format2_I2) bits = 2;
			if (toFormat == TextureFormat.Format3_I4) bits = 4;
			if (toFormat == TextureFormat.Format4_I8) bits = 8;
			if (toFormat == TextureFormat.Format7_16bpp) bits = 16;

			int colors = 1<<bits;

			Corona.Image ret = null; 

			using (Util.TempFile tmpIn = new Util.TempFile("png"), tmpOut = new Util.TempFile("png"))
			{
				sourceImage.Save(tmpIn.Path, Corona.FileFormat.PNG);

				string output = Run("-treedepth", 4, "-colors", colors, tmpIn, tmpOut);

				if (toFormat == TextureFormat.Format7_16bpp)
					ret = Corona.Image.Open(tmpOut.Path, Corona.PixelFormat.R8G8B8A8, Corona.FileFormat.PNG);
				else
					ret = Corona.Image.Open(tmpOut.Path, Corona.PixelFormat.I8, Corona.FileFormat.PNG);

			}
			return ret;
		}

		public string Run(params object[] args)
		{
			var strings = Escape(args);
			StringBuilder sbCmdline = new StringBuilder();
			for (int i = 0; i < strings.Length; i++)
			{
				sbCmdline.Append(strings[i].ToString());
				if (i != args.Length - 1) sbCmdline.Append(' ');
			}

			ProcessStartInfo oInfo = new ProcessStartInfo("convert.exe", sbCmdline.ToString());
			oInfo.UseShellExecute = false;
			oInfo.CreateNoWindow = true;
			oInfo.RedirectStandardOutput = true;
			oInfo.RedirectStandardError = true;

			Process proc = System.Diagnostics.Process.Start(oInfo);
			proc.WaitForExit();
			string result = proc.StandardError.ReadToEnd();

			return result;
		}

	}
}