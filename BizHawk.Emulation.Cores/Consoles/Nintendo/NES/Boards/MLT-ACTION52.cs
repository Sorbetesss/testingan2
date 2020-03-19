﻿using BizHawk.Common;
using BizHawk.Common.NumberExtensions;

// http://wiki.nesdev.com/w/index.php/INES_Mapper_228
namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	public sealed class MLT_ACTION52 : NES.NESBoardBase
	{
		[MapperProp]
		public bool prg_mode = false;
		[MapperProp]
		public int prg_reg = 0;
		public int chr_reg;
		public int chip_offset;
		public bool cheetahmen = false;
		ByteBuffer eRAM = new ByteBuffer(4);
		int chr_bank_mask_8k, prg_bank_mask_16k, prg_bank_mask_32k;

		public override bool Configure(NES.EDetectionOrigin origin)
		{
			switch (Cart.board_type)
			{
				case "MAPPER228":
				case "MLT-ACTION52":
					break;
				default:
					return false;
			}

			AssertPrg(256, 1536);

			chr_bank_mask_8k = Cart.chr_size / 8 - 1;
			prg_bank_mask_16k = Cart.prg_size / 16 - 1;
			prg_bank_mask_32k = Cart.prg_size / 32 - 1;

			if (Cart.prg_size == 256)
			{
				cheetahmen = true;
			}
			else
			{
				prg_bank_mask_16k = 0x1F;
				prg_bank_mask_32k = 0xF;
			}

			AutoMapperProps.Apply(this);

			return true;
		}

		public override void SyncState(Serializer ser)
		{
			ser.Sync(nameof(prg_reg), ref prg_reg);
			ser.Sync(nameof(chr_reg), ref chr_reg);
			ser.Sync(nameof(prg_mode), ref prg_mode);
			ser.Sync("chip", ref chip_offset);
			ser.Sync(nameof(eRAM), ref eRAM);
			base.SyncState(ser);
		}

		public override void Dispose()
		{
			eRAM.Dispose();
			base.Dispose();
		}

		public override void WriteEXP(int addr, byte value)
		{
			if (addr >= 0x1800)
			{
				eRAM[(addr & 0x07)] = (byte)(value & 0x0F);
			}
		}

		public override byte ReadEXP(int addr)
		{
			if (addr >= 0x1800)
			{
				return eRAM[(addr & 0x07)];
			}
			else
			{
				return base.ReadEXP(addr);
			}
		}

		public override void WritePRG(int addr, byte value)
		{
			//$8000-FFFF:    [.... ..CC]   Low 2 bits of CHR
			//A~[..MH HPPP PPO. CCCC]

			addr += 0x8000;

			if (addr.Bit(13))
			{
				SetMirrorType(EMirrorType.Horizontal);
			}
			else
			{
				SetMirrorType(EMirrorType.Vertical);
			}

			prg_mode = addr.Bit(5);
			prg_reg = (addr >> 6) & 0x1F;
			chr_reg = ((addr & 0x0F) << 2) | (value & 0x03);
			if (!cheetahmen)
			{
				int chip = ((addr >> 11) & 0x03);
				switch (chip)
				{
					case 0:
						chip_offset = 0x0;
						break;
					case 1:
						chip_offset = 0x80000;
						break;
					case 2:
						break; //TODO: this chip doesn't exist and should access open bus
					case 3:
						chip_offset = 0x100000;
						break;
				}
			}
		}

		public override byte ReadPPU(int addr)
		{
			if (addr < 0x2000)
			{
				return VROM[((chr_reg & chr_bank_mask_8k) * 0x2000) + addr];
			}
			return base.ReadPPU(addr);
		}

		public override byte ReadPRG(int addr)
		{
			if (prg_mode == false)
			{
				int bank = (prg_reg >> 1) & prg_bank_mask_32k;
				return ROM[(bank * 0x8000) + addr + chip_offset];
			}
			else
			{
				return ROM[((prg_reg & prg_bank_mask_16k) * 0x4000) + (addr & 0x3FFF) + chip_offset];
			}
		}
	}
}
