using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using BizHawk.Common;
using BizHawk.Client.Common;
using BizHawk.Emulation.Common;

namespace BizHawk.Client.EmuHawk
{
	public partial class RAIntegration : RetroAchievements
	{
		private static RAInterface RA;
		public static bool IsAvailable => RA != null;

		static RAIntegration()
		{
			try
			{
				if (OSTailoredCode.IsUnixHost)
				{
					throw new NotSupportedException("RAIntegration is Windows only!");
				}

				AttachDll();
			}
			catch
			{
				DetachDll();
			}
		}

		private readonly RAInterface.IsActiveDelegate _isActive;
		private readonly RAInterface.UnpauseDelegate _unpause;
		private readonly RAInterface.PauseDelegate _pause;
		private readonly RAInterface.RebuildMenuDelegate _rebuildMenu;
		private readonly RAInterface.EstimateTitleDelegate _estimateTitle;
		private readonly RAInterface.ResetEmulatorDelegate _resetEmulator;
		private readonly RAInterface.LoadROMDelegate _loadROM;

		private readonly RAInterface.MenuItem[] _menuItems = new RAInterface.MenuItem[40];

		// Memory may be accessed by another thread (for rich presence)
		// and peeks for us are not thread safe, so we need to guard it
		private readonly AutoResetEvent _memAccessReady = new(false);
		private readonly AutoResetEvent _memAccessGo = new(false);
		private readonly AutoResetEvent _memAccessDone = new(false);
		private readonly RAMemGuard _memGuard;

		private void RebuildMenu()
		{
			var numItems = RA.GetPopupMenuItems(_menuItems);
			var tsmiddi = _raDropDownItems;
			tsmiddi.Clear();
			{
				var tsi = new ToolStripMenuItem("Shutdown RetroAchievements");
				tsi.Click += (_, _) => _shutdownRACallback();
				tsmiddi.Add(tsi);

				tsi = new ToolStripMenuItem("Autostart RetroAchievements")
				{
					Checked = _getConfig().RAAutostart,
					CheckOnClick = true,
				};
				tsi.CheckedChanged += (_, _) => _getConfig().RAAutostart ^= true;
				tsmiddi.Add(tsi);

				var tss = new ToolStripSeparator();
				tsmiddi.Add(tss);
			}
			for (int i = 0; i < numItems; i++)
			{
				if (_menuItems[i].Label != IntPtr.Zero)
				{
					var tsi = new ToolStripMenuItem(Marshal.PtrToStringUni(_menuItems[i].Label))
					{
						Checked = _menuItems[i].Checked != 0,
					};
					var id = _menuItems[i].ID;
					tsi.Click += (_, _) =>
					{
						RA.InvokeDialog(id);
						_mainForm.UpdateWindowTitle();
					};
					tsmiddi.Add(tsi);
				}
				else
				{
					var tss = new ToolStripSeparator();
					tsmiddi.Add(tss);
				}
			}
		}

		protected override void HandleHardcoreModeDisable(string reason)
		{
			_mainForm.ShowMessageBox(null, $"{reason} Disabling hardcore mode.", "Warning", EMsgBoxIcon.Warning);
			RA.WarnDisableHardcore(null);
		}

		protected override int IdentifyHash(string hash)
			=> RA.IdentifyHash(hash);

		protected override int IdentifyRom(byte[] rom)
			=> RA.IdentifyRom(rom, rom.Length);

		public RAIntegration(IMainFormForRetroAchievements mainForm, InputManager inputManager, ToolManager tools,
			Func<Config> getConfig, ToolStripItemCollection raDropDownItems, Action shutdownRACallback)
			: base(mainForm, inputManager, tools, getConfig, raDropDownItems, shutdownRACallback)
		{
			_memGuard = new(_memAccessReady, _memAccessGo, _memAccessDone);

			RA.InitClient(_mainForm.Handle, "BizHawk", VersionInfo.GetEmuVersion());

			_isActive = () => !Emu.IsNull();
			_unpause = _mainForm.UnpauseEmulator;
			_pause = _mainForm.PauseEmulator;
			_rebuildMenu = RebuildMenu;
			_estimateTitle = buffer =>
			{
				var name = Encoding.UTF8.GetBytes(Game?.Name ?? "No Game Info Available");
				Marshal.Copy(name, 0, buffer, Math.Min(name.Length, 256));
			};
			_resetEmulator = () => _mainForm.RebootCore();
			_loadROM = path => _mainForm.LoadRom(path, new LoadRomArgs { OpenAdvanced = OpenAdvancedSerializer.ParseWithLegacy(path) });

			RA.InstallSharedFunctionsExt(_isActive, _unpause, _pause, _rebuildMenu, _estimateTitle, _resetEmulator, _loadROM);

			RA.AttemptLogin(true);
		}

