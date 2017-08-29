﻿using System;

using BizHawk.Common.BufferExtensions;
using BizHawk.Emulation.Common;
using BizHawk.Emulation.Common.Components.LR35902;
using BizHawk.Common.NumberExtensions;

namespace BizHawk.Emulation.Cores.Nintendo.GBHawk
{
	[Core(
		"GBHawk",
		"",
		isPorted: false,
		isReleased: true)]
	[ServiceNotApplicable(typeof(ISettable<,>), typeof(IDriveLight))]
	public partial class GBHawk : IEmulator, ISaveRam, IDebuggable, IStatable, IInputPollable, IRegionable,
	ISettable<GBHawk.GBSettings, GBHawk.GBSyncSettings>
	{
		// this register controls whether or not the GB BIOS is mapped into memory
		public byte GB_bios_register;

		public byte input_register;

		public byte serial_control;
		public byte serial_data_out;
		public byte serial_data_in;
		public bool serial_start_old;

		// The unused bits in this register are still read/writable
		public byte REG_FFFF;
		// The unused bits in this register (interrupt flags) are always set
		public byte REG_FF0F = 0xE0;
		public bool enable_VBL;
		public bool enable_STAT;
		public bool enable_TIMO;
		public bool enable_SER;
		public bool enable_PRS;


		// memory domains
		public byte[] RAM = new byte[0x2000];
		public byte[] ZP_RAM = new byte[0x80];
		public byte[] CHR_RAM = new byte[0x1800];
		public byte[] BG_map_1 = new byte[0x400];
		public byte[] BG_map_2 = new byte[0x400];
		public byte[] OAM = new byte[0xA0];

		public readonly byte[] _rom;
		public readonly byte[] _bios;
		public readonly byte[] _sram = new byte[2048];
		public readonly byte[] header = new byte[0x50];

		public byte[] cart_RAM;

		private int _frame = 0;

		public MapperBase mapper;

		private readonly ITraceable _tracer;

		public LR35902 cpu;
		public PPU ppu;
		public Timer timer;
		public Audio audio;

		[CoreConstructor("GB")]
		public GBHawk(CoreComm comm, GameInfo game, byte[] rom, /*string gameDbFn,*/ object settings, object syncSettings)
		{
			var ser = new BasicServiceProvider(this);

			cpu = new LR35902
			{
				ReadMemory = ReadMemory,
				WriteMemory = WriteMemory,
				PeekMemory = ReadMemory,
				DummyReadMemory = ReadMemory,
				OnExecFetch = ExecFetch
			};
			ppu = new PPU();
			timer = new Timer();
			audio = new Audio();

			CoreComm = comm;

			_settings = (GBSettings)settings ?? new GBSettings();
			_syncSettings = (GBSyncSettings)syncSettings ?? new GBSyncSettings();
			_controllerDeck = new GBHawkControllerDeck(_syncSettings.Port1);

			byte[] Bios = comm.CoreFileProvider.GetFirmware("GB", "World", false, "BIOS Not Found, Cannot Load");
			_bios = Bios;

			Buffer.BlockCopy(rom, 0x100, header, 0, 0x50);

			string hash_md5 = null;
			hash_md5 = "md5:" + rom.HashMD5(0, rom.Length);
			Console.WriteLine(hash_md5);

			_rom = rom;
			Setup_Mapper();

			_frameHz = 60;

			timer.Core = this;
			audio.Core = this;
			ppu.Core = this;

			ser.Register<IVideoProvider>(this);
			ser.Register<ISoundProvider>(audio);
			ServiceProvider = ser;

			_tracer = new TraceBuffer { Header = cpu.TraceHeader };
			ser.Register<ITraceable>(_tracer);

			SetupMemoryDomains();
			HardReset();		
		}

		public DisplayType Region => DisplayType.NTSC;

		private readonly GBHawkControllerDeck _controllerDeck;

		private void HardReset()
		{
			GB_bios_register = 0; // bios enable
			in_vblank = true; // we start off in vblank since the LCD is off
			in_vblank_old = true;

			Register_Reset();
			timer.Reset();
			ppu.Reset();

			cpu.SetCallbacks(ReadMemory, ReadMemory, ReadMemory, WriteMemory);

			_vidbuffer = new int[VirtualWidth * VirtualHeight];
		}

		private void ExecFetch(ushort addr)
		{
			MemoryCallbacks.CallExecutes(addr);
		}

		private void Setup_Mapper()
		{
			// setup up mapper based on header entry
			switch (header[0x47])
			{
				case 0x0: mapper = new MapperDefault();				break;
				case 0x1: mapper = new MapperMBC1();				break;
				case 0x2: mapper = new MapperMBC1();				break;
				case 0x3: mapper = new MapperMBC1();				break;
				case 0x5: mapper = new MapperMBC2();				break;
				case 0x6: mapper = new MapperMBC2();				break;
				case 0x8: mapper = new MapperDefault();				break;
				case 0x9: mapper = new MapperDefault();				break;
				case 0xB: mapper = new MapperMMM01();				break;
				case 0xC: mapper = new MapperMMM01();				break;
				case 0xD: mapper = new MapperMMM01();				break;
				case 0xF: mapper = new MapperMBC3();				break;
				case 0x10: mapper = new MapperMBC3();				break;
				case 0x11: mapper = new MapperMBC3();				break;
				case 0x12: mapper = new MapperMBC3();				break;
				case 0x13: mapper = new MapperMBC3();				break;
				case 0x19: mapper = new MapperMBC5();				break;
				case 0x1A: mapper = new MapperMBC5();				break;
				case 0x1B: mapper = new MapperMBC5();				break;
				case 0x1C: mapper = new MapperMBC5();				break;
				case 0x1D: mapper = new MapperMBC5();				break;
				case 0x1E: mapper = new MapperMBC5();				break;
				case 0x20: mapper = new MapperMBC6();				break;
				case 0x22: mapper = new MapperMBC7();				break;
				case 0xFC: mapper = new MapperCamera();				break;
				case 0xFD: mapper = new MapperTAMA5();				break;
				case 0xFE: mapper = new MapperHuC3();				break;
				case 0xFF: mapper = new MapperHuC1();				break;

				case 0x4:
				case 0x7:
				case 0xA:
				case 0xE:
				case 0x14:
				case 0x15:
				case 0x16:
				case 0x17:
				case 0x18:
				case 0x1F:
				case 0x21:
				default:
					// mapper not implemented
					throw new Exception("Mapper not implemented");
					break;

			}

			Console.Write("Mapper: ");
			Console.WriteLine(header[0x47]);

			cart_RAM = null;

			switch (header[0x49])
			{
				case 1:
					cart_RAM = new byte[0x800];
					break;
				case 2:
					cart_RAM = new byte[0x2000];
					break;
				case 3:
					cart_RAM = new byte[0x8000];
					break;
				case 4:
					cart_RAM = new byte[0x20000];
					break;
				case 5:
					cart_RAM = new byte[0x10000];
					break;

			}

			mapper.Core = this;
			mapper.Initialize();
		}
	}
}
