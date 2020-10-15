﻿using System;
using System.Threading;

using BizHawk.Bizware.DirectX;
using BizHawk.Emulation.Common;
using BizHawk.Client.Common;
using BizHawk.Common;

namespace BizHawk.Client.EmuHawk
{
	/// <remarks>TODO rename to <c>HostAudioManager</c></remarks>
	public class Sound : IHostAudioManager, IDisposable
	{
		public int SampleRate { get; } = 44100;

		public int BytesPerSample { get; } = 2;

		public int ChannelCount { get; } = 2;

		public int BlockAlign { get; }

		private bool _disposed;
		private readonly ISoundOutput _outputDevice;
		private readonly SoundOutputProvider _outputProvider = new SoundOutputProvider(); // Buffer for Sync sources
		private readonly BufferedAsync _bufferedAsync = new BufferedAsync(); // Buffer for Async sources
		private IBufferedSoundProvider _bufferedProvider; // One of the preceding buffers, or null if no source is set

		public int ConfigBufferSizeMs => GlobalWin.Config.SoundBufferSizeMs;

		public string ConfigDevice => GlobalWin.Config.SoundDevice;

		public Sound(IntPtr mainWindowHandle)
		{
			BlockAlign = BytesPerSample * ChannelCount;

			if (OSTailoredCode.IsUnixHost)
			{
				// if DirectSound or XAudio is chosen, use OpenAL, otherwise comply with the user's choice
				_outputDevice = GlobalWin.Config.SoundOutputMethod == ESoundOutputMethod.Dummy
					? (ISoundOutput) new DummySoundOutput(this)
					: new OpenALSoundOutput(this);
			}
			else
			{
				_outputDevice = GlobalWin.Config.SoundOutputMethod switch
				{
					ESoundOutputMethod.DirectSound => new DirectSoundSoundOutput(this, mainWindowHandle, GlobalWin.Config.SoundDevice),
					ESoundOutputMethod.XAudio2 => new XAudio2SoundOutput(this),
					ESoundOutputMethod.OpenAL => new OpenALSoundOutput(this),
					_ => new DummySoundOutput(this)
				};
			}
		}

		/// <summary>
		/// The maximum number of milliseconds the sound output buffer can go below full before causing a noticeable sound interruption.
		/// </summary>
		public int SoundMaxBufferDeficitMs { get; set; }

		public void Dispose()
		{
			if (_disposed) return;

			StopSound();

			_outputDevice.Dispose();

			_disposed = true;
		}

		public bool IsStarted { get; private set; }

		public void StartSound()
		{
			if (_disposed) return;
			if (!GlobalWin.Config.SoundEnabled) return;
			if (IsStarted) return;

			_outputDevice.StartSound();

			_outputProvider.MaxSamplesDeficit = _outputDevice.MaxSamplesDeficit;

			SoundMaxBufferDeficitMs = (int) Math.Ceiling(this.SamplesToMilliseconds(_outputDevice.MaxSamplesDeficit));

			IsStarted = true;
		}

		public void StopSound()
		{
			if (!IsStarted) return;

			_outputDevice.StopSound();

			_bufferedProvider?.DiscardSamples();

			SoundMaxBufferDeficitMs = 0;

			IsStarted = false;
		}

		/// <summary>
		/// Attaches a new input pin which will run either in sync or async mode depending
		/// on its SyncMode property. Once attached, the sync mode must not change unless
		/// the pin is re-attached.
		/// </summary>
		public void SetInputPin(ISoundProvider source)
		{
			if (_bufferedProvider != null)
			{
				_bufferedProvider.BaseSoundProvider = null;
				_bufferedProvider.DiscardSamples();
				_bufferedProvider = null;
			}

			if (source == null) return;

			if (source.SyncMode == SyncSoundMode.Sync)
			{
				_bufferedProvider = _outputProvider;
			}
			else if (source.SyncMode == SyncSoundMode.Async)
			{
				_bufferedAsync.RecalculateMagic(GlobalWin.Emulator.VsyncRate());
				_bufferedProvider = _bufferedAsync;
			}
			else throw new InvalidOperationException("Unsupported sync mode.");

			_bufferedProvider.BaseSoundProvider = source;
		}

		public bool LogUnderruns { get; set; }

		public void HandleInitializationOrUnderrun(bool isUnderrun, ref int samplesNeeded)
		{
			// Fill device buffer with silence but leave enough room for one frame
			int samplesPerFrame = (int)Math.Round(SampleRate / (double)GlobalWin.Emulator.VsyncRate());
			int silenceSamples = Math.Max(samplesNeeded - samplesPerFrame, 0);
			_outputDevice.WriteSamples(new short[silenceSamples * 2], 0, silenceSamples);
			samplesNeeded -= silenceSamples;

			if (isUnderrun)
			{
				if (LogUnderruns) Console.WriteLine("Sound underrun detected!");
				_outputProvider.OnVolatility();
			}
		}

		public void UpdateSound(float atten)
		{
			if (!GlobalWin.Config.SoundEnabled || !IsStarted || _bufferedProvider == null || _disposed)
			{
				_bufferedProvider?.DiscardSamples();
				return;
			}

			if (atten < 0) atten = 0;
			if (atten > 1) atten = 1;
			_outputDevice.ApplyVolumeSettings(atten);

			int samplesNeeded = _outputDevice.CalculateSamplesNeeded();
			short[] samples;
			int sampleOffset;
			int sampleCount;

			if (atten == 0)
			{
				samples = new short[samplesNeeded * ChannelCount];
				sampleOffset = 0;
				sampleCount = samplesNeeded;

				_bufferedProvider.DiscardSamples();
			}
			else if (_bufferedProvider == _outputProvider)
			{
				if (GlobalWin.Config.SoundThrottle)
				{
					_outputProvider.BaseSoundProvider.GetSamplesSync(out samples, out sampleCount);
					sampleOffset = 0;

					if (GlobalWin.DisableSecondaryThrottling && sampleCount > samplesNeeded)
					{
						return;
					}

					int samplesPerMs = SampleRate / 1000;
					int outputThresholdMs = 20;
					while (sampleCount > samplesNeeded)
					{
						if (samplesNeeded >= outputThresholdMs * samplesPerMs)
						{
							// If we were given a large enough number of samples (e.g. larger than the device's
							// buffer), the device will never need that many samples no matter how long we
							// wait, so we have to start splitting up the output
							_outputDevice.WriteSamples(samples, sampleOffset, samplesNeeded);
							sampleOffset += samplesNeeded;
							sampleCount -= samplesNeeded;
						}
						else
						{
							// Let the audio clock control sleep time
							Thread.Sleep(Math.Min((sampleCount - samplesNeeded) / samplesPerMs, outputThresholdMs));
						}
						samplesNeeded = _outputDevice.CalculateSamplesNeeded();
					}
				}
				else
				{
					if (GlobalWin.DisableSecondaryThrottling) // This indicates rewind or fast-forward
					{
						_outputProvider.OnVolatility();
					}
					_outputProvider.GetSamples(samplesNeeded, out samples, out sampleCount);
					sampleOffset = 0;
				}
			}
			else if (_bufferedProvider == _bufferedAsync)
			{
				samples = new short[samplesNeeded * ChannelCount];

				_bufferedAsync.GetSamplesAsync(samples);

				sampleOffset = 0;
				sampleCount = samplesNeeded;
			}
			else
			{
				return;
			}

			_outputDevice.WriteSamples(samples, sampleOffset, sampleCount);
		}
	}
}
