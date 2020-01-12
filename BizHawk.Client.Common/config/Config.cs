﻿using System;
using System.Collections.Generic;
using System.Drawing;

using BizHawk.Common;
using BizHawk.Emulation.Common;

// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable InconsistentNaming
// ReSharper disable StyleCop.SA1310
// ReSharper disable StyleCop.SA1401
namespace BizHawk.Client.Common
{
	public class Config
	{
		public static string ControlDefaultPath => PathManager.MakeProgramRelativePath("defctrl.json");

		public void ConfigCheckAllControlDefaults()
		{
			if (AllTrollers.Count == 0 && AllTrollersAutoFire.Count == 0 && AllTrollersAnalog.Count == 0)
			{
				var cd = ConfigService.Load<ControlDefaults>(ControlDefaultPath);
				AllTrollers = cd.AllTrollers;
				AllTrollersAutoFire = cd.AllTrollersAutoFire;
				AllTrollersAnalog = cd.AllTrollersAnalog;
			}
		}

		public Config()
		{
			ConfigCheckAllControlDefaults();
		}

		public void ResolveDefaults()
		{
			PathEntries.ResolveWithDefaults();
			HotkeyBindings.ResolveWithDefaults();
			PathManager.RefreshTempPath();
		}

		// Core preference for generic file extension, key: file extension, value: a systemID or empty if no preference
		public Dictionary<string, string> PreferredPlatformsForExtensions = new Dictionary<string, string>
		{
			{ ".bin", "" },
			{ ".rom", "" },
			{ ".iso", "" },
			{ ".img", "" },
			{ ".cue", "" }
		};

		// Path Settings ************************************/
		public bool UseRecentForROMs = false;
		public string LastRomPath = ".";
		public PathEntryCollection PathEntries = new PathEntryCollection();

		// BIOS Paths
		public Dictionary<string, string> FirmwareUserSpecifications = new Dictionary<string, string>(); // key: sysid+firmwareId; value: absolute path

		// General Client Settings
		public int Input_Hotkey_OverrideOptions = 0;
		public bool StackOSDMessages = true;

		// TODO: move me
		public class ToomFactors : Dictionary<string, int>
		{
			public new int this[string index]
			{
				get
				{
					if (!ContainsKey(index))
					{
						Add(index, 2);
					}

					return base[index];
				}

				set => base[index] = value;
			}
		}

		public ToomFactors TargetZoomFactors = new ToomFactors();

		public int TargetScanlineFilterIntensity = 128; // choose between 0 and 256
		public int TargetDisplayFilter = 0;
		public int DispFinalFilter = 0; // None
		public string DispUserFilterPath = "";
		public RecentFiles RecentRoms = new RecentFiles(10);
		public bool PauseWhenMenuActivated = true;
		public bool SaveWindowPosition = true;
		public bool StartPaused = false;
		public bool StartFullscreen = false;
		public int MainWndx = -1; // Negative numbers will be ignored
		public int MainWndy = -1;
		public bool RunInBackground = true;
		public bool AcceptBackgroundInput = false;
		public bool AcceptBackgroundInputControllerOnly = false;
		public bool HandleAlternateKeyboardLayouts = false;
		public bool SingleInstanceMode = false;
		public bool AllowUD_LR = false;
		public bool ForbidUD_LR = false;
		public bool ShowContextMenu = true;
		public bool EnableBackupMovies = true;
		public bool MoviesOnDisk = false;
		public bool MoviesInAWE = false;
		public bool HotkeyConfigAutoTab = true;
		public bool InputConfigAutoTab = true;
		public bool BackupSavestates = true;
		public bool SaveScreenshotWithStates = true;
		public int BigScreenshotSize = 128 * 1024;
		public bool NoLowResLargeScreenshotWithStates = false;
		public int AutofireOn = 1;
		public int AutofireOff = 1;
		public bool AutofireLagFrames = true;
		public int SaveSlot = 0; // currently selected savestate slot
		public bool AutoLoadLastSaveSlot = false;
		public bool SkipLagFrame = false;
		public bool SuppressAskSave = false;
		public bool AVI_CaptureOSD = false;
		public bool Screenshot_CaptureOSD = false;
		public bool FirstBoot = true;
		public bool Update_AutoCheckEnabled = false;
		public DateTime? Update_LastCheckTimeUTC = null;
		public string Update_LatestVersion = "";
		public string Update_IgnoreVersion = "";
		public bool SkipOutdatedOSCheck = false;
		public bool CDLAutoSave = true, CDLAutoStart = true, CDLAutoResume = true;

