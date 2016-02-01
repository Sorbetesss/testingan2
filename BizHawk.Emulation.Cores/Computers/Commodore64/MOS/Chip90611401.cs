﻿using System;
using BizHawk.Common;

namespace BizHawk.Emulation.Cores.Computers.Commodore64.MOS
{
	// emulates the PLA
	// which handles all bank switching

	public sealed class Chip90611401
	{
		// ------------------------------------

		public Func<int, int> PeekBasicRom;
		public Func<int, int> PeekCartridgeLo;
		public Func<int, int> PeekCartridgeHi;
		public Func<int, int> PeekCharRom;
		public Func<int, int> PeekCia0;
		public Func<int, int> PeekCia1;
		public Func<int, int> PeekColorRam;
		public Func<int, int> PeekExpansionLo;
		public Func<int, int> PeekExpansionHi;
		public Func<int, int> PeekKernalRom;
		public Func<int, int> PeekMemory;
		public Func<int, int> PeekSid;
		public Func<int, int> PeekVic;
		public Action<int, int> PokeCartridgeLo;
		public Action<int, int> PokeCartridgeHi;
		public Action<int, int> PokeCia0;
		public Action<int, int> PokeCia1;
		public Action<int, int> PokeColorRam;
		public Action<int, int> PokeExpansionLo;
		public Action<int, int> PokeExpansionHi;
		public Action<int, int> PokeMemory;
		public Action<int, int> PokeSid;
		public Action<int, int> PokeVic;
        public Func<bool> ReadAEC;
        public Func<bool> ReadBA;
		public Func<int, int> ReadBasicRom;
		public Func<int, int> ReadCartridgeLo;
		public Func<int, int> ReadCartridgeHi;
		public Func<bool> ReadCharen;
		public Func<int, int> ReadCharRom;
		public Func<int, int> ReadCia0;
		public Func<int, int> ReadCia1;
		public Func<int, int> ReadColorRam;
		public Func<int, int> ReadExpansionLo;
		public Func<int, int> ReadExpansionHi;
		public Func<bool> ReadExRom;
		public Func<bool> ReadGame;
		public Func<bool> ReadHiRam;
		public Func<int, int> ReadKernalRom;
		public Func<bool> ReadLoRam;
		public Func<int, int> ReadMemory;
		public Func<int, int> ReadSid;
		public Func<int, int> ReadVic;
		public Action<int, int> WriteCartridgeLo;
		public Action<int, int> WriteCartridgeHi;
		public Action<int, int> WriteCia0;
		public Action<int, int> WriteCia1;
		public Action<int, int> WriteColorRam;
		public Action<int, int> WriteExpansionLo;
		public Action<int, int> WriteExpansionHi;
		public Action<int, int> WriteMemory;
		public Action<int, int> WriteSid;
		public Action<int, int> WriteVic;
	
		// ------------------------------------

        enum PLABank
		{
			None,
			RAM,
			BasicROM,
			KernalROM,
			CharROM,
			IO,
			CartridgeLo,
			CartridgeHi,
			Vic,
			Sid,
			ColorRam,
			Cia0,
			Cia1,
			Expansion0,
			Expansion1
		}

		// ------------------------------------

        bool p24;
        bool p25;
        bool p26;
        bool p27;
        bool p28;
        bool loram;
        bool hiram;
        bool game;
        bool exrom;
        bool charen;
        bool a15;
        bool a14;
        bool a13;
        bool a12;

		private PLABank Bank(int addr, bool read)
		{
            loram = ReadLoRam();
            hiram = ReadHiRam();
            game = ReadGame();

            a15 = (addr & 0x08000) != 0;
            a14 = (addr & 0x04000) != 0;
            a13 = (addr & 0x02000) != 0;
            a12 = (addr & 0x01000) != 0;

            // upper memory regions 8000-FFFF
            exrom = ReadExRom();
            if (a15)
            {
                // io/character access
                if (a14 && !a13 && a12)
                {
                    // character rom, banked in at D000-DFFF
                    charen = ReadCharen();
                    if (read && !charen && (((hiram || loram) && game) || (hiram && !exrom && !game)))
                        return PLABank.CharROM;

                    // io block, banked in at D000-DFFF
                    if ((charen && (hiram || loram)) || (exrom && !game))
                    {
                        if (addr < 0xD400)
                            return PLABank.Vic;
                        if (addr < 0xD800)
                            return PLABank.Sid;
                        if (addr < 0xDC00)
                            return PLABank.ColorRam;
                        if (addr < 0xDD00)
                            return PLABank.Cia0;
                        if (addr < 0xDE00)
                            return PLABank.Cia1;
                        if (addr < 0xDF00)
                            return PLABank.Expansion0;
                        return PLABank.Expansion1;
                    }
                }

                // cartridge high, banked either at A000-BFFF or E000-FFFF depending
                if (a13 && !game && ((hiram && !a14 && read && !exrom) || (a14 && exrom)))
                    return PLABank.CartridgeHi;

                // cartridge low, banked at 8000-9FFF
                if (!a14 && !a13 && ((loram && hiram && read && !exrom) || (exrom && !game)))
                    return PLABank.CartridgeLo;

                // kernal rom, banked at E000-FFFF
                if (hiram && a14 && a13 && read && (game || (!exrom && !game)))
                    return PLABank.KernalROM;

                // basic rom, banked at A000-BFFF
                if (loram && hiram && !a14 && a13 && read && game)
                    return PLABank.BasicROM;
            }

            // ultimax mode ram exclusion
            if (exrom && !game)
            {
                p24 = !a15 && !a14 && a12;        //00x1 1000-1FFF, 3000-3FFF
                p25 = !a15 && !a14 && a13;        //001x 2000-3FFF
                p26 = !a15 && a14;                //01xx 4000-7FFF
                p27 = a15 && !a14 && a13;         //101x A000-BFFF
                p28 = a15 && a14 && !a13 && !a12; //1100 C000-CFFF
                if (p24 || p25 || p26 || p27 || p28)
                    return PLABank.None;
            }

            return PLABank.RAM;
        }

