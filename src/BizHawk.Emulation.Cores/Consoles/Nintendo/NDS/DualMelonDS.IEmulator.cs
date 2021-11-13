using System;
using System.Linq;

using BizHawk.Common;
using BizHawk.Emulation.Common;

namespace BizHawk.Emulation.Cores.Consoles.Nintendo.NDS
{
	partial class DualNDS : IEmulator
	{
		public IEmulatorServiceProvider ServiceProvider => _serviceProvider;

		public ControllerDefinition ControllerDefinition => DualNDSController;

		private static ControllerDefinition DualNDSController { get; } = CreateControllerDefinition();

		private static ControllerDefinition CreateControllerDefinition()
		{
			var ret = new ControllerDefinition { Name = "Dual NDS Controller" };
			for (int i = 1; i <= 2; i++)
			{
				ret.BoolButtons.AddRange(
					new[] { "Up", "Down", "Left", "Right", "Start", "Select", "B", "A", "Y", "X", "L", "R", "LidOpen", "LidClose", "Touch", "Power" }
						.Select(s => $"P{i} {s}"));
				ret.AddXYPair($"P{i} " + "Touch {0}", AxisPairOrientation.RightAndUp, 0.RangeTo(255), 128, 0.RangeTo(191), 96);
				ret.AddAxis($"P{i} " + "Mic Volume", (0).RangeTo(100), 0);
			}
			return ret;
		}

		public bool FrameAdvance(IController controller, bool render, bool rendersound = true)
		{
			LCont.Clear();
			RCont.Clear();

			foreach (var s in DualNDSController.BoolButtons)
			{
				if (controller.IsPressed(s))
				{
					if (s.Contains("P1 "))
					{
						LCont.Set(s.Replace("P1 ", ""));
					}
					else if (s.Contains("P2 "))
					{
						RCont.Set(s.Replace("P2 ", ""));
					}
				}
			}

			foreach (var s in DualNDSController.Axes)
			{
				if (s.Key.Contains("P1 "))
				{
					LCont.SetAxisValue(s.Key.Replace("P1 ", ""), controller.AxisValue(s.Key));
				}
				else if (s.Key.Contains("P2 "))
				{
					RCont.SetAxisValue(s.Key.Replace("P2 ", ""), controller.AxisValue(s.Key));
				}
			}

			var lframe = L.FrameStepPrep(LCont, render, rendersound);
			var rframe = R.FrameStepPrep(RCont, render, rendersound);

			bool lrunning = true;
			bool rrunning = true;
			while (lrunning || rrunning)
			{
				if (lrunning)
					lrunning = L.FrameStep();
				if (rrunning)
					rrunning = R.FrameStep();

				// todo: actually handle lan packets
			}

			unsafe
			{
				fixed (int* lvb = &VideoBuffer[0], rvb = &VideoBuffer[256])
				fixed (short* lsb = &SampleBuffer[0], rsb = &SampleBuffer[1])
				{
					lframe.VideoBuffer = (IntPtr)lvb;
					lframe.SoundBuffer = (IntPtr)lsb;
					L.FrameStepPost(lframe);
					rframe.VideoBuffer = (IntPtr)rvb;
					rframe.SoundBuffer = (IntPtr)rsb;
					R.FrameStepPost(rframe);
				}
			}

			IsLagFrame = lframe.Lagged != 0 && rframe.Lagged != 0;
			if (IsLagFrame)
			{
				LagCount++;
			}

			Frame++;

			if (rendersound)
			{
				_sampleBufferContains = Math.Min(lframe.Samples, rframe.Samples);
			}
			else
			{
				_sampleBufferContains = 0;
			}

			return true;
		}

		public int Frame { get; private set; }

		public string SystemId => "Dual NDS";

		public bool DeterministicEmulation => L.DeterministicEmulation && R.DeterministicEmulation;

		public void ResetCounters()
		{
			Frame = 0;
			LagCount = 0;
			IsLagFrame = false;
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				L.Dispose();
				L = null;

				R.Dispose();
				R = null;

				_disposed = true;
			}
		}
	}
}