		/// <summary>
		/// Makes a .bak file before any saveram-writing operation (could be extended to make timestamped backups)
		/// </summary>
		public bool BackupSaveram = true;

		/// <summary>
		/// Whether to make AutoSave files at periodic intervals
		/// </summary>
		public bool AutosaveSaveRAM;

		/// <summary>
		/// Intervals at which to make AutoSave files
		/// </summary>
		public int FlushSaveRamFrames;

		public enum ELuaEngine
		{
			/// <remarks>Don't change this member's ordinal (don't reorder) without changing <c>BizHawk.Client.EmuHawk.Program.CurrentDomain_AssemblyResolve</c></remarks>
			LuaPlusLuaInterface,
			NLuaPlusKopiLua
		}

		/// <remarks>Don't rename this without changing <c>BizHawk.Client.EmuHawk.Program.CurrentDomain_AssemblyResolve</c></remarks>
		public ELuaEngine LuaEngine = ELuaEngine.LuaPlusLuaInterface;

		public bool TurboSeek { get; set; }

		public enum EDispMethod
		{
			OpenGL, GdiPlus, SlimDX9
		}

		public enum ESoundOutputMethod
		{
			DirectSound, XAudio2, OpenAL, Dummy
		}

		public enum EDispManagerAR
		{
			None,
			System, 

			// actually, custom SIZE (fixme on major release)
			Custom, 
			CustomRatio
		}

		public enum SaveStateTypeE
		{
			Default, Binary, Text
		}

		public MovieEndAction MovieEndAction = MovieEndAction.Finish;

		public enum ClientProfile
		{
			Unknown = 0,
			Casual = 1,
			Longplay = 2,
			Tas = 3,
			N64Tas = 4,
			Custom = 99
		}

		public ClientProfile SelectedProfile = ClientProfile.Unknown;

		// N64
		public bool N64UseCircularAnalogConstraint = true;

		// Run-Control settings
		public int FrameProgressDelayMs = 500; // how long until a frame advance hold turns into a frame progress?
		public int FrameSkip = 4;
		public int SpeedPercent = 100;
		public int SpeedPercentAlternate = 400;
		public bool ClockThrottle = true;
		public bool AutoMinimizeSkipping = true;
		public bool VSyncThrottle = false;

		// Rewind settings
		public bool Rewind_UseDelta = true;
		public bool RewindEnabledSmall = true;
		public bool RewindEnabledMedium = false;
		public bool RewindEnabledLarge = false;
		public int RewindFrequencySmall = 1;
		public int RewindFrequencyMedium = 4;
		public int RewindFrequencyLarge = 60;
		public int Rewind_MediumStateSize = 262144; // 256kb
		public int Rewind_LargeStateSize = 1048576; // 1mb
		public int Rewind_BufferSize = 128; // in mb
		public bool Rewind_OnDisk = false;
		public bool Rewind_IsThreaded = Environment.ProcessorCount > 1;
		public int RewindSpeedMultiplier = 1;

		// Savestate settings
		public SaveStateTypeE SaveStateType = SaveStateTypeE.Default;
		public const int DefaultSaveStateCompressionLevelNormal = 1;
		public int SaveStateCompressionLevelNormal = DefaultSaveStateCompressionLevelNormal;
		public const int DefaultSaveStateCompressionLevelRewind = 0; // this isn't actually used yet 
		public int SaveStateCompressionLevelRewind = DefaultSaveStateCompressionLevelRewind; // this isn't actually used yet 
		public int MovieCompressionLevel = 2;

		/// <summary>
		/// Use vsync when presenting all 3d accelerated windows.
		/// For the main window, if VSyncThrottle = false, this will try to use vsync without throttling to it
		/// </summary>
		public bool VSync = false;

		/// <summary>
		/// Tries to use an alternate vsync mechanism, for video cards that just can't do it right
		/// </summary>
		public bool DispAlternateVsync = false;

		// Display options
		public bool DisplayFPS = false;
		public bool DisplayFrameCounter = false;
		public bool DisplayLagCounter = false;
		public bool DisplayInput = false;
		public bool DisplayRerecordCount = false;
		public bool DisplayMessages = true;

		public bool DispFixAspectRatio = true;
		public bool DispFixScaleInteger = false;
		public bool DispFullscreenHacks = true;
		public bool DispAutoPrescale = true;
		public int DispSpeedupFeatures = 2;

