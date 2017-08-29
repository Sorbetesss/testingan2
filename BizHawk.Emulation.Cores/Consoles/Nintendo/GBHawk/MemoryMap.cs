﻿using System;

using BizHawk.Common.BufferExtensions;
using BizHawk.Emulation.Common;


/*
	$FFFF 			Interrupt Enable Flag
	$FF80-$FFFE 	Zero Page - 127 bytes
	$FF00-$FF7F 	Hardware I/O Registers
	$FEA0-$FEFF 	Unusable Memory
	$FE00-$FE9F 	OAM - Object Attribute Memory
	$E000-$FDFF 	Echo RAM - Reserved, Do Not Use
	$D000-$DFFF 	Internal RAM - Bank 1-7 (switchable - CGB only)
	$C000-$CFFF 	Internal RAM - Bank 0 (fixed)
	$A000-$BFFF 	Cartridge RAM (If Available)
	$9C00-$9FFF 	BG Map Data 2
	$9800-$9BFF 	BG Map Data 1
	$8000-$97FF 	Character RAM
	$4000-$7FFF 	Cartridge ROM - Switchable Banks 1-xx
	$0150-$3FFF 	Cartridge ROM - Bank 0 (fixed)
	$0100-$014F 	Cartridge Header Area
	$0000-$00FF 	Restart and Interrupt Vectors
*/

namespace BizHawk.Emulation.Cores.Nintendo.GBHawk
{
	public partial class GBHawk
	{
		public byte ReadMemory(ushort addr)
		{
			MemoryCallbacks.CallReads(addr);

			if (addr < 0x100)
			{
				// return Either BIOS ROM or Game ROM
				if ((GB_bios_register & 0x1) == 0)
				{
					return _bios[addr]; // Return BIOS
				}
				else
				{
					return mapper.ReadMemory(addr);
				}
			}
			else if (addr < 0x8000)
			{
				return mapper.ReadMemory(addr);
			}
			else if (addr < 0x9800)
			{
				return CHR_RAM[addr - 0x8000];
			}
			else if (addr < 0x9C00)
			{
				return BG_map_1[addr - 0x9800];
			}
			else if (addr < 0xA000)
			{
				return BG_map_2[addr - 0x9C00];
			}
			else if (addr < 0xC000)
			{
				return mapper.ReadMemory(addr);
			}
			else if (addr < 0xE000)
			{
				return RAM[addr - 0xC000];
			}
			else if (addr < 0xFE00)
			{
				return RAM[addr - 0xE000];
			}
			else if (addr < 0xFEA0 && ppu.OAM_access)
			{
				return OAM[addr - 0xFE00];
			}
			else if (addr < 0xFF00)
			{
				// unmapped memory, returns 0xFF
				return 0xFF;
			}
			else if (addr < 0xFF80)
			{
				return Read_Registers(addr);
			}
			else if (addr < 0xFFFF)
			{
				return ZP_RAM[addr - 0xFF80];
			}
			else
			{
				return Read_Registers(addr);
			}

		}

		public void WriteMemory(ushort addr, byte value)
		{
			MemoryCallbacks.CallWrites(addr);

			if (addr < 0x100)
			{
				// return Either BIOS ROM or Game ROM
				if ((GB_bios_register & 0x1) == 0)
				{
					// Can't write to BIOS region
				}
				else
				{
					mapper.WriteMemory(addr, value);
				}
			}
			else if (addr < 0x8000)
			{
				mapper.WriteMemory(addr, value);
			}
			else if (addr < 0x9800)
			{
				CHR_RAM[addr - 0x8000] = value;
			}
			else if (addr < 0x9C00)
			{
				BG_map_1[addr - 0x9800] = value;
			}
			else if (addr < 0xA000)
			{
				BG_map_2[addr - 0x9C00] = value;
			}
			else if (addr < 0xC000)
			{
				mapper.WriteMemory(addr, value);
			}
			else if (addr < 0xE000)
			{
				RAM[addr - 0xC000] = value;
			}
			else if (addr < 0xFE00)
			{
				RAM[addr - 0xE000] = value;
			}
			else if (addr < 0xFEA0 && ppu.OAM_access)
			{
				OAM[addr - 0xFE00] = value;
			}
			else if (addr < 0xFF00)
			{
				// unmapped, writing has no effect
			}
			else if (addr < 0xFF80)
			{
				Write_Registers(addr, value);
			}
			else if (addr < 0xFFFF)
			{
				ZP_RAM[addr - 0xFF80] = value;
			}
			else
			{
				Write_Registers(addr, value);
			}
		}
	}
}
