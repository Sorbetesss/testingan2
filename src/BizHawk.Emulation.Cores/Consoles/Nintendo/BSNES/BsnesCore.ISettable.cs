﻿using BizHawk.Emulation.Common;

namespace BizHawk.Emulation.Cores.Nintendo.BSNES
{
	public partial class BsnesCore : ISettable<BsnesCore.SnesSettings, BsnesCore.SnesSyncSettings>
	{
		public SnesSettings GetSettings()
		{
			return _settings.Clone();
		}

		SNES.IBSNESForGfxDebugger.SettingsObj SNES.IBSNESForGfxDebugger.GetSettings()
			=> GetSettings();

		public SnesSyncSettings GetSyncSettings()
		{
			return _syncSettings.Clone();
		}

		public PutSettingsDirtyBits PutSettings(SnesSettings o)
		{
			_settings = o;

			return PutSettingsDirtyBits.None;
		}

		void SNES.IBSNESForGfxDebugger.PutSettings(SNES.IBSNESForGfxDebugger.SettingsObj s)
			=> PutSettings((SnesSettings) s);

		public PutSettingsDirtyBits PutSyncSettings(SnesSyncSettings o)
		{
			bool ret = o.LeftPort != _syncSettings.LeftPort
				|| o.RightPort != _syncSettings.RightPort
				|| o.LimitAnalogChangeSensitivity != _syncSettings.LimitAnalogChangeSensitivity
				|| o.Entropy != _syncSettings.Entropy
				|| o.RegionOverride != _syncSettings.RegionOverride
				|| o.Hotfixes != _syncSettings.Hotfixes
				|| o.FastPPU != _syncSettings.FastPPU
				|| o.FastDSP != _syncSettings.FastDSP
				|| o.FastCoprocessors != _syncSettings.FastCoprocessors
				|| o.UseSGB2 != _syncSettings.UseSGB2;

			_syncSettings = o;
			return ret ? PutSettingsDirtyBits.RebootCore : PutSettingsDirtyBits.None;
		}

		private SnesSettings _settings;
		private SnesSyncSettings _syncSettings;

		public class SnesSettings : SNES.IBSNESForGfxDebugger.SettingsObj
		{
			public bool ShowBG1_0 { get; set; } = true;
			public bool ShowBG2_0 { get; set; } = true;
			public bool ShowBG3_0 { get; set; } = true;
			public bool ShowBG4_0 { get; set; } = true;
			public bool ShowBG1_1 { get; set; } = true;
			public bool ShowBG2_1 { get; set; } = true;
			public bool ShowBG3_1 { get; set; } = true;
			public bool ShowBG4_1 { get; set; } = true;
			public bool ShowOBJ_0 { get; set; } = true;
			public bool ShowOBJ_1 { get; set; } = true;
			public bool ShowOBJ_2 { get; set; } = true;
			public bool ShowOBJ_3 { get; set; } = true;

			public bool AlwaysDoubleSize { get; set; }
			public bool CropSGBFrame { get; set; }
			public bool NoPPUSpriteLimit { get; set; }
			public bool ShowOverscan { get; set; }
			public BsnesApi.ASPECT_RATIO_CORRECTION AspectRatioCorrection { get; set; } = BsnesApi.ASPECT_RATIO_CORRECTION.Auto;

			public SnesSettings Clone()
			{
				return (SnesSettings) MemberwiseClone();
			}
		}

		public class SnesSyncSettings
		{
			public BsnesApi.BSNES_PORT1_INPUT_DEVICE LeftPort { get; set; } = BsnesApi.BSNES_PORT1_INPUT_DEVICE.Gamepad;

			public BsnesApi.BSNES_INPUT_DEVICE RightPort { get; set; } = BsnesApi.BSNES_INPUT_DEVICE.None;

			public bool LimitAnalogChangeSensitivity { get; set; } = true;

			public BsnesApi.ENTROPY Entropy { get; set; } = BsnesApi.ENTROPY.Low;

			public BsnesApi.REGION_OVERRIDE RegionOverride { get; set; } = BsnesApi.REGION_OVERRIDE.Auto;

			public bool Hotfixes { get; set; } = true;

			public bool FastPPU { get; set; } = true;

			public bool FastDSP { get; set; } = true;

			public bool FastCoprocessors { get; set; } = true;

			public bool UseSGB2 { get; set; } = true;

			public SnesSyncSettings Clone()
			{
				return (SnesSyncSettings) MemberwiseClone();
			}
		}
	}
}