		public MessagePosition Fps = DefaultMessagePositions.Fps.Clone();
		public MessagePosition FrameCounter = DefaultMessagePositions.FrameCounter.Clone();
		public MessagePosition LagCounter = DefaultMessagePositions.LagCounter.Clone();
		public MessagePosition InputDisplay = DefaultMessagePositions.InputDisplay.Clone();
		public MessagePosition ReRecordCounter = DefaultMessagePositions.ReRecordCounter.Clone();
		public MessagePosition MultitrackRecorder = DefaultMessagePositions.MultitrackRecorder.Clone();
		public MessagePosition Messages = DefaultMessagePositions.Messages.Clone();
		public MessagePosition Autohold = DefaultMessagePositions.Autohold.Clone();
		public MessagePosition RamWatches = DefaultMessagePositions.RamWatches.Clone();

		public int MessagesColor = DefaultMessagePositions.MessagesColor;
		public int AlertMessageColor = DefaultMessagePositions.AlertMessageColor;
		public int LastInputColor = DefaultMessagePositions.LastInputColor;
		public int MovieInput = DefaultMessagePositions.MovieInput;
		
		public int DispPrescale = 1;

		/// <remarks>warning: we dont even want to deal with changing this at runtime. but we want it changed here for config purposes. so dont check this variable. check in GlobalWin or something like that.</remarks>
		public EDispMethod DispMethod = EDispMethod.OpenGL;

		public int DispChrome_FrameWindowed = 2;
		public bool DispChrome_StatusBarWindowed = true;
		public bool DispChrome_CaptionWindowed = true;
		public bool DispChrome_MenuWindowed = true;
		public bool DispChrome_StatusBarFullscreen = false;
		public bool DispChrome_MenuFullscreen = false;
		public bool DispChrome_Fullscreen_AutohideMouse = true;
		public bool DispChrome_AllowDoubleClickFullscreen = true;

		public EDispManagerAR DispManagerAR = EDispManagerAR.System;

		// these are misnomers. they're actually a fixed size (fixme on major release)
		public int DispCustomUserARWidth = -1;
		public int DispCustomUserARHeight = -1;

		// these are more like the actual AR ratio (i.e. 4:3) (fixme on major release)
		public float DispCustomUserARX = -1;
		public float DispCustomUserARY = -1;

		//these default to 0 because by default we crop nothing
		public int DispCropLeft = 0;
		public int DispCropTop = 0;
		public int DispCropRight = 0;
		public int DispCropBottom = 0;

		// Sound options
		public ESoundOutputMethod SoundOutputMethod = ESoundOutputMethod.OpenAL;
		public bool SoundEnabled = true;
		public bool SoundEnabledNormal = true;
		public bool SoundEnabledRWFF = true;
		public bool MuteFrameAdvance = true;
		public int SoundVolume = 100; // Range 0-100
		public int SoundVolumeRWFF = 50; // Range 0-100
		public bool SoundThrottle = false;
		public string SoundDevice = "";
		public int SoundBufferSizeMs = 100;

		// Lua
		public RecentFiles RecentLua = new RecentFiles(8);
		public RecentFiles RecentLuaSession = new RecentFiles(8);
		public bool DisableLuaScriptsOnLoad = false;
		public bool ToggleAllIfNoneSelected = true;
		public bool LuaReloadOnScriptFileChange = false;
		public bool RunLuaDuringTurbo = true;

		// Watch Settings
		public RecentFiles RecentWatches = new RecentFiles(8);
		public PreviousType RamWatchDefinePrevious = PreviousType.LastFrame;
		public bool DisplayRamWatch = false;

		// Hex Editor Colors
		public Color HexBackgrndColor = SystemColors.Control;
		public Color HexForegrndColor = SystemColors.ControlText;
		public Color HexMenubarColor = SystemColors.Control;
		public Color HexFreezeColor = Color.LightBlue;
		public Color HexHighlightColor = Color.Pink;
		public Color HexHighlightFreezeColor = Color.Violet;

		// Video dumping settings
		public string VideoWriter = "";
		public int JMDCompression = 3;
		public int JMDThreads = 3;
		public string FFmpegFormat = "";
		public string FFmpegCustomCommand = "-c:a foo -c:v bar -f baz";
		public string AVICodecToken = "";
		public int GifWriterFrameskip = 3;
		public int GifWriterDelay = -1;
		public bool VideoWriterAudioSync = true;

		#region emulation core settings

		public Dictionary<string, object> CoreSettings = new Dictionary<string, object>();
		public Dictionary<string, object> CoreSyncSettings = new Dictionary<string, object>();

		public object GetCoreSettings<T>()
			where T : IEmulator
		{
			return GetCoreSettings(typeof(T));
		}

		public object GetCoreSettings(Type t)
		{
			object ret;
			CoreSettings.TryGetValue(t.ToString(), out ret);
			return ret;
		}

		public void PutCoreSettings<T>(object o)
			where T : IEmulator
		{
			PutCoreSettings(o, typeof(T));
		}

