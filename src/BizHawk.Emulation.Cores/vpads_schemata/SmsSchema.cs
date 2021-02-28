﻿using System;
using System.Collections.Generic;
using System.Drawing;
using BizHawk.Emulation.Common;
using BizHawk.Emulation.Cores.Sega.MasterSystem;

namespace BizHawk.Emulation.Cores
{
	internal abstract class SmsSchemaControls
	{
		public static PadSchema StandardController(int controller, bool isSms)
		{
			return new PadSchema
			{
				Size = new Size(174, 90),
				Buttons = StandardButtons(controller, isSms)
			};
		}

		public static IEnumerable<ButtonSchema> StandardButtons(int controller, bool isSms)
		{

			yield return ButtonSchema.Up(14, 12, controller);
			yield return ButtonSchema.Down(14, 56, controller);
			yield return ButtonSchema.Left(2, 34, controller);
			yield return ButtonSchema.Right(24, 34, controller);
			yield return new ButtonSchema(122, 34, controller, "B1", "1");
			yield return new ButtonSchema(146, 34, controller, "B2", "2");
			if (!isSms)
			{
				yield return new ButtonSchema(134, 12, controller, "Start", "S");
			}
		}

		public static PadSchema Console(bool isSms)
		{
			return new ConsoleSchema
			{
				Size = new Size(150, 50),
				Buttons = ConsoleButtons(isSms)
			};
		}

		public static IEnumerable<ButtonSchema> ConsoleButtons(bool isSms)
		{
			yield return new ButtonSchema(10, 15, "Reset");
			if (isSms)
			{
				yield return new ButtonSchema(58, 15, "Pause");
			}
		}
	}

	[Schema("GG")]
	public class GameGearSchema : IVirtualPadSchema
	{
		public IEnumerable<PadSchema> GetPadSchemas(IEmulator core, Action<string> showMessageBox)
		{
			yield return SmsSchemaControls.StandardController(1, false);
			yield return SmsSchemaControls.Console(false);
		}
	}

	[Schema("SG")]
	public class SG1000Schema : SMSSchema {} // are these really the same controller layouts? --yoshi

	[Schema("SMS")]
	public class SMSSchema : IVirtualPadSchema
	{
		public IEnumerable<PadSchema> GetPadSchemas(IEmulator core, Action<string> showMessageBox)
		{
			yield return SmsSchemaControls.StandardController(1, true);
			yield return SmsSchemaControls.StandardController(2, true);
			yield return SmsSchemaControls.Console(true);
		}
	}
}
