﻿using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BizHawk.Client.Common;
using BizHawk.Common;
using BizHawk.Emulation.Common;

using static BizHawk.Emulation.Common.ControllerDefinition;

namespace BizHawk.Tests.Client.Common.Display
{
	[TestClass]
	public class InputDisplayTests
	{
		private const int MidValue = 100;
		private SimpleController _boolController = null!;
		private SimpleController _axisController = null!;

		[TestInitialize]
		public void Initializer()
		{
			_boolController = new SimpleController
			{
				Definition = new ControllerDefinition { BoolButtons = { "A" } }
			};

			_axisController = new SimpleController
			{
				Definition = new ControllerDefinition().AddXYPair("Stick{0}", AxisPairOrientation.RightAndUp, 0.RangeTo(200), MidValue)
			};
		}

		[TestMethod]
		public void Generate_BoolPressed_GeneratesMnemonic()
		{
			_boolController["A"] = true;
			var displayGenerator = new Bk2InputDisplayGenerator("NES", _boolController);
			var actual = displayGenerator.Generate();
			Assert.AreEqual("A", actual);
		}

		[TestMethod]
		public void Generate_BoolUnPressed_GeneratesSpace()
		{
			_boolController["A"] = false;
			var displayGenerator = new Bk2InputDisplayGenerator("NES", _boolController);
			var actual = displayGenerator.Generate();
			Assert.AreEqual(" ", actual);
		}

		[TestMethod]
		public void Generate_Floats()
		{
			var displayGenerator = new Bk2InputDisplayGenerator("NES", _axisController);
			var actual = displayGenerator.Generate();
			Assert.AreEqual("    0,    0,", actual);
		}

		[TestMethod]
		public void Generate_MidRangeDisplaysEmpty()
		{
			_axisController.AcceptNewAxis("StickX", MidValue);
			var displayGenerator = new Bk2InputDisplayGenerator("NES", _axisController);
			var actual = displayGenerator.Generate();
			Assert.AreEqual("          0,", actual);
		}
	}
}
