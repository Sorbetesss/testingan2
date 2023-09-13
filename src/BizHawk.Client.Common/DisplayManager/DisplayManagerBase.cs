﻿// TODO
// we could flag textures as 'actually' render targets (keep a reference to the render target?) which could allow us to convert between them more quickly in some cases

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;

using BizHawk.Bizware.BizwareGL;
using BizHawk.Bizware.BizwareGL.DrawingExtensions;
using BizHawk.Client.Common.FilterManager;
using BizHawk.Client.Common.Filters;
using BizHawk.Common.CollectionExtensions;
using BizHawk.Common.PathExtensions;
using BizHawk.Emulation.Common;
using BizHawk.Emulation.Cores.Consoles.Nintendo.N3DS;
using BizHawk.Emulation.Cores.Consoles.Nintendo.NDS;
using BizHawk.Emulation.Cores.Sony.PSX;

namespace BizHawk.Client.Common
{
	/// <summary>
	/// A DisplayManager is destined forevermore to drive the PresentationPanel it gets initialized with.
	/// Its job is to receive OSD and emulator outputs, and produce one single buffer (BitmapBuffer? Texture2d?) for display by the PresentationPanel.
	/// Details TBD
	/// </summary>
	public abstract class DisplayManagerBase : IDisposable
	{
		private static DisplaySurface CreateDisplaySurface(int w, int h) => new(w, h);

		protected class DisplayManagerRenderTargetProvider : IRenderTargetProvider
		{
			private readonly Func<Size, RenderTarget> _callback;

			RenderTarget IRenderTargetProvider.Get(Size size)
			{
				return _callback(size);
			}

			public DisplayManagerRenderTargetProvider(Func<Size, RenderTarget> callback)
			{
				_callback = callback;
			}
		}

		public OSDManager OSD { get; }

		protected Config GlobalConfig;

		private IEmulator GlobalEmulator;

		protected DisplayManagerBase(
			Config config,
			IEmulator emulator,
			InputManager inputManager,
			IMovieSession movieSession,
			EDispMethod dispMethod,
			IGL gl,
			IGuiRenderer renderer)
		{
			GlobalConfig = config;
			GlobalEmulator = emulator;
			OSD = new(config, emulator, inputManager, movieSession);
			_gl = gl;
			_renderer = renderer;

			// it's sort of important for these to be initialized to something nonzero
			_currEmuWidth = _currEmuHeight = 1;

			_videoTextureFrugalizer = new(_gl);

			_shaderChainFrugalizers = new RenderTargetFrugalizer[16]; // hacky hardcoded limit.. need some other way to manage these
			for (var i = 0; i < 16; i++)
			{
				_shaderChainFrugalizers[i] = new(_gl);
			}

			{
				using var xml = ReflectionCache.EmbeddedResourceStream("Resources.courier16px.fnt");
				using var tex = ReflectionCache.EmbeddedResourceStream("Resources.courier16px_0.png");
				_theOneFont = new(_gl, xml, tex);
				using var gens = ReflectionCache.EmbeddedResourceStream("Resources.gens.ttf");
				LoadCustomFont(gens);
				using var fceux = ReflectionCache.EmbeddedResourceStream("Resources.fceux.ttf");
				LoadCustomFont(fceux);
			}

			if (dispMethod is EDispMethod.OpenGL or EDispMethod.D3D9)
			{
				var fiHq2x = new FileInfo(Path.Combine(PathUtils.ExeDirectoryPath, "Shaders/BizHawk/hq2x.cgp"));
				if (fiHq2x.Exists)
				{
					using var stream = fiHq2x.OpenRead();
					_shaderChainHq2X = new(_gl, new(stream), Path.Combine(PathUtils.ExeDirectoryPath, "Shaders/BizHawk"));
				}
				var fiScanlines = new FileInfo(Path.Combine(PathUtils.ExeDirectoryPath, "Shaders/BizHawk/BizScanlines.cgp"));
				if (fiScanlines.Exists)
				{
					using var stream = fiScanlines.OpenRead();
					_shaderChainScanlines = new(_gl, new(stream), Path.Combine(PathUtils.ExeDirectoryPath, "Shaders/BizHawk"));
				}
				var bicubicPath = dispMethod == EDispMethod.D3D9 ? "Shaders/BizHawk/bicubic-normal.cgp" : "Shaders/BizHawk/bicubic-fast.cgp";
				var fiBicubic = new FileInfo(Path.Combine(PathUtils.ExeDirectoryPath, bicubicPath));
				if (fiBicubic.Exists)
				{
					using var stream = fiBicubic.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
					_shaderChainBicubic = new(_gl, new(stream), Path.Combine(PathUtils.ExeDirectoryPath, "Shaders/BizHawk"));
				}
			}

			_apiHawkSurfaceSets[DisplaySurfaceID.EmuCore] = new(CreateDisplaySurface);
			_apiHawkSurfaceSets[DisplaySurfaceID.Client] = new(CreateDisplaySurface);
			_apiHawkSurfaceFrugalizers[DisplaySurfaceID.EmuCore] = new(_gl);
			_apiHawkSurfaceFrugalizers[DisplaySurfaceID.Client] = new(_gl);

			RefreshUserShader();
		}

