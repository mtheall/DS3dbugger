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
			this.vpDSScreen = new RetainedViewportPanel();
			this.btnAcquireScreen = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.btnCopyScreen = new System.Windows.Forms.Button();
			this.txtFilename = new System.Windows.Forms.TextBox();
			this.btnChooseFile = new System.Windows.Forms.Button();
			this.btnSend = new System.Windows.Forms.Button();
			this.listView1 = new System.Windows.Forms.ListView();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.btnNewProject = new System.Windows.Forms.Button();
			this.btnOpenProject = new System.Windows.Forms.Button();
			this.btnSaveProject = new System.Windows.Forms.Button();
			this.grpTexInfo = new System.Windows.Forms.GroupBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.txtTexInfoPalAddr = new System.Windows.Forms.TextBox();
			this.txtTexInfoTexAddr = new System.Windows.Forms.TextBox();
			this.txtTexInfoSize = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.txtTexInfoDims = new System.Windows.Forms.TextBox();
			this.txtTexInfoName = new System.Windows.Forms.TextBox();
			this.checkColor0Transparent = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.rbTexFmt7_16bpp = new System.Windows.Forms.RadioButton();
			this.rbTexFmt6_A5I3 = new System.Windows.Forms.RadioButton();
			this.rbTexFmt5_4x4 = new System.Windows.Forms.RadioButton();
			this.rbTexFmt4_I8 = new System.Windows.Forms.RadioButton();
			this.rbTexFmt3_I4 = new System.Windows.Forms.RadioButton();
			this.rbTexFmt2_I2 = new System.Windows.Forms.RadioButton();
			this.rbTexFmt1_A3I5 = new System.Windows.Forms.RadioButton();
			this.label4 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.vpTexLarge = new RetainedViewportPanel();
			this.lvTextures = new System.Windows.Forms.ListView();
			this.button1 = new System.Windows.Forms.Button();
			this.txtTotalTexSize = new System.Windows.Forms.TextBox();
			this.button2 = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.grpTexInfo.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnConnect
			// 
			this.btnConnect.Location = new System.Drawing.Point(827, 11);
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.Size = new System.Drawing.Size(75, 23);
			this.btnConnect.TabIndex = 0;
			this.btnConnect.Text = "Connect";
			this.btnConnect.UseVisualStyleBackColor = true;
			this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
			// 
			// txtHost
			// 
			this.txtHost.Location = new System.Drawing.Point(540, 13);
			this.txtHost.Name = "txtHost";
			this.txtHost.Size = new System.Drawing.Size(281, 20);
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
			// vpDSScreen
			// 
			this.vpDSScreen.Location = new System.Drawing.Point(3, 3);
			this.vpDSScreen.Name = "vpDSScreen";
			this.vpDSScreen.Size = new System.Drawing.Size(512, 384);
			this.vpDSScreen.TabIndex = 2;
			this.vpDSScreen.Text = "label1";
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
			// txtFilename
			// 
			this.txtFilename.Location = new System.Drawing.Point(540, 42);
			this.txtFilename.Name = "txtFilename";
			this.txtFilename.ReadOnly = true;
			this.txtFilename.Size = new System.Drawing.Size(281, 20);
			this.txtFilename.TabIndex = 7;
			// 
			// btnChooseFile
			// 
			this.btnChooseFile.Location = new System.Drawing.Point(827, 40);
			this.btnChooseFile.Name = "btnChooseFile";
			this.btnChooseFile.Size = new System.Drawing.Size(75, 23);
			this.btnChooseFile.TabIndex = 8;
			this.btnChooseFile.Text = "Choose File";
			this.btnChooseFile.UseVisualStyleBackColor = true;
			this.btnChooseFile.Click += new System.EventHandler(this.btnChooseFile_Click);
			// 
			// btnSend
			// 
			this.btnSend.Location = new System.Drawing.Point(540, 442);
			this.btnSend.Name = "btnSend";
			this.btnSend.Size = new System.Drawing.Size(75, 23);
			this.btnSend.TabIndex = 9;
			this.btnSend.Text = "Send";
			this.btnSend.UseVisualStyleBackColor = true;
			this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
			// 
			// listView1
			// 
			this.listView1.Location = new System.Drawing.Point(540, 137);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(249, 299);
			this.listView1.TabIndex = 10;
			this.listView1.UseCompatibleStateImageBehavior = false;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(537, 119);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(84, 13);
			this.label2.TabIndex = 11;
			this.label2.Text = "Display List (tbd)";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(14, 462);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(48, 13);
			this.label3.TabIndex = 13;
			this.label3.Text = "Textures";
			// 
			// btnNewProject
			// 
			this.btnNewProject.Location = new System.Drawing.Point(540, 68);
			this.btnNewProject.Name = "btnNewProject";
			this.btnNewProject.Size = new System.Drawing.Size(75, 23);
			this.btnNewProject.TabIndex = 14;
			this.btnNewProject.Text = "New Project";
			this.btnNewProject.UseVisualStyleBackColor = true;
			this.btnNewProject.Click += new System.EventHandler(this.btnNewProject_Click);
			// 
			// btnOpenProject
			// 
			this.btnOpenProject.Location = new System.Drawing.Point(621, 68);
			this.btnOpenProject.Name = "btnOpenProject";
			this.btnOpenProject.Size = new System.Drawing.Size(79, 23);
			this.btnOpenProject.TabIndex = 15;
			this.btnOpenProject.Text = "Open Project";
			this.btnOpenProject.UseVisualStyleBackColor = true;
			this.btnOpenProject.Click += new System.EventHandler(this.btnOpenProject_Click);
			// 
			// btnSaveProject
			// 
			this.btnSaveProject.Location = new System.Drawing.Point(706, 68);
			this.btnSaveProject.Name = "btnSaveProject";
			this.btnSaveProject.Size = new System.Drawing.Size(79, 23);
			this.btnSaveProject.TabIndex = 16;
			this.btnSaveProject.Text = "Save Project";
			this.btnSaveProject.UseVisualStyleBackColor = true;
			this.btnSaveProject.Click += new System.EventHandler(this.btnSaveProject_Click);
			// 
			// grpTexInfo
			// 
			this.grpTexInfo.Controls.Add(this.label8);
			this.grpTexInfo.Controls.Add(this.label7);
			this.grpTexInfo.Controls.Add(this.txtTexInfoPalAddr);
			this.grpTexInfo.Controls.Add(this.txtTexInfoTexAddr);
			this.grpTexInfo.Controls.Add(this.txtTexInfoSize);
			this.grpTexInfo.Controls.Add(this.label6);
			this.grpTexInfo.Controls.Add(this.txtTexInfoDims);
			this.grpTexInfo.Controls.Add(this.txtTexInfoName);
			this.grpTexInfo.Controls.Add(this.checkColor0Transparent);
			this.grpTexInfo.Controls.Add(this.label5);
			this.grpTexInfo.Controls.Add(this.groupBox2);
			this.grpTexInfo.Controls.Add(this.label4);
			this.grpTexInfo.Controls.Add(this.panel2);
			this.grpTexInfo.Location = new System.Drawing.Point(540, 480);
			this.grpTexInfo.Name = "grpTexInfo";
			this.grpTexInfo.Size = new System.Drawing.Size(281, 296);
			this.grpTexInfo.TabIndex = 17;
			this.grpTexInfo.TabStop = false;
			this.grpTexInfo.Text = "Texture Info";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(5, 263);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(39, 13);
			this.label8.TabIndex = 17;
			this.label8.Text = "PAddr:";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(11, 237);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(32, 13);
			this.label7.TabIndex = 16;
			this.label7.Text = "Addr:";
			// 
			// txtTexInfoPalAddr
			// 
			this.txtTexInfoPalAddr.Location = new System.Drawing.Point(45, 260);
			this.txtTexInfoPalAddr.Name = "txtTexInfoPalAddr";
			this.txtTexInfoPalAddr.ReadOnly = true;
			this.txtTexInfoPalAddr.Size = new System.Drawing.Size(99, 20);
			this.txtTexInfoPalAddr.TabIndex = 15;
			// 
			// txtTexInfoTexAddr
			// 
			this.txtTexInfoTexAddr.Location = new System.Drawing.Point(44, 234);
			this.txtTexInfoTexAddr.Name = "txtTexInfoTexAddr";
			this.txtTexInfoTexAddr.ReadOnly = true;
			this.txtTexInfoTexAddr.Size = new System.Drawing.Size(99, 20);
			this.txtTexInfoTexAddr.TabIndex = 14;
			// 
			// txtTexInfoSize
			// 
			this.txtTexInfoSize.Location = new System.Drawing.Point(45, 211);
			this.txtTexInfoSize.Name = "txtTexInfoSize";
			this.txtTexInfoSize.ReadOnly = true;
			this.txtTexInfoSize.Size = new System.Drawing.Size(99, 20);
			this.txtTexInfoSize.TabIndex = 12;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(11, 214);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(30, 13);
			this.label6.TabIndex = 11;
			this.label6.Text = "Size:";
			// 
			// txtTexInfoDims
			// 
			this.txtTexInfoDims.Location = new System.Drawing.Point(45, 189);
			this.txtTexInfoDims.Name = "txtTexInfoDims";
			this.txtTexInfoDims.ReadOnly = true;
			this.txtTexInfoDims.Size = new System.Drawing.Size(99, 20);
			this.txtTexInfoDims.TabIndex = 10;
			// 
			// txtTexInfoName
			// 
			this.txtTexInfoName.Location = new System.Drawing.Point(45, 163);
			this.txtTexInfoName.Name = "txtTexInfoName";
			this.txtTexInfoName.ReadOnly = true;
			this.txtTexInfoName.Size = new System.Drawing.Size(99, 20);
			this.txtTexInfoName.TabIndex = 9;
			// 
			// checkColor0Transparent
			// 
			this.checkColor0Transparent.AutoSize = true;
			this.checkColor0Transparent.Enabled = false;
			this.checkColor0Transparent.Location = new System.Drawing.Point(161, 211);
			this.checkColor0Transparent.Name = "checkColor0Transparent";
			this.checkColor0Transparent.Size = new System.Drawing.Size(83, 17);
			this.checkColor0Transparent.TabIndex = 8;
			this.checkColor0Transparent.Text = "Color0Trans";
			this.checkColor0Transparent.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(11, 192);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(33, 13);
			this.label5.TabIndex = 7;
			this.label5.Text = "Dims:";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.rbTexFmt7_16bpp);
			this.groupBox2.Controls.Add(this.rbTexFmt6_A5I3);
			this.groupBox2.Controls.Add(this.rbTexFmt5_4x4);
			this.groupBox2.Controls.Add(this.rbTexFmt4_I8);
			this.groupBox2.Controls.Add(this.rbTexFmt3_I4);
			this.groupBox2.Controls.Add(this.rbTexFmt2_I2);
			this.groupBox2.Controls.Add(this.rbTexFmt1_A3I5);
			this.groupBox2.Location = new System.Drawing.Point(151, 19);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(110, 186);
			this.groupBox2.TabIndex = 6;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Format";
			// 
			// rbTexFmt7_16bpp
			// 
			this.rbTexFmt7_16bpp.AutoSize = true;
			this.rbTexFmt7_16bpp.Location = new System.Drawing.Point(10, 158);
			this.rbTexFmt7_16bpp.Name = "rbTexFmt7_16bpp";
			this.rbTexFmt7_16bpp.Size = new System.Drawing.Size(70, 17);
			this.rbTexFmt7_16bpp.TabIndex = 6;
			this.rbTexFmt7_16bpp.TabStop = true;
			this.rbTexFmt7_16bpp.Text = "7 - 16bpp";
			this.rbTexFmt7_16bpp.UseVisualStyleBackColor = true;
			this.rbTexFmt7_16bpp.CheckedChanged += new System.EventHandler(this.rbTexFmt_CheckedChanged);
			// 
			// rbTexFmt6_A5I3
			// 
			this.rbTexFmt6_A5I3.AutoSize = true;
			this.rbTexFmt6_A5I3.Location = new System.Drawing.Point(10, 135);
			this.rbTexFmt6_A5I3.Name = "rbTexFmt6_A5I3";
			this.rbTexFmt6_A5I3.Size = new System.Drawing.Size(62, 17);
			this.rbTexFmt6_A5I3.TabIndex = 5;
			this.rbTexFmt6_A5I3.TabStop = true;
			this.rbTexFmt6_A5I3.Text = "6 - A5I3";
			this.rbTexFmt6_A5I3.UseVisualStyleBackColor = true;
			this.rbTexFmt6_A5I3.CheckedChanged += new System.EventHandler(this.rbTexFmt_CheckedChanged);
			// 
			// rbTexFmt5_4x4
			// 
			this.rbTexFmt5_4x4.AutoSize = true;
			this.rbTexFmt5_4x4.Enabled = false;
			this.rbTexFmt5_4x4.Location = new System.Drawing.Point(10, 112);
			this.rbTexFmt5_4x4.Name = "rbTexFmt5_4x4";
			this.rbTexFmt5_4x4.Size = new System.Drawing.Size(57, 17);
			this.rbTexFmt5_4x4.TabIndex = 4;
			this.rbTexFmt5_4x4.TabStop = true;
			this.rbTexFmt5_4x4.Text = "5 - 4x4";
			this.rbTexFmt5_4x4.UseVisualStyleBackColor = true;
			this.rbTexFmt5_4x4.CheckedChanged += new System.EventHandler(this.rbTexFmt_CheckedChanged);
			// 
			// rbTexFmt4_I8
			// 
			this.rbTexFmt4_I8.AutoSize = true;
			this.rbTexFmt4_I8.Location = new System.Drawing.Point(10, 89);
			this.rbTexFmt4_I8.Name = "rbTexFmt4_I8";
			this.rbTexFmt4_I8.Size = new System.Drawing.Size(90, 17);
			this.rbTexFmt4_I8.TabIndex = 3;
			this.rbTexFmt4_I8.TabStop = true;
			this.rbTexFmt4_I8.Text = "4 - I8 (256col)";
			this.rbTexFmt4_I8.UseVisualStyleBackColor = true;
			this.rbTexFmt4_I8.CheckedChanged += new System.EventHandler(this.rbTexFmt_CheckedChanged);
			// 
			// rbTexFmt3_I4
			// 
			this.rbTexFmt3_I4.AutoSize = true;
			this.rbTexFmt3_I4.Location = new System.Drawing.Point(10, 66);
			this.rbTexFmt3_I4.Name = "rbTexFmt3_I4";
			this.rbTexFmt3_I4.Size = new System.Drawing.Size(84, 17);
			this.rbTexFmt3_I4.TabIndex = 2;
			this.rbTexFmt3_I4.TabStop = true;
			this.rbTexFmt3_I4.Text = "3 - I4 (16col)";
			this.rbTexFmt3_I4.UseVisualStyleBackColor = true;
			this.rbTexFmt3_I4.CheckedChanged += new System.EventHandler(this.rbTexFmt_CheckedChanged);
			// 
			// rbTexFmt2_I2
			// 
			this.rbTexFmt2_I2.AutoSize = true;
			this.rbTexFmt2_I2.Location = new System.Drawing.Point(10, 43);
			this.rbTexFmt2_I2.Name = "rbTexFmt2_I2";
			this.rbTexFmt2_I2.Size = new System.Drawing.Size(78, 17);
			this.rbTexFmt2_I2.TabIndex = 1;
			this.rbTexFmt2_I2.TabStop = true;
			this.rbTexFmt2_I2.Text = "2 - I2 (4col)";
			this.rbTexFmt2_I2.UseVisualStyleBackColor = true;
			this.rbTexFmt2_I2.CheckedChanged += new System.EventHandler(this.rbTexFmt_CheckedChanged);
			// 
			// rbTexFmt1_A3I5
			// 
			this.rbTexFmt1_A3I5.AutoSize = true;
			this.rbTexFmt1_A3I5.Location = new System.Drawing.Point(10, 20);
			this.rbTexFmt1_A3I5.Name = "rbTexFmt1_A3I5";
			this.rbTexFmt1_A3I5.Size = new System.Drawing.Size(62, 17);
			this.rbTexFmt1_A3I5.TabIndex = 0;
			this.rbTexFmt1_A3I5.TabStop = true;
			this.rbTexFmt1_A3I5.Text = "1 - A3I5";
			this.rbTexFmt1_A3I5.UseVisualStyleBackColor = true;
			this.rbTexFmt1_A3I5.CheckedChanged += new System.EventHandler(this.rbTexFmt_CheckedChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(3, 166);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(38, 13);
			this.label4.TabIndex = 5;
			this.label4.Text = "Name:";
			// 
			// panel2
			// 
			this.panel2.AutoSize = true;
			this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel2.Controls.Add(this.vpTexLarge);
			this.panel2.Location = new System.Drawing.Point(6, 19);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(138, 138);
			this.panel2.TabIndex = 4;
			// 
			// vpTexLarge
			// 
			this.vpTexLarge.Location = new System.Drawing.Point(3, 3);
			this.vpTexLarge.Name = "vpTexLarge";
			this.vpTexLarge.Size = new System.Drawing.Size(128, 128);
			this.vpTexLarge.TabIndex = 2;
			this.vpTexLarge.Text = "label1";
			// 
			// lvTextures
			// 
			this.lvTextures.AllowDrop = true;
			this.lvTextures.HideSelection = false;
			this.lvTextures.Location = new System.Drawing.Point(17, 480);
			this.lvTextures.Name = "lvTextures";
			this.lvTextures.Size = new System.Drawing.Size(512, 296);
			this.lvTextures.TabIndex = 12;
			this.lvTextures.UseCompatibleStateImageBehavior = false;
			this.lvTextures.SelectedIndexChanged += new System.EventHandler(this.lvTextures_SelectedIndexChanged);
			this.lvTextures.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvTextures_DragDrop);
			this.lvTextures.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvTextures_DragEnter);
			this.lvTextures.DragOver += new System.Windows.Forms.DragEventHandler(this.lvTextures_DragOver);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(757, 459);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(111, 23);
			this.button1.TabIndex = 18;
			this.button1.Text = "Send Tex (debug)";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// txtTotalTexSize
			// 
			this.txtTotalTexSize.Location = new System.Drawing.Point(68, 459);
			this.txtTotalTexSize.Name = "txtTotalTexSize";
			this.txtTotalTexSize.ReadOnly = true;
			this.txtTotalTexSize.Size = new System.Drawing.Size(216, 20);
			this.txtTotalTexSize.TabIndex = 19;
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(795, 430);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(111, 23);
			this.button2.TabIndex = 20;
			this.button2.Text = "Send list (debug)";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(914, 804);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.txtTotalTexSize);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.grpTexInfo);
			this.Controls.Add(this.btnSaveProject);
			this.Controls.Add(this.btnOpenProject);
			this.Controls.Add(this.btnNewProject);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.lvTextures);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.btnSend);
			this.Controls.Add(this.btnChooseFile);
			this.Controls.Add(this.txtFilename);
			this.Controls.Add(this.btnCopyScreen);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnAcquireScreen);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.txtHost);
			this.Controls.Add(this.btnConnect);
			this.Name = "MainForm";
			this.Text = "MainForm";
			this.panel1.ResumeLayout(false);
			this.grpTexInfo.ResumeLayout(false);
			this.grpTexInfo.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.panel2.ResumeLayout(false);
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
		private System.Windows.Forms.TextBox txtFilename;
		private System.Windows.Forms.Button btnChooseFile;
		private System.Windows.Forms.Button btnSend;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListView lvTextures;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnNewProject;
		private System.Windows.Forms.Button btnOpenProject;
		private System.Windows.Forms.Button btnSaveProject;
		private System.Windows.Forms.GroupBox grpTexInfo;
		private System.Windows.Forms.Panel panel2;
		private RetainedViewportPanel vpTexLarge;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton rbTexFmt7_16bpp;
		private System.Windows.Forms.RadioButton rbTexFmt6_A5I3;
		private System.Windows.Forms.RadioButton rbTexFmt5_4x4;
		private System.Windows.Forms.RadioButton rbTexFmt4_I8;
		private System.Windows.Forms.RadioButton rbTexFmt3_I4;
		private System.Windows.Forms.RadioButton rbTexFmt2_I2;
		private System.Windows.Forms.RadioButton rbTexFmt1_A3I5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox checkColor0Transparent;
		private System.Windows.Forms.TextBox txtTexInfoDims;
		private System.Windows.Forms.TextBox txtTexInfoName;
		private System.Windows.Forms.TextBox txtTexInfoSize;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox txtTexInfoPalAddr;
		private System.Windows.Forms.TextBox txtTexInfoTexAddr;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox txtTotalTexSize;
		private System.Windows.Forms.Button button2;
	}
}