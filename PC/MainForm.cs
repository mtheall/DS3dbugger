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

namespace DS3dbugger
{
	public partial class MainForm : Form
	{
		class ConfigStruct
		{
			public string host;
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

		private void btnConnect_Click(object sender, EventArgs e)
		{

		}
	}

}
