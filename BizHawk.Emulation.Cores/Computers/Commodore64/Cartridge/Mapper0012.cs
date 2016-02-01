﻿using System;
using System.Collections.Generic;

using BizHawk.Common;

namespace BizHawk.Emulation.Cores.Computers.Commodore64
{
	sealed public class Mapper0012 : Cart
	{
		private byte[] bankMain;
		private byte[][] bankHigh;
		private byte[] bankHighSelected;
		private int bankIndex;
		private byte[] dummyBank;

		// Zaxxon and Super Zaxxon cartridges
		// - read to 8xxx selects bank 0 in A000-BFFF
		// - read to 9xxx selects bank 1 in A000-BFFF

		public Mapper0012(IList<int> newAddresses, IList<int> newBanks, IList<byte[]> newData)
		{
			bankMain = new byte[0x2000];
			bankHigh = new byte[2][];
			dummyBank = new byte[0x2000];

			// create dummy bank just in case
			for (var i = 0; i < 0x2000; i++)
				dummyBank[i] = 0xFF;

			bankHigh[0] = dummyBank;
			bankHigh[1] = dummyBank;

			// load in the banks
			for (var i = 0; i < newAddresses.Count; i++)
			{
				if (newAddresses[i] == 0x8000)
					Array.Copy(newData[i], bankMain, 0x1000);
				else if ((newAddresses[i] == 0xA000 || newAddresses[i] == 0xE000) && newBanks[i] < 2)
					bankHigh[newBanks[i]] = newData[i];
			}

			// mirror the main rom from 8000 to 9000
			Array.Copy(bankMain, 0x0000, bankMain, 0x1000, 0x1000);

			// set both pins low for 16k rom config
			pinExRom = false;
			pinGame = false;

		}

		public override int Peek8000(int addr)
		{
			return bankMain[addr];
		}

		public override int PeekA000(int addr)
		{
			return bankHighSelected[addr];
		}

		public override int Read8000(int addr)
		{
			bankIndex = (addr & 0x1000) >> 12;
			bankHighSelected = bankHigh[bankIndex];
			return bankMain[addr];
		}

		public override int ReadA000(int addr)
		{
			return bankHighSelected[addr];
		}

		public override void SyncState(Serializer ser)
		{
			base.SyncState(ser);
			if (ser.IsReader)
				bankHighSelected = bankHigh[bankIndex];
		}
	}
}
