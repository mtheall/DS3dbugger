using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

	public static class ConfigService
	{
		public static T Load<T>(string filepath) where T : new()
		{
			T config = new T();

			try
			{
				var file = new FileInfo(filepath);
				if (file.Exists)
					using (var reader = file.OpenText())
					{
						var s = new JsonSerializer();
						var r = new JsonTextReader(reader);
						config = (T)s.Deserialize(r, typeof(T));
					}
			}
            catch (Exception e) { MessageBox.Show(e.ToString()); }
			if (config == null) return new T();
			else return config;
		}

		public static void Save(string filepath, object config)
		{
			var file = new FileInfo(filepath);
			using (var writer = file.CreateText())
			{
				var s = new JsonSerializer();
				var w = new JsonTextWriter(writer) { Formatting = Formatting.Indented };
				s.Serialize(w, config);
			}
		}
	}
