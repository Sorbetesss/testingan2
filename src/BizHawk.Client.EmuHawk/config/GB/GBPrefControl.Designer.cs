﻿namespace BizHawk.Client.EmuHawk
{
	partial class GBPrefControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.buttonDefaults = new System.Windows.Forms.Button();
            this.buttonPalette = new System.Windows.Forms.Button();
            this.cbRgbdsSyntax = new System.Windows.Forms.CheckBox();
            this.checkBoxMuted = new System.Windows.Forms.CheckBox();
            this.cbShowBorder = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid1.Location = new System.Drawing.Point(3, 3);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propertyGrid1.Size = new System.Drawing.Size(402, 368);
            this.propertyGrid1.TabIndex = 0;
            this.propertyGrid1.ToolbarVisible = false;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.PropertyGrid1_PropertyValueChanged);
            // 
            // buttonDefaults
            // 
            this.buttonDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDefaults.Location = new System.Drawing.Point(330, 377);
            this.buttonDefaults.Name = "buttonDefaults";
            this.buttonDefaults.Size = new System.Drawing.Size(75, 23);
            this.buttonDefaults.TabIndex = 1;
            this.buttonDefaults.Text = "Defaults";
            this.buttonDefaults.UseVisualStyleBackColor = true;
            this.buttonDefaults.Click += new System.EventHandler(this.ButtonDefaults_Click);
            // 
            // buttonPalette
            // 
            this.buttonPalette.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonPalette.Location = new System.Drawing.Point(3, 377);
            this.buttonPalette.Name = "buttonPalette";
            this.buttonPalette.Size = new System.Drawing.Size(75, 23);
            this.buttonPalette.TabIndex = 2;
            this.buttonPalette.Text = "Palette...";
            this.buttonPalette.UseVisualStyleBackColor = true;
            this.buttonPalette.Click += new System.EventHandler(this.ButtonPalette_Click);
            // 
            // cbRgbdsSyntax
            // 
            this.cbRgbdsSyntax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbRgbdsSyntax.AutoSize = true;
            this.cbRgbdsSyntax.Location = new System.Drawing.Point(138, 381);
            this.cbRgbdsSyntax.Name = "cbRgbdsSyntax";
            this.cbRgbdsSyntax.Size = new System.Drawing.Size(99, 17);
            this.cbRgbdsSyntax.TabIndex = 3;
            this.cbRgbdsSyntax.Text = "RGBDS Syntax";
            this.cbRgbdsSyntax.UseVisualStyleBackColor = true;
            this.cbRgbdsSyntax.CheckedChanged += new System.EventHandler(this.CbRgbdsSyntax_CheckedChanged);
            // 
            // checkBoxMuted
            // 
            this.checkBoxMuted.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxMuted.AutoSize = true;
            this.checkBoxMuted.Location = new System.Drawing.Point(82, 381);
            this.checkBoxMuted.Name = "checkBoxMuted";
            this.checkBoxMuted.Size = new System.Drawing.Size(50, 17);
            this.checkBoxMuted.TabIndex = 4;
            this.checkBoxMuted.Text = "Mute";
            this.checkBoxMuted.UseVisualStyleBackColor = true;
            this.checkBoxMuted.CheckedChanged += new System.EventHandler(this.CheckBoxMuted_CheckedChanged);
            // 
            // cbShowBorder
            // 
            this.cbShowBorder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbShowBorder.AutoSize = true;
            this.cbShowBorder.Location = new System.Drawing.Point(243, 381);
            this.cbShowBorder.Name = "cbShowBorder";
            this.cbShowBorder.Size = new System.Drawing.Size(87, 17);
            this.cbShowBorder.TabIndex = 5;
            this.cbShowBorder.Text = "Show Border";
            this.cbShowBorder.UseVisualStyleBackColor = true;
            this.cbShowBorder.CheckedChanged += new System.EventHandler(this.CbShowBorder_CheckedChanged);
            // 
            // GBPrefControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.cbRgbdsSyntax);
            this.Controls.Add(this.checkBoxMuted);
            this.Controls.Add(this.cbShowBorder);
            this.Controls.Add(this.buttonPalette);
            this.Controls.Add(this.buttonDefaults);
            this.Controls.Add(this.propertyGrid1);
            this.Name = "GBPrefControl";
            this.Size = new System.Drawing.Size(408, 403);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PropertyGrid propertyGrid1;
		private System.Windows.Forms.Button buttonDefaults;
		private System.Windows.Forms.Button buttonPalette;
		private System.Windows.Forms.CheckBox cbRgbdsSyntax;
		private System.Windows.Forms.CheckBox checkBoxMuted;
		private System.Windows.Forms.CheckBox cbShowBorder;
	}
}
