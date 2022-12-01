﻿using System;
using System.Runtime.InteropServices;
using BizHawk.Common;
using BizHawk.Emulation.Common;

namespace BizHawk.Emulation.Cores.Nintendo.BSNES
{
	public partial class BsnesCore : IEmulator
	{
		public IEmulatorServiceProvider ServiceProvider { get; }

		public ControllerDefinition ControllerDefinition => _controllers.Definition;

		private short[] _audioBuffer = Array.Empty<short>();

		public bool FrameAdvance(IController controller, bool render, bool renderSound)
		{
			using (Api.EnterExit())
			{
				FrameAdvancePre(controller, render, renderSound);

				bool resetSignal = controller.IsPressed("Reset");
				if (resetSignal)
				{
					Api.core.snes_reset();
				}

				bool powerSignal = controller.IsPressed("Power");
				if (powerSignal)
				{
					Api.core.snes_power();
				}

				IsLagFrame = true;
				// run the core for one frame
				Api.core.snes_run(false);
				FrameAdvancePost();

				return true;
			}
		}

		internal void FrameAdvancePre(IController controller, bool render, bool renderSound)
		{
			_controller = controller;

			var enables = new BsnesApi.LayerEnables
			{
				BG1_Prio0 = _settings.ShowBG1_0,
				BG1_Prio1 = _settings.ShowBG1_1,
				BG2_Prio0 = _settings.ShowBG2_0,
				BG2_Prio1 = _settings.ShowBG2_1,
				BG3_Prio0 = _settings.ShowBG3_0,
				BG3_Prio1 = _settings.ShowBG3_1,
				BG4_Prio0 = _settings.ShowBG4_0,
				BG4_Prio1 = _settings.ShowBG4_1,
				Obj_Prio0 = _settings.ShowOBJ_0,
				Obj_Prio1 = _settings.ShowOBJ_1,
				Obj_Prio2 = _settings.ShowOBJ_2,
				Obj_Prio3 = _settings.ShowOBJ_3
			};
			// TODO: I really don't think stuff like this should be set every single frame (only on change)
			Api.core.snes_set_layer_enables(ref enables);
			Api.core.snes_set_hooks_enabled(MemoryCallbacks.HasReads, MemoryCallbacks.HasWrites, MemoryCallbacks.HasExecutes);
			Api.core.snes_set_trace_enabled(_tracer.IsEnabled());
			Api.core.snes_set_video_enabled(render);
			Api.core.snes_set_audio_enabled(renderSound);
			Api.core.snes_set_ppu_sprite_limit_enabled(!_settings.NoPPUSpriteLimit);
			Api.core.snes_set_overscan_enabled(_settings.ShowOverscan);
		}

		internal void FrameAdvancePost()
		{
			int numSamples = UpdateAudioBuffer();
			_soundProvider.PutSamples(_audioBuffer, numSamples / 2);
			Frame++;

			if (IsLagFrame)
			{
				LagCount++;
			}
		}

		private int UpdateAudioBuffer()
		{
			var rawAudioBuffer = Api.core.snes_get_audiobuffer_and_size(out var size);
			if (size == 0) return 0;
			if (size > _audioBuffer.Length)
				_audioBuffer = new short[size];
			Marshal.Copy(rawAudioBuffer, _audioBuffer, 0, size);

			return size;
		}

		public int Frame { get; private set; }

		public string SystemId => VSystemID.Raw.SNES;

		public bool DeterministicEmulation => true;

		public void ResetCounters()
		{
			Frame = 0;
			LagCount = 0;
			IsLagFrame = false;
		}

		public void Dispose()
		{
			if (_disposed)
			{
				return;
			}

			Api.Dispose();
			_currentMsuTrack?.Dispose();

			_disposed = true;
		}
	}
}
