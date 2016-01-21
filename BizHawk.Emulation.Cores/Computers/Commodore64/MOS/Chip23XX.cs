﻿using System;
using BizHawk.Common;

namespace BizHawk.Emulation.Cores.Computers.Commodore64.MOS
{
	// ROM chips
	// 2332: 32 kbit (4kbyte)
	// 2364: 64 kbit (8kbyte)
	// 23128: 128 kbit (16kbyte)

	public enum Chip23XXmodel
	{
		Chip2332,
		Chip2364,
		Chip23128
	}

	sealed public class Chip23XX
	{
	    readonly int addrMask;
	    readonly int[] rom;

		public Chip23XX(Chip23XXmodel model, byte[] data)
		{
			switch (model)
			{
				case Chip23XXmodel.Chip2332:
					rom = new int[0x1000];
					addrMask = 0xFFF;
					break;
				case Chip23XXmodel.Chip2364:
					rom = new int[0x2000];
					addrMask = 0x1FFF;
					break;
				case Chip23XXmodel.Chip23128:
					rom = new int[0x4000];
					addrMask = 0x3FFF;
					break;
				default:
					throw new Exception("Invalid chip model.");
			}
			Array.Copy(data, rom, rom.Length);
		}

		public int Peek(int addr)
		{
			return rom[addr & addrMask];
		}

		public int Read(int addr)
		{
			return rom[addr & addrMask];
		}

		public void SyncState(Serializer ser)
		{
			SaveState.SyncObject(ser, this);
		}
	}
}
