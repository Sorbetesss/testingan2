﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BizHawk.Emulation.Common;
using BizHawk.Emulation.Cores.ColecoVision;

namespace BizHawk.Client.EmuHawk
{
	[Schema("Coleco")]
	// ReSharper disable once UnusedMember.Global
	public class ColecoSchema : IVirtualPadSchema
	{
		public IEnumerable<PadSchema> GetPadSchemas(IEmulator core)
		{
			var deck = ((ColecoVision)core).ControllerDeck;
			var ports = new[] { deck.Port1.GetType(), deck.Port2.GetType() };

			for (int i = 0; i < 2; i++)
			{
				if (ports[i] == typeof(UnpluggedController))
				{
					break;
				}

				if (ports[i] == typeof(StandardController))
				{
					yield return StandardController(i + 1);
				}
				else if (ports[i] == typeof(ColecoTurboController))
				{
					yield return TurboController(i + 1);
				}
				else if (ports[i] == typeof(ColecoSuperActionController))
				{
					yield return SuperActionController(i + 1);
				}
			}
		}

		public static PadSchema StandardController(int controller)
		{
			return new PadSchema
			{
				Size = new Size(128, 200),
				Buttons = StandardButtons(controller)
			};
		}

		private static PadSchema TurboController(int controller)
		{
			var defAxes = new ColecoTurboController(controller).Definition.Axes;
			return new PadSchema
			{
				Size = new Size(275, 260),
				Buttons = new PadSchemaControl[]
				{
					new AnalogSchema(6, 14, $"P{controller} Disc X")
					{
						Spec = defAxes.SpecAtIndex(0),
						SecondarySpec = defAxes.SpecAtIndex(1)
					},
					new ButtonSchema(6, 224, controller, "Pedal")
				}
			};
		}

		private static PadSchema SuperActionController(int controller)
		{
			return new PadSchema
			{
				Size = new Size(195, 260),
				Buttons = StandardButtons(controller).Concat(new PadSchemaControl[]
				{
					new SingleAxisSchema(6, 200, controller, "Disc X")
					{
						DisplayName = "Disc",
						TargetSize = new Size(180, 55),
						MinValue = -360,
						MaxValue = 360
					},
					new ButtonSchema(126, 15, controller, "Yellow"),
					new ButtonSchema(126, 40, controller, "Red"),
					new ButtonSchema(126, 65, controller, "Purple"),
					new ButtonSchema(126, 90, controller, "Blue")
				})
			};
		}

		private static IEnumerable<ButtonSchema> StandardButtons(int controller)
		{
			return new[]
			{
				ButtonSchema.Up(50, 11, controller),
				ButtonSchema.Down(50, 32, controller),
				ButtonSchema.Left(29, 22, controller),
				ButtonSchema.Right(71, 22, controller),
				new ButtonSchema(27, 85, controller, "1"),
				new ButtonSchema(50, 85, controller, "2"),
				new ButtonSchema(73, 85, controller, "3"),
				new ButtonSchema(27, 108, controller, "4"),
				new ButtonSchema(50, 108, controller, "5"),
				new ButtonSchema(73, 108, controller, "6"),
				new ButtonSchema(27, 131, controller, "7"),
				new ButtonSchema(50, 131, controller, "8"),
				new ButtonSchema(73, 131, controller, "9"),
				new ButtonSchema(27, 154, controller, "Star") { DisplayName = "*" },
				new ButtonSchema(50, 154, controller, "0"),
				new ButtonSchema(73, 154, controller, "Pound") { DisplayName = "#" }
			};
		}
	}
}