		public void UpdateGlobals(Config config, IEmulator emulator)
		{
			GlobalConfig = config;
			GlobalEmulator = emulator;
			OSD.UpdateGlobals(config, emulator);
		}

		public bool Disposed { get; private set; }

		public void Dispose()
		{
			if (Disposed)
			{
				return;
			}

			Disposed = true;

			// OpenGL context needs to be active when Dispose()'ing
			ActivateOpenGLContext();

			_videoTextureFrugalizer.Dispose();
			foreach (var f in _apiHawkSurfaceFrugalizers.Values)
			{
				f.Dispose();
			}

			foreach (var f in _shaderChainFrugalizers)
			{
				f?.Dispose();
			}

			foreach (var s in new[] { _shaderChainHq2X, _shaderChainScanlines, _shaderChainBicubic, _shaderChainUser })
			{
				s?.Dispose();
			}

			_theOneFont.Dispose();
			_renderer.Dispose();
		}

		// rendering resources:
		protected readonly IGL _gl;

		private readonly StringRenderer _theOneFont;

		private readonly IGuiRenderer _renderer;

		// layer resources
		protected FilterProgram _currentFilterProgram;

		/// <summary>
		/// these variables will track the dimensions of the last frame's (or the next frame? this is confusing) emulator native output size
		/// THIS IS OLD JUNK. I should get rid of it, I think. complex results from the last filter ingestion should be saved instead.
		/// </summary>
		private int _currEmuWidth, _currEmuHeight;

		/// <summary>
		/// additional pixels added at the unscaled level for the use of lua drawing. essentially increases the input video provider dimensions
		/// </summary>
		public (int Left, int Top, int Right, int Bottom) GameExtraPadding { get; set; }

		/// <summary>
		/// additional pixels added at the native level for the use of lua drawing. essentially just gets tacked onto the final calculated window sizes.
		/// </summary>
		public (int Left, int Top, int Right, int Bottom) ClientExtraPadding { get; set; }

		/// <summary>
		/// custom fonts that don't need to be installed on the user side
		/// </summary>
		public PrivateFontCollection CustomFonts { get; } = new();

		private readonly TextureFrugalizer _videoTextureFrugalizer;

		private readonly Dictionary<DisplaySurfaceID, TextureFrugalizer> _apiHawkSurfaceFrugalizers = new();

		protected readonly RenderTargetFrugalizer[] _shaderChainFrugalizers;

		private readonly RetroShaderChain _shaderChainHq2X;

		private readonly RetroShaderChain _shaderChainScanlines;

		private readonly RetroShaderChain _shaderChainBicubic;

		private RetroShaderChain _shaderChainUser;

		public abstract void ActivateOpenGLContext();

		protected abstract void ActivateGraphicsControlContext();

		protected abstract void SwapBuffersOfGraphicsControl();

		public void RefreshUserShader()
		{
			_shaderChainUser?.Dispose();
			if (File.Exists(GlobalConfig.DispUserFilterPath))
			{
				var fi = new FileInfo(GlobalConfig.DispUserFilterPath);
				using var stream = fi.OpenRead();
				_shaderChainUser = new(_gl, new(stream), Path.GetDirectoryName(GlobalConfig.DispUserFilterPath));
			}
		}

		private (int Left, int Top, int Right, int Bottom) CalculateCompleteContentPadding(bool user, bool source)
		{
			var padding = (Left: 0, Top: 0, Right: 0, Bottom: 0);

			if (user)
			{
				padding.Left += GameExtraPadding.Left;
				padding.Top += GameExtraPadding.Top;
				padding.Right += GameExtraPadding.Right;
				padding.Bottom += GameExtraPadding.Bottom;
			}

			// an experimental feature
			if (source && GlobalEmulator is Octoshock psx)
			{
				var corePadding = psx.VideoProvider_Padding;
				padding.Left += corePadding.Width / 2;
				padding.Right += corePadding.Width - corePadding.Width / 2;
				padding.Top += corePadding.Height / 2;
				padding.Bottom += corePadding.Height - corePadding.Height / 2;
			}

			// apply user's crop selections as a negative padding (believe it or not, this largely works)
			// is there an issue with the aspect ratio? I don't know--but if there is, there would be with the padding too
			padding.Left -= GlobalConfig.DispCropLeft;
			padding.Right -= GlobalConfig.DispCropRight;
			padding.Top -= GlobalConfig.DispCropTop;
			padding.Bottom -= GlobalConfig.DispCropBottom;

			return padding;
		}