		public override void Dispose()
		{
			RA?.Shutdown();
			_memGuard.Dispose();
		}

		public override void OnSaveState(string path)
			=> RA.OnSaveState(path);

		public override void OnLoadState(string path)
		{
			if (RA.HardcoreModeIsActive())
			{
				HandleHardcoreModeDisable("Loading savestates is not allowed in hardcore mode.");
			}

			RA.OnLoadState(path);
		}

		public override void Stop()
		{
			RA.ClearMemoryBanks();
			RA.ActivateGame(0);
		}

		public override void Restart()
		{
			var consoleId = SystemIdToConsoleId();
			RA.SetConsoleID(consoleId);

			RA.ClearMemoryBanks();

			if (Emu.HasMemoryDomains())
			{
				_memFunctions = CreateMemoryBanks(consoleId, Domains, Emu.CanDebug() ? Emu.AsDebuggable() : null);

				for (int i = 0; i < _memFunctions.Count; i++)
				{
					_memFunctions[i].MemGuard = _memGuard;
					RA.InstallMemoryBank(i, _memFunctions[i].ReadFunc, _memFunctions[i].WriteFunc, _memFunctions[i].BankSize);
					RA.InstallMemoryBankBlockReader(i, _memFunctions[i].ReadBlockFunc);
				}
			}

			AllGamesVerified = true;

			if (_mainForm.CurrentlyOpenRomArgs is not null)
			{
				var ids = GetRAGameIds(_mainForm.CurrentlyOpenRomArgs.OpenAdvanced, consoleId);

				AllGamesVerified = !ids.Contains(0);

				RA.ActivateGame(ids.Count > 0 ? ids[0] : 0);
			}
			else
			{
				RA.ActivateGame(0);
			}

			Update();
			RebuildMenu();

			// workaround a bug in RA which will cause the window title to be changed despite us not calling UpdateAppTitle
			_mainForm.UpdateWindowTitle();

			// note: this can only catch quicksaves (probably only case of accidential use from hotkeys)
			_mainForm.EmuClient.BeforeQuickLoad += (_, e) =>
			{
				if (RA.HardcoreModeIsActive())
				{
					e.Handled = !RA.WarnDisableHardcore("load a quicksave");
				}
			};
		}

		public override void Update()
		{
			if (RA.HardcoreModeIsActive())
			{
				CheckHardcoreModeConditions();
			}

			if (_inputManager.ClientControls["Open RA Overlay"])
			{
				RA.SetPaused(true);
			}

			if (RA.IsOverlayFullyVisible())
			{
				var ci = new RAInterface.ControllerInput
				{
					UpPressed = _inputManager.ClientControls["RA Up"],
					DownPressed = _inputManager.ClientControls["RA Down"],
					LeftPressed = _inputManager.ClientControls["RA Left"],
					RightPressed = _inputManager.ClientControls["RA Right"],
					ConfirmPressed = _inputManager.ClientControls["RA Confirm"],
					CancelPressed = _inputManager.ClientControls["RA Cancel"],
					QuitPressed = _inputManager.ClientControls["RA Quit"],
				};

				RA.NavigateOverlay(ref ci);

				// todo: suppress user inputs with overlay active?
			}

			if (_memAccessReady.WaitOne(0))
			{
				_memAccessGo.Set();
				_memAccessDone.WaitOne();
			}
		}

		public override void OnFrameAdvance()
		{
			var input = _inputManager.ControllerOutput;
			foreach (var resetButton in input.Definition.BoolButtons.Where(b => b.Contains("Power") || b.Contains("Reset")))
			{
				if (input.IsPressed(resetButton))
				{
					RA.OnReset();
					break;
				}
			}

			if (Emu.HasMemoryDomains())
			{
				// we want to EnterExit to prevent wbx host spam when peeks are spammed
				using (Domains.MainMemory.EnterExit())
				{
					RA.DoAchievementsFrame();
				}
			}
			else
			{
				RA.DoAchievementsFrame();
			}
		}
	}
}
