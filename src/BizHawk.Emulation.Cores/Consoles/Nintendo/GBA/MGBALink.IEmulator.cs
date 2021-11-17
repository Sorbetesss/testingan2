using System;
using System.Linq;

using BizHawk.Common;
using BizHawk.Emulation.Common;

namespace BizHawk.Emulation.Cores.Nintendo.GBA
{
	public partial class MGBALink : IEmulator
	{
		public IEmulatorServiceProvider ServiceProvider => _serviceProvider;

		public ControllerDefinition ControllerDefinition => GBALinkController;

		private static ControllerDefinition GBALinkController { get; set; }

		private ControllerDefinition CreateControllerDefinition()
		{
			var ret = new ControllerDefinition { Name = $"GBA Link {_numCores}x Controller" };
			for (int i = 0; i < _numCores; i++)
			{
				ret.BoolButtons.AddRange(
					new[] { "Up", "Down", "Left", "Right", "Start", "Select", "B", "A", "L", "R", "Power" }
						.Select(s => $"P{i + 1} {s}"));
				ret.AddXYZTriple($"P{i + 1} " + "Tilt {0}", (-32767).RangeTo(32767), 0);
				ret.AddAxis($"P{i + 1} " + "Light Sensor", 0.RangeTo(255), 0);
			}
			return ret;
		}

		private long RTCTime(int coreNum)
		{
			if (!_linkedCores[coreNum].DeterministicEmulation && _linkedCores[coreNum].GetSyncSettings().RTCUseRealTime)
			{
				return (long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
			}

			long baseTime = (long)_linkedCores[coreNum].GetSyncSettings().RTCInitialTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
			long increment = Frame * 4389L >> 18;
			return baseTime + increment;
		}

		public bool FrameAdvance(IController controller, bool render, bool rendersound = true)
		{
			for (int i = 0; i < _numCores; i++)
			{
				_linkedConts[i].Clear();
			}

			foreach (var s in GBALinkController.BoolButtons)
			{
				if (controller.IsPressed(s))
				{
					for (int i = 0; i < _numCores; i++)
					{
						if (s.Contains($"P{i + 1} "))
						{
							_linkedConts[i].Set(s.Replace($"P{i + 1} ", ""));
						}
					}
				}
			}

			foreach (var s in GBALinkController.Axes)
			{
				for (int i = 0; i < _numCores; i++)
				{
					if (s.Key.Contains($"P{i + 1} "))
					{
						_linkedConts[i].SetAxisValue(s.Key.Replace($"P{i + 1} ", ""), controller.AxisValue(s.Key));
					}
				}
			}

			IsLagFrame = true;

			// todo: link!
			for (int i = 0; i < _numCores; i++)
			{
				if (_linkedConts[i].IsPressed("Power"))
				{
					_linkedCores[i].Reset();
				}
				MGBAHawk.LibmGBA.BizStepPrep(
					_linkedCores[i].Core,
					LibmGBA.GetButtons(_linkedConts[i]),
					RTCTime(i),
					(short)_linkedConts[i].AxisValue("Tilt X"),
					(short)_linkedConts[i].AxisValue("Tilt Y"),
					(short)_linkedConts[i].AxisValue("Tilt Z"),
					(byte)(255 - _linkedConts[i].AxisValue("Light Sensor")));
			}

			while (true)
			{
				for (int i = 0; i < _numCores; i++)
				{
					if (_frameOverflow[i] < CyclesPerFrame)
					{
						if (_stepOverflow[i] < StepLength)
						{
							int stepTarget = StepLength - _stepOverflow[i];
							if (_frameOverflow[i] + stepTarget > CyclesPerFrame)
							{
								stepTarget -= CyclesPerFrame - _frameOverflow[i];
							}
							unsafe
							{
								fixed (int* vbuff = &_videobuff[i * 240])
								{
									_stepOverflow[i] = MGBAHawk.LibmGBA.BizStep(_linkedCores[i].Core, stepTarget, vbuff, _numCores);
								}
							}
							_frameOverflow[i] += stepTarget + _stepOverflow[i];
						}
						else
						{
							_stepOverflow[i] -= StepLength; // skip a step
						}
					}
				}

				bool exit = true;
				for (int i = 0; i < _numCores; i++)
				{
					exit &= _frameOverflow[i] >= CyclesPerFrame;
				}

				if (exit)
				{
					for (int i = 0; i < _numCores; i++)
					{
						_frameOverflow[i] -= CyclesPerFrame;
					}
					break;
				}
			}

			IsLagFrame = true;
			for (int i = 0; i < _numCores; i++)
			{
				IsLagFrame &= MGBAHawk.LibmGBA.BizStepPost(_linkedCores[i].Core, ref _linkedCores[i]._nsamp, _linkedCores[i]._soundbuff);
			}

			unsafe
			{
				_linkedCores[P1].GetSamplesSync(out short[] lsamples, out int lnsamp);
				fixed (short* ls = &lsamples[0], sb = &_soundbuff[0])
				{
					for (int i = 0; i < lnsamp; i++)
					{
						int lsamp = (lsamples[i * 2] + lsamples[i * 2 + 1]) / 2;
						sb[i * 2] = (short)lsamp;
					}
				}

				_linkedCores[P2].GetSamplesSync(out short[] rsamples, out int rnsamp);
				fixed (short* rs = &rsamples[0], sb = &_soundbuff[1])
				{
					for (int i = 0; i < rnsamp; i++)
					{
						int rsamp = (rsamples[i * 2] + rsamples[i * 2 + 1]) / 2;
						sb[i * 2] = (short)rsamp;
					}
				}

				if (rendersound)
				{
					_nsamp = Math.Min(lnsamp, rnsamp);
				}
				else
				{
					_nsamp = 0;
				}
			}

			if (IsLagFrame)
			{
				LagCount++;
			}

			Frame++;

			return true;
		}

		public int Frame { get; private set; }

		public string SystemId => "GBALink";

		public bool DeterministicEmulation => LinkedDeterministicEmulation();

		private bool LinkedDeterministicEmulation()
		{
			bool deterministicEmulation = true;
			for (int i = 0; i < _numCores; i++)
			{
				deterministicEmulation &= _linkedCores[i].DeterministicEmulation;
			}
			return deterministicEmulation;
		}

		public void ResetCounters()
		{
			Frame = 0;
			LagCount = 0;
			IsLagFrame = false;

			for (int i = 0; i < _numCores; i++)
			{
				_frameOverflow[i] = 0;
				_stepOverflow[i] = 0;
			}
		}

		public void Dispose()
		{
			if (_numCores > 0)
			{
				for (int i = 0; i < _numCores; i++)
				{
					_linkedCores[i].Dispose();
					_linkedCores[i] = null;
				}

				MGBAHawk.LibmGBA.BizDestroyLockstep(_lockstep);
				_lockstep = IntPtr.Zero;

				_numCores = 0;
			}
		}
	}
}