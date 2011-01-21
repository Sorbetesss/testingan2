﻿namespace BizHawk.MultiClient
{
    partial class RamWatch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RamWatch));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.filesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.appendFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.restoreWindowSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.watchesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newWatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editWatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeWatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateWatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WatchListView = new System.Windows.Forms.ListView();
            this.Address = new System.Windows.Forms.ColumnHeader();
            this.Value = new System.Windows.Forms.ColumnHeader();
            this.Notes = new System.Windows.Forms.ColumnHeader();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.newToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.NewWatchStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.EditWatchToolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.cutToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.DuplicateWatchToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.MoveUpStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.MoveDownStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.WatchCountLabel = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filesToolStripMenuItem,
            this.watchesToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(364, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // filesToolStripMenuItem
            // 
            this.filesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newListToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.appendFileToolStripMenuItem,
            this.recentToolStripMenuItem,
            this.toolStripSeparator1,
            this.restoreWindowSizeToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.filesToolStripMenuItem.Name = "filesToolStripMenuItem";
            this.filesToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.filesToolStripMenuItem.Text = "&Files";
            this.filesToolStripMenuItem.DropDownOpened += new System.EventHandler(this.filesToolStripMenuItem_DropDownOpened);
            // 
            // newListToolStripMenuItem
            // 
            this.newListToolStripMenuItem.Name = "newListToolStripMenuItem";
            this.newListToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newListToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.newListToolStripMenuItem.Text = "&New List";
            this.newListToolStripMenuItem.Click += new System.EventHandler(this.newListToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.S)));
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // appendFileToolStripMenuItem
            // 
            this.appendFileToolStripMenuItem.Name = "appendFileToolStripMenuItem";
            this.appendFileToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.appendFileToolStripMenuItem.Text = "A&ppend File...";
            this.appendFileToolStripMenuItem.Click += new System.EventHandler(this.appendFileToolStripMenuItem_Click);
            // 
            // recentToolStripMenuItem
            // 
            this.recentToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noneToolStripMenuItem,
            this.toolStripSeparator4,
            this.clearToolStripMenuItem,
            this.autoLoadToolStripMenuItem});
            this.recentToolStripMenuItem.Name = "recentToolStripMenuItem";
            this.recentToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.recentToolStripMenuItem.Text = "Recent";
            this.recentToolStripMenuItem.DropDownOpened += new System.EventHandler(this.recentToolStripMenuItem_DropDownOpened);
            // 
            // noneToolStripMenuItem
            // 
            this.noneToolStripMenuItem.Name = "noneToolStripMenuItem";
            this.noneToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.noneToolStripMenuItem.Text = "None";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(132, 6);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            // 
            // autoLoadToolStripMenuItem
            // 
            this.autoLoadToolStripMenuItem.Name = "autoLoadToolStripMenuItem";
            this.autoLoadToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.autoLoadToolStripMenuItem.Text = "Auto-Load";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(201, 6);
            // 
            // restoreWindowSizeToolStripMenuItem
            // 
            this.restoreWindowSizeToolStripMenuItem.Name = "restoreWindowSizeToolStripMenuItem";
            this.restoreWindowSizeToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.restoreWindowSizeToolStripMenuItem.Text = "Restore Window Size";
            this.restoreWindowSizeToolStripMenuItem.Click += new System.EventHandler(this.restoreWindowSizeToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(201, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.exitToolStripMenuItem.Text = "&Close";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // watchesToolStripMenuItem
            // 
            this.watchesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newWatchToolStripMenuItem,
            this.editWatchToolStripMenuItem,
            this.removeWatchToolStripMenuItem,
            this.duplicateWatchToolStripMenuItem,
            this.toolStripSeparator3,
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem});
            this.watchesToolStripMenuItem.Name = "watchesToolStripMenuItem";
            this.watchesToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.watchesToolStripMenuItem.Text = "&Watches";
            // 
            // newWatchToolStripMenuItem
            // 
            this.newWatchToolStripMenuItem.Name = "newWatchToolStripMenuItem";
            this.newWatchToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.newWatchToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.newWatchToolStripMenuItem.Text = "&New Watch";
            this.newWatchToolStripMenuItem.Click += new System.EventHandler(this.newWatchToolStripMenuItem_Click);
            // 
            // editWatchToolStripMenuItem
            // 
            this.editWatchToolStripMenuItem.Name = "editWatchToolStripMenuItem";
            this.editWatchToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.editWatchToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.editWatchToolStripMenuItem.Text = "&Edit Watch";
            this.editWatchToolStripMenuItem.Click += new System.EventHandler(this.editWatchToolStripMenuItem_Click);
            // 
            // removeWatchToolStripMenuItem
            // 
            this.removeWatchToolStripMenuItem.Name = "removeWatchToolStripMenuItem";
            this.removeWatchToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.removeWatchToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.removeWatchToolStripMenuItem.Text = "&Remove Watch";
            this.removeWatchToolStripMenuItem.Click += new System.EventHandler(this.removeWatchToolStripMenuItem_Click);
            // 
            // duplicateWatchToolStripMenuItem
            // 
            this.duplicateWatchToolStripMenuItem.Name = "duplicateWatchToolStripMenuItem";
            this.duplicateWatchToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.duplicateWatchToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.duplicateWatchToolStripMenuItem.Text = "&Duplicate Watch";
            this.duplicateWatchToolStripMenuItem.Click += new System.EventHandler(this.duplicateWatchToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(172, 6);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Up)));
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.moveUpToolStripMenuItem.Text = "Move &Up";
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.moveUpToolStripMenuItem_Click);
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Down)));
            this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.moveDownToolStripMenuItem.Text = "Move &Down";
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.moveDownToolStripMenuItem_Click);
            // 
            // WatchListView
            // 
            this.WatchListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.WatchListView.AutoArrange = false;
            this.WatchListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Address,
            this.Value,
            this.Notes});
            this.WatchListView.FullRowSelect = true;
            this.WatchListView.GridLines = true;
            this.WatchListView.LabelEdit = true;
            this.WatchListView.Location = new System.Drawing.Point(25, 76);
            this.WatchListView.Name = "WatchListView";
            this.WatchListView.Size = new System.Drawing.Size(314, 324);
            this.WatchListView.TabIndex = 1;
            this.WatchListView.UseCompatibleStateImageBehavior = false;
            this.WatchListView.View = System.Windows.Forms.View.Details;
            this.WatchListView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.WatchListView_AfterLabelEdit);
            this.WatchListView.SelectedIndexChanged += new System.EventHandler(this.WatchListView_SelectedIndexChanged);
            // 
            // Address
            // 
            this.Address.Text = "Address";
            // 
            // Value
            // 
            this.Value.Text = "Value";
            this.Value.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Notes
            // 
            this.Notes.Text = "Notes";
            this.Notes.Width = 190;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripButton,
            this.openToolStripButton,
            this.saveToolStripButton,
            this.toolStripSeparator,
            this.NewWatchStripButton1,
            this.EditWatchToolStripButton1,
            this.cutToolStripButton,
            this.DuplicateWatchToolStripButton,
            this.toolStripSeparator5,
            this.MoveUpStripButton1,
            this.MoveDownStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(364, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // newToolStripButton
            // 
            this.newToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("newToolStripButton.Image")));
            this.newToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newToolStripButton.Name = "newToolStripButton";
            this.newToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.newToolStripButton.Text = "&New";
            this.newToolStripButton.Click += new System.EventHandler(this.newToolStripButton_Click);
            // 
            // openToolStripButton
            // 
            this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripButton.Image")));
            this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripButton.Name = "openToolStripButton";
            this.openToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.openToolStripButton.Text = "&Open";
            this.openToolStripButton.Click += new System.EventHandler(this.openToolStripButton_Click);
            // 
            // saveToolStripButton
            // 
            this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripButton.Image")));
            this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripButton.Name = "saveToolStripButton";
            this.saveToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.saveToolStripButton.Text = "&Save";
            this.saveToolStripButton.Click += new System.EventHandler(this.saveToolStripButton_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // NewWatchStripButton1
            // 
            this.NewWatchStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.NewWatchStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("NewWatchStripButton1.Image")));
            this.NewWatchStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.NewWatchStripButton1.Name = "NewWatchStripButton1";
            this.NewWatchStripButton1.Size = new System.Drawing.Size(23, 22);
            this.NewWatchStripButton1.Text = "New Watch";
            this.NewWatchStripButton1.ToolTipText = "New Watch";
            this.NewWatchStripButton1.Click += new System.EventHandler(this.NewWatchStripButton1_Click);
            // 
            // EditWatchToolStripButton1
            // 
            this.EditWatchToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.EditWatchToolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("EditWatchToolStripButton1.Image")));
            this.EditWatchToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.EditWatchToolStripButton1.Name = "EditWatchToolStripButton1";
            this.EditWatchToolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.EditWatchToolStripButton1.Text = "Edit Watch";
            this.EditWatchToolStripButton1.Click += new System.EventHandler(this.EditWatchToolStripButton1_Click);
            // 
            // cutToolStripButton
            // 
            this.cutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cutToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripButton.Image")));
            this.cutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cutToolStripButton.Name = "cutToolStripButton";
            this.cutToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.cutToolStripButton.Text = "C&ut";
            this.cutToolStripButton.ToolTipText = "Remove Watch";
            this.cutToolStripButton.Click += new System.EventHandler(this.cutToolStripButton_Click);
            // 
            // DuplicateWatchToolStripButton
            // 
            this.DuplicateWatchToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.DuplicateWatchToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("DuplicateWatchToolStripButton.Image")));
            this.DuplicateWatchToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.DuplicateWatchToolStripButton.Name = "DuplicateWatchToolStripButton";
            this.DuplicateWatchToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.DuplicateWatchToolStripButton.Text = "Duplicate Watch";
            this.DuplicateWatchToolStripButton.Click += new System.EventHandler(this.DuplicateWatchToolStripButton_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // MoveUpStripButton1
            // 
            this.MoveUpStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MoveUpStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("MoveUpStripButton1.Image")));
            this.MoveUpStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MoveUpStripButton1.Name = "MoveUpStripButton1";
            this.MoveUpStripButton1.Size = new System.Drawing.Size(23, 22);
            this.MoveUpStripButton1.Text = "Move Up";
            this.MoveUpStripButton1.Click += new System.EventHandler(this.MoveUpStripButton1_Click);
            // 
            // MoveDownStripButton1
            // 
            this.MoveDownStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MoveDownStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("MoveDownStripButton1.Image")));
            this.MoveDownStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MoveDownStripButton1.Name = "MoveDownStripButton1";
            this.MoveDownStripButton1.Size = new System.Drawing.Size(23, 22);
            this.MoveDownStripButton1.Text = "Move Down";
            this.MoveDownStripButton1.Click += new System.EventHandler(this.MoveDownStripButton1_Click);
            // 
            // WatchCountLabel
            // 
            this.WatchCountLabel.AutoSize = true;
            this.WatchCountLabel.Location = new System.Drawing.Point(22, 57);
            this.WatchCountLabel.Name = "WatchCountLabel";
            this.WatchCountLabel.Size = new System.Drawing.Size(56, 13);
            this.WatchCountLabel.TabIndex = 4;
            this.WatchCountLabel.Text = "0 watches";
            // 
            // RamWatch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 412);
            this.Controls.Add(this.WatchCountLabel);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.WatchListView);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "RamWatch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Ram Watch";
            this.Load += new System.EventHandler(this.RamWatch_Load);
            this.Resize += new System.EventHandler(this.RamWatch_Resize);
            this.LocationChanged += new System.EventHandler(this.RamWatch_LocationChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem filesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem watchesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem appendFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recentToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ListView WatchListView;
        private System.Windows.Forms.ToolStripMenuItem newWatchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editWatchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeWatchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem duplicateWatchToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ColumnHeader Address;
        private System.Windows.Forms.ColumnHeader Value;
        private System.Windows.Forms.ColumnHeader Notes;
        private System.Windows.Forms.Label WatchCountLabel;
        private System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoLoadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restoreWindowSizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton newToolStripButton;
        private System.Windows.Forms.ToolStripButton openToolStripButton;
        private System.Windows.Forms.ToolStripButton saveToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripButton cutToolStripButton;
        private System.Windows.Forms.ToolStripButton NewWatchStripButton1;
        private System.Windows.Forms.ToolStripButton MoveUpStripButton1;
        private System.Windows.Forms.ToolStripButton MoveDownStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton EditWatchToolStripButton1;
        private System.Windows.Forms.ToolStripButton DuplicateWatchToolStripButton;
    }
}