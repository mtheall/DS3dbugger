using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace DS3dbugger
{
	public static class PathManager
	{
		public static string CurrDirectory = GetExeDirectoryAbsolute();
		public static string CurrProjectPath;


		public static string GetExeDirectoryAbsolute()
		{
			string p = Path.GetDirectoryName(Assembly.GetEntryAssembly().GetName().CodeBase);
			if (p.Substring(0, 6) == "file:\\")
				p = p.Remove(0, 6);
			string z = p;
			return p;
		}


		public static string Relativeize(string fn)
		{
			Uri uriItem = new Uri(fn);
			Uri uriRoot = new Uri(CurrDirectory + "/");
			Uri uriNew = uriRoot.MakeRelativeUri(uriItem);
			string strNewUri = uriNew.ToString();
			if (Path.IsPathRooted(strNewUri))
				return null;
			return strNewUri;
		}

		public static string FullyQualify(string fn)
		{
			return Path.Combine(CurrDirectory, fn);
		}
	}
}