		public int Peek(int addr)
		{
			switch (Bank(addr, true))
			{
				case PLABank.BasicROM:
					return PeekBasicRom(addr);
				case PLABank.CartridgeHi:
					return PeekCartridgeHi(addr);
				case PLABank.CartridgeLo:
					return PeekCartridgeLo(addr);
				case PLABank.CharROM:
					return PeekCharRom(addr);
				case PLABank.Cia0:
					return PeekCia0(addr);
				case PLABank.Cia1:
					return PeekCia1(addr);
				case PLABank.ColorRam:
					return PeekColorRam(addr);
				case PLABank.Expansion0:
					return PeekExpansionLo(addr);
				case PLABank.Expansion1:
					return PeekExpansionHi(addr);
				case PLABank.KernalROM:
					return PeekKernalRom(addr);
				case PLABank.RAM:
					return PeekMemory(addr);
				case PLABank.Sid:
					return PeekSid(addr);
				case PLABank.Vic:
					return PeekVic(addr);
			}
			return 0xFF;
		}

		public void Poke(int addr, int val)
		{
			switch (Bank(addr, false))
			{
				case PLABank.CartridgeHi:
					PokeCartridgeHi(addr, val);
					break;
				case PLABank.CartridgeLo:
					PokeCartridgeLo(addr, val);
					break;
				case PLABank.Cia0:
					PokeCia0(addr, val);
					break;
				case PLABank.Cia1:
					PokeCia1(addr, val);
					break;
				case PLABank.ColorRam:
					PokeColorRam(addr, val);
					break;
				case PLABank.Expansion0:
					PokeExpansionLo(addr, val);
					break;
				case PLABank.Expansion1:
					PokeExpansionHi(addr, val);
					break;
				case PLABank.RAM:
					PokeMemory(addr, val);
					break;
				case PLABank.Sid:
					PokeSid(addr, val);
					break;
				case PLABank.Vic:
					PokeVic(addr, val);
					break;
			}
		}

		public int Read(int addr)
		{
			switch (Bank(addr, true))
			{
				case PLABank.BasicROM:
					return ReadBasicRom(addr);
				case PLABank.CartridgeHi:
					return ReadCartridgeHi(addr);
				case PLABank.CartridgeLo:
					return ReadCartridgeLo(addr);
				case PLABank.CharROM:
					return ReadCharRom(addr);
				case PLABank.Cia0:
					return ReadCia0(addr);
				case PLABank.Cia1:
					return ReadCia1(addr);
				case PLABank.ColorRam:
					return ReadColorRam(addr);
				case PLABank.Expansion0:
					return ReadExpansionLo(addr);
				case PLABank.Expansion1:
					return ReadExpansionHi(addr);
				case PLABank.KernalROM:
					return ReadKernalRom(addr);
				case PLABank.RAM:
					return ReadMemory(addr);
				case PLABank.Sid:
					return ReadSid(addr);
				case PLABank.Vic:
					return ReadVic(addr);
			}
			return 0xFF;
		}

        public void SyncState(Serializer ser)
        {
            SaveState.SyncObject(ser, this);
        }

        public int VicRead(int addr)
        {
            game = ReadGame();
            exrom = ReadExRom();
            a14 = (addr & 0x04000) == 0;
            a13 = (addr & 0x02000) != 0;
            a12 = (addr & 0x01000) != 0;

            // read char rom at 1000-1FFF and 9000-9FFF
            if (a14 && !a13 && a12 && (game || !exrom))
                return ReadCharRom(addr);

            // read cartridge rom in ultimax mode
            if (a13 && a12 && exrom && !game)
                return ReadCartridgeHi(addr);

            return ReadMemory(addr);
        }

        public void Write(int addr, int val)
		{
			switch (Bank(addr, false))
			{
				case PLABank.CartridgeHi:
					WriteCartridgeHi(addr, val);
                    if (ReadGame() || !ReadExRom())
                    {
                        WriteMemory(addr, val);
                    }
					break;
				case PLABank.CartridgeLo:
					WriteCartridgeLo(addr, val);
                    if (ReadGame() || !ReadExRom())
                    {
                        WriteMemory(addr, val);
                    }
					break;
				case PLABank.Cia0:
					WriteCia0(addr, val);
					break;
				case PLABank.Cia1:
					WriteCia1(addr, val);
					break;
				case PLABank.ColorRam:
					WriteColorRam(addr, val);
					break;
				case PLABank.Expansion0:
					WriteExpansionLo(addr, val);
					return;
				case PLABank.Expansion1:
					WriteExpansionHi(addr, val);
                    return;
				case PLABank.RAM:
			        WriteMemory(addr, val);
					break;
				case PLABank.Sid:
					WriteSid(addr, val);
					break;
				case PLABank.Vic:
					WriteVic(addr, val);
					break;
			}
		}
	}
}
