﻿using System.Collections.Generic;
using System.Linq;

namespace BizHawk.Client.Common
{
	public sealed class InputApi : IInputApi
	{
		private readonly DisplayManagerBase _displayManager;

		private readonly InputManager _inputManager;

		public InputApi(DisplayManagerBase displayManager, InputManager inputManager)
		{
			_displayManager = displayManager;
			_inputManager = inputManager;
		}

		public Dictionary<string, bool> Get()
		{
			var buttons = new Dictionary<string, bool>();
			foreach (var kvp in _inputManager.ControllerInputCoalescer.BoolButtons().Where(kvp => kvp.Value)) buttons[kvp.Key] = true;
			return buttons;
		}

		public Dictionary<string, object> GetMouse()
		{
			var (pos, scroll, lmb, mmb, rmb, x1mb, x2mb) = _inputManager.GetMainFormMouseInfo();
			// TODO - need to specify whether in "emu" or "native" coordinate space.
			var p = _displayManager.UntransformPoint(pos);
			return new Dictionary<string, object>
			{
				["X"] = p.X,
				["Y"] = p.Y,
				["Left"] = lmb,
				["Middle"] = mmb,
				["Right"] = rmb,
				["XButton1"] = x1mb,
				["XButton2"] = x2mb,
				["Wheel"] = scroll
			};
		}
	}
}
