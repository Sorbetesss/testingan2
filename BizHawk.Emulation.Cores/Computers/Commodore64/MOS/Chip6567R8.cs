﻿using System.Drawing;

namespace BizHawk.Emulation.Cores.Computers.Commodore64.MOS
{
	// vic ntsc
	public static class Chip6567R8
	{
		static int cycles = 65;
		static int scanwidth = cycles * 8;
		static int lines = 263;
		static int vblankstart = 0x00D % lines;
		static int vblankend = 0x018 % lines;
		static int hblankoffset = 20;
		static int hblankstart = (0x18C + hblankoffset) % scanwidth - 8; // -8 because the VIC repeats internal pixel cycles around 0x18C
		static int hblankend = (0x1F0 + hblankoffset) % scanwidth - 8;

		static int[] timing = Vic.TimingBuilder_XRaster(0x19C, 0x200, scanwidth, 0x18C, 8);
		static int[] fetch = Vic.TimingBuilder_Fetch(timing, 0x174);
		static int[] ba = Vic.TimingBuilder_BA(fetch);
		static int[] act = Vic.TimingBuilder_Act(timing, 0x004, 0x14C, hblankstart, hblankend);

		static int[][] pipeline = {
				timing,
				fetch,
				ba,
                act
			};

		public static Vic Create()
		{
			return new Vic(
				cycles, lines,
				pipeline,
				14318181 / 14,
				hblankstart, hblankend,
				vblankstart, vblankend
				);
		}
	}
}