		private (int Horizontal, int Vertical) CalculateCompleteContentPaddingSum(bool user, bool source)
		{
			var p = CalculateCompleteContentPadding(user: user, source: source);
			return (p.Left + p.Right, p.Top + p.Bottom);
		}

		private FilterProgram BuildDefaultChain(Size chainInSize, Size chainOutSize, bool includeOSD, bool includeUserFilters)
		{
			// select user special FX shader chain
			var selectedChainProperties = new Dictionary<string, object>();
			RetroShaderChain selectedChain = null;
			switch (GlobalConfig.TargetDisplayFilter)
			{
				case 1 when _shaderChainHq2X is { Available: true }:
					selectedChain = _shaderChainHq2X;
					break;
				case 2 when _shaderChainScanlines is { Available: true }:
					selectedChain = _shaderChainScanlines;
					selectedChainProperties["uIntensity"] = 1.0f - GlobalConfig.TargetScanlineFilterIntensity / 256.0f;
					break;
				case 3 when _shaderChainUser is { Available: true }:
					selectedChain = _shaderChainUser;
					break;
			}

			if (!includeUserFilters)
			{
				selectedChain = null;
			}

			var fCoreScreenControl = CreateCoreScreenControl();

			var fPresent = new FinalPresentation(chainOutSize);
			var fInput = new SourceImage(chainInSize);
			var fOSD = new OSD(includeOSD, OSD, _theOneFont);

			var chain = new FilterProgram();

			//add the first filter, encompassing output from the emulator core
			chain.AddFilter(fInput, "input");

			if (fCoreScreenControl != null)
			{
				chain.AddFilter(fCoreScreenControl, "CoreScreenControl");
			}

			// if a non-zero padding is required, add a filter to allow for that
			// note, we have two sources of padding right now.. one can come from the VideoProvider and one from the user.
			// we're combining these now and just using black, for sake of being lean, despite the discussion below:
			// keep in mind, the VideoProvider design in principle might call for another color.
			// we haven't really been using this very hard, but users will probably want black there (they could fill it to another color if needed tho)
			var padding = CalculateCompleteContentPadding(true, true);
			if (padding != (0, 0, 0, 0))
			{
				// TODO - add another filter just for this, its cumbersome to use final presentation... I think. but maybe there's enough similarities to justify it.
				var size = chainInSize;
				size.Width += padding.Left + padding.Right;
				size.Height += padding.Top + padding.Bottom;

				// in case the user requested so much padding that the dimensions are now negative, just turn it to something small
				if (size.Width < 1) size.Width = 1;
				if (size.Height < 1) size.Height = 1;

				var fPadding = new FinalPresentation(size);
				chain.AddFilter(fPadding, "padding");
				fPadding.Config_PadOnly = true;
				fPadding.Padding = padding;
			}

			// add lua layer 'emu'
			AppendApiHawkLayer(chain, DisplaySurfaceID.EmuCore);

			if (includeUserFilters)
			{
				if (GlobalConfig.DispPrescale != 1)
				{
					var fPrescale = new PrescaleFilter() { Scale = GlobalConfig.DispPrescale };
					chain.AddFilter(fPrescale, "user_prescale");
				}
			}

			// add user-selected retro shader
			if (selectedChain != null)
			{
				AppendRetroShaderChain(chain, "retroShader", selectedChain, selectedChainProperties);
			}

			// AutoPrescale makes no sense for a None final filter
			if (GlobalConfig.DispAutoPrescale && GlobalConfig.DispFinalFilter != (int)FinalPresentation.eFilterOption.None)
			{
				var apf = new AutoPrescaleFilter();
				chain.AddFilter(apf, "auto_prescale");
			}

			// choose final filter
			var finalFilter = GlobalConfig.DispFinalFilter switch
			{
				1 => FinalPresentation.eFilterOption.Bilinear,
				2 => FinalPresentation.eFilterOption.Bicubic,
				_ => FinalPresentation.eFilterOption.None
			};

			// if bicubic is selected and unavailable, don't use it. use bilinear instead I guess
			if (finalFilter == FinalPresentation.eFilterOption.Bicubic)
			{
				if (_shaderChainBicubic is not { Available: true })
				{
					finalFilter = FinalPresentation.eFilterOption.Bilinear;
				}
			}

			fPresent.FilterOption = finalFilter;

			// now if bicubic is chosen, insert it
			if (finalFilter == FinalPresentation.eFilterOption.Bicubic)
			{
				AppendRetroShaderChain(chain, "bicubic", _shaderChainBicubic, null);
			}

			// add final presentation
			if (includeUserFilters)
			{
				chain.AddFilter(fPresent, "presentation");
			}

			//add lua layer 'native'
			AppendApiHawkLayer(chain, DisplaySurfaceID.Client);

			// and OSD goes on top of that
			// TODO - things break if this isn't present (the final presentation filter gets messed up when used with prescaling)
			// so, always include it (we'll handle includeOSD on Run)
			chain.AddFilter(fOSD, "osd");

			return chain;
		}

