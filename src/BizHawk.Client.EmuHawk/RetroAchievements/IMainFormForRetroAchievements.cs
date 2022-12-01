using System;

using BizHawk.Client.Common;
using BizHawk.Emulation.Common;

namespace BizHawk.Client.EmuHawk
{
	public interface IMainFormForRetroAchievements : IDialogController
	{
		LoadRomArgs CurrentlyOpenRomArgs { get; }

		EmuClientApi EmuClient { get; }

		IEmulator Emulator { get; }

		bool FrameInch { get; set; }

		bool FastForward { get; set; }

		GameInfo Game { get; }

		IntPtr Handle { get; }

		IMovieSession MovieSession { get; }

		SettingsAdapter GetSettingsAdapterForLoadedCoreUntyped();

		bool LoadRom(string path, LoadRomArgs args);

		void PauseEmulator();

		bool RebootCore();

		void UpdateWindowTitle();

		void UnpauseEmulator();
	}
}
