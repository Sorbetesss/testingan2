﻿using System;
using System.Runtime.InteropServices;

using BizHawk.BizInvoke;
using BizHawk.Common.PathExtensions;
using BizHawk.Emulation.Common;
using BizHawk.Emulation.Cores.Waterbox;
using BizHawk.Common;
using BizHawk.Emulation.DiscSystem;
using System.Linq;

namespace BizHawk.Emulation.Cores.Consoles.Sega.gpgx
{
	[PortedCore(CoreNames.Gpgx, "Eke-Eke", "25a90c6", "https://github.com/ekeeke/Genesis-Plus-GX")]
	public partial class GPGX : IEmulator, IVideoProvider, ISaveRam, IStatable, IRegionable,
		IInputPollable, IDebuggable, IDriveLight, ICodeDataLogger, IDisassemblable
	{
		[CoreConstructor(VSystemID.Raw.GEN)]
		public GPGX(CoreLoadParameters<GPGXSettings, GPGXSyncSettings> lp)
		{
			LoadCallback = load_archive;
			_inputCallback = input_callback;
			InitMemCallbacks(); // ExecCallback, ReadCallback, WriteCallback
			CDCallback = CDCallbackProc;
			cd_callback_handle = CDRead;

			ServiceProvider = new BasicServiceProvider(this);
			// this can influence some things internally (autodetect romtype, etc)
			string romextension = "GEN";

			// three or six button?
			// http://www.sega-16.com/forum/showthread.php?4398-Forgotten-Worlds-giving-you-GAME-OVER-immediately-Fix-inside&highlight=forgotten%20worlds

			//hack, don't use
			if (lp.Roms.FirstOrDefault()?.RomData.Length > 32 * 1024 * 1024)
			{
				throw new InvalidOperationException("ROM too big!  Did you try to load a CD as a ROM?");
			}

			_elf = new WaterboxHost(new WaterboxOptions
			{
				Path = PathUtils.DllDirectoryPath,
				Filename = "gpgx.wbx",
				SbrkHeapSizeKB = 512,
				SealedHeapSizeKB = 4 * 1024,
				InvisibleHeapSizeKB = 4 * 1024,
				PlainHeapSizeKB = 4 * 1024,
				MmapHeapSizeKB = 1 * 1024,
				SkipCoreConsistencyCheck = lp.Comm.CorePreferences.HasFlag(CoreComm.CorePreferencesFlags.WaterboxCoreConsistencyCheck),
				SkipMemoryConsistencyCheck = lp.Comm.CorePreferences.HasFlag(CoreComm.CorePreferencesFlags.WaterboxMemoryConsistencyCheck),
			});

			var callingConventionAdapter = CallingConventionAdapters.MakeWaterbox(new Delegate[]
			{
				LoadCallback, _inputCallback, ExecCallback, ReadCallback, WriteCallback,
				CDCallback, cd_callback_handle,
			}, _elf);

			using (_elf.EnterExit())
			{
				Core = BizInvoker.GetInvoker<LibGPGX>(_elf, _elf, callingConventionAdapter);
				_syncSettings = lp.SyncSettings ?? new GPGXSyncSettings();
				_settings = lp.Settings ?? new GPGXSettings();

				CoreComm = lp.Comm;

				_romfile = lp.Roms.FirstOrDefault()?.RomData;

				if (lp.Discs.Count > 0)
				{
					if (lp.Discs.Count > 128)
					{
						throw new("Too many discs loaded at once!");
					}

					_cds = lp.Discs.Select(d => d.DiscData).ToArray();
					_cdReaders = _cds.Select(c => new DiscSectorReader(c)).ToArray();
					Core.gpgx_set_cdd_callback(cd_callback_handle);
					DriveLightEnabled = true;
				}

				LibGPGX.INPUT_SYSTEM system_a = SystemForSystem(_syncSettings.ControlTypeLeft);
				LibGPGX.INPUT_SYSTEM system_b = SystemForSystem(_syncSettings.ControlTypeRight);

				var initResult = Core.gpgx_init(romextension, LoadCallback, _syncSettings.GetNativeSettings(lp.Game));

				if (!initResult)
					throw new Exception($"{nameof(Core.gpgx_init)}() failed");

				{
					int fpsnum = 60;
					int fpsden = 1;
					Core.gpgx_get_fps(ref fpsnum, ref fpsden);
					VsyncNumerator = fpsnum;
					VsyncDenominator = fpsden;
					Region = VsyncNumerator / VsyncDenominator > 55 ? DisplayType.NTSC : DisplayType.PAL;
				}

				// when we call Seal, ANY pointer passed from managed code must be 0.
				// this is so the initial state is clean
				// the only two pointers set so far are LoadCallback, which the core zeroed itself,
				// and CdCallback
				Core.gpgx_set_cdd_callback(null);
				_elf.Seal();
				Core.gpgx_set_cdd_callback(cd_callback_handle);

				SetControllerDefinition();

				// pull the default video size from the core
				UpdateVideo();

				SetMemoryDomains();

				Core.gpgx_set_input_callback(_inputCallback);

				// process the non-init settings now
				PutSettings(_settings);

				KillMemCallbacks();

				_tracer = new GPGXTraceBuffer(this, _memoryDomains, this);
				(ServiceProvider as BasicServiceProvider).Register<ITraceable>(_tracer);
			}

			_romfile = null;
		}

		private static LibGPGX.INPUT_SYSTEM SystemForSystem(ControlType c)
		{
			switch (c)
			{
				default:
				case ControlType.None:
					return LibGPGX.INPUT_SYSTEM.SYSTEM_NONE;
				case ControlType.Normal:
					return LibGPGX.INPUT_SYSTEM.SYSTEM_MD_GAMEPAD;
				case ControlType.Xea1p:
					return LibGPGX.INPUT_SYSTEM.SYSTEM_XE_A1P;
				case ControlType.Activator:
					return LibGPGX.INPUT_SYSTEM.SYSTEM_ACTIVATOR;
				case ControlType.Teamplayer:
					return LibGPGX.INPUT_SYSTEM.SYSTEM_TEAMPLAYER;
				case ControlType.Wayplay:
					return LibGPGX.INPUT_SYSTEM.SYSTEM_WAYPLAY;
				case ControlType.Mouse:
					return LibGPGX.INPUT_SYSTEM.SYSTEM_MOUSE;
			}
		}

		private readonly LibGPGX Core;
		private readonly WaterboxHost _elf;

		private readonly Disc[] _cds;
		private int _discIndex;
		private readonly DiscSectorReader[] _cdReaders;
		private bool _prevDiskPressed;
		private bool _nextDiskPressed;

		private readonly byte[] _romfile;

		private bool _disposed = false;

		private LibGPGX.load_archive_cb LoadCallback;

		private readonly LibGPGX.InputData input = new LibGPGX.InputData();

		public enum ControlType
		{
			None,
			Normal,
			Xea1p,
			Activator,
			Teamplayer,
			Wayplay,
			Mouse
		}


		/// <summary>
		/// core callback for file loading
		/// </summary>
		/// <param name="filename">string identifying file to be loaded</param>
		/// <param name="buffer">buffer to load file to</param>
		/// <param name="maxsize">maximum length buffer can hold</param>
		/// <returns>actual size loaded, or 0 on failure</returns>
		private int load_archive(string filename, IntPtr buffer, int maxsize)
		{
			byte[] srcdata = null;

			if (buffer == IntPtr.Zero)
			{
				Console.WriteLine("Couldn't satisfy firmware request {0} because buffer == NULL", filename);
				return 0;
			}

			if (filename == "PRIMARY_ROM")
			{
				if (_romfile == null)
				{
					Console.WriteLine("Couldn't satisfy firmware request PRIMARY_ROM because none was provided.");
					return 0;
				}
				srcdata = _romfile;
			}
			else if (filename is "PRIMARY_CD" or "SECONDARY_CD")
			{
				if (filename == "PRIMARY_CD" && _romfile != null)
				{
					Console.WriteLine("Declined to satisfy firmware request PRIMARY_CD because PRIMARY_ROM was provided.");
					return 0;
				}
				else
				{
					if (_cds == null)
					{
						Console.WriteLine("Couldn't satisfy firmware request {0} because none was provided.", filename);
						return 0;
					}
					srcdata = GetCDData(_cds[0]);
					if (srcdata.Length != maxsize)
					{
						Console.WriteLine("Couldn't satisfy firmware request {0} because of struct size ({1} != {2}).", filename, srcdata.Length, maxsize);
						return 0;
					}
				}
			}
			else
			{
				// use fromtend firmware interface

				string firmwareID = null;
				switch (filename)
				{
					case "CD_BIOS_EU": firmwareID = "CD_BIOS_EU"; break;
					case "CD_BIOS_JP": firmwareID = "CD_BIOS_JP"; break;
					case "CD_BIOS_US": firmwareID = "CD_BIOS_US"; break;
					default:
						break;
				}
				if (firmwareID != null)
				{
					// this path will be the most common PEBKAC error, so be a bit more vocal about the problem
					srcdata = CoreComm.CoreFileProvider.GetFirmware(new("GEN", firmwareID), "GPGX firmwares are usually required.");
					if (srcdata == null)
					{
						Console.WriteLine("Frontend couldn't satisfy firmware request GEN:{0}", firmwareID);
						return 0;
					}
				}
				else
				{
					Console.WriteLine("Unrecognized firmware request {0}", filename);
					return 0;
				}
			}

			if (srcdata != null)
			{
				if (srcdata.Length > maxsize)
				{
					Console.WriteLine("Couldn't satisfy firmware request {0} because {1} > {2}", filename, srcdata.Length, maxsize);
					return 0;
				}
				else
				{
					Marshal.Copy(srcdata, 0, buffer, srcdata.Length);
					Console.WriteLine("Firmware request {0} satisfied at size {1}", filename, srcdata.Length);
					return srcdata.Length;
				}
			}
			else
			{
				throw new InvalidOperationException("Unknown error processing firmware");
			}

		}

		private CoreComm CoreComm { get; }

		private readonly byte[] _sectorBuffer = new byte[2448];

		private void CDRead(int lba, IntPtr dest, bool subcode)
		{
			if ((uint)_discIndex < _cds.Length)
			{
				if (subcode)
				{
					_cdReaders[_discIndex].ReadLBA_2448(lba, _sectorBuffer, 0);
					Marshal.Copy(_sectorBuffer, 2352, dest, 96);
				}
				else
				{
					_cdReaders[_discIndex].ReadLBA_2352(lba, _sectorBuffer, 0);
					Marshal.Copy(_sectorBuffer, 0, dest, 2352);
					_driveLight = true;
				}
			}
		}

		private readonly LibGPGX.cd_read_cb cd_callback_handle;

		public static LibGPGX.CDData GetCDDataStruct(Disc cd)
		{
			var ret = new LibGPGX.CDData();

			var ses = cd.Session1;
			var ntrack = ses.InformationTrackCount;

			// bet you a dollar this is all wrong
			// zero 07-jul-2015 - throws a dollar in the pile, since he probably messed it up worse
			for (var i = 0; i < LibGPGX.CD_MAX_TRACKS; i++)
			{
				ret.tracks[i].fd = IntPtr.Zero;
				ret.tracks[i].offset = 0;
				ret.tracks[i].loopEnabled = 0;
				ret.tracks[i].loopOffset = 0;

				if (i < ntrack)
				{
					ret.tracks[i].start = ses.Tracks[i + 1].LBA;
					ret.tracks[i].end = ses.Tracks[i + 2].LBA;
					ret.tracks[i].mode = ses.Tracks[i + 1].Mode;
					if (i == ntrack - 1)
					{
						ret.end = ret.tracks[i].end;
						ret.last = ntrack;
					}
				}
				else
				{
					ret.tracks[i].start = 0;
					ret.tracks[i].end = 0;
					ret.tracks[i].mode = 0;
				}
			}

			ret.sub = IntPtr.Zero;
			return ret;
		}

		public static unsafe byte[] GetCDData(Disc cd)
		{
			var ret = GetCDDataStruct(cd);
			var size = Marshal.SizeOf(ret);
			var retdata = new byte[size];

			fixed (byte* p = retdata)
			{
				Marshal.StructureToPtr(ret, (IntPtr)p, false);
			}

			return retdata;
		}

		/// <summary>
		/// size of native input struct
		/// </summary>
		private int inputsize;

		private GPGXControlConverter ControlConverter;

		private void SetControllerDefinition()
		{
			inputsize = Marshal.SizeOf(typeof(LibGPGX.InputData));
			if (!Core.gpgx_get_control(input, inputsize))
				throw new Exception($"{nameof(Core.gpgx_get_control)}() failed");

			ControlConverter = new GPGXControlConverter(input, _cds != null);
			ControllerDefinition = ControlConverter.ControllerDef;
		}

		public LibGPGX.INPUT_DEVICE[] GetDevices()
		{
			return (LibGPGX.INPUT_DEVICE[])input.dev.Clone();
		}

		public bool IsMegaCD => _cds != null;

		public class VDPView : IMonitor
		{
			private readonly IMonitor _m;

			public VDPView(LibGPGX.VDPView v, IMonitor m)
			{
				_m = m;
				VRAM = v.VRAM;
				PatternCache = v.PatternCache;
				ColorCache = v.ColorCache;
				NTA = v.NTA;
				NTB = v.NTB;
				NTW = v.NTW;
			}

			public IntPtr VRAM;
			public IntPtr PatternCache;
			public IntPtr ColorCache;
			public LibGPGX.VDPNameTable NTA;
			public LibGPGX.VDPNameTable NTB;
			public LibGPGX.VDPNameTable NTW;


			public void Enter()
			{
				_m.Enter();
			}

			public void Exit()
			{
				_m.Exit();
			}
		}

		public VDPView UpdateVDPViewContext()
		{
			var v = new LibGPGX.VDPView();
			Core.gpgx_get_vdp_view(v);
			Core.gpgx_flush_vram(); // fully regenerate internal caches as needed
			return new VDPView(v, _elf);
		}
		
		public int AddDeepFreezeValue(int address, byte value)
		{
			return Core.gpgx_add_deepfreeze_list_entry(address, value);
		}

		public void ClearDeepFreezeList()
		{
			Core.gpgx_clear_deepfreeze_list();
		}

		public DisplayType Region { get; }
	}
}