		private static void AppendRetroShaderChain(FilterProgram program, string name, RetroShaderChain retroChain, Dictionary<string, object> properties)
		{
			for (var i = 0; i < retroChain.Passes.Length; i++)
			{
				var rsp = new RetroShaderPass(retroChain, i);
				var fname = $"{name}[{i}]";
				program.AddFilter(rsp, fname);
				rsp.Parameters = properties;
			}
		}

		private void AppendApiHawkLayer(FilterProgram chain, DisplaySurfaceID surfaceID)
		{
			var luaNativeSurface = _apiHawkSurfaceSets[surfaceID].GetCurrent();
			if (luaNativeSurface == null)
			{
				return;
			}

			var luaNativeTexture = _apiHawkSurfaceFrugalizers[surfaceID].Get(luaNativeSurface);
			var fLuaLayer = new LuaLayer();
			fLuaLayer.SetTexture(luaNativeTexture);
			chain.AddFilter(fLuaLayer, surfaceID.GetName());
		}

		protected abstract Point GraphicsControlPointToClient(Point p);

		/// <summary>
		/// Using the current filter program, turn a mouse coordinate from window space to the original emulator screen space.
		/// </summary>
		public Point UntransformPoint(Point p)
		{
			// first, turn it into a window coordinate
			p = GraphicsControlPointToClient(p);

			// now, if there's no filter program active, just give up
			if (_currentFilterProgram == null) return p;

			// otherwise, have the filter program untransform it
			var v = new Vector2(p.X, p.Y);
			v = _currentFilterProgram.UntransformPoint("default", v);

			return new((int)v.X, (int)v.Y);
		}

		/// <summary>
		/// Using the current filter program, turn a emulator screen space coordinate to a window coordinate (suitable for lua layer drawing)
		/// </summary>
		public Point TransformPoint(Point p)
		{
			// now, if there's no filter program active, just give up
			if (_currentFilterProgram == null)
			{
				return p;
			}

			// otherwise, have the filter program untransform it
			var v = new Vector2(p.X, p.Y);
			v = _currentFilterProgram.TransformPoint("default", v);
			return new((int)v.X, (int)v.Y);
		}

		public abstract Size GetPanelNativeSize();

		protected abstract Size GetGraphicsControlSize();

		/// <summary>
		/// This will receive an emulated output frame from an IVideoProvider and run it through the complete frame processing pipeline
		/// Then it will stuff it into the bound PresentationPanel.
		/// </summary>
		public void UpdateSource(IVideoProvider videoProvider)
		{
			var displayNothing = GlobalConfig.DispSpeedupFeatures == 0;
			var job = new JobInfo
			{
				VideoProvider = videoProvider,
				Simulate = displayNothing,
				ChainOutsize = GetGraphicsControlSize(),
				IncludeOSD = true,
				IncludeUserFilters = true
			};

			UpdateSourceInternal(job);
		}

		private BaseFilter CreateCoreScreenControl()
		{
			return GlobalEmulator switch
			{
				NDS nds => new ScreenControlNDS(nds),
				Citra citra => new ScreenControl3DS(citra),
				_ => null
			};
		}

		/// <summary>
		/// Does the entire display process to an offscreen buffer, suitable for a 'client' screenshot.
		/// </summary>
		public BitmapBuffer RenderOffscreen(IVideoProvider videoProvider, bool includeOSD)
		{
			var job = new JobInfo
			{
				VideoProvider = videoProvider,
				Simulate = false,
				ChainOutsize = GetGraphicsControlSize(),
				Offscreen = true,
				IncludeOSD = includeOSD,
				IncludeUserFilters = true,
			};

			UpdateSourceInternal(job);
			return job.OffscreenBb;
		}
		/// <summary>
		/// Does the display process to an offscreen buffer, suitable for a Lua-inclusive movie.
		/// </summary>
		public BitmapBuffer RenderOffscreenLua(IVideoProvider videoProvider)
		{
			var job = new JobInfo
			{
				VideoProvider = videoProvider,
				Simulate = false,
				ChainOutsize = new(videoProvider.BufferWidth, videoProvider.BufferHeight),
				Offscreen = true,
				IncludeOSD = false,
				IncludeUserFilters = false,
			};

			UpdateSourceInternal(job);
			return job.OffscreenBb;
		}

		private class FakeVideoProvider : IVideoProvider
		{
			public FakeVideoProvider(int bw, int bh, int vw, int vh)
			{
				BufferWidth = bw;
				BufferHeight = bh;
				VirtualWidth = vw;
				VirtualHeight = vh;
			}

			public int[] GetVideoBuffer()
				=> Array.Empty<int>();

			public int VirtualWidth { get; }
			public int VirtualHeight { get; }
			public int BufferWidth { get; }
			public int BufferHeight { get; }
			public int BackgroundColor => 0;

			public int VsyncNumerator => throw new NotImplementedException();
			public int VsyncDenominator => throw new NotImplementedException();
		}

