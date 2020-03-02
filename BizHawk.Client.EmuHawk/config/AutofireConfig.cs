﻿using System;
using System.Windows.Forms;
using BizHawk.Client.Common;

namespace BizHawk.Client.EmuHawk
{
	public partial class AutofireConfig : Form
	{
		private readonly Config _config;
		private readonly AutofireController _autoFireController;
		private readonly AutoFireStickyXorAdapter _stickyXorAdapter;

		public AutofireConfig(
			Config config,
			AutofireController autoFireController,
			AutoFireStickyXorAdapter stickyXorAdapter)
		{
			_config = config;
			_autoFireController = autoFireController;
			_stickyXorAdapter = stickyXorAdapter;
			InitializeComponent();
		}

		private void AutofireConfig_Load(object sender, EventArgs e)
		{
			if (_config.AutofireOn < nudPatternOn.Minimum)
			{
				nudPatternOn.Value = nudPatternOn.Minimum;
			}
			else if (_config.AutofireOn > nudPatternOn.Maximum)
			{
				nudPatternOn.Value = nudPatternOn.Maximum;
			}
			else
			{
				nudPatternOn.Value = _config.AutofireOn;
			}

			if (_config.AutofireOff < nudPatternOff.Minimum)
			{
				nudPatternOff.Value = nudPatternOff.Minimum;
			}
			else if (_config.AutofireOff > nudPatternOff.Maximum)
			{
				nudPatternOff.Value = nudPatternOff.Maximum;
			}
			else
			{
				nudPatternOff.Value = _config.AutofireOff;
			}

			cbConsiderLag.Checked = _config.AutofireLagFrames;
		}

		private void btnDialogOK_Click(object sender, EventArgs e)
		{
			_autoFireController.On = _config.AutofireOn = (int)nudPatternOn.Value;
			_autoFireController.Off = _config.AutofireOff = (int)nudPatternOff.Value;
			_config.AutofireLagFrames = cbConsiderLag.Checked;
			_stickyXorAdapter.SetOnOffPatternFromConfig();

			Close();
		}

		private void btnDialogCancel_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