		public void PutCoreSettings(object o, Type t)
		{
			if (o != null)
			{
				CoreSettings[t.ToString()] = o;
			}
			else
			{
				CoreSettings.Remove(t.ToString());
			}
		}

		public object GetCoreSyncSettings<T>()
			where T : IEmulator
		{
			return GetCoreSyncSettings(typeof(T));
		}

		public object GetCoreSyncSettings(Type t)
		{
			CoreSyncSettings.TryGetValue(t.ToString(), out var ret);
			return ret;
		}

		public void PutCoreSyncSettings<T>(object o)
			where T : IEmulator
		{
			PutCoreSyncSettings(o, typeof(T));
		}

		public void PutCoreSyncSettings(object o, Type t)
		{
			if (o != null)
			{
				CoreSyncSettings[t.ToString()] = o;
			}
			else
			{
				CoreSyncSettings.Remove(t.ToString());
			}
		}

		#endregion

		public Dictionary<string, ToolDialogSettings> CommonToolSettings = new Dictionary<string, ToolDialogSettings>();
		public Dictionary<string, Dictionary<string, object>> CustomToolSettings = new Dictionary<string, Dictionary<string, object>>();

		// Cheats
		public bool DisableCheatsOnLoad = false;
		public bool LoadCheatFileByGame = true;
		public bool CheatsAutoSaveOnClose = true;
		public RecentFiles RecentCheats = new RecentFiles(8);

		// TAStudio
		public TasStateManagerSettings DefaultTasProjSettings = new TasStateManagerSettings();

		// Macro Tool
		public RecentFiles RecentMacros = new RecentFiles(8);

		// Movie Settings
		public RecentFiles RecentMovies = new RecentFiles(8);
		public string DefaultAuthor = "default user";
		public bool UseDefaultAuthor = true;
		public bool DisplaySubtitles = true;
		public bool VBAStyleMovieLoadState = false;
		public bool MoviePlaybackPokeMode = false;

		// Play Movie Dialog
		public bool PlayMovie_IncludeSubdir = false;
		public bool PlayMovie_MatchHash = true;

		// TI83
		public ToolDialogSettings TI83KeypadSettings = new ToolDialogSettings();
		public bool TI83autoloadKeyPad = true;
		public bool TI83ToolTips = true;

		public BindingCollection HotkeyBindings = new BindingCollection();

		// Analog Hotkey values
		public int Analog_LargeChange = 10;
		public int Analog_SmallChange = 1;

		public struct AnalogBind
		{
			/// <summary>the physical stick that we're bound to</summary>
			public string Value;

			/// <summary>sensitivity and flip</summary>
			public float Mult;

			/// <summary>portion of axis to ignore</summary>
			public float Deadzone;

			public AnalogBind(string value, float mult, float deadzone)
			{
				Value = value;
				Mult = mult;
				Deadzone = deadzone;
			}
		}

		// [ControllerType][ButtonName] => Physical Bind
		public Dictionary<string, Dictionary<string, string>> AllTrollers = new Dictionary<string, Dictionary<string, string>>();
		public Dictionary<string, Dictionary<string, string>> AllTrollersAutoFire = new Dictionary<string, Dictionary<string, string>>();
		public Dictionary<string, Dictionary<string, AnalogBind>> AllTrollersAnalog = new Dictionary<string, Dictionary<string, AnalogBind>>();

		// Core Pick
		// as this setting spans multiple cores and doesn't actually affect the behavior of any core,
		// it hasn't been absorbed into the new system
		public bool GB_AsSGB = false;
		public bool UseSubNESHawk = false;
		public bool NES_InQuickNES = true;
		public bool SNES_InSnes9x = true;
		public bool GBA_UsemGBA = true;
		public bool SGB_UseBsnes = false;
		public bool GB_UseGBHawk = false;
		public bool CoreForcingViaGameDB = true;
		public string LibretroCore;

		public string LastWrittenFrom = VersionInfo.Mainversion;
		public string LastWrittenFromDetailed = VersionInfo.GetEmuVersion();
	}

	// These are used in the defctrl.json or wherever
	public class ControlDefaults
	{
		public Dictionary<string, Dictionary<string, string>> AllTrollers = new Dictionary<string, Dictionary<string, string>>();
		public Dictionary<string, Dictionary<string, string>> AllTrollersAutoFire = new Dictionary<string, Dictionary<string, string>>();
		public Dictionary<string, Dictionary<string, Config.AnalogBind>> AllTrollersAnalog = new Dictionary<string, Dictionary<string, Config.AnalogBind>>();
	}
}