		private static void FixRatio(float x, float y, int inw, int inh, out int outW, out int outH)
		{
			var ratio = x / y;
			if (ratio <= 1)
			{
				// taller. weird. expand height.
				outW = inw;
				outH = (int)(inw / ratio);
			}
			else
			{
				// wider. normal. expand width.
				outW = (int)(inh * ratio);
				outH = inh;
			}
		}

		/// <summary>
		/// Attempts to calculate a good client size with the given zoom factor, considering the user's DisplayManager preferences
		/// TODO - this needs to be redone with a concept different from zoom factor.
		/// basically, each increment of a 'zoom-like' factor should definitely increase the viewable area somehow, even if it isnt strictly by an entire zoom level.
		/// </summary>
		public Size CalculateClientSize(IVideoProvider videoProvider, int zoom)
		{
			var arActive = GlobalConfig.DispFixAspectRatio;
			var arSystem = GlobalConfig.DispManagerAR == EDispManagerAR.System;
			var arCustom = GlobalConfig.DispManagerAR == EDispManagerAR.CustomSize;
			var arCustomRatio = GlobalConfig.DispManagerAR == EDispManagerAR.CustomRatio;
			var arCorrect = arSystem || arCustom || arCustomRatio;
			var arInteger = GlobalConfig.DispFixScaleInteger;

			var bufferWidth = videoProvider.BufferWidth;
			var bufferHeight = videoProvider.BufferHeight;
			var virtualWidth = videoProvider.VirtualWidth;
			var virtualHeight = videoProvider.VirtualHeight;

			if (arCustom)
			{
				virtualWidth = GlobalConfig.DispCustomUserARWidth;
				virtualHeight = GlobalConfig.DispCustomUserARHeight;
			}

			if (arCustomRatio)
			{
				FixRatio(GlobalConfig.DispCustomUserArx, GlobalConfig.DispCustomUserAry,
					videoProvider.BufferWidth, videoProvider.BufferHeight, out virtualWidth, out virtualHeight);
			}

			// TODO: it is bad that this is happening outside the filter chain
			// the filter chain has the ability to add padding...
			// for now, we have to have some hacks. this could be improved by refactoring the filter setup hacks to be in one place only though
			// could the PADDING be done as filters too? that would be nice.
			var fCoreScreenControl = CreateCoreScreenControl();
			if (fCoreScreenControl != null)
			{
				var sz = fCoreScreenControl.PresizeInput("default", new(bufferWidth, bufferHeight));
				virtualWidth = bufferWidth = sz.Width;
				virtualHeight = bufferHeight = sz.Height;
			}

			var padding = CalculateCompleteContentPaddingSum(true, false);
			virtualWidth += padding.Horizontal;
			virtualHeight += padding.Vertical;

			padding = CalculateCompleteContentPaddingSum(true, true);
			bufferWidth += padding.Horizontal;
			bufferHeight += padding.Vertical;

			// in case the user requested so much padding that the dimensions are now negative, just turn it to something small.
			if (virtualWidth < 1) virtualWidth = 1;
			if (virtualHeight < 1) virtualHeight = 1;
			if (bufferWidth < 1) bufferWidth = 1;
			if (bufferHeight < 1) bufferHeight = 1;

			// old stuff
			var fvp = new FakeVideoProvider(bufferWidth, bufferHeight, virtualWidth, virtualHeight);
			Size chainOutsize;

			if (arActive)
			{
				if (arCorrect)
				{
					if (arInteger)
					{
						// ALERT COPYPASTE LAUNDROMAT
						var AR = new Vector2(virtualWidth / (float) bufferWidth, virtualHeight / (float) bufferHeight);
						var targetPar = AR.X / AR.Y;

						// this would malfunction for AR <= 0.5 or AR >= 2.0
						// EDIT - in fact, we have AR like that coming from PSX, sometimes, so maybe we should solve this better
						var PS = Vector2.One; // this would malfunction for AR <= 0.5 or AR >= 2.0

						// here's how we define zooming, in this case:
						// make sure each step is an increment of zoom for at least one of the dimensions (or maybe both of them)
						// look for the increment which helps the AR the best
						// TODO - this cant possibly support scale factors like 1.5x
						// TODO - also, this might be messing up zooms and stuff, we might need to run this on the output size of the filter chain

						Span<Vector2> trials = stackalloc Vector2[3];
						for (var i = 1; i < zoom; i++)
						{
							// would not be good to run this per frame, but it seems to only run when the resolution changes, etc.
							trials[0] = PS + Vector2.UnitX;
							trials[1] = PS + Vector2.UnitY;
							trials[2] = PS + Vector2.One;

							var bestIndex = -1;
							var bestValue = 1000.0f;
							for (var t = 0; t < trials.Length; t++)
							{
								//I.
								var testAr = trials[t].X / trials[t].Y;

								// II.
								// var calc = Vector2.Multiply(trials[t], VS);
								// var test_ar = calc.X / calc.Y;

								// not clear which approach is superior

								var deviationLinear = Math.Abs(testAr - targetPar);
								if (deviationLinear < bestValue)
								{
									bestIndex = t;
									bestValue = deviationLinear;
								}
							}

							// is it possible to get here without selecting one? doubtful.
							// EDIT: YES IT IS. it happened with an 0,0 buffer size. of course, that was a mistake, but we shouldn't crash
							if (bestIndex != -1) // so, what now? well, this will result in 0,0 getting picked, so that's probably all we can do
							{
								PS = trials[bestIndex];
							}
						}

						chainOutsize = new((int)(bufferWidth * PS.X), (int)(bufferHeight * PS.Y));
					}
					else
					{
						// obey the AR, but allow free scaling: just zoom the virtual size
						chainOutsize = new(virtualWidth * zoom, virtualHeight * zoom);
					}
				}
				else
				{
					// ar_unity:
					// just choose to zoom the buffer (make no effort to incorporate AR)
					chainOutsize = new(bufferWidth * zoom, bufferHeight * zoom);
				}
			}
			else
			{
				// !ar_active:
				// just choose to zoom the buffer (make no effort to incorporate AR)
				chainOutsize = new(bufferWidth * zoom, bufferHeight * zoom);
			}

			chainOutsize.Width += ClientExtraPadding.Left + ClientExtraPadding.Right;
			chainOutsize.Height += ClientExtraPadding.Top + ClientExtraPadding.Bottom;

			var job = new JobInfo
			{
				VideoProvider = fvp,
				Simulate = true,
				ChainOutsize = chainOutsize,
				IncludeUserFilters = true,
				IncludeOSD = true,
			};
			var filterProgram = UpdateSourceInternal(job);

			// this only happens when we're forcing the client to size itself with autoload and the core says 0x0....
			// we need some other more sensible client size.
			if (filterProgram == null)
			{
				return new(256, 192);
			}

			var size = filterProgram.Filters[filterProgram.Filters.Count - 1].FindOutput().SurfaceFormat.Size;
			return size;
		}

