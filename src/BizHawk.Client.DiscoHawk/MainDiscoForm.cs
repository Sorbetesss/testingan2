﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using BizHawk.Common.StringExtensions;
using BizHawk.Emulation.DiscSystem;

namespace BizHawk.Client.DiscoHawk
{
	public partial class MainDiscoForm : Form
	{
		// Release TODO:
		// An input (queue) list
		// An outputted list showing new file name
		// Progress bar should show file being converted
		// Add disc button, which puts it on the progress cue (converts it)
		public MainDiscoForm()
		{
			InitializeComponent();
			var icoStream = typeof(MainDiscoForm).Assembly.GetManifestResourceStream("BizHawk.Client.DiscoHawk.discohawk.ico");
			if (icoStream != null) Icon = new Icon(icoStream);
			else Console.WriteLine("couldn't load .ico EmbeddedResource?");
		}

		private void MainDiscoForm_Load(object sender, EventArgs e)
		{
			lvCompareTargets.Columns[0].Width = lvCompareTargets.ClientSize.Width;
		}

		private void ExitButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void lblMagicDragArea_DragDrop(object sender, DragEventArgs e)
		{
			List<string> files = ValidateDrop(e.Data);
			if (files.Count == 0) return;
			try
			{
				Cursor = Cursors.WaitCursor;
				foreach (var file in files)
				{
					var success = DiscoHawkLogic.HawkAndWriteFile(
						inputPath: file,
						errorCallback: err => MessageBox.Show(err, "Error loading disc"));
					if (!success) break;
				}

				Cursor = Cursors.Default;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Error loading disc");
				throw;
			}
		}

#if false // API has changed
		bool Dump(CueBin cueBin, string directoryTo, CueBinPrefs prefs)
		{
			ProgressReport pr = new ProgressReport();
			Thread workThread = new Thread(() =>
			{
				cueBin.Dump(directoryTo, prefs, pr);
			});

			ProgressDialog pd = new ProgressDialog(pr);
			pd.Show(this);
			this.Enabled = false;
			workThread.Start();
			for (; ; )
			{
				Application.DoEvents();
				Thread.Sleep(10);
				if (workThread.ThreadState != ThreadState.Running)
					break;
				pd.Update();
			}
			this.Enabled = true;
			pd.Dispose();
			return !pr.CancelSignal;
		}
#endif

		private void LblMagicDragArea_DragEnter(object sender, DragEventArgs e)
		{
			List<string> files = ValidateDrop(e.Data);
			e.Effect = files.Count > 0
				? DragDropEffects.Link
				: DragDropEffects.None;
		}

		private static List<string> ValidateDrop(IDataObject ido)
		{
			var ret = new List<string>();
			string[] files = (string[])ido.GetData(DataFormats.FileDrop);
			if (files == null) return new List<string>();
			foreach (string str in files)
			{
				var ext = Path.GetExtension(str) ?? string.Empty;
				if(!ext.In(".CUE", ".ISO", ".CCD", ".MDS"))
				{
					return new List<string>();
				}

				ret.Add(str);
			}

			return ret;
		}

		private void LblMp3ExtractMagicArea_DragDrop(object sender, DragEventArgs e)
		{
			var files = ValidateDrop(e.Data);
			if (files.Count == 0) return;
			foreach (var file in files)
			{
				using var disc = Disc.LoadAutomagic(file);
				var path = Path.GetDirectoryName(file);
				var filename = Path.GetFileNameWithoutExtension(file);
				AudioExtractor.Extract(disc, path, filename);
			}
		}

		private void BtnAbout_Click(object sender, EventArgs e)
		{
			new About().ShowDialog();
		}
	}
}
