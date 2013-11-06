﻿using System;
using System.Collections.Generic;
using System.IO;
using BizHawk.Common;

namespace BizHawk.Emulation.Common
{
	public class NullEmulator : IEmulator, IVideoProvider, ISoundProvider
	{
		public string SystemId { get { return "NULL"; } }
		public static readonly ControllerDefinition NullController = new ControllerDefinition { Name = "Null Controller" };

		public string BoardName { get { return null; } }

		private readonly int[] frameBuffer = new int[256 * 192];
		private readonly Random rand = new Random();
		public CoreComm CoreComm { get; private set; }
		public IVideoProvider VideoProvider { get { return this; } }
		public ISoundProvider SoundProvider { get { return this; } }
		public ISyncSoundProvider SyncSoundProvider { get { return new FakeSyncSound(this, 735); } }
		public bool StartAsyncSound() { return true; }
		public void EndAsyncSound() { }
		
		public NullEmulator(CoreComm comm)
		{
			CoreComm = comm;
			var domains = new MemoryDomainList(
				new List<MemoryDomain>
				{
					new MemoryDomain("Main RAM", 1, MemoryDomain.Endian.Little, addr => 0, (a, v) => { })
				});
		}
		public void ResetCounters()
		{
			Frame = 0;
			// no lag frames on this stub core
		}

		public void FrameAdvance(bool render, bool rendersound)
		{
			if (render == false) return;
			for (int i = 0; i < 256*192; i++)
			{
				frameBuffer[i] = Colors.Luminosity((byte) rand.Next());
			}
		}
		public ControllerDefinition ControllerDefinition { get { return NullController; } }
		public IController Controller { get; set; }

		public int Frame { get; set; }
		public int LagCount { get { return 0; } set { return; } }
		public bool IsLagFrame { get { return false; } }

		public byte[] ReadSaveRam() { return null; }
		public void StoreSaveRam(byte[] data) { }
		public void ClearSaveRam() { }
		public bool DeterministicEmulation { get { return true; } }
		public bool SaveRamModified { get; set; }
		public void SaveStateText(TextWriter writer) { }
		public void LoadStateText(TextReader reader) { }
		public void SaveStateBinary(BinaryWriter writer) { }
		public void LoadStateBinary(BinaryReader reader) { }
		public byte[] SaveStateBinary() { return new byte[1]; }
		public bool BinarySaveStatesPreferred { get { return false; } }
		public int[] GetVideoBuffer() { return frameBuffer; }
		public int VirtualWidth { get { return 256; } }
		public int BufferWidth { get { return 256; } }
		public int BufferHeight { get { return 192; } }
		public int BackgroundColor { get { return 0; } }
		public void GetSamples(short[] samples) { }
		public void DiscardSamples() { }
		public int MaxVolume { get; set; }
		private readonly MemoryDomainList memoryDomains;
		public MemoryDomainList MemoryDomains { get { return memoryDomains; } }
		public void Dispose() { }
	}

	public class NullSound : ISoundProvider
	{
		public static readonly NullSound SilenceProvider = new NullSound();

		public void GetSamples(short[] samples) { }
		public void DiscardSamples() { }
		public int MaxVolume { get; set; }
	}
}