		protected class JobInfo
		{
			public IVideoProvider VideoProvider;
			public bool Simulate;
			public Size ChainOutsize;
			public bool Offscreen;
			public BitmapBuffer OffscreenBb;
			public bool IncludeOSD;

			/// <summary>
			/// This has been changed a bit to mean "not raw".
			/// Someone needs to rename it, but the sense needs to be inverted and some method args need renaming too
			/// Suggested: IsRaw (with inverted sense)
			/// </summary>
			public bool IncludeUserFilters;
		}

		private FilterProgram UpdateSourceInternal(JobInfo job)
		{
			// no drawing actually happens
			if (!job.Simulate)
			{
				ActivateGraphicsControlContext();

				if (job.ChainOutsize.Width == 0 || job.ChainOutsize.Height == 0)
				{
					// this has to be a NOP, because lots of stuff will malfunction on a 0-sized viewport
					if (_currentFilterProgram != null)
					{
						UpdateSourceDrawingWork(job); //but we still need to do this, because of vsync
					}

					return null;
				}
			}

			var videoProvider = job.VideoProvider;
			var simulate = job.Simulate;
			var chainOutsize = job.ChainOutsize;

			var bufferWidth = videoProvider.BufferWidth;
			var bufferHeight = videoProvider.BufferHeight;
			var presenterTextureWidth = bufferWidth;
			var presenterTextureHeight = bufferHeight;

			var vw = videoProvider.VirtualWidth;
			var vh = videoProvider.VirtualHeight;

			// TODO: it is bad that this is happening outside the filter chain
			// the filter chain has the ability to add padding...
			// for now, we have to have some hacks. this could be improved by refactoring the filter setup hacks to be in one place only though
			// could the PADDING be done as filters too? that would be nice.
			var fCoreScreenControl = CreateCoreScreenControl();
			if(fCoreScreenControl != null)
			{
				var sz = fCoreScreenControl.PresizeInput("default", new(bufferWidth, bufferHeight));
				presenterTextureWidth = vw = sz.Width;
				presenterTextureHeight = vh = sz.Height;
			}

			if (GlobalConfig.DispFixAspectRatio)
			{
				switch (GlobalConfig.DispManagerAR)
				{
					case EDispManagerAR.None:
						vw = bufferWidth;
						vh = bufferHeight;
						break;
					case EDispManagerAR.System:
						// Already set
						break;
					case EDispManagerAR.CustomSize:
						// not clear what any of these other options mean for "screen controlled" systems
						vw = GlobalConfig.DispCustomUserARWidth;
						vh = GlobalConfig.DispCustomUserARHeight;
						break;
					case EDispManagerAR.CustomRatio:
						// not clear what any of these other options mean for "screen controlled" systems
						FixRatio(GlobalConfig.DispCustomUserArx, GlobalConfig.DispCustomUserAry, videoProvider.BufferWidth, videoProvider.BufferHeight, out vw, out vh);
						break;
					default:
						throw new InvalidOperationException();
				}
			}

			var padding = CalculateCompleteContentPaddingSum(true,false);
			vw += padding.Horizontal;
			vh += padding.Vertical;

			//in case the user requested so much padding that the dimensions are now negative, just turn it to something small.
			if (vw < 1) vw = 1;
			if (vh < 1) vh = 1;

			BitmapBuffer bb = null;
			Texture2d videoTexture = null;
			if (!simulate)
			{
				if (videoProvider is IGLTextureProvider glTextureProvider && _gl.DispMethodEnum == EDispMethod.OpenGL)
				{
					// FYI: this is a million years from happening on n64, since it's all geriatric non-FBO code
					videoTexture = _gl.WrapGLTexture2d(new(glTextureProvider.GetGLTexture()), bufferWidth, bufferHeight);
				}
				else
				{
					// wrap the VideoProvider data in a BitmapBuffer (no point to refactoring that many IVideoProviders)
					bb = new(bufferWidth, bufferHeight, videoProvider.GetVideoBuffer());
					bb.DiscardAlpha();

					//now, acquire the data sent from the videoProvider into a texture
					videoTexture = _videoTextureFrugalizer.Get(bb);

					// lets not use this. lets define BizwareGL to make clamp by default (TBD: check opengl)
					// _gl.SetTextureWrapMode(videoTexture, true);
				}
			}

			// record the size of what we received, since lua and stuff is gonna want to draw onto it
			_currEmuWidth = bufferWidth;
			_currEmuHeight = bufferHeight;

			//build the default filter chain and set it up with services filters will need
			var chainInsize = new Size(bufferWidth, bufferHeight);

			var filterProgram = BuildDefaultChain(chainInsize, chainOutsize, job.IncludeOSD, job.IncludeUserFilters);
			filterProgram.GuiRenderer = _renderer;
			filterProgram.GL = _gl;

			//setup the source image filter
			var fInput = (SourceImage)filterProgram["input"];
			fInput.Texture = videoTexture;

			//setup the final presentation filter
			var fPresent = (FinalPresentation)filterProgram["presentation"];
			if (fPresent != null)
			{
				fPresent.VirtualTextureSize = new(vw, vh);
				fPresent.TextureSize = new(presenterTextureWidth, presenterTextureHeight);
				fPresent.BackgroundColor = videoProvider.BackgroundColor;
				fPresent.Config_FixAspectRatio = GlobalConfig.DispFixAspectRatio;
				fPresent.Config_FixScaleInteger = GlobalConfig.DispFixScaleInteger;
				fPresent.Padding = (ClientExtraPadding.Left, ClientExtraPadding.Top, ClientExtraPadding.Right, ClientExtraPadding.Bottom);
			}

			filterProgram.Compile("default", chainInsize, chainOutsize, !job.Offscreen);

			if (simulate)
			{
			}
			else
			{
				_currentFilterProgram = filterProgram;
				UpdateSourceDrawingWork(job);
			}

			// cleanup:
			bb?.Dispose();

			return filterProgram;
		}

