﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;

using BizHawk.API.ApiHawk;
using BizHawk.Common;
using BizHawk.Emulation.Common;
using BizHawk.Emulation.Cores.Waterbox;

namespace BizHawk.Emulation.Cores.Consoles.SNK
{
	[Core("NeoPop", "Thomas Klausner, Mednafen Team", true, true, "1.24.3",
		"https://mednafen.github.io/releases/", false)]
	public class NeoGeoPort : NymaCore,
		ISaveRam // NGP provides its own saveram interface
	{
		private readonly LibNeoGeoPort _neopop;

		[CoreConstructor("NGP")]
		public NeoGeoPort(CoreComm comm, byte[] rom, GameInfo game,
			NymaSettings settings, NymaSyncSettings syncSettings, bool deterministic, string extension)
			: base(comm, "NGP", "NeoGeo Portable Controller", settings, syncSettings)
		{
			_neopop = DoInit<LibNeoGeoPort>(game, rom, null, "ngp.wbx", extension, deterministic);
		}

		public new bool SaveRamModified
		{
			get
			{
				_exe.AddTransientFile(new byte[0], "SAV:flash");
				if (!_neopop.GetSaveRam())
					throw new InvalidOperationException("Error divining saveram");
				return _exe.RemoveTransientFile("SAV:flash").Length > 0;
			}
		}

		public new byte[] CloneSaveRam()
		{
			_exe.AddTransientFile(new byte[0], "SAV:flash");

			if (!_neopop.GetSaveRam())
				throw new InvalidOperationException("Error returning saveram");
			return _exe.RemoveTransientFile("SAV:flash");
		}

		public new void StoreSaveRam(byte[] data)
		{
			_exe.AddTransientFile(data, "SAV:flash");
			if (!_neopop.PutSaveRam())
				throw new InvalidOperationException("Core rejected the saveram");
			_exe.RemoveTransientFile("SAV:flash");
		}
	}
}
