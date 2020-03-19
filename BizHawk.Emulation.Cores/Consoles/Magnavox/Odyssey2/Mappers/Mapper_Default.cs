﻿using BizHawk.Emulation.Cores.Components.I8048;

namespace BizHawk.Emulation.Cores.Consoles.O2Hawk
{
	// Default mapper with no bank switching
	public class MapperDefault : MapperBase
	{
		public override void Initialize()
		{
			// nothing to initialize
		}

		public override byte ReadMemory(ushort addr)
		{
			if (addr < 0x8000)
			{
				return Core._rom[addr & (Core._rom.Length - 1)];
			}

			if (Core.cart_RAM != null)
			{
				return Core.cart_RAM[addr - 0xA000];
			}

			return 0;
		}

		public override void MapCDL(ushort addr, I8048.eCDLogMemFlags flags)
		{
			if (addr < 0x8000)
			{
				SetCDLROM(flags, addr);
			}
			else
			{
				if (Core.cart_RAM != null)
				{
					SetCDLRAM(flags, addr - 0xA000);
				}
			}
		}

		public override void WriteMemory(ushort addr, byte value)
		{
			if (addr < 0x8000)
			{
				// no mapping hardware available
			}
			else
			{
				if (Core.cart_RAM != null)
				{
					Core.cart_RAM[addr - 0xA000] = value;
				}
			}
		}
	}
}