		public void Blank()
		{
			ActivateGraphicsControlContext();
			_gl.BeginScene();
			_gl.BindRenderTarget(null);
			_gl.ClearColor(Color.Black);
			_gl.EndScene();
			SwapBuffersOfGraphicsControl();
		}

		protected virtual void UpdateSourceDrawingWork(JobInfo job)
		{
			if (!job.Offscreen) throw new InvalidOperationException();

			// begin rendering on this context
			// should this have been done earlier?
			// do i need to check this on an intel video card to see if running excessively is a problem? (it used to be in the FinalTarget command below, shouldn't be a problem)
			//GraphicsControl.Begin(); // CRITICAL POINT for yabause+GL

			//TODO - auto-create and age these (and dispose when old)
			var rtCounter = 0;
			// ReSharper disable once AccessToModifiedClosure
			_currentFilterProgram.RenderTargetProvider = new DisplayManagerRenderTargetProvider(size => _shaderChainFrugalizers[rtCounter++].Get(size));

			_gl.BeginScene();
			RunFilterChainSteps(ref rtCounter, out var rtCurr, out _);
			_gl.EndScene();

			job.OffscreenBb = rtCurr.Texture2d.Resolve();
			job.OffscreenBb.DiscardAlpha();
		}

		protected void RunFilterChainSteps(ref int rtCounter, out RenderTarget rtCurr, out bool inFinalTarget)
		{
			Texture2d texCurr = null;
			rtCurr = null;
			inFinalTarget = false;
			foreach (var step in _currentFilterProgram.Program) switch (step.Type)
			{
				case FilterProgram.ProgramStepType.Run:
					var f = _currentFilterProgram.Filters[(int) step.Args];
					f.SetInput(texCurr);
					f.Run();
					if (f.FindOutput() is { SurfaceDisposition: SurfaceDisposition.Texture })
					{
						texCurr = f.GetOutput();
						rtCurr = null;
					}
					break;
				case FilterProgram.ProgramStepType.NewTarget:
					_currentFilterProgram.CurrRenderTarget = rtCurr = _shaderChainFrugalizers[rtCounter++].Get((Size) step.Args);
					rtCurr.Bind();
					break;
				case FilterProgram.ProgramStepType.FinalTarget:
					_currentFilterProgram.CurrRenderTarget = rtCurr = null;
					_gl.BindRenderTarget(rtCurr);
					inFinalTarget = true;
					break;
				default:
					throw new InvalidOperationException();
			}
		}

