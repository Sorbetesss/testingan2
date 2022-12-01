﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

using NLua;

using BizHawk.Common;
using BizHawk.Emulation.Common;
using BizHawk.Client.Common;

namespace BizHawk.Client.EmuHawk
{
	public class Win32LuaLibraries : IPlatformLuaLibEnv
	{
		public Win32LuaLibraries(
			LuaFileList scriptList,
			LuaFunctionList registeredFuncList,
			IEmulatorServiceProvider serviceProvider,
			MainForm mainForm,
			DisplayManagerBase displayManager,
			InputManager inputManager,
			Config config,
			IEmulator emulator,
			IGameInfo game)
		{
			void EnumerateLuaFunctions(string name, Type type, LuaLibraryBase instance)
			{
				if (instance != null) _lua.NewTable(name);
				foreach (var method in type.GetMethods())
				{
					var foundAttrs = method.GetCustomAttributes(typeof(LuaMethodAttribute), false);
					if (foundAttrs.Length == 0) continue;
					if (instance != null) _lua.RegisterFunction($"{name}.{((LuaMethodAttribute)foundAttrs[0]).Name}", instance, method);
					LibraryFunction libFunc = new(
						name,
						type.GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>()
							.Select(descAttr => descAttr.Description).FirstOrDefault() ?? string.Empty,
						method
					);
					Docs.Add(libFunc);
#if DEBUG
					// these don't catch object or LuaTable!
					if (method.GetParameters().Any(static pi => pi.ParameterType == typeof(string)
						&& !pi.CustomAttributes.Any(static a => typeof(LuaStringParamAttributeBase).IsAssignableFrom(a.AttributeType))))
					{
						Console.WriteLine($"Lua function {name}.{libFunc.Name} has an unclassified string param");
					}
					if (method.ReturnParameter!.ParameterType == typeof(string)
						&& !method.ReturnParameter.CustomAttributes.Any(static a => typeof(LuaStringParamAttributeBase).IsAssignableFrom(a.AttributeType)))
					{
						Console.WriteLine($"Lua function {name}.{libFunc.Name} has an unclassified string return value");
					}
#endif
				}
			}

			_lua.State.Encoding = Encoding.UTF8;
			_th = new NLuaTableHelper(_lua, LogToLuaConsole);
			_displayManager = displayManager;
			_inputManager = inputManager;
			_mainForm = mainForm;
			LuaWait = new AutoResetEvent(false);
			PathEntries = config.PathEntries;
			RegisteredFunctions = registeredFuncList;
			ScriptList = scriptList;
			Docs.Clear();
			_apiContainer = ApiManager.RestartLua(serviceProvider, LogToLuaConsole, _mainForm, _displayManager, _inputManager, _mainForm.MovieSession, _mainForm.Tools, config, emulator, game);

			// Register lua libraries
			foreach (var lib in Client.Common.ReflectionCache.Types.Concat(EmuHawk.ReflectionCache.Types)
				.Where(t => typeof(LuaLibraryBase).IsAssignableFrom(t) && t.IsSealed && ServiceInjector.IsAvailable(serviceProvider, t)))
			{
				if (VersionInfo.DeveloperBuild || lib.GetCustomAttribute<LuaLibraryAttribute>(inherit: false)?.Released == true)
				{
					var instance = (LuaLibraryBase)Activator.CreateInstance(lib, this, _apiContainer, (Action<string>)LogToLuaConsole);
					ServiceInjector.UpdateServices(serviceProvider, instance);

					// TODO: make EmuHawk libraries have a base class with common properties such as this
					// and inject them here
					if (instance is ClientLuaLibrary clientLib)
					{
						clientLib.MainForm = _mainForm;
					}
					else if (instance is ConsoleLuaLibrary consoleLib)
					{
						consoleLib.Tools = _mainForm.Tools;
						_logToLuaConsoleCallback = consoleLib.Log;
					}
					else if (instance is FormsLuaLibrary formsLib)
					{
						formsLib.MainForm = _mainForm;
					}
					else if (instance is GuiLuaLibrary guiLib)
					{
						// emu lib may be null now, depending on order of ReflectionCache.Types, but definitely won't be null when this is called
						guiLib.CreateLuaCanvasCallback = (width, height, x, y) =>
						{
							var canvas = new LuaCanvas(EmulationLuaLibrary, width, height, x, y, _th, LogToLuaConsole);
							canvas.Show();
							return _th.ObjectToTable(canvas);
						};
					}
					else if (instance is TAStudioLuaLibrary tastudioLib)
					{
						tastudioLib.Tools = _mainForm.Tools;
					}

					EnumerateLuaFunctions(instance.Name, lib, instance);
					Libraries.Add(lib, instance);
				}
			}

			_lua.RegisterFunction("print", this, typeof(Win32LuaLibraries).GetMethod(nameof(Print)));

			EmulationLuaLibrary.FrameAdvanceCallback = Frameadvance;
			EmulationLuaLibrary.YieldCallback = EmuYield;

			EnumerateLuaFunctions(nameof(LuaCanvas), typeof(LuaCanvas), null); // add LuaCanvas to Lua function reference table
		}

