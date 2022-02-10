using System;
using System.IO;

using BizHawk.Common;
using BizHawk.Emulation.Common;
using BizHawk.Emulation.Cores.Properties;
using BizHawk.Emulation.Cores.Waterbox;

namespace BizHawk.Emulation.Cores.Consoles.Nintendo.Ares64
{
	[PortedCore(CoreNames.Ares64, "ares team, Near", "v126", "https://ares-emulator.github.io/", isReleased: false)]
	[ServiceNotApplicable(new[] { typeof(IDriveLight), })]
	public partial class Ares64 : WaterboxCore, IRegionable
	{
		private readonly LibAres64 _core;

		[CoreConstructor(VSystemID.Raw.N64)]
		public Ares64(CoreLoadParameters<object, Ares64SyncSettings> lp)
			: base(lp.Comm, new Configuration
			{
				DefaultWidth = 640,
				DefaultHeight = 480,
				MaxWidth = 640,
				MaxHeight = 576,
				MaxSamples = 2048,
				DefaultFpsNumerator = 60000,
				DefaultFpsDenominator = 1001,
				SystemId = VSystemID.Raw.N64,
			})
		{
			_syncSettings = lp.SyncSettings ?? new();

			N64Controller = CreateControllerDefinition(_syncSettings);

			_core = PreInit<LibAres64>(new WaterboxOptions
			{
				Filename = "ares64.wbx",
				SbrkHeapSizeKB = 2 * 1024,
				SealedHeapSizeKB = 4,
				InvisibleHeapSizeKB = 6 * 1024,
				PlainHeapSizeKB = 4,
				MmapHeapSizeKB = 512 * 1024,
				SkipCoreConsistencyCheck = CoreComm.CorePreferences.HasFlag(CoreComm.CorePreferencesFlags.WaterboxCoreConsistencyCheck),
				SkipMemoryConsistencyCheck = CoreComm.CorePreferences.HasFlag(CoreComm.CorePreferencesFlags.WaterboxMemoryConsistencyCheck),
			});

			var rom = lp.Roms[0].RomData;

			Region = rom[0x3E] switch
			{
				0x44 or 0x46 or 0x49 or 0x50 or 0x53 or 0x55 or 0x58 or 0x59 => DisplayType.PAL,
				_ => DisplayType.NTSC,
			};

			var pal = Region == DisplayType.PAL;

			if (pal)
			{
				VsyncNumerator = 50;
				VsyncDenominator = 1;
			}

			var pif = Util.DecompressGzipFile(new MemoryStream(pal ? Resources.PIF_PAL_ROM.Value : Resources.PIF_NTSC_ROM.Value));

			_exe.AddReadonlyFile(pif, pal ? "pif.pal.rom" : "pif.ntsc.rom");
			_exe.AddReadonlyFile(rom, "program.rom");

			var controllers = new LibAres64.ControllerType[4]
			{
				_syncSettings.P1Controller,
				_syncSettings.P2Controller,
				_syncSettings.P3Controller,
				_syncSettings.P4Controller,
			};

			if (!_core.Init(controllers, pal))
			{
				throw new InvalidOperationException("Init returned false!");
			}

			_exe.RemoveReadonlyFile(pal ? "pif.pal.rom" : "pif.ntsc.rom");
			_exe.RemoveReadonlyFile("program.rom");

			PostInit();
			DeterministicEmulation = true;
		}

		public DisplayType Region { get; }

		public override ControllerDefinition ControllerDefinition => N64Controller;

		private ControllerDefinition N64Controller { get; }

