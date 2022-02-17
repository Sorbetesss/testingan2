﻿using System;
using System.Runtime.InteropServices;

using BizHawk.Emulation.Cores.Consoles.Nintendo.Gameboy;

namespace BizHawk.Emulation.Cores.Nintendo.Sameboy
{
	/// <summary>
	/// static bindings into libsameboy.dll
	/// </summary>
	public static class LibSameboy
	{
		[Flags]
		public enum Buttons : uint
		{
			RIGHT = 0x01,
			LEFT = 0x02,
			UP = 0x04,
			DOWN = 0x08,
			A = 0x10,
			B = 0x20,
			SELECT = 0x40,
			START = 0x80,
		}

		// mirror of GB_direct_access_t
		public enum MemoryAreas : uint
		{
			ROM,
			RAM,
			CART_RAM,
			VRAM,
			HRAM,
			IO,
			BOOTROM,
			OAM,
			BGP,
			OBP,
			IE,
			BGPRGB,
			OBPRGB,
		}

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr sameboy_create(byte[] romdata, int romlength, byte[] biosdata, int bioslength, Sameboy.SameboySyncSettings.GBModel model, bool realtime);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern void sameboy_destroy(IntPtr core);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void InputCallback();

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern void sameboy_setinputcallback(IntPtr core, InputCallback callback);
		 
		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern void sameboy_frameadvance(IntPtr core, Buttons buttons, ushort x, ushort y, short[] soundbuf, ref int nsamps, int[] videobuf, bool render, bool border);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern void sameboy_reset(IntPtr core);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool sameboy_iscgbdmg(IntPtr core);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern void sameboy_savesram(IntPtr core, byte[] dest);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern void sameboy_loadsram(IntPtr core, byte[] data, int len);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern int sameboy_sramlen(IntPtr core);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern void sameboy_savestate(IntPtr core, byte[] data);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool sameboy_loadstate(IntPtr core, byte[] data, int len);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern int sameboy_statelen(IntPtr core);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool sameboy_getmemoryarea(IntPtr core, MemoryAreas which, ref IntPtr data, ref int length);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern byte sameboy_cpuread(IntPtr core, ushort addr);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern void sameboy_cpuwrite(IntPtr core, ushort addr, byte val);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern long sameboy_getcyclecount(IntPtr core);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern void sameboy_setcyclecount(IntPtr core, long newcc);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void TraceCallback(ushort pc);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern void sameboy_settracecallback(IntPtr core, TraceCallback callback);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern void sameboy_getregs(IntPtr core, int[] buf);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern void sameboy_setreg(IntPtr core, int which, int value);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void MemoryCallback(ushort address);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern void sameboy_setmemorycallback(IntPtr core, int which, MemoryCallback callback);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern void sameboy_setprintercallback(IntPtr core, PrinterCallback callback);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern void sameboy_setscanlinecallback(IntPtr core, ScanlineCallback callback, int sl);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern void sameboy_setpalette(IntPtr core, Sameboy.SameboySettings.GBPaletteType which);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern void sameboy_setcolorcorrection(IntPtr core, Sameboy.SameboySettings.ColorCorrectionMode which);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern void sameboy_setlighttemperature(IntPtr core, int temperature);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern void sameboy_sethighpassfilter(IntPtr core, Sameboy.SameboySettings.HighPassFilterMode which);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern void sameboy_setinterferencevolume(IntPtr core, int volume);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern void sameboy_setrtcdivisoroffset(IntPtr core, int offset);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern void sameboy_setbgwinenabled(IntPtr core, bool enabled);

		[DllImport("libsameboy", CallingConvention = CallingConvention.Cdecl)]
		public static extern void sameboy_setobjenabled(IntPtr core, bool enabled);
	}
}