		private ApiContainer _apiContainer;

		private readonly DisplayManagerBase _displayManager;

		private GuiApi GuiAPI => (GuiApi)_apiContainer.Gui;

		private readonly InputManager _inputManager;

		private readonly MainForm _mainForm;

		private Lua _lua = new();
		private LuaThread _currThread;

		private readonly NLuaTableHelper _th;

		private static Action<object[]> _logToLuaConsoleCallback = a => Console.WriteLine("a Lua lib is logging during init and the console lib hasn't been initialised yet");

		private FormsLuaLibrary FormsLibrary => (FormsLuaLibrary)Libraries[typeof(FormsLuaLibrary)];

		public LuaDocumentation Docs { get; } = new LuaDocumentation();

		private EmulationLuaLibrary EmulationLuaLibrary => (EmulationLuaLibrary)Libraries[typeof(EmulationLuaLibrary)];

		public string EngineName => "KeraLua";

		public bool IsRebootingCore { get; set; }

		public bool IsUpdateSupressed { get; set; }

		private readonly IDictionary<Type, LuaLibraryBase> Libraries = new Dictionary<Type, LuaLibraryBase>();

		private EventWaitHandle LuaWait;

		public PathEntryCollection PathEntries { get; private set; }

		public LuaFileList ScriptList { get; }

		private static void LogToLuaConsole(object outputs) => _logToLuaConsoleCallback(new[] { outputs });

		public NLuaTableHelper GetTableHelper() => _th;

		public void Restart(
			IEmulatorServiceProvider newServiceProvider,
			Config config,
			IEmulator emulator,
			IGameInfo game)
		{
			_apiContainer = ApiManager.RestartLua(newServiceProvider, LogToLuaConsole, _mainForm, _displayManager, _inputManager, _mainForm.MovieSession, _mainForm.Tools, config, emulator, game);
			PathEntries = config.PathEntries;
			foreach (var lib in Libraries.Values)
			{
				lib.APIs = _apiContainer;
				ServiceInjector.UpdateServices(newServiceProvider, lib);
			}
		}

		public bool FrameAdvanceRequested { get; private set; }

		public LuaFunctionList RegisteredFunctions { get; }

		public void CallSaveStateEvent(string name)
		{
			try
			{
				foreach (var lf in RegisteredFunctions.Where(static l => l.Event == NamedLuaFunction.EVENT_TYPE_SAVESTATE).ToList())
				{
					lf.Call(name);
				}
				GuiAPI.ThisIsTheLuaAutounlockHack();
			}
			catch (Exception e)
			{
				GuiAPI.ThisIsTheLuaAutounlockHack();
				LogToLuaConsole($"error running function attached by lua function event.onsavestate\nError message: {e.Message}");
			}
		}

		public void CallLoadStateEvent(string name)
		{
			try
			{
				foreach (var lf in RegisteredFunctions.Where(static l => l.Event == NamedLuaFunction.EVENT_TYPE_LOADSTATE).ToList())
				{
					lf.Call(name);
				}
				GuiAPI.ThisIsTheLuaAutounlockHack();
			}
			catch (Exception e)
			{
				GuiAPI.ThisIsTheLuaAutounlockHack();
				LogToLuaConsole($"error running function attached by lua function event.onloadstate\nError message: {e.Message}");
			}
		}