		private void LoadCustomFont(Stream fontStream)
		{
			var data = Marshal.AllocCoTaskMem((int)fontStream.Length);
			try
			{
				var fontData = new byte[fontStream.Length];
				fontStream.Read(fontData, 0, (int)fontStream.Length);
				Marshal.Copy(fontData, 0, data, (int)fontStream.Length);
				CustomFonts.AddMemoryFont(data, fontData.Length);
			}
			finally
			{
				Marshal.FreeCoTaskMem(data);
				fontStream.Close();
			}
		}

		private readonly Dictionary<DisplaySurfaceID, IDisplaySurface> _apiHawkIDToSurface = new();

		/// <remarks>Can't this just be a prop of <see cref="IDisplaySurface"/>? --yoshi</remarks>
		private readonly Dictionary<IDisplaySurface, DisplaySurfaceID> _apiHawkSurfaceToID = new();

		private readonly Dictionary<DisplaySurfaceID, SwappableDisplaySurfaceSet<DisplaySurface>> _apiHawkSurfaceSets = new();

		/// <summary>
		/// Peeks a locked lua surface, or returns null if it isn't locked
		/// </summary>
		public IDisplaySurface PeekApiHawkLockedSurface(DisplaySurfaceID surfaceID)
			=> _apiHawkIDToSurface.TryGetValue(surfaceID, out var surface) ? surface : null;

		public IDisplaySurface LockApiHawkSurface(DisplaySurfaceID surfaceID, bool clear)
		{
			if (_apiHawkIDToSurface.ContainsKey(surfaceID))
			{
				throw new InvalidOperationException($"ApiHawk/Lua surface is already locked: {surfaceID.GetName()}");
			}

			var sdss = _apiHawkSurfaceSets.GetValueOrPut(surfaceID, static _ => new(CreateDisplaySurface));

			// placeholder logic for more abstracted surface definitions from filter chain
			var (currNativeWidth, currNativeHeight) = GetPanelNativeSize();
			currNativeWidth += ClientExtraPadding.Left + ClientExtraPadding.Right;
			currNativeHeight += ClientExtraPadding.Top + ClientExtraPadding.Bottom;

			var (width, height) = surfaceID switch
			{
				DisplaySurfaceID.EmuCore => (GameExtraPadding.Left + _currEmuWidth + GameExtraPadding.Right, GameExtraPadding.Top + _currEmuHeight + GameExtraPadding.Bottom),
				DisplaySurfaceID.Client => (currNativeWidth, currNativeHeight),
				_ => throw new InvalidOperationException()
			};

			IDisplaySurface ret = sdss.AllocateSurface(width, height, clear);
			_apiHawkIDToSurface[surfaceID] = ret;
			_apiHawkSurfaceToID[ret] = surfaceID;
			return ret;
		}

		public void ClearApiHawkSurfaces()
		{
			foreach (var kvp in _apiHawkSurfaceSets)
			{
				try
				{
					if (PeekApiHawkLockedSurface(kvp.Key) == null)
					{
						var surfLocked = LockApiHawkSurface(kvp.Key, true);
						if (surfLocked != null)
						{
							UnlockApiHawkSurface(surfLocked);
						}
					}

					_apiHawkSurfaceSets[kvp.Key].SetPending(null);
				}
				catch (InvalidOperationException)
				{
					// ignored
				}
			}
		}

		/// <summary>unlocks this IDisplaySurface which had better have been locked as a lua surface</summary>
		/// <exception cref="InvalidOperationException">already unlocked</exception>
		public void UnlockApiHawkSurface(IDisplaySurface surface)
		{
			if (surface is not DisplaySurface dispSurfaceImpl)
			{
				throw new ArgumentException("don't mix " + nameof(IDisplaySurface) + " implementations!", nameof(surface));
			}

			if (!_apiHawkSurfaceToID.TryGetValue(dispSurfaceImpl, out var surfaceID))
			{
				throw new InvalidOperationException("Surface was not locked as a lua surface");
			}

			_apiHawkSurfaceToID.Remove(dispSurfaceImpl);
			_apiHawkIDToSurface.Remove(surfaceID);
			_apiHawkSurfaceSets[surfaceID].SetPending(dispSurfaceImpl);
		}
	}
}
