﻿using BizHawk.Client.Common;

namespace BizHawk.Client.EmuHawk
{
	public partial class TAStudio : IControlMainform
	{
		private bool _suppressAskSave;

		public bool NamedStatePending { get; set; }

		public bool WantsToControlSavestates => !NamedStatePending;

		public void SaveState()
		{
			BookMarkControl.UpdateBranchExternal();
		}

		public void LoadState()
		{
			BookMarkControl.LoadBranchExternal();
		}

		public void SaveStateAs()
		{
			// dummy
		}

		public void LoadStateAs()
		{
			// dummy
		}

		public void SaveQuickSave(int slot)
		{
			BookMarkControl.UpdateBranchExternal(slot is 0 ? 9 : slot - 1);
		}

		public void LoadQuickSave(int slot)
		{
			BookMarkControl.LoadBranchExternal(slot is 0 ? 9 : slot - 1);
		}

		public bool SelectSlot(int slot)
		{
			BookMarkControl.SelectBranchExternal(slot is 0 ? 9 : slot - 1);
			return false;
		}

		public bool PreviousSlot()
		{
			BookMarkControl.SelectBranchExternal(false);
			return false;
		}

		public bool NextSlot()
		{
			BookMarkControl.SelectBranchExternal(true);
			return false;
		}

		public bool WantsToControlReadOnly => true;

		public void ToggleReadOnly()
		{
			if (CurrentTasMovie.IsPlayingOrFinished())
			{
				TastudioRecordMode();
			}
			else if (CurrentTasMovie.IsRecording())
			{
				TastudioPlayMode();
			}
		}

		public bool WantsToControlStopMovie { get; private set; }

		public void StopMovie(bool suppressSave)
		{
			if (!MainForm.GameIsClosing)
			{
				Focus();
				_suppressAskSave = suppressSave;
				NewTasMenuItem_Click(null, null);
				_suppressAskSave = false;
			}
		}

		public bool WantsToControlRewind => true;

		public void CaptureRewind()
		{
			// Do nothing, Tastudio handles this just fine
		}

		public bool Rewind()
		{
			// copy pasted from TasView_MouseWheel(), just without notch logic
			if (MainForm.IsSeeking && !MainForm.EmulatorPaused)
			{
				MainForm.PauseOnFrame--;

				// that's a weird condition here, but for whatever reason it works best
				if (Emulator.Frame >= MainForm.PauseOnFrame)
				{
					MainForm.PauseEmulator();
					StopSeeking();
					GoToPreviousFrame();
				}

				RefreshDialog();
			}
			else
			{
				StopSeeking(); // late breaking memo: don't know whether this is needed
				GoToPreviousFrame();
			}

			return true;
		}

		public bool WantsToControlRestartMovie { get; }

		public bool RestartMovie()
		{
			if (!AskSaveChanges()) return false;
			WantsToControlStopMovie = false;
			var success = StartNewMovieWrapper(CurrentTasMovie);
			WantsToControlStopMovie = true;
			RefreshDialog();
			return success;
		}

		public bool WantsToControlReboot { get; private set; } = true;

		public void RebootCore()
		{
			WantsToControlReboot = false;
			NewTasMenuItem_Click(null, null);
			WantsToControlReboot = true;
		}
	}
}
