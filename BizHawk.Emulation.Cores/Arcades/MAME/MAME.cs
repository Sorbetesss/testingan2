﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

using BizHawk.Emulation.Common;

namespace BizHawk.Emulation.Cores.Arcades.MAME
{
	[Core(
		name: "MAME",
		author: "MAMEDev",
		isPorted: true,
		portedVersion: "0.209",
		portedUrl: "https://github.com/mamedev/mame.git",
		singleInstance: false)]
	public partial class MAME : IEmulator, IVideoProvider
	{
		public MAME(CoreComm comm, string dir, string file)
		{
			ServiceProvider = new BasicServiceProvider(this);

			CoreComm = comm;
			GameDirectory = dir;
			GameFilename = file;

			MAMEThread = new Thread(ExecuteMAMEThread);
			AsyncLaunchMAME();
		}

		private Thread MAMEThread;
		private ManualResetEvent MAMEStartupComplete = new ManualResetEvent(false);
		private string GameDirectory;
		private string GameFilename;

		private void AsyncLaunchMAME()
		{
			MAMEThread.Start();
			MAMEStartupComplete.WaitOne();
		}

		private void ExecuteMAMEThread()
		{
			LibMAME.mame_set_boot_callback(MAMEBoot);
			LibMAME.mame_set_log_callback(MAMELog);

			string[] args = build_options(GameDirectory, GameFilename);
			LibMAME.mame_launch(args.Length, args);
		}


		// https://docs.mamedev.org/commandline/commandline-index.html
		private string[] build_options(string directory, string rom)
		{
			return new string[] {
				"mame",                 // dummy, internally discarded by index, so has to go first
				rom,                    // no dash for rom names, internally called "unadorned" option
				"-rompath", directory,  // mame doesn't load roms from full paths, only from dirs for scan
				"-window",              // forbid fullscreen
				"-nokeepaspect",        // forbid mame from stretching the window
				"-nomaximize",          // forbid windowed fullscreen
				"-noreadconfig",        // forbid reading any config files
				"-norewind",            // forbid rewind savestates, captured upon frame advance
				"-skip_gameinfo",       // forbid showing this blocking screen that requires user input
				"-video", "none"
			};
		}


		private void MAMEBoot()
		{
			LibMAME.mame_lua_execute("emu.pause()");
			_updateVideo();
			MAMEStartupComplete.Set();
			//int ok = lol;
		}


		// mame sends osd_output_channel casted to int, we implicitly cast is back
		private void MAMELog(LibMAME.osd_output_channel channel, int size, string data)
		{
			Console.WriteLine(string.Format(
				"[MAME {0}] {1}",
				channel.ToString(),
				data.Replace('\n', ' ')));
		}


		public CoreComm CoreComm { get; private set; }

		public IEmulatorServiceProvider ServiceProvider { get; private set; }

		public ControllerDefinition ControllerDefinition => MAMEController;

		public static readonly ControllerDefinition MAMEController = new ControllerDefinition
		{
			Name = "MAME Controller",
			BoolButtons =
			{
				"Up", "Down", "Left", "Right", "Start", "Select", "B", "A", "L", "R", "Power"
			}
		};

		private int[] frameBuffer = new int[0];
		public string SystemId => "MAME";
		public int[] GetVideoBuffer() { return frameBuffer; }
		public int Frame { get; private set; }
		public bool DeterministicEmulation => true;
		public int VirtualWidth => 240;
		public int VirtualHeight => 320;
		public int BufferWidth { get; private set; }
		public int BufferHeight { get; private set; }
		public int VsyncNumerator => 60;
		public int VsyncDenominator => 1;
		public int BackgroundColor => unchecked((int)0xFF0000FF);

		private void _updateVideo()
		{
			int length;
			BufferWidth = LibMAME.mame_lua_get_int("return manager:machine():render():ui_target():width()");
			BufferHeight = LibMAME.mame_lua_get_int("return manager:machine():render():ui_target():height()");
			IntPtr ptr = LibMAME.mame_lua_get_string("return manager:machine().screens[\":screen\"]:pixels()", out length);
			frameBuffer = new int[BufferWidth * BufferHeight];
			Marshal.Copy(ptr, frameBuffer, 0, BufferWidth * BufferHeight);
			//string see = Marshal.PtrToStringAnsi(ptr, length);
			bool test = LibMAME.mame_lua_free_string(ptr);
		}

		public bool FrameAdvance(IController controller, bool render, bool rendersound = true)
		{
			Frame++;
			LibMAME.mame_lua_execute("emu.step()");
			_updateVideo();
			return true;
		}

		public void ResetCounters()
		{
			Frame = 0;
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~MAME() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}

		#endregion

	}
}