		private static ControllerDefinition CreateControllerDefinition(Ares64SyncSettings syncSettings)
		{
			var ret = new ControllerDefinition("Nintendo 64 Controller");
			var controllerTypes = new[]
			{
				syncSettings.P1Controller,
				syncSettings.P2Controller,
				syncSettings.P3Controller,
				syncSettings.P4Controller,
			};
			for (int i = 0; i < 4; i++)
			{
				if (controllerTypes[i] != LibAres64.ControllerType.Unplugged)
				{
					ret.BoolButtons.Add($"P{i + 1} DPad U");
					ret.BoolButtons.Add($"P{i + 1} DPad D");
					ret.BoolButtons.Add($"P{i + 1} DPad L");
					ret.BoolButtons.Add($"P{i + 1} DPad R");
					ret.BoolButtons.Add($"P{i + 1} Start");
					ret.BoolButtons.Add($"P{i + 1} Z");
					ret.BoolButtons.Add($"P{i + 1} B");
					ret.BoolButtons.Add($"P{i + 1} A");
					ret.BoolButtons.Add($"P{i + 1} C Up");
					ret.BoolButtons.Add($"P{i + 1} C Down");
					ret.BoolButtons.Add($"P{i + 1} C Left");
					ret.BoolButtons.Add($"P{i + 1} C Right");
					ret.BoolButtons.Add($"P{i + 1} L");
					ret.BoolButtons.Add($"P{i + 1} R");
					ret.AddXYPair($"P{i + 1} {{0}} Axis", AxisPairOrientation.RightAndUp, (-32768).RangeTo(32767), 0);
					if (controllerTypes[i] == LibAres64.ControllerType.Rumblepak)
					{
						ret.HapticsChannels.Add($"P{i + 1} Rumble Pak");
					}
				}
			}
			ret.BoolButtons.Add("Reset");
			ret.BoolButtons.Add("Power");
			return ret.MakeImmutable();
		}

		private static LibAres64.Buttons GetButtons(IController controller, int num)
		{
			LibAres64.Buttons ret = 0;

			if (controller.IsPressed($"P{num} DPad U"))
				ret |= LibAres64.Buttons.UP;
			if (controller.IsPressed($"P{num} DPad D"))
				ret |= LibAres64.Buttons.DOWN;
			if (controller.IsPressed($"P{num} DPad L"))
				ret |= LibAres64.Buttons.LEFT;
			if (controller.IsPressed($"P{num} DPad R"))
				ret |= LibAres64.Buttons.RIGHT;
			if (controller.IsPressed($"P{num} B"))
				ret |= LibAres64.Buttons.B;
			if (controller.IsPressed($"P{num} A"))
				ret |= LibAres64.Buttons.A;
			if (controller.IsPressed($"P{num} C Up"))
				ret |= LibAres64.Buttons.C_UP;
			if (controller.IsPressed($"P{num} C Down"))
				ret |= LibAres64.Buttons.C_DOWN;
			if (controller.IsPressed($"P{num} C Left"))
				ret |= LibAres64.Buttons.C_LEFT;
			if (controller.IsPressed($"P{num} C Right"))
				ret |= LibAres64.Buttons.C_RIGHT;
			if (controller.IsPressed($"P{num} L"))
				ret |= LibAres64.Buttons.L;
			if (controller.IsPressed($"P{num} R"))
				ret |= LibAres64.Buttons.R;
			if (controller.IsPressed($"P{num} Z"))
				ret |= LibAres64.Buttons.Z;
			if (controller.IsPressed($"P{num} Start"))
				ret |= LibAres64.Buttons.START;

			return ret;
		}

		protected override LibWaterboxCore.FrameInfo FrameAdvancePrep(IController controller, bool render, bool rendersound)
		{
			return new LibAres64.FrameInfo
			{
				P1Buttons = GetButtons(controller, 1),
				P1XAxis = (short)controller.AxisValue("P1 X Axis"),
				P1YAxis = (short)controller.AxisValue("P1 Y Axis"),

				P2Buttons = GetButtons(controller, 2),
				P2XAxis = (short)controller.AxisValue("P2 X Axis"),
				P2YAxis = (short)controller.AxisValue("P2 Y Axis"),

				P3Buttons = GetButtons(controller, 3),
				P3XAxis = (short)controller.AxisValue("P3 X Axis"),
				P3YAxis = (short)controller.AxisValue("P3 Y Axis"),

				P4Buttons = GetButtons(controller, 4),
				P4XAxis = (short)controller.AxisValue("P4 X Axis"),
				P4YAxis = (short)controller.AxisValue("P4 Y Axis"),

				Reset = controller.IsPressed("Reset"),
				Power = controller.IsPressed("Power"),
			};
		}

		protected override void FrameAdvancePost()
		{
			if (BufferWidth == 0)
			{
				BufferWidth = BufferHeight == 239 ? 320 : 640;
			}
		}
	}
}
