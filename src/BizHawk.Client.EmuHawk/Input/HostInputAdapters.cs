#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

using BizHawk.Client.Common;

namespace BizHawk.Client.EmuHawk
{
	internal sealed class DirectInputAdapter : HostInputAdapter
	{
		public void DeInitAll()
		{
			KeyInput.Cleanup();
			GamePad.Cleanup();
		}

		public void FirstInitAll(IntPtr mainFormHandle)
		{
			KeyInput.Initialize(mainFormHandle);
			IPCKeyInput.Initialize();
			ReInitGamepads(mainFormHandle);
		}

		public void ReInitGamepads(IntPtr mainFormHandle)
		{
			GamePad.Initialize(mainFormHandle);
			GamePad360.Initialize();
		}

		public void PreprocessHostGamepads()
		{
			GamePad.UpdateAll();
			GamePad360.UpdateAll();
		}

		public void ProcessHostGamepads(Action<string?, bool, ClientInputFocus> handleButton, Action<string?, int> handleAxis)
		{
			foreach (var pad in GamePad360.EnumerateDevices())
			{
				var inputNamePrefix = $"X{pad.PlayerNumber} ";
				for (int b = 0, n = pad.NumButtons; b < n; b++) handleButton(inputNamePrefix + pad.ButtonName(b), pad.Pressed(b), ClientInputFocus.Pad);
				foreach (var (axisName, f) in pad.GetAxes()) handleAxis(inputNamePrefix + axisName, (int) f);
			}
			foreach (var pad in GamePad.EnumerateDevices())
			{
				var inputNamePrefix = $"J{pad.PlayerNumber} ";
				for (int b = 0, n = pad.NumButtons; b < n; b++) handleButton(inputNamePrefix + pad.ButtonName(b), pad.Pressed(b), ClientInputFocus.Pad);
				foreach (var (axisName, f) in pad.GetAxes()) handleAxis(inputNamePrefix + axisName, (int) f);
			}
		}

		public IEnumerable<KeyEvent> ProcessHostKeyboards() => KeyInput.Update().Concat(IPCKeyInput.Update());
	}

	internal sealed class OpenTKInputAdapter : HostInputAdapter
	{
		public void DeInitAll() {}

		public void FirstInitAll(IntPtr mainFormHandle)
		{
			OTK_Keyboard.Initialize();
			OTK_GamePad.Initialize();
		}

		public void ReInitGamepads(IntPtr mainFormHandle) {}

		public void PreprocessHostGamepads() => OTK_GamePad.UpdateAll();

		public void ProcessHostGamepads(Action<string?, bool, ClientInputFocus> handleButton, Action<string?, int> handleAxis)
		{
			foreach (var pad in OTK_GamePad.EnumerateDevices())
			{
				foreach (var but in pad.buttonObjects) handleButton(pad.InputNamePrefix + but.ButtonName, but.ButtonAction(), ClientInputFocus.Pad);
				foreach (var (axisID, f) in pad.GetAxes()) handleAxis($"{pad.InputNamePrefix}{axisID} Axis", (int) f);
			}
		}

		public IEnumerable<KeyEvent> ProcessHostKeyboards() => OTK_Keyboard.Update();
	}
}
