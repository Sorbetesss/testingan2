﻿using System;
using System.Text;
using BizHawk.Common;
using BizHawk.Emulation.Common;

namespace BizHawk.Emulation.Cores.Computers.MSX
{
	[Core(CoreNames.MSXHawk, "", isReleased: true)]
	[ServiceNotApplicable(new[] { typeof(IDriveLight) })]
	public partial class MSX : IEmulator, IVideoProvider, ISoundProvider, ISaveRam, IInputPollable, IRegionable, ISettable<MSX.MSXSettings, MSX.MSXSyncSettings>
	{
		[CoreConstructor(VSystemID.Raw.MSX)]
		public MSX(CoreComm comm, GameInfo game, byte[] rom, MSX.MSXSettings settings, MSX.MSXSyncSettings syncSettings)
		{
			ServiceProvider = new BasicServiceProvider(this);
			Settings = (MSXSettings)settings ?? new MSXSettings();
			SyncSettings = (MSXSyncSettings)syncSettings ?? new MSXSyncSettings();

			RomData = new byte[rom.Length];

			// look up game in db before transforming ROM
			var hash_md5 = MD5Checksum.ComputePrefixedHex(rom);
			var gi = Database.CheckDatabase(hash_md5);
			var dict = gi.GetOptions();
			string s_mapper;

			for (int i = 0; i < rom.Length; i++)
			{
				RomData[i] = rom[i];
			}

			int size = RomData.Length;

			int mapper_1 = 0;

			if (RomData.Length % BankSize != 0)
			{
				Array.Resize(ref RomData, ((RomData.Length / BankSize) + 1) * BankSize);
			}

			// we want all ROMS to be multiples of 64K for easy memory mapping later
			if (RomData.Length < 0x10000)
			{
				Array.Resize(ref RomData, 0x10000);
			}
			else
			{
				// Assume default konami style mapper
				if (gi == null)
				{
					mapper_1 = 3;
					Console.WriteLine("Using Ascii 8 KB Mapper");
				}
				else if (!dict.TryGetValue("mapper", out s_mapper))
				{
					mapper_1 = 3;
					Console.WriteLine("Using Ascii 8 KB Mapper");
				}
				else
				{
					if (s_mapper == "1")
					{
						mapper_1 = 1;
						Console.WriteLine("Using Konami Mapper");
					}

					if (s_mapper == "2")
					{
						mapper_1 = 2;
						Console.WriteLine("Using Konami Mapper with SCC");
					}
				}			
				
			}

			// if the original was not 64 or 48 k, move it (may need to do this case by case)

			if (size == 0x8000)
			{
				for (int i = 0x7FFF; i >= 0; i--)
				{
					RomData[i + 0x4000] = RomData[i];
				}
				for (int i = 0; i < 0x4000; i++)
				{
					RomData[i] = 0; 
				}
			}

			if (size == 0x4000)
			{
				for (int i = 0; i < 0x4000; i++)
				{
					RomData[i + 0x4000] = RomData[i];
					RomData[i + 0x8000] = RomData[i];
					RomData[i + 0xC000] = RomData[i];
				}
			}

			// loook for combination BIOS + BASIC files first
			byte[] loc_bios = null;
			if (SyncSettings.Region_Setting == MSXSyncSettings.RegionType.USA)
			{
				loc_bios = comm.CoreFileProvider.GetFirmware(new("MSX", "bios_basic_usa"));
			}
			else
			{
				loc_bios = comm.CoreFileProvider.GetFirmwareOrThrow(new("MSX", "bios_basic_jpn"));
			}
			
			// look for individual files (not implemented yet)
			if (loc_bios == null)
			{
				throw new Exception("Cannot load, no BIOS files found for selected region.");
			} 

			if (loc_bios.Length == 32768)
			{
				Bios = new byte[0x4000];
				Basic = new byte[0x4000];

				for (int i = 0; i < 0x4000; i++)
				{
					Bios[i] = loc_bios[i];
					Basic[i] = loc_bios[i + 0x4000];
				}
			}

			//only use one rom cart for now
			RomData2 = new byte[0x10000];

			for (int i = 0; i < 0x10000; i++) { RomData2[i] = 0; }
			
			MSX_Pntr = LibMSX.MSX_create();

			LibMSX.MSX_load_bios(MSX_Pntr, Bios, Basic);
			LibMSX.MSX_load(MSX_Pntr, RomData, (uint)RomData.Length, mapper_1, RomData2, (uint)RomData2.Length, 0);

			blip.SetRates(3579545, 44100);

			(ServiceProvider as BasicServiceProvider).Register<ISoundProvider>(this);

			SetupMemoryDomains();

			Header_Length = LibMSX.MSX_getheaderlength(MSX_Pntr);
			Disasm_Length = LibMSX.MSX_getdisasmlength(MSX_Pntr);
			Reg_String_Length = LibMSX.MSX_getregstringlength(MSX_Pntr);

			var newHeader = new StringBuilder(Header_Length);
			LibMSX.MSX_getheader(MSX_Pntr, newHeader, Header_Length);

			Console.WriteLine(Header_Length + " " + Disasm_Length + " " + Reg_String_Length);

			Tracer = new TraceBuffer(newHeader.ToString());

			var serviceProvider = ServiceProvider as BasicServiceProvider;
			serviceProvider.Register<ITraceable>(Tracer);
			serviceProvider.Register<IStatable>(new StateSerializer(SyncState));

			current_controller = SyncSettings.Contr_Setting == MSXSyncSettings.ContrType.Keyboard ? MSXControllerKB : MSXControllerJS;
		}

		public void HardReset()
		{

		}

		private IntPtr MSX_Pntr { get; set; } = IntPtr.Zero;
		private byte[] MSX_core = new byte[0x28000];
		public static byte[] Bios = null;
		public static byte[] Basic;

		// Constants
		private const int BankSize = 16384;

		// ROM
		public static byte[] RomData;
		public static byte[] RomData2;

		// Machine resources
		private IController _controller = NullController.Instance;

		private readonly ControllerDefinition current_controller = null;

		private int _frame = 0;

		public DisplayType Region => DisplayType.NTSC;

		private readonly ITraceable Tracer;

		private LibMSX.TraceCallback tracecb;

		// these will be constant values assigned during core construction
		private int Header_Length;
		private readonly int Disasm_Length;
		private readonly int Reg_String_Length;

		private void MakeTrace(int t)
		{
			StringBuilder new_d = new StringBuilder(Disasm_Length);
			StringBuilder new_r = new StringBuilder(Reg_String_Length);

			LibMSX.MSX_getdisassembly(MSX_Pntr, new_d, t, Disasm_Length);
			LibMSX.MSX_getregisterstate(MSX_Pntr, new_r, t, Reg_String_Length);

			Tracer.Put(new(disassembly: new_d.ToString().PadRight(36), registerInfo: new_r.ToString()));
		}

		private readonly MemoryCallbackSystem _memorycallbacks = new MemoryCallbackSystem(new[] { "System Bus" });
		public IMemoryCallbackSystem MemoryCallbacks => _memorycallbacks;
	}
}
