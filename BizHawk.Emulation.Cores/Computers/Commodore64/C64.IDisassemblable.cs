﻿using System;
using System.Collections.Generic;

using BizHawk.Emulation.Common;

namespace BizHawk.Emulation.Cores.Computers.Commodore64
{
	public partial class C64 : IDisassemblable
	{
        [SaveState.DoNotSave]
		public string Cpu
		{
			get { return "6510"; } set { }
		}

        [SaveState.DoNotSave]
        public string PCRegisterName
		{
			get { return "PC"; }
		}

        [SaveState.DoNotSave]
        public IEnumerable<string> AvailableCpus
		{
			get { yield return "6510"; }
		}

		public string Disassemble(MemoryDomain m, uint addr, out int length)
		{
			return Components.M6502.MOS6502X.Disassemble((ushort)addr, out length, (a) => m.PeekByte(a));
		}
	}
}
