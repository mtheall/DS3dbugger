namespace DS3dbugger
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnConnect = new System.Windows.Forms.Button();
			this.txtHost = new System.Windows.Forms.TextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnAcquireScreen = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.vpDSScreen = new RetainedViewportPanel();
			this.btnCopyScreen = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnConnect
			// 
			this.btnConnect.Location = new System.Drawing.Point(830, 12);
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.Size = new System.Drawing.Size(75, 23);
			this.btnConnect.TabIndex = 0;
			this.btnConnect.Text = "Connect";
			this.btnConnect.UseVisualStyleBackColor = true;
			this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
			// 
			// txtHost
			// 
			this.txtHost.Location = new System.Drawing.Point(724, 12);
			this.txtHost.Name = "txtHost";
			this.txtHost.Size = new System.Drawing.Size(100, 20);
			this.txtHost.TabIndex = 1;
			// 
			// panel1
			// 
			this.panel1.AutoSize = true;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(this.vpDSScreen);
			this.panel1.Location = new System.Drawing.Point(12, 12);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(522, 394);
			this.panel1.TabIndex = 3;
			// 
			// btnAcquireScreen
			// 
			this.btnAcquireScreen.Location = new System.Drawing.Point(93, 412);
			this.btnAcquireScreen.Name = "btnAcquireScreen";
			this.btnAcquireScreen.Size = new System.Drawing.Size(63, 23);
			this.btnAcquireScreen.TabIndex = 4;
			this.btnAcquireScreen.Text = "Acquire";
			this.btnAcquireScreen.UseVisualStyleBackColor = true;
			this.btnAcquireScreen.Click += new System.EventHandler(this.btnAcquireScreen_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(14, 417);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(73, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "NDS 3d @ 2x";
			// 
			// vpDSScreen
			// 
			this.vpDSScreen.Location = new System.Drawing.Point(3, 3);
			this.vpDSScreen.Name = "vpDSScreen";
			this.vpDSScreen.Size = new System.Drawing.Size(512, 384);
			this.vpDSScreen.TabIndex = 2;
			this.vpDSScreen.Text = "label1";
			// 
			// btnCopyScreen
			// 
			this.btnCopyScreen.Location = new System.Drawing.Point(162, 412);
			this.btnCopyScreen.Name = "btnCopyScreen";
			this.btnCopyScreen.Size = new System.Drawing.Size(63, 23);
			this.btnCopyScreen.TabIndex = 6;
			this.btnCopyScreen.Text = "Copy";
			this.btnCopyScreen.UseVisualStyleBackColor = true;
			this.btnCopyScreen.Click += new System.EventHandler(this.btnCopyScreen_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(917, 479);
			this.Controls.Add(this.btnCopyScreen);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnAcquireScreen);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.txtHost);
			this.Controls.Add(this.btnConnect);
			this.Name = "MainForm";
			this.Text = "MainForm";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnConnect;
		private System.Windows.Forms.TextBox txtHost;
		private RetainedViewportPanel vpDSScreen;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnAcquireScreen;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnCopyScreen;
	}
}