using System;
using System.Runtime.InteropServices;
using BizHawk.Common;
using BizHawk.Emulation.Common;
using BizHawk.Emulation.Cores.Waterbox;
using BizHawk.Emulation.DiscSystem;

namespace BizHawk.Emulation.Cores.Sony.PS2
{
	[Core("DobieStation", "PSI", true, false, "fa33778b056aa32", "https://github.com/PSI-Rockin/DobieStation", false)]
	public unsafe class DobieStation : WaterboxCore, ISettable<object, DobieStation.DobieSyncSettings>
	{
		private readonly LibDobieStation _core;
		[CoreConstructor("PS2")]
		public DobieStation(CoreLoadParameters<object, DobieSyncSettings> lp)
			:base(lp.Comm, new Configuration
			{
				MaxWidth = 640,
				MaxHeight = 480,
				DefaultWidth = 640,
				DefaultHeight = 480,
				DefaultFpsNumerator = 294912000,
				DefaultFpsDenominator = 4920115,
				MaxSamples = 1024,
				SystemId = "PS2"
			})
		{
			if (lp.Discs.Count != 1)
			{
				throw new InvalidOperationException("Must load a CD or DVD with PS2 core!");
			}
			ControllerDefinition = DualShock;
			_syncSettings = lp.SyncSettings ?? new DobieSyncSettings();
			_syncSettingsActual = lp.SyncSettings ?? new DobieSyncSettings();

			_disc = new DiscSectorReader(lp.Discs[0].DiscData);
			_cdCallback = ReadCd;
			_core = PreInit<LibDobieStation>(new WaterboxOptions
			{
				Filename = "dobie.wbx",
				SbrkHeapSizeKB = 4 * 1024,
				SealedHeapSizeKB = 4 * 1024,
				InvisibleHeapSizeKB = 4 * 1024,
				PlainHeapSizeKB = 256,
				MmapHeapSizeKB = 2 * 1024 * 1024,
				SkipCoreConsistencyCheck = lp.Comm.CorePreferences.HasFlag(CoreComm.CorePreferencesFlags.WaterboxCoreConsistencyCheck),
				SkipMemoryConsistencyCheck = lp.Comm.CorePreferences.HasFlag(CoreComm.CorePreferencesFlags.WaterboxMemoryConsistencyCheck),
			}, new[] { _cdCallback });

			var bios = lp.Comm.CoreFileProvider.GetFirmware("PS2", "BIOS", true);
			_exe.AddReadonlyFile(new byte[0x840000], "MEMCARD0");

			var worked = _core.Initialize(bios,
				(ulong)(lp.Discs[0].DiscData.Session1.Tracks[2].LBA - lp.Discs[0].DiscData.Session1.Tracks[1].LBA) * 2048,
				_cdCallback,
				_syncSettingsActual.GetNativeSettings()
			);

			if (!worked)
			{
				throw new InvalidOperationException("Initialize failed!");
			}

			_exe.RemoveReadonlyFile("MEMCARD0");

			PostInit();

			_resampler = new SpeexResampler((SpeexResampler.Quality)6, 480, 441, 48000, 44100, null, this);
			_serviceProvider.Register<ISoundProvider>(_resampler);
		}

		private SpeexResampler _resampler;

		public override void Dispose()
		{
			base.Dispose();
			if (_resampler != null)
			{
				_resampler.Dispose();
				_resampler = null;
			}
		}

		private readonly LibDobieStation.CdCallback _cdCallback;
		private DiscSectorReader _disc;
		private void ReadCd(ulong sector, byte* dest)
		{
			var tmp = new byte[2048];
			_disc.ReadLBA_2048((int)sector, tmp, 0);
			Marshal.Copy(tmp, 0, (IntPtr)dest, 2048);
		}

		protected override LibWaterboxCore.FrameInfo FrameAdvancePrep(IController controller, bool render, bool rendersound)
		{
			var ret = new LibDobieStation.FrameInfo();
			for (int i = 0; i < DualShock.BoolButtons.Count; i++)
			{
				if (controller.IsPressed(DualShock.BoolButtons[i]))
				{
					ret.Buttons |= 1u << i;
				}
			}
			for (int i = 0; i < DualShock.Axes.Count; i++)
			{
				ret.Axes |= (uint)controller.AxisValue(DualShock.Axes[i]) << (i * 8);
			}
			return ret;
		}
		protected override void FrameAdvancePost()
		{
			// DobieStation core kicks back 0 values sometimes.  Not sure what they mean, no image produced?
			// Easiest to just fix them here.
			if (BufferWidth == 0)
				BufferWidth = 640;
			if (BufferHeight == 0)
				BufferHeight = 480;
		}

		public object GetSettings() => new object();
		public PutSettingsDirtyBits PutSettings(object o) => PutSettingsDirtyBits.None;

		private DobieSyncSettings _syncSettings;
		private readonly DobieSyncSettings _syncSettingsActual;

		public DobieSyncSettings GetSyncSettings()
		{
			return _syncSettings.Clone();
		}

		public PutSettingsDirtyBits PutSyncSettings(DobieSyncSettings o)
		{
			_syncSettings = o;
			return DobieSyncSettings.NeedsReboot(_syncSettings, _syncSettingsActual)
				? PutSettingsDirtyBits.RebootCore
				: PutSettingsDirtyBits.None;
		}

		private static readonly ControllerDefinition DualShock = new ControllerDefinition
		{
			Name = "PS2 DualShock",
			BoolButtons =
			{
				"SELECT",
				"L3",
				"R3",
				"START",
				"UP",
				"RIGHT",
				"DOWN",
				"LEFT",
				"L2",
				"R2",
				"L1",
				"R1",
				"TRIANGLE",
				"CIRCLE",
				"CROSS",
				"SQUARE",
			},
			Axes =
			{
				{ "RIGHT X", new AxisSpec(RangeExtensions.MutableRangeTo(0, 255), 128) },
				{ "RIGHT Y", new AxisSpec(RangeExtensions.MutableRangeTo(0, 255), 128) },
				{ "LEFT X", new AxisSpec(RangeExtensions.MutableRangeTo(0, 255), 128) },
				{ "LEFT Y", new AxisSpec(RangeExtensions.MutableRangeTo(0, 255), 128) },
			}
		};

		public class DobieSyncSettings
		{
			public enum CpuMode
			{
				Jit,
				Interpreter
			}

			public CpuMode EEMode { get; set;}
			public CpuMode VU0Mode { get; set; }
			public CpuMode VU1Mode { get; set; }

			public static bool NeedsReboot(DobieSyncSettings x, DobieSyncSettings y)
			{
				return !DeepEquality.DeepEquals(x, y); 
			}

			public DobieSyncSettings Clone()
			{
				return (DobieSyncSettings)MemberwiseClone();
			}

			public LibDobieStation.SyncSettings GetNativeSettings()
			{
				return new LibDobieStation.SyncSettings
				{
					EEJit = EEMode == CpuMode.Jit,
					VU0Jit = VU0Mode == CpuMode.Jit,
					VU1Jit = VU1Mode == CpuMode.Jit
				};
			}
		}
	}
}