		public void CallFrameBeforeEvent()
		{
			if (IsUpdateSupressed) return;
			try
			{
				foreach (var lf in RegisteredFunctions.Where(static l => l.Event == NamedLuaFunction.EVENT_TYPE_PREFRAME).ToList())
				{
					lf.Call();
				}
				GuiAPI.ThisIsTheLuaAutounlockHack();
			}
			catch (Exception e)
			{
				GuiAPI.ThisIsTheLuaAutounlockHack();
				LogToLuaConsole($"error running function attached by lua function event.onframestart\nError message: {e.Message}");
			}
		}

		public void CallFrameAfterEvent()
		{
			if (IsUpdateSupressed) return;
			try
			{
				foreach (var lf in RegisteredFunctions.Where(static l => l.Event == NamedLuaFunction.EVENT_TYPE_POSTFRAME).ToList())
				{
					lf.Call();
				}
				GuiAPI.ThisIsTheLuaAutounlockHack();
			}
			catch (Exception e)
			{
				GuiAPI.ThisIsTheLuaAutounlockHack();
				LogToLuaConsole($"error running function attached by lua function event.onframeend\nError message: {e.Message}");
			}
		}

		public void CallExitEvent(LuaFile lf)
		{
			foreach (var exitCallback in RegisteredFunctions
				.Where(l => l.Event == NamedLuaFunction.EVENT_TYPE_ENGINESTOP
					&& (l.LuaFile.Path == lf.Path || ReferenceEquals(l.LuaFile.Thread, lf.Thread)))
				.ToList())
			{
				exitCallback.Call();
			}
			GuiAPI.ThisIsTheLuaAutounlockHack();
		}

		public void Close()
		{
			foreach (var closeCallback in RegisteredFunctions
				.Where(static l => l.Event == NamedLuaFunction.EVENT_TYPE_CONSOLECLOSE)
				.ToList())
			{
				closeCallback.Call();
			}

			RegisteredFunctions.Clear(_mainForm.Emulator);
			ScriptList.Clear();
			FormsLibrary.DestroyAll();
			_lua.Dispose();
			_lua = new Lua();
		}

		public INamedLuaFunction CreateAndRegisterNamedFunction(
			LuaFunction function,
			string theEvent,
			Action<string> logCallback,
			LuaFile luaFile,
			[LuaArbitraryStringParam] string name = null)
		{
			var nlf = new NamedLuaFunction(function, theEvent, logCallback, luaFile, name);
			RegisteredFunctions.Add(nlf);
			return nlf;
		}

		public bool RemoveNamedFunctionMatching(Func<INamedLuaFunction, bool> predicate)
		{
			var nlf = (NamedLuaFunction)RegisteredFunctions.FirstOrDefault(predicate);
			if (nlf == null) return false;
			RegisteredFunctions.Remove(nlf, _mainForm.Emulator);
			return true;
		}

		public LuaThread SpawnCoroutine(string file)
		{
			var content = File.ReadAllText(file);
			var main = _lua.LoadString(content, "main");
			_lua.NewThread(main, out var ret);
			ret.State.Encoding = Encoding.UTF8;
			return ret;
		}

		public void SpawnAndSetFileThread(string pathToLoad, LuaFile lf)
		{
			lf.LuaRef = _lua; // ref kept around for DetachFromScript in NamedLuaFunction
			lf.Thread = SpawnCoroutine(pathToLoad);
		}

		public void ExecuteString(string command)
			=> _lua.DoString(command);

		public (bool WaitForFrame, bool Terminated) ResumeScript(LuaFile lf)
		{
			_currThread = lf.Thread;

			try
			{
				LuaLibraryBase.SetCurrentThread(lf);

				var execResult = _currThread.State.Resume(null, 0);
				GuiAPI.ThisIsTheLuaAutounlockHack();

				_currThread = null;
				var result = execResult == 0
					? (WaitForFrame: false, Terminated: true) // terminated
					: (WaitForFrame: FrameAdvanceRequested, Terminated: false); // yielded

				FrameAdvanceRequested = false;
				return result;
			}
			catch (Exception)
			{
				GuiAPI.ThisIsTheLuaAutounlockHack();
				throw;
			}
			finally
			{
				LuaLibraryBase.ClearCurrentThread();
			}
		}

		public static void Print(params object[] outputs)
		{
			_logToLuaConsoleCallback(outputs);
		}

		private void Frameadvance()
		{
			FrameAdvanceRequested = true;
			_currThread.State.Yield(0);
		}

		private void EmuYield()
		{
			_currThread.State.Yield(0);
		}
	}
}
