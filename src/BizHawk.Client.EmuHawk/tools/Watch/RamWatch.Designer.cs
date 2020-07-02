﻿using BizHawk.WinForms.Controls;

namespace BizHawk.Client.EmuHawk
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
            this.components = new System.ComponentModel.Container();
            this.WatchCountLabel = new BizHawk.WinForms.Controls.LocLabelEx();
            this.ListViewContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RemoveContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DuplicateContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PokeContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FreezeContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UnfreezeAllContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewInHexEditorContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Separator4 = new System.Windows.Forms.ToolStripSeparator();
            this.ReadBreakpointContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WriteBreakpointContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Separator6 = new System.Windows.Forms.ToolStripSeparator();
            this.InsertSeperatorContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MoveUpContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MoveDownContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MoveTopContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MoveBottomContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new StatusStripEx();
            this.ErrorIconButton = new System.Windows.Forms.ToolStripButton();
            this.MessageLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new ToolStripEx();
            this.newToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.newWatchToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.editWatchToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.cutToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.clearChangeCountsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.duplicateWatchToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.PokeAddressToolBarItem = new System.Windows.Forms.ToolStripButton();
            this.FreezeAddressToolBarItem = new System.Windows.Forms.ToolStripButton();
            this.seperatorToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.moveUpToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.moveDownToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.RamWatchMenu = new MenuStripEx();
            this.FileSubMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.NewListMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AppendMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RecentSubMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.noneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ExitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WatchesSubMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.MemoryDomainsSubMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.Separator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.NewWatchMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditWatchMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RemoveWatchMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DuplicateWatchMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PokeAddressMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FreezeAddressMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.InsertSeparatorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ClearChangeCountsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.MoveUpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MoveDownMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MoveTopMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MoveBottomMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SelectAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.OptionsSubMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.DefinePreviousValueSubMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.PreviousFrameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LastChangeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OriginalMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WatchesOnScreenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveWindowPositionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AlwaysOnTopMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FloatingWindowMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.RestoreWindowSizeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WatchListView = new InputRoll();
            this.ListViewContextMenu.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.RamWatchMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // WatchCountLabel
            // 
            this.WatchCountLabel.Location = new System.Drawing.Point(16, 57);
            this.WatchCountLabel.Name = "WatchCountLabel";
            this.WatchCountLabel.Text = "0 watches";
            // 
            // ListViewContextMenu
            // 
            this.ListViewContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.EditContextMenuItem,
            this.RemoveContextMenuItem,
            this.DuplicateContextMenuItem,
            this.PokeContextMenuItem,
            this.FreezeContextMenuItem,
            this.UnfreezeAllContextMenuItem,
            this.ViewInHexEditorContextMenuItem,
            this.Separator4,
            this.ReadBreakpointContextMenuItem,
            this.WriteBreakpointContextMenuItem,
            this.Separator6,
            this.InsertSeperatorContextMenuItem,
            this.MoveUpContextMenuItem,
            this.MoveDownContextMenuItem,
            this.MoveTopContextMenuItem,
            this.MoveBottomContextMenuItem});
            this.ListViewContextMenu.Name = "contextMenuStrip1";
            this.ListViewContextMenu.Size = new System.Drawing.Size(245, 346);
            this.ListViewContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.ListViewContextMenu_Opening);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.newToolStripMenuItem.Text = "&New Watch";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.NewWatchMenuItem_Click);
            // 
            // EditContextMenuItem
            // 
            this.EditContextMenuItem.Name = "EditContextMenuItem";
            this.EditContextMenuItem.ShortcutKeyDisplayString = "Ctrl+E";
            this.EditContextMenuItem.Size = new System.Drawing.Size(244, 22);
            this.EditContextMenuItem.Text = "&Edit";
            this.EditContextMenuItem.Click += new System.EventHandler(this.EditWatchMenuItem_Click);
            // 
            // RemoveContextMenuItem
            // 
            this.RemoveContextMenuItem.Name = "RemoveContextMenuItem";
            this.RemoveContextMenuItem.ShortcutKeyDisplayString = "Ctrl+R";
            this.RemoveContextMenuItem.Size = new System.Drawing.Size(244, 22);
            this.RemoveContextMenuItem.Text = "&Remove";
            this.RemoveContextMenuItem.Click += new System.EventHandler(this.RemoveWatchMenuItem_Click);
            // 
            // DuplicateContextMenuItem
            // 
            this.DuplicateContextMenuItem.Name = "DuplicateContextMenuItem";
            this.DuplicateContextMenuItem.ShortcutKeyDisplayString = "Ctrl+D";
            this.DuplicateContextMenuItem.Size = new System.Drawing.Size(244, 22);
            this.DuplicateContextMenuItem.Text = "&Duplicate";
            this.DuplicateContextMenuItem.Click += new System.EventHandler(this.DuplicateWatchMenuItem_Click);
            // 
            // PokeContextMenuItem
            // 
            this.PokeContextMenuItem.Name = "PokeContextMenuItem";
            this.PokeContextMenuItem.ShortcutKeyDisplayString = "Ctrl+P";
            this.PokeContextMenuItem.Size = new System.Drawing.Size(244, 22);
            this.PokeContextMenuItem.Text = "&Poke";
            this.PokeContextMenuItem.Click += new System.EventHandler(this.PokeAddressMenuItem_Click);
            // 
            // FreezeContextMenuItem
            // 
            this.FreezeContextMenuItem.Name = "FreezeContextMenuItem";
            this.FreezeContextMenuItem.ShortcutKeyDisplayString = "Ctrl+F";
            this.FreezeContextMenuItem.Size = new System.Drawing.Size(244, 22);
            this.FreezeContextMenuItem.Text = "&Freeze";
            this.FreezeContextMenuItem.Click += new System.EventHandler(this.FreezeAddressMenuItem_Click);
            // 
            // UnfreezeAllContextMenuItem
            // 
			this.UnfreezeAllContextMenuItem.Name = "UnfreezeAllContextMenuItem";
            this.UnfreezeAllContextMenuItem.Size = new System.Drawing.Size(244, 22);
            this.UnfreezeAllContextMenuItem.Text = "Unfreeze &All";
            this.UnfreezeAllContextMenuItem.Click += new System.EventHandler(this.UnfreezeAllContextMenuItem_Click);
            // 
            // ViewInHexEditorContextMenuItem
            // 
            this.ViewInHexEditorContextMenuItem.Name = "ViewInHexEditorContextMenuItem";
            this.ViewInHexEditorContextMenuItem.Size = new System.Drawing.Size(244, 22);
            this.ViewInHexEditorContextMenuItem.Text = "View in Hex Editor";
            this.ViewInHexEditorContextMenuItem.Click += new System.EventHandler(this.ViewInHexEditorContextMenuItem_Click);
            // 
            // Separator4
            // 
            this.Separator4.Name = "Separator4";
            this.Separator4.Size = new System.Drawing.Size(241, 6);
            // 
            // ReadBreakpointContextMenuItem
            // 
            this.ReadBreakpointContextMenuItem.Name = "ReadBreakpointContextMenuItem";
            this.ReadBreakpointContextMenuItem.Size = new System.Drawing.Size(244, 22);
            this.ReadBreakpointContextMenuItem.Text = "Set Read Breakpoint";
            this.ReadBreakpointContextMenuItem.Click += new System.EventHandler(this.ReadBreakpointContextMenuItem_Click);
            // 
            // WriteBreakpointContextMenuItem
            // 
            this.WriteBreakpointContextMenuItem.Name = "WriteBreakpointContextMenuItem";
            this.WriteBreakpointContextMenuItem.Size = new System.Drawing.Size(244, 22);
            this.WriteBreakpointContextMenuItem.Text = "Set Write Breakpoint";
            this.WriteBreakpointContextMenuItem.Click += new System.EventHandler(this.WriteBreakpointContextMenuItem_Click);
            // 
            // Separator6
            // 
            this.Separator6.Name = "Separator6";
            this.Separator6.Size = new System.Drawing.Size(241, 6);
            // 
            // InsertSeperatorContextMenuItem
            // 
            this.InsertSeperatorContextMenuItem.Name = "InsertSeperatorContextMenuItem";
            this.InsertSeperatorContextMenuItem.ShortcutKeyDisplayString = "Ctrl+I";
            this.InsertSeperatorContextMenuItem.Size = new System.Drawing.Size(244, 22);
            this.InsertSeperatorContextMenuItem.Text = "&Insert Separator";
            this.InsertSeperatorContextMenuItem.Click += new System.EventHandler(this.InsertSeparatorMenuItem_Click);
            // 
            // MoveUpContextMenuItem
            // 
            this.MoveUpContextMenuItem.Name = "MoveUpContextMenuItem";
            this.MoveUpContextMenuItem.ShortcutKeyDisplayString = "Ctrl+Up";
            this.MoveUpContextMenuItem.Size = new System.Drawing.Size(244, 22);
            this.MoveUpContextMenuItem.Text = "Move &Up";
            this.MoveUpContextMenuItem.Click += new System.EventHandler(this.MoveUpMenuItem_Click);
            // 
            // MoveDownContextMenuItem
            // 
            this.MoveDownContextMenuItem.Name = "MoveDownContextMenuItem";
            this.MoveDownContextMenuItem.ShortcutKeyDisplayString = "Ctrl+Down";
            this.MoveDownContextMenuItem.Size = new System.Drawing.Size(244, 22);
            this.MoveDownContextMenuItem.Text = "Move &Down";
            this.MoveDownContextMenuItem.Click += new System.EventHandler(this.MoveDownMenuItem_Click);
            // 
            // MoveTopContextMenuItem
            // 
            this.MoveTopContextMenuItem.Name = "MoveTopContextMenuItem";
            this.MoveTopContextMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Up)));
            this.MoveTopContextMenuItem.Size = new System.Drawing.Size(244, 22);
            this.MoveTopContextMenuItem.Text = "Move &Top";
            this.MoveTopContextMenuItem.Click += new System.EventHandler(this.MoveTopMenuItem_Click);
            // 
            // MoveBottomContextMenuItem
            // 
            this.MoveBottomContextMenuItem.Name = "MoveBottomContextMenuItem";
            this.MoveBottomContextMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Down)));
            this.MoveBottomContextMenuItem.Size = new System.Drawing.Size(244, 22);
            this.MoveBottomContextMenuItem.Text = "Move &Bottom";
            this.MoveBottomContextMenuItem.Click += new System.EventHandler(this.MoveBottomMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ErrorIconButton,
            this.MessageLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 356);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.TabIndex = 8;
            // 
            // ErrorIconButton
            // 
            this.ErrorIconButton.BackColor = System.Drawing.Color.NavajoWhite;
            this.ErrorIconButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ErrorIconButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ErrorIconButton.Name = "ErrorIconButton";
            this.ErrorIconButton.Size = new System.Drawing.Size(23, 20);
            this.ErrorIconButton.Text = "Warning! Out of Range Addresses in list, click to remove them";
            this.ErrorIconButton.Click += new System.EventHandler(this.ErrorIconButton_Click);
            // 
            // MessageLabel
            // 
            this.MessageLabel.Name = "MessageLabel";
            this.MessageLabel.Size = new System.Drawing.Size(31, 17);
            this.MessageLabel.Text = "        ";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripButton,
            this.openToolStripButton,
            this.saveToolStripButton,
            this.toolStripSeparator,
            this.newWatchToolStripButton,
            this.editWatchToolStripButton,
            this.cutToolStripButton,
            this.clearChangeCountsToolStripButton,
            this.duplicateWatchToolStripButton,
            this.PokeAddressToolBarItem,
            this.FreezeAddressToolBarItem,
            this.seperatorToolStripButton,
            this.toolStripSeparator6,
            this.moveUpToolStripButton,
            this.moveDownToolStripButton,
            this.toolStripSeparator5});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.TabStop = true;
            // 
            // newToolStripButton
            // 
            this.newToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newToolStripButton.Name = "newToolStripButton";
            this.newToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.newToolStripButton.Text = "&New";
            this.newToolStripButton.Click += new System.EventHandler(this.NewListMenuItem_Click);
            // 
            // openToolStripButton
            // 
            this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripButton.Name = "openToolStripButton";
            this.openToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.openToolStripButton.Text = "&Open";
            this.openToolStripButton.Click += new System.EventHandler(this.OpenMenuItem_Click);
            // 
            // saveToolStripButton
            // 
            this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripButton.Name = "saveToolStripButton";
            this.saveToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.saveToolStripButton.Text = "&Save";
            this.saveToolStripButton.Click += new System.EventHandler(this.SaveMenuItem_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // newWatchToolStripButton
            // 
            this.newWatchToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newWatchToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newWatchToolStripButton.Name = "newWatchToolStripButton";
            this.newWatchToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.newWatchToolStripButton.Text = "New Watch";
            this.newWatchToolStripButton.ToolTipText = "New Watch";
            this.newWatchToolStripButton.Click += new System.EventHandler(this.NewWatchMenuItem_Click);
            // 
            // editWatchToolStripButton
            // 
            this.editWatchToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.editWatchToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editWatchToolStripButton.Name = "editWatchToolStripButton";
            this.editWatchToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.editWatchToolStripButton.Text = "Edit Watch";
            this.editWatchToolStripButton.Click += new System.EventHandler(this.EditWatchMenuItem_Click);
            // 
            // cutToolStripButton
            // 
            this.cutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cutToolStripButton.Name = "cutToolStripButton";
            this.cutToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.cutToolStripButton.Text = "C&ut";
            this.cutToolStripButton.ToolTipText = "Remove Watch";
            this.cutToolStripButton.Click += new System.EventHandler(this.RemoveWatchMenuItem_Click);
            // 
            // clearChangeCountsToolStripButton
            // 
            this.clearChangeCountsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.clearChangeCountsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearChangeCountsToolStripButton.Name = "clearChangeCountsToolStripButton";
            this.clearChangeCountsToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.clearChangeCountsToolStripButton.Text = "C";
            this.clearChangeCountsToolStripButton.ToolTipText = "Clear Change Counts";
            this.clearChangeCountsToolStripButton.Click += new System.EventHandler(this.ClearChangeCountsMenuItem_Click);
            // 
            // duplicateWatchToolStripButton
            // 
            this.duplicateWatchToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.duplicateWatchToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.duplicateWatchToolStripButton.Name = "duplicateWatchToolStripButton";
            this.duplicateWatchToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.duplicateWatchToolStripButton.Text = "Duplicate Watch";
            this.duplicateWatchToolStripButton.Click += new System.EventHandler(this.DuplicateWatchMenuItem_Click);
            // 
            // PokeAddressToolBarItem
            // 
            this.PokeAddressToolBarItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.PokeAddressToolBarItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.PokeAddressToolBarItem.Name = "PokeAddressToolBarItem";
            this.PokeAddressToolBarItem.Size = new System.Drawing.Size(23, 22);
            this.PokeAddressToolBarItem.Text = "toolStripButton2";
            this.PokeAddressToolBarItem.ToolTipText = "Poke address";
            this.PokeAddressToolBarItem.Click += new System.EventHandler(this.PokeAddressMenuItem_Click);
            // 
            // FreezeAddressToolBarItem
            // 
            this.FreezeAddressToolBarItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.FreezeAddressToolBarItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.FreezeAddressToolBarItem.Name = "FreezeAddressToolBarItem";
            this.FreezeAddressToolBarItem.Size = new System.Drawing.Size(23, 22);
            this.FreezeAddressToolBarItem.Text = "Freeze Address";
            this.FreezeAddressToolBarItem.Click += new System.EventHandler(this.FreezeAddressMenuItem_Click);
            // 
            // seperatorToolStripButton
            // 
            this.seperatorToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.seperatorToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.seperatorToolStripButton.Name = "seperatorToolStripButton";
            this.seperatorToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.seperatorToolStripButton.Text = "-";
            this.seperatorToolStripButton.ToolTipText = "Insert Separator";
            this.seperatorToolStripButton.Click += new System.EventHandler(this.InsertSeparatorMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // moveUpToolStripButton
            // 
            this.moveUpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.moveUpToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.moveUpToolStripButton.Name = "moveUpToolStripButton";
            this.moveUpToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.moveUpToolStripButton.Text = "Move Up";
            this.moveUpToolStripButton.Click += new System.EventHandler(this.MoveUpMenuItem_Click);
            // 
            // moveDownToolStripButton
            // 
            this.moveDownToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.moveDownToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.moveDownToolStripButton.Name = "moveDownToolStripButton";
            this.moveDownToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.moveDownToolStripButton.Text = "Move Down";
            this.moveDownToolStripButton.Click += new System.EventHandler(this.MoveDownMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
			// 
            // RamWatchMenu
            // 
            this.RamWatchMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileSubMenu,
            this.WatchesSubMenu,
            this.OptionsSubMenu});
            this.RamWatchMenu.Name = "RamWatchMenu";
            this.RamWatchMenu.TabIndex = 3;
            this.RamWatchMenu.Text = "menuStrip1";
            // 
            // FileSubMenu
            // 
            this.FileSubMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewListMenuItem,
            this.OpenMenuItem,
            this.SaveMenuItem,
            this.SaveAsMenuItem,
            this.AppendMenuItem,
            this.RecentSubMenu,
            this.toolStripSeparator1,
            this.ExitMenuItem});
            this.FileSubMenu.Name = "FileSubMenu";
            this.FileSubMenu.Size = new System.Drawing.Size(42, 20);
            this.FileSubMenu.Text = "&Files";
            this.FileSubMenu.DropDownOpened += new System.EventHandler(this.FileSubMenu_DropDownOpened);
            // 
            // NewListMenuItem
            // 
            this.NewListMenuItem.Name = "NewListMenuItem";
            this.NewListMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.NewListMenuItem.Size = new System.Drawing.Size(195, 22);
            this.NewListMenuItem.Text = "&New List";
            this.NewListMenuItem.Click += new System.EventHandler(this.NewListMenuItem_Click);
            // 
            // OpenMenuItem
            // 
            this.OpenMenuItem.Name = "OpenMenuItem";
            this.OpenMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.OpenMenuItem.Size = new System.Drawing.Size(195, 22);
            this.OpenMenuItem.Text = "&Open...";
            this.OpenMenuItem.Click += new System.EventHandler(this.OpenMenuItem_Click);
            // 
            // SaveMenuItem
            // 
            this.SaveMenuItem.Name = "SaveMenuItem";
            this.SaveMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.SaveMenuItem.Size = new System.Drawing.Size(195, 22);
            this.SaveMenuItem.Text = "&Save";
            this.SaveMenuItem.Click += new System.EventHandler(this.SaveMenuItem_Click);
            // 
            // SaveAsMenuItem
            // 
            this.SaveAsMenuItem.Name = "SaveAsMenuItem";
            this.SaveAsMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.SaveAsMenuItem.Size = new System.Drawing.Size(195, 22);
            this.SaveAsMenuItem.Text = "Save &As...";
            this.SaveAsMenuItem.Click += new System.EventHandler(this.SaveAsMenuItem_Click);
            // 
            // AppendMenuItem
            // 
            this.AppendMenuItem.Name = "AppendMenuItem";
            this.AppendMenuItem.Size = new System.Drawing.Size(195, 22);
            this.AppendMenuItem.Text = "A&ppend File...";
            this.AppendMenuItem.Click += new System.EventHandler(this.OpenMenuItem_Click);
            // 
            // RecentSubMenu
            // 
            this.RecentSubMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noneToolStripMenuItem});
            this.RecentSubMenu.Name = "RecentSubMenu";
            this.RecentSubMenu.Size = new System.Drawing.Size(195, 22);
            this.RecentSubMenu.Text = "Recent";
            this.RecentSubMenu.DropDownOpened += new System.EventHandler(this.RecentSubMenu_DropDownOpened);
            // 
            // noneToolStripMenuItem
            // 
            this.noneToolStripMenuItem.Name = "noneToolStripMenuItem";
            this.noneToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.noneToolStripMenuItem.Text = "None";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(192, 6);
            // 
            // ExitMenuItem
            // 
            this.ExitMenuItem.Name = "ExitMenuItem";
            this.ExitMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.ExitMenuItem.Size = new System.Drawing.Size(195, 22);
            this.ExitMenuItem.Text = "&Close";
            this.ExitMenuItem.Click += new System.EventHandler(this.ExitMenuItem_Click);
            // 
            // WatchesSubMenu
            // 
            this.WatchesSubMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MemoryDomainsSubMenu,
            this.toolStripSeparator8,
            this.NewWatchMenuItem,
            this.EditWatchMenuItem,
            this.RemoveWatchMenuItem,
            this.DuplicateWatchMenuItem,
            this.PokeAddressMenuItem,
            this.FreezeAddressMenuItem,
            this.InsertSeparatorMenuItem,
            this.ClearChangeCountsMenuItem,
            this.toolStripSeparator3,
            this.MoveUpMenuItem,
            this.MoveDownMenuItem,
            this.MoveTopMenuItem,
            this.MoveBottomMenuItem,
            this.SelectAllMenuItem});
            this.WatchesSubMenu.Name = "WatchesSubMenu";
            this.WatchesSubMenu.Size = new System.Drawing.Size(64, 20);
            this.WatchesSubMenu.Text = "&Watches";
            this.WatchesSubMenu.DropDownOpened += new System.EventHandler(this.WatchesSubMenu_DropDownOpened);
            // 
            // MemoryDomainsSubMenu
            // 
            this.MemoryDomainsSubMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Separator2});
            this.MemoryDomainsSubMenu.Name = "MemoryDomainsSubMenu";
            this.MemoryDomainsSubMenu.Size = new System.Drawing.Size(244, 22);
            this.MemoryDomainsSubMenu.Text = "Default Domain";
            this.MemoryDomainsSubMenu.DropDownOpened += new System.EventHandler(this.MemoryDomainsSubMenu_DropDownOpened);
            // 
            // Separator2
            // 
            this.Separator2.Name = "Separator2";
            this.Separator2.Size = new System.Drawing.Size(57, 6);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(241, 6);
            // 
            // NewWatchMenuItem
            // 
            this.NewWatchMenuItem.Name = "NewWatchMenuItem";
            this.NewWatchMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.NewWatchMenuItem.Size = new System.Drawing.Size(244, 22);
            this.NewWatchMenuItem.Text = "&New Watch";
            this.NewWatchMenuItem.Click += new System.EventHandler(this.NewWatchMenuItem_Click);
            // 
            // EditWatchMenuItem
            // 
            this.EditWatchMenuItem.Name = "EditWatchMenuItem";
            this.EditWatchMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.EditWatchMenuItem.Size = new System.Drawing.Size(244, 22);
            this.EditWatchMenuItem.Text = "&Edit Watch";
            this.EditWatchMenuItem.Click += new System.EventHandler(this.EditWatchMenuItem_Click);
            // 
            // RemoveWatchMenuItem
            // 
            this.RemoveWatchMenuItem.Name = "RemoveWatchMenuItem";
            this.RemoveWatchMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.RemoveWatchMenuItem.Size = new System.Drawing.Size(244, 22);
            this.RemoveWatchMenuItem.Text = "&Remove Watch";
            this.RemoveWatchMenuItem.Click += new System.EventHandler(this.RemoveWatchMenuItem_Click);
            // 
            // DuplicateWatchMenuItem
            // 
            this.DuplicateWatchMenuItem.Name = "DuplicateWatchMenuItem";
            this.DuplicateWatchMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.DuplicateWatchMenuItem.Size = new System.Drawing.Size(244, 22);
            this.DuplicateWatchMenuItem.Text = "&Duplicate Watch";
            this.DuplicateWatchMenuItem.Click += new System.EventHandler(this.DuplicateWatchMenuItem_Click);
            // 
            // PokeAddressMenuItem
            // 
            this.PokeAddressMenuItem.Name = "PokeAddressMenuItem";
            this.PokeAddressMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.PokeAddressMenuItem.Size = new System.Drawing.Size(244, 22);
            this.PokeAddressMenuItem.Text = "Poke Address";
            this.PokeAddressMenuItem.Click += new System.EventHandler(this.PokeAddressMenuItem_Click);
            // 
            // FreezeAddressMenuItem
            // 
            this.FreezeAddressMenuItem.Name = "FreezeAddressMenuItem";
            this.FreezeAddressMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.FreezeAddressMenuItem.Size = new System.Drawing.Size(244, 22);
            this.FreezeAddressMenuItem.Text = "Freeze Address";
            this.FreezeAddressMenuItem.Click += new System.EventHandler(this.FreezeAddressMenuItem_Click);
            // 
            // InsertSeparatorMenuItem
            // 
            this.InsertSeparatorMenuItem.Name = "InsertSeparatorMenuItem";
            this.InsertSeparatorMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.InsertSeparatorMenuItem.Size = new System.Drawing.Size(244, 22);
            this.InsertSeparatorMenuItem.Text = "Insert Separator";
            this.InsertSeparatorMenuItem.Click += new System.EventHandler(this.InsertSeparatorMenuItem_Click);
            // 
            // ClearChangeCountsMenuItem
            // 
            this.ClearChangeCountsMenuItem.Name = "ClearChangeCountsMenuItem";
            this.ClearChangeCountsMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
            this.ClearChangeCountsMenuItem.Size = new System.Drawing.Size(244, 22);
            this.ClearChangeCountsMenuItem.Text = "&Clear Change Counts";
            this.ClearChangeCountsMenuItem.Click += new System.EventHandler(this.ClearChangeCountsMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(241, 6);
            // 
            // MoveUpMenuItem
            // 
            this.MoveUpMenuItem.Name = "MoveUpMenuItem";
            this.MoveUpMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Up)));
            this.MoveUpMenuItem.Size = new System.Drawing.Size(244, 22);
            this.MoveUpMenuItem.Text = "Move &Up";
            this.MoveUpMenuItem.Click += new System.EventHandler(this.MoveUpMenuItem_Click);
            // 
            // MoveDownMenuItem
            // 
            this.MoveDownMenuItem.Name = "MoveDownMenuItem";
            this.MoveDownMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Down)));
            this.MoveDownMenuItem.Size = new System.Drawing.Size(244, 22);
            this.MoveDownMenuItem.Text = "Move &Down";
            this.MoveDownMenuItem.Click += new System.EventHandler(this.MoveDownMenuItem_Click);
            // 
            // MoveTopMenuItem
            // 
            this.MoveTopMenuItem.Name = "MoveTopMenuItem";
            this.MoveTopMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Up)));
            this.MoveTopMenuItem.Size = new System.Drawing.Size(244, 22);
            this.MoveTopMenuItem.Text = "Move &Top";
            this.MoveTopMenuItem.Click += new System.EventHandler(this.MoveTopMenuItem_Click);
            // 
            // MoveBottomMenuItem
            // 
            this.MoveBottomMenuItem.Name = "MoveBottomMenuItem";
            this.MoveBottomMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Down)));
            this.MoveBottomMenuItem.Size = new System.Drawing.Size(244, 22);
            this.MoveBottomMenuItem.Text = "Move &Bottom";
            this.MoveBottomMenuItem.Click += new System.EventHandler(this.MoveBottomMenuItem_Click);
            // 
            // SelectAllMenuItem
            // 
            this.SelectAllMenuItem.Name = "SelectAllMenuItem";
            this.SelectAllMenuItem.ShortcutKeyDisplayString = "Ctrl+A";
            this.SelectAllMenuItem.Size = new System.Drawing.Size(244, 22);
            this.SelectAllMenuItem.Text = "Select &All";
            this.SelectAllMenuItem.Click += new System.EventHandler(this.SelectAllMenuItem_Click);
			// 
            // OptionsSubMenu
            // 
            this.OptionsSubMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DefinePreviousValueSubMenu,
            this.WatchesOnScreenMenuItem,
            this.SaveWindowPositionMenuItem,
            this.AlwaysOnTopMenuItem,
            this.FloatingWindowMenuItem,
            this.toolStripSeparator7,
            this.RestoreWindowSizeMenuItem});
            this.OptionsSubMenu.Name = "OptionsSubMenu";
            this.OptionsSubMenu.Size = new System.Drawing.Size(61, 20);
            this.OptionsSubMenu.Text = "&Options";
            this.OptionsSubMenu.DropDownOpened += new System.EventHandler(this.OptionsSubMenu_DropDownOpened);
            // 
            // DefinePreviousValueSubMenu
            // 
            this.DefinePreviousValueSubMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PreviousFrameMenuItem,
            this.LastChangeMenuItem,
            this.OriginalMenuItem});
            this.DefinePreviousValueSubMenu.Name = "DefinePreviousValueSubMenu";
            this.DefinePreviousValueSubMenu.Size = new System.Drawing.Size(217, 22);
            this.DefinePreviousValueSubMenu.Text = "Define Previous Value";
            this.DefinePreviousValueSubMenu.DropDownOpened += new System.EventHandler(this.DefinePreviousValueSubMenu_DropDownOpened);
            // 
            // PreviousFrameMenuItem
            // 
            this.PreviousFrameMenuItem.Name = "PreviousFrameMenuItem";
            this.PreviousFrameMenuItem.Size = new System.Drawing.Size(155, 22);
            this.PreviousFrameMenuItem.Text = "Previous Frame";
            this.PreviousFrameMenuItem.Click += new System.EventHandler(this.PreviousFrameMenuItem_Click);
            // 
            // LastChangeMenuItem
            // 
            this.LastChangeMenuItem.Name = "LastChangeMenuItem";
            this.LastChangeMenuItem.Size = new System.Drawing.Size(155, 22);
            this.LastChangeMenuItem.Text = "Last Change";
            this.LastChangeMenuItem.Click += new System.EventHandler(this.LastChangeMenuItem_Click);
            // 
            // OriginalMenuItem
            // 
            this.OriginalMenuItem.Name = "OriginalMenuItem";
            this.OriginalMenuItem.Size = new System.Drawing.Size(155, 22);
            this.OriginalMenuItem.Text = "&Original";
            this.OriginalMenuItem.Click += new System.EventHandler(this.OriginalMenuItem_Click);
            // 
            // WatchesOnScreenMenuItem
            // 
            this.WatchesOnScreenMenuItem.Name = "WatchesOnScreenMenuItem";
            this.WatchesOnScreenMenuItem.Size = new System.Drawing.Size(217, 22);
            this.WatchesOnScreenMenuItem.Text = "Display Watches On Screen";
            this.WatchesOnScreenMenuItem.Click += new System.EventHandler(this.WatchesOnScreenMenuItem_Click);
            // 
            // SaveWindowPositionMenuItem
            // 
            this.SaveWindowPositionMenuItem.Name = "SaveWindowPositionMenuItem";
            this.SaveWindowPositionMenuItem.Size = new System.Drawing.Size(217, 22);
            this.SaveWindowPositionMenuItem.Text = "Save Window Position";
            this.SaveWindowPositionMenuItem.Click += new System.EventHandler(this.SaveWindowPositionMenuItem_Click);
            // 
            // AlwaysOnTopMenuItem
            // 
            this.AlwaysOnTopMenuItem.Name = "AlwaysOnTopMenuItem";
            this.AlwaysOnTopMenuItem.Size = new System.Drawing.Size(217, 22);
            this.AlwaysOnTopMenuItem.Text = "&Always On Top";
            this.AlwaysOnTopMenuItem.Click += new System.EventHandler(this.AlwaysOnTopMenuItem_Click);
            // 
            // FloatingWindowMenuItem
            // 
            this.FloatingWindowMenuItem.Name = "FloatingWindowMenuItem";
            this.FloatingWindowMenuItem.Size = new System.Drawing.Size(217, 22);
            this.FloatingWindowMenuItem.Text = "&Floating Window";
            this.FloatingWindowMenuItem.Click += new System.EventHandler(this.FloatingWindowMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(214, 6);
            // 
            // RestoreWindowSizeMenuItem
            // 
            this.RestoreWindowSizeMenuItem.Name = "RestoreWindowSizeMenuItem";
            this.RestoreWindowSizeMenuItem.Size = new System.Drawing.Size(217, 22);
            this.RestoreWindowSizeMenuItem.Text = "Restore Default Settings";
            this.RestoreWindowSizeMenuItem.Click += new System.EventHandler(this.RestoreDefaultsMenuItem_Click);
            // 
            // WatchListView
            // 
			this.WatchListView.CellWidthPadding = 3;
            this.WatchListView.AllowColumnResize = true;
            this.WatchListView.AllowColumnReorder = true;
            this.WatchListView.FullRowSelect = true;
            this.WatchListView.MultiSelect = true;
            this.WatchListView.AllowColumnReorder = true;
            this.WatchListView.AllowDrop = true;
            this.WatchListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WatchListView.ContextMenuStrip = this.ListViewContextMenu;
            this.WatchListView.FullRowSelect = true;
            this.WatchListView.GridLines = true;
            this.WatchListView.Location = new System.Drawing.Point(16, 76);
            this.WatchListView.Name = "WatchListView";
            this.WatchListView.Size = new System.Drawing.Size(363, 281);
            this.WatchListView.TabIndex = 2;
            this.WatchListView.ColumnClick += new BizHawk.Client.EmuHawk.InputRoll.ColumnClickEventHandler(this.WatchListView_ColumnClick);
            this.WatchListView.SelectedIndexChanged += new System.EventHandler(this.WatchListView_SelectedIndexChanged);
            this.WatchListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.RamWatch_DragDrop);
            this.WatchListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.DragEnterWrapper);
            this.WatchListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WatchListView_KeyDown);
            this.WatchListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.WatchListView_MouseDoubleClick);
            // 
            // RamWatch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 378);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.WatchCountLabel);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.RamWatchMenu);
            this.Controls.Add(this.WatchListView);
            this.Name = "RamWatch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = " RAM Watch";
            this.Load += new System.EventHandler(this.RamWatch_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.RamWatch_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.DragEnterWrapper);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WatchListView_KeyDown);
            this.ListViewContextMenu.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.RamWatchMenu.ResumeLayout(false);
            this.RamWatchMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private InputRoll WatchListView;
		private MenuStripEx RamWatchMenu;
		private System.Windows.Forms.ToolStripMenuItem FileSubMenu;
		private System.Windows.Forms.ToolStripMenuItem NewListMenuItem;
		private System.Windows.Forms.ToolStripMenuItem OpenMenuItem;
		private System.Windows.Forms.ToolStripMenuItem SaveMenuItem;
		private System.Windows.Forms.ToolStripMenuItem SaveAsMenuItem;
		private System.Windows.Forms.ToolStripMenuItem AppendMenuItem;
		private System.Windows.Forms.ToolStripMenuItem RecentSubMenu;
        private System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem ExitMenuItem;
		private System.Windows.Forms.ToolStripMenuItem WatchesSubMenu;
		private System.Windows.Forms.ToolStripMenuItem MemoryDomainsSubMenu;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
		private System.Windows.Forms.ToolStripMenuItem NewWatchMenuItem;
		private System.Windows.Forms.ToolStripMenuItem EditWatchMenuItem;
		private System.Windows.Forms.ToolStripMenuItem RemoveWatchMenuItem;
		private System.Windows.Forms.ToolStripMenuItem DuplicateWatchMenuItem;
		private System.Windows.Forms.ToolStripMenuItem PokeAddressMenuItem;
		private System.Windows.Forms.ToolStripMenuItem FreezeAddressMenuItem;
		private System.Windows.Forms.ToolStripMenuItem InsertSeparatorMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ClearChangeCountsMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem MoveUpMenuItem;
		private System.Windows.Forms.ToolStripMenuItem MoveDownMenuItem;
		private System.Windows.Forms.ToolStripMenuItem SelectAllMenuItem;
		private System.Windows.Forms.ToolStripMenuItem OptionsSubMenu;
		private System.Windows.Forms.ToolStripMenuItem DefinePreviousValueSubMenu;
		private System.Windows.Forms.ToolStripMenuItem PreviousFrameMenuItem;
		private System.Windows.Forms.ToolStripMenuItem LastChangeMenuItem;
		private System.Windows.Forms.ToolStripMenuItem WatchesOnScreenMenuItem;
		private System.Windows.Forms.ToolStripMenuItem SaveWindowPositionMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
		private System.Windows.Forms.ToolStripMenuItem RestoreWindowSizeMenuItem;
		private ToolStripEx toolStrip1;
		private System.Windows.Forms.ToolStripButton newToolStripButton;
		private System.Windows.Forms.ToolStripButton openToolStripButton;
		private System.Windows.Forms.ToolStripButton saveToolStripButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
		private System.Windows.Forms.ToolStripButton newWatchToolStripButton;
		private System.Windows.Forms.ToolStripButton editWatchToolStripButton;
		private System.Windows.Forms.ToolStripButton cutToolStripButton;
		private System.Windows.Forms.ToolStripButton clearChangeCountsToolStripButton;
		private System.Windows.Forms.ToolStripButton duplicateWatchToolStripButton;
		private System.Windows.Forms.ToolStripButton PokeAddressToolBarItem;
		private System.Windows.Forms.ToolStripButton FreezeAddressToolBarItem;
		private System.Windows.Forms.ToolStripButton seperatorToolStripButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripButton moveUpToolStripButton;
		private System.Windows.Forms.ToolStripButton moveDownToolStripButton;
		private BizHawk.WinForms.Controls.LocLabelEx WatchCountLabel;
		private System.Windows.Forms.ToolStripSeparator Separator2;
		private System.Windows.Forms.ToolStripMenuItem OriginalMenuItem;
		private System.Windows.Forms.ContextMenuStrip ListViewContextMenu;
		private System.Windows.Forms.ToolStripMenuItem EditContextMenuItem;
		private System.Windows.Forms.ToolStripMenuItem RemoveContextMenuItem;
		private System.Windows.Forms.ToolStripMenuItem DuplicateContextMenuItem;
		private System.Windows.Forms.ToolStripMenuItem PokeContextMenuItem;
		private System.Windows.Forms.ToolStripMenuItem FreezeContextMenuItem;
		private System.Windows.Forms.ToolStripMenuItem UnfreezeAllContextMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ViewInHexEditorContextMenuItem;
		private System.Windows.Forms.ToolStripSeparator Separator6;
		private System.Windows.Forms.ToolStripMenuItem InsertSeperatorContextMenuItem;
		private System.Windows.Forms.ToolStripMenuItem MoveUpContextMenuItem;
		private System.Windows.Forms.ToolStripMenuItem MoveDownContextMenuItem;
		private System.Windows.Forms.ToolStripMenuItem AlwaysOnTopMenuItem;
		private System.Windows.Forms.ToolStripMenuItem FloatingWindowMenuItem;
		private StatusStripEx statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel MessageLabel;
		private System.Windows.Forms.ToolStripButton ErrorIconButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripSeparator Separator4;
		private System.Windows.Forms.ToolStripMenuItem ReadBreakpointContextMenuItem;
		private System.Windows.Forms.ToolStripMenuItem WriteBreakpointContextMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MoveTopMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MoveBottomMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MoveTopContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MoveBottomContextMenuItem;
    }
}
