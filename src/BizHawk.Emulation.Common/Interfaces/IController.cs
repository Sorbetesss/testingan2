﻿namespace BizHawk.Emulation.Common
{
	public interface IController
	{
		/// <summary>
		/// Gets a definition of the controller schema, including all currently available buttons and their types
		/// </summary>
		IVGamepadDef Definition { get; }

		/// <summary>
		/// Returns the current state of a boolean control
		/// </summary>
		bool IsPressed(string button);

		/// <summary>
		/// Returns the state of an axis control
		/// </summary>
		int AxisValue(string name);
	}
}
