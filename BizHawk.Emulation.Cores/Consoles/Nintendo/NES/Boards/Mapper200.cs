﻿using BizHawk.Common;
using BizHawk.Common.NumberExtensions;

// https://wiki.nesdev.com/w/index.php/INES_Mapper_200
namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	public sealed class Mapper200 : NES.NESBoardBase
	{
		int prg_reg_16k, chr_reg_8k;
		int prg_bank_mask_16k;
		int chr_bank_mask_8k;

		public override bool Configure(NES.EDetectionOrigin origin)
		{
			switch (Cart.board_type)
			{
				case "MAPPER200":
				case "MAPPER229":
					break;
				default:
					return false;
			}

			prg_bank_mask_16k = Cart.prg_size / 16 - 1;
			chr_bank_mask_8k = Cart.chr_size / 8 - 1;

			return true;
		}

		public override void SyncState(Serializer ser)
		{
			ser.Sync(nameof(prg_reg_16k), ref prg_reg_16k);
			ser.Sync(nameof(chr_reg_8k), ref chr_reg_8k);
			base.SyncState(ser);
		}

		public override void WritePRG(int addr, byte value)
		{
			if (addr.Bit(3))
			{
				SetMirrorType(EMirrorType.Horizontal);
			}
			else
			{
				SetMirrorType(EMirrorType.Vertical);
			}
			int reg = addr & 0x07;
			prg_reg_16k = reg & prg_bank_mask_16k;
			chr_reg_8k = reg & chr_bank_mask_8k;
		}

		public override byte ReadPRG(int addr)
		{
			if (addr < 0x4000)
			{
				return ROM[(prg_reg_16k * 0x4000) + addr];
			}
			else
			{
				return ROM[(prg_reg_16k * 0x4000) + addr - 0x4000];
			}
		}

		public override byte ReadPPU(int addr)
		{
			if (addr < 0x2000)
			{
				return VROM[(chr_reg_8k * 0x2000) + addr];
			}
			return base.ReadPPU(addr);
		}
	}
}
