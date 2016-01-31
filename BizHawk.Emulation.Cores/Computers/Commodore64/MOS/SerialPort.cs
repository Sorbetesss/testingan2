﻿using System;
using BizHawk.Common;

namespace BizHawk.Emulation.Cores.Computers.Commodore64.MOS
{
	// the functions on this port are at the point of
	// view of an external device.

	public sealed class SerialPort
	{
		public Func<bool> ReadAtnOut;
		public Func<bool> ReadClockOut;
		public Func<bool> ReadDataOut;

		public void HardReset()
		{
		}

		public void SyncState(Serializer ser)
		{
			SaveState.SyncObject(ser, this);
		}

		public bool WriteClockIn()
		{
			return true;
		}

		public bool WriteDataIn()
		{
			return true;
		}
	}
}
