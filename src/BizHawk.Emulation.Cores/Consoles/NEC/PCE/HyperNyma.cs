using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using BizHawk.BizInvoke;
using BizHawk.Common;
using BizHawk.Emulation.Common;
using BizHawk.Emulation.Cores.PCEngine;
using BizHawk.Emulation.Cores.Waterbox;

namespace BizHawk.Emulation.Cores.Consoles.NEC.PCE
{
	[Core(CoreNames.HyperNyma, "Mednafen Team", true, true, "1.24.3", "https://mednafen.github.io/releases/", false, "PCE")]
	public class HyperNyma : NymaCore, IRegionable, IPceGpuView
	{
		private readonly LibHyperNyma _hyperNyma;
		private readonly bool _hasCds;

		[CoreConstructor("PCE", Priority = CorePriority.Low)]
		[CoreConstructor("SGX", Priority = CorePriority.Low)]
		[CoreConstructor("PCECD", Priority = CorePriority.Low)]
		public HyperNyma(CoreLoadParameters<NymaSettings, NymaSyncSettings> lp)
			: base(lp.Comm, "PCE", "PC Engine Controller", lp.Settings, lp.SyncSettings)
		{
			var firmwares = new Dictionary<string, (string, string)>();
			if (lp.Discs.Count > 0)
			{
				_hasCds = true;
				firmwares.Add("FIRMWARE:syscard3.pce", ("PCECD", "Bios"));
			}

			_hyperNyma = DoInit<LibHyperNyma>(lp, "hyper.wbx", firmwares);
		}

		public override string SystemId => IsSgx
			? _hasCds ? "SGXCD" : "SGX"
			: _hasCds ? "PCECD" : "PCE";

		protected override IDictionary<string, SettingOverride> SettingOverrides { get; } = new Dictionary<string, SettingOverride>
		{
			{ "pce_fast.mouse_sensitivity", new SettingOverride { Hide = true } },
			{ "pce_fast.disable_softreset", new SettingOverride { Hide = true } },
			{ "pce_fast.cdbios", new SettingOverride { Hide = true } },
			{ "nyma.rtcinitialtime", new SettingOverride { Hide = true } },
			{ "nyma.rtcrealtime", new SettingOverride { Hide = true } },
			{ "pce_fast.slstart", new SettingOverride { NonSync = true, NoRestart = true } },
			{ "pce_fast.slend", new SettingOverride { NonSync = true, NoRestart = true } },

			{ "pce_fast.correct_aspect", new SettingOverride { NonSync = true } },
			{ "pce_fast.nospritelimit", new SettingOverride { NonSync = true } },
		};

		// pce always has two layers, sgx always has 4, and mednafen knows this
		public bool IsSgx => SettingsInfo.LayerNames.Count == 4;

		public unsafe void GetGpuData(int vdc, Action<PceGpuData> callback)
		{
			using(_exe.EnterExit())
			{
				var palScratch = new int[512];
				var v = new PceGpuData();
				_hyperNyma.GetVramInfo(v, vdc);
				fixed(int* p = palScratch)
				{
					for (var i = 0; i < 512; i++)
						p[i] = v.PaletteCache[i] | unchecked((int)0xff000000);
					v.PaletteCache = p;
					callback(v);
				}
				
			}
		}
	}

	public abstract class LibHyperNyma : LibNymaCore
	{
		[BizImport(CallingConvention.Cdecl, Compatibility = true)]
		public abstract void GetVramInfo([Out]PceGpuData v, int vdcIndex);
	}
}
