﻿namespace BizHawk.Client.EmuHawk
{
	partial class DualNDSCoreConfig
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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.propertyGrid2 = new System.Windows.Forms.PropertyGrid();
			this.OkBtn = new System.Windows.Forms.Button();
			this.CancelBtn = new System.Windows.Forms.Button();
			this.DefaultsBtn = new System.Windows.Forms.Button();
			this.checkBoxLeftAccurateAudioBitrate = new System.Windows.Forms.CheckBox();
			this.checkBoxRightAccurateAudioBitrate = new System.Windows.Forms.CheckBox();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(12, 12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(400, 312);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.propertyGrid1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(312, 305);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Left Sync Settings";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
			this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid1.Location = new System.Drawing.Point(3, 3);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.NoSort;
			this.propertyGrid1.Size = new System.Drawing.Size(306, 299);
			this.propertyGrid1.TabIndex = 0;
			this.propertyGrid1.ToolbarVisible = false;
			this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.PropertyGrid1_PropertyValueChanged);
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.propertyGrid2);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(392, 286);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Right Sync Settings";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// propertyGrid2
			// 
			this.propertyGrid2.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
			this.propertyGrid2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid2.Location = new System.Drawing.Point(3, 3);
			this.propertyGrid2.Name = "propertyGrid2";
			this.propertyGrid2.PropertySort = System.Windows.Forms.PropertySort.NoSort;
			this.propertyGrid2.Size = new System.Drawing.Size(386, 280);
			this.propertyGrid2.TabIndex = 0;
			this.propertyGrid2.ToolbarVisible = false;
			this.propertyGrid2.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.PropertyGrid2_PropertyValueChanged);
			// 
			// OkBtn
			// 
			this.OkBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.OkBtn.Location = new System.Drawing.Point(256, 330);
			this.OkBtn.Name = "OkBtn";
			this.OkBtn.Size = new System.Drawing.Size(75, 42);
			this.OkBtn.TabIndex = 1;
			this.OkBtn.Text = "OK";
			this.OkBtn.UseVisualStyleBackColor = true;
			this.OkBtn.Click += new System.EventHandler(this.OkBtn_Click);
			// 
			// CancelBtn
			// 
			this.CancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.Location = new System.Drawing.Point(337, 330);
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.Size = new System.Drawing.Size(75, 42);
			this.CancelBtn.TabIndex = 2;
			this.CancelBtn.Text = "Cancel";
			this.CancelBtn.UseVisualStyleBackColor = true;
			// 
			// DefaultsBtn
			// 
			this.DefaultsBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.DefaultsBtn.Location = new System.Drawing.Point(12, 330);
			this.DefaultsBtn.Name = "DefaultsBtn";
			this.DefaultsBtn.Size = new System.Drawing.Size(75, 40);
			this.DefaultsBtn.TabIndex = 3;
			this.DefaultsBtn.Text = "Defaults";
			this.DefaultsBtn.UseVisualStyleBackColor = true;
			this.DefaultsBtn.Click += new System.EventHandler(this.DefaultsBtn_Click);
			// 
			// checkBoxLeftAccurateAudioBitrate
			// 
			this.checkBoxLeftAccurateAudioBitrate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBoxLeftAccurateAudioBitrate.AutoSize = true;
			this.checkBoxLeftAccurateAudioBitrate.Location = new System.Drawing.Point(93, 330);
			this.checkBoxLeftAccurateAudioBitrate.Name = "checkBoxLeftAccurateAudioBitrate";
			this.checkBoxLeftAccurateAudioBitrate.Size = new System.Drawing.Size(153, 17);
			this.checkBoxLeftAccurateAudioBitrate.TabIndex = 6;
			this.checkBoxLeftAccurateAudioBitrate.Text = "Left Accurate Audio Bitrate";
			this.checkBoxLeftAccurateAudioBitrate.UseVisualStyleBackColor = true;
			this.checkBoxLeftAccurateAudioBitrate.CheckedChanged += new System.EventHandler(this.CheckBoxLeftAccurateAudioBitrate_CheckedChanged);
			// 
			// checkBoxRightAccurateAudioBitrate
			// 
			this.checkBoxRightAccurateAudioBitrate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBoxRightAccurateAudioBitrate.AutoSize = true;
			this.checkBoxRightAccurateAudioBitrate.Location = new System.Drawing.Point(93, 353);
			this.checkBoxRightAccurateAudioBitrate.Name = "checkBoxRightAccurateAudioBitrate";
			this.checkBoxRightAccurateAudioBitrate.Size = new System.Drawing.Size(160, 17);
			this.checkBoxRightAccurateAudioBitrate.TabIndex = 5;
			this.checkBoxRightAccurateAudioBitrate.Text = "Right Accurate Audio Bitrate";
			this.checkBoxRightAccurateAudioBitrate.UseVisualStyleBackColor = true;
			this.checkBoxRightAccurateAudioBitrate.CheckedChanged += new System.EventHandler(this.CheckBoxRightAccurateAudioBitrate_CheckedChanged);
			// 
			// DualNDSCoreConfig
			// 
			this.AcceptButton = this.OkBtn;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CancelBtn;
			this.ClientSize = new System.Drawing.Size(424, 384);
			this.Controls.Add(this.checkBoxLeftAccurateAudioBitrate);
			this.Controls.Add(this.checkBoxRightAccurateAudioBitrate);
			this.Controls.Add(this.DefaultsBtn);
			this.Controls.Add(this.CancelBtn);
			this.Controls.Add(this.OkBtn);
			this.Controls.Add(this.tabControl1);
			this.Name = "DualNDSCoreConfig";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "DualNDSCoreConfig";
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.PropertyGrid propertyGrid1;
		private System.Windows.Forms.PropertyGrid propertyGrid2;
		private System.Windows.Forms.Button OkBtn;
		private System.Windows.Forms.Button CancelBtn;
		private System.Windows.Forms.Button DefaultsBtn;
		private System.Windows.Forms.CheckBox checkBoxLeftAccurateAudioBitrate;
		private System.Windows.Forms.CheckBox checkBoxRightAccurateAudioBitrate;
	}
}