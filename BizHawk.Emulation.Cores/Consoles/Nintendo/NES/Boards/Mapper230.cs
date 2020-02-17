﻿using BizHawk.Common;
using BizHawk.Common.NumberExtensions;

// http://wiki.nesdev.com/w/index.php/INES_Mapper_230
namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	public sealed class Mapper230 : NES.NESBoardBase
	{
		//TODO: soft reset back to contra = fails
		public int prg_page;
		public bool prg_mode;
		public bool contra_mode;
		public int chip0_prg_bank_mask_16k = 0x07;
		public int chip1_prg_bank_mask_16k = 0x1F;
		public int chip1_offset = 0x20000;

		public override bool Configure(NES.EDetectionOrigin origin)
		{
			switch (Cart.board_type)
			{
				case "MAPPER230":
					break;
				default:
					return false;
			}
			contra_mode = true;
			SetMirrorType(EMirrorType.Vertical);

			return true;
		}

		public override void SyncState(Serializer ser)
		{
			ser.Sync(nameof(contra_mode), ref contra_mode);
			ser.Sync(nameof(prg_page), ref prg_page);
			ser.Sync(nameof(prg_mode), ref prg_mode);
			base.SyncState(ser);
		}

		public override void WritePRG(int addr, byte value)
		{
			if (contra_mode)
			{
				prg_page = value & 0x07;
			}
			else
			{
				prg_page = value & 0x1F;
				prg_mode = value.Bit(5);

				if (value.Bit(6))
				{
					SetMirrorType(EMirrorType.Vertical);
				}
				else
				{
					SetMirrorType(EMirrorType.Horizontal);
				}
			}
		}

		public override byte ReadPRG(int addr)
		{
			if (contra_mode)
			{
				if (addr < 0x4000)
				{
					return ROM[((prg_page & chip0_prg_bank_mask_16k) * 0x4000) + addr];
				}
				else
				{
					return ROM[(7 * 0x4000) + (addr & 0x3FFF)];
				}
			}
			else
			{
				if (prg_mode == false)
				{
					return ROM[((prg_page >> 1) * 0x8000) + addr + chip1_offset];
				}
				else
				{
					int page = prg_page + 8;
					return ROM[(page * 0x4000) + (addr & 0x03FFF)];
				}
			}
		}

		public override void NESSoftReset()
		{
			contra_mode ^= true;
			prg_page = 0;
			prg_mode = false;
			SetMirrorType(EMirrorType.Vertical);
		}
	}
}
