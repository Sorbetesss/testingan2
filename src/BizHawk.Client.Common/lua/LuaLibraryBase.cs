﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;

using BizHawk.Common;
using BizHawk.Common.StringExtensions;

namespace BizHawk.Client.Common
{
	public abstract class LuaLibraryBase
	{
		[return: NotNullIfNotNull("s")]
		public static string FixString(string s)
			=> s is null
				? null
				: Encoding.UTF8.GetString(OSTailoredCode.IsUnixHost ? s.ToCharCodepointArray() : Encoding.Default.GetBytes(s)); // default is CP-1252 on Win10 in English, CP-1251 in Russian, and probably other things elsewhere, but that's what Lua is sending us so ¯\_(ツ)_/¯

		[return: NotNullIfNotNull("s")]
		public static string UnFixString(string s)
			=> s is null
				? null
				: OSTailoredCode.IsUnixHost
					? StringExtensions.CharCodepointsToString(Encoding.UTF8.GetBytes(s))
					: Encoding.Default.GetString(Encoding.UTF8.GetBytes(s));

		public PathEntryCollection PathEntries { get; set; }

		protected LuaLibraryBase(IPlatformLuaLibEnv luaLibsImpl, ApiContainer apiContainer, Action<string> logOutputCallback)
		{
			LogOutputCallback = logOutputCallback;
			_luaLibsImpl = luaLibsImpl;
			_th = _luaLibsImpl.GetTableHelper();
			APIs = apiContainer;
			PathEntries = _luaLibsImpl.PathEntries;
		}

		protected static LuaFile CurrentFile { get; private set; }

		private static Thread _currentHostThread;
		private static readonly object ThreadMutex = new object();

		public abstract string Name { get; }

		public ApiContainer APIs { get; set; }

		protected readonly Action<string> LogOutputCallback;

		protected readonly IPlatformLuaLibEnv _luaLibsImpl;

		protected readonly NLuaTableHelper _th;

		public static void ClearCurrentThread()
		{
			lock (ThreadMutex)
			{
				_currentHostThread = null;
				CurrentFile = null;
			}
		}

		/// <exception cref="InvalidOperationException">attempted to have Lua running in two host threads at once</exception>
		public static void SetCurrentThread(LuaFile luaFile)
		{
			lock (ThreadMutex)
			{
				if (_currentHostThread != null)
				{
					throw new InvalidOperationException("Can't have lua running in two host threads at a time!");
				}

				_currentHostThread = Thread.CurrentThread;
				CurrentFile = luaFile;
			}
		}

		protected static int LuaInt(object luaArg)
		{
			return (int)(double)luaArg;
		}

		protected void Log(string message)
			=> LogOutputCallback?.Invoke(message);
	}
}
