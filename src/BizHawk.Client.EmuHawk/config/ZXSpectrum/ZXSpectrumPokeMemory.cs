﻿using System;
using System.Windows.Forms;
using BizHawk.Emulation.Cores.Computers.SinclairSpectrum;

namespace BizHawk.Client.EmuHawk
{
	public partial class ZxSpectrumPokeMemory : Form
	{
		private readonly ZXSpectrum _speccy;

		public ZxSpectrumPokeMemory(ZXSpectrum speccy)
		{
			_speccy = speccy;

			InitializeComponent();
			Icon = Properties.Resources.GameControllerIcon;
		}

		private void OkBtn_Click(object sender, EventArgs e)
		{
			var addr = (ushort)numericUpDownAddress.Value;
			var val = (byte)numericUpDownByte.Value;

			_speccy.PokeMemory(addr, val);

			DialogResult = DialogResult.OK;
			Close();
		}

		private void CancelBtn_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
