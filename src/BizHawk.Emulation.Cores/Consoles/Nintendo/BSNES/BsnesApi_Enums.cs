﻿namespace BizHawk.Emulation.Cores.Nintendo.BSNES
{
	public partial class BsnesApi
	{
		public enum SNES_MEMORY
		{
			CARTRIDGE_RAM,
			CARTRIDGE_ROM,
			SGB_ROM,

			BSX_RAM,
			BSX_PRAM,
			SUFAMI_TURBO_A_RAM,
			SUFAMI_TURBO_B_RAM,
			SA1_IRAM,
			SA1_BWRAM,

			WRAM,
			APURAM,
			VRAM,
			// OAM, // needs some work in the core probably? or we return an objects pointer
			CGRAM
		}

		public enum BSNES_INPUT_DEVICE
		{
			None = 0,
			Gamepad = 1,
			Mouse = 2,
			SuperMultitap = 3,
			Payload = 4,
			SuperScope = 5,
			Justifier = 6,
			Justifiers = 7
		}

		/// this a subset of the <see cref="BSNES_INPUT_DEVICE"/> enum with all lightgun controllers removed
		public enum BSNES_PORT1_INPUT_DEVICE
		{
			None = 0,
			Gamepad = 1,
			Mouse = 2,
			SuperMultitap = 3,
			Payload = 4
		}

		public enum ENTROPY
		{
			None,
			Low,
			High
		}

		public enum SNES_MAPPER : byte
		{
			LOROM = 0,
			HIROM = 1,
			EXLOROM = 2,
			EXHIROM = 3,
			SUPERFXROM = 4,
			SA1ROM = 5,
			SPC7110ROM = 6,
			BSCLOROM = 7,
			BSCHIROM = 8,
			BSXROM = 9,
			STROM = 10
		}

		public enum SNES_REGION : uint
		{
			NTSC = 0,
			PAL = 1
		}

		public enum REGION_OVERRIDE : uint
		{
			Auto,
			NTSC,
			PAL
		}
	}
}
