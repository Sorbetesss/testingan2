using System;
using System.Runtime.InteropServices;

using BizHawk.BizInvoke;
using BizHawk.Emulation.Cores.Waterbox;

namespace BizHawk.Emulation.Cores.Consoles.Nintendo.Ares64
{
	public abstract class LibAres64 : LibWaterboxCore
	{
		[Flags]
		public enum Buttons : uint
		{
			UP      = 1 <<  0,
			DOWN    = 1 <<  1,
			LEFT    = 1 <<  2,
			RIGHT   = 1 <<  3,
			B       = 1 <<  4,
			A       = 1 <<  5,
			C_UP    = 1 <<  6,
			C_DOWN  = 1 <<  7,
			C_LEFT  = 1 <<  8,
			C_RIGHT = 1 <<  9,
			L       = 1 << 10,
			R       = 1 << 11,
			Z       = 1 << 12,
			START   = 1 << 13,
		}

		public enum ControllerType : uint
		{
			Unplugged,
			Standard,
			Mempak,
			Rumblepak,
		}

		[StructLayout(LayoutKind.Sequential)]
		public new class FrameInfo : LibWaterboxCore.FrameInfo
		{
			public Buttons P1Buttons;
			public short P1XAxis;
			public short P1YAxis;

			public Buttons P2Buttons;
			public short P2XAxis;
			public short P2YAxis;

			public Buttons P3Buttons;
			public short P3XAxis;
			public short P3YAxis;

			public Buttons P4Buttons;
			public short P4XAxis;
			public short P4YAxis;

			public bool Reset;
			public bool Power;
		}

		[BizImport(CC)]
		public abstract bool Init(ControllerType[] controllerSettings, bool pal);

		[BizImport(CC)]
		public abstract bool GetRumbleStatus(int num);
	}
}
