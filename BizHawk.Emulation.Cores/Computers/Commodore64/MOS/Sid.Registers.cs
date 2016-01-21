﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizHawk.Emulation.Cores.Computers.Commodore64.MOS
{
	public sealed partial class Sid
	{
		public int Peek(int addr)
		{
			return ReadRegister(addr & 0x1F);
		}

		public void Poke(int addr, int val)
		{
			WriteRegister(addr & 0x1F, val);
		}

		public int Read(int addr)
		{
			addr &= 0x1F;
            var result = 0x00;
			switch (addr)
			{
				case 0x19:
				case 0x1A:
				case 0x1B:
				case 0x1C:
					Flush();
					result = ReadRegister(addr);
					break;
			}
			return result;
		}

		private int ReadRegister(int addr)
		{
            var result = 0x00;

			switch (addr)
			{
				case 0x00: result = voices[0].FrequencyLo; break;
				case 0x01: result = voices[0].FrequencyHi; break;
				case 0x02: result = voices[0].PulseWidthLo; break;
				case 0x03: result = voices[0].PulseWidthHi; break;
				case 0x04:
					result = (envelopes[0].Gate ? 0x01 : 0x00) |
					         (voices[0].Sync ? 0x02 : 0x00) |
					         (voices[0].RingMod ? 0x04 : 0x00) |
					         (voices[0].Test ? 0x08 : 0x00) |
					         (voices[0].Waveform << 4);
					break;
				case 0x05:
					result = (envelopes[0].Attack << 4) |
					         envelopes[0].Decay;
					break;
				case 0x06:
					result = (envelopes[0].Sustain << 4) |
					         envelopes[0].Release;
					break;
				case 0x07: result = voices[1].FrequencyLo; break;
				case 0x08: result = voices[1].FrequencyHi; break;
				case 0x09: result = voices[1].PulseWidthLo; break;
				case 0x0A: result = voices[1].PulseWidthHi; break;
				case 0x0B:
					result = (envelopes[1].Gate ? 0x01 : 0x00) |
					         (voices[1].Sync ? 0x02 : 0x00) |
					         (voices[1].RingMod ? 0x04 : 0x00) |
					         (voices[1].Test ? 0x08 : 0x00) |
					         (voices[1].Waveform << 4);
					break;
				case 0x0C:
					result = (envelopes[1].Attack << 4) |
					         envelopes[1].Decay;
					break;
				case 0x0D:
					result = (envelopes[1].Sustain << 4) |
					         envelopes[1].Release;
					break;
				case 0x0E: result = voices[2].FrequencyLo; break;
				case 0x0F: result = voices[2].FrequencyHi; break;
				case 0x10: result = voices[2].PulseWidthLo; break;
				case 0x11: result = voices[2].PulseWidthHi; break;
				case 0x12:
					result = (envelopes[2].Gate ? 0x01 : 0x00) |
					         (voices[2].Sync ? 0x02 : 0x00) |
					         (voices[2].RingMod ? 0x04 : 0x00) |
					         (voices[2].Test ? 0x08 : 0x00) |
					         (voices[2].Waveform << 4);
					break;
				case 0x13:
					result = (envelopes[2].Attack << 4) |
					         envelopes[2].Decay;
					break;
				case 0x14:
					result = (envelopes[2].Sustain << 4) |
					         envelopes[2].Release;
					break;
				case 0x15: result = filterFrequency & 0x7; break;
				case 0x16: result = (filterFrequency >> 3) & 0xFF; break;
				case 0x17:
					result = (filterEnable[0] ? 0x01 : 0x00) |
					         (filterEnable[1] ? 0x02 : 0x00) |
					         (filterEnable[2] ? 0x04 : 0x00) |
					         (filterResonance << 4);
					break;
				case 0x18:
					result = volume |
					         (filterSelectLoPass ? 0x10 : 0x00) |
					         (filterSelectBandPass ? 0x20 : 0x00) |
					         (filterSelectHiPass ? 0x40 : 0x00) |
					         (disableVoice3 ? 0x80 : 0x00);
					break;
				case 0x19: result = potX; break;
				case 0x1A: result = potY; break;
				case 0x1B: result = voiceOutput[2] >> 4; break;
				case 0x1C: result = envelopeOutput[2]; break;
			}

			return result;
		}

		public void Write(int addr, int val)
		{
			addr &= 0x1F;
			switch (addr)
			{
				case 0x19:
				case 0x1A:
				case 0x1B:
				case 0x1C:
				case 0x1D:
				case 0x1E:
				case 0x1F:
					// can't write to these
					break;
				default:
					Flush();
					WriteRegister(addr, val);
					break;
			}
		}

		private void WriteRegister(int addr, int val)
		{
			switch (addr)
			{
				case 0x00: voices[0].FrequencyLo = val; break;
				case 0x01: voices[0].FrequencyHi = val; break;
				case 0x02: voices[0].PulseWidthLo = val; break;
				case 0x03: voices[0].PulseWidthHi = val; break;
				case 0x04: voices[0].Control = val; envelopes[0].Gate = (val & 0x01) != 0; break;
				case 0x05: envelopes[0].Attack = val >> 4; envelopes[0].Decay = val & 0xF; break;
				case 0x06: envelopes[0].Sustain = val >> 4; envelopes[0].Release = val & 0xF; break;
				case 0x07: voices[1].FrequencyLo = val; break;
				case 0x08: voices[1].FrequencyHi = val; break;
				case 0x09: voices[1].PulseWidthLo = val; break;
				case 0x0A: voices[1].PulseWidthHi = val; break;
				case 0x0B: voices[1].Control = val; envelopes[1].Gate = (val & 0x01) != 0; break;
				case 0x0C: envelopes[1].Attack = val >> 4; envelopes[1].Decay = val & 0xF; break;
				case 0x0D: envelopes[1].Sustain = val >> 4; envelopes[1].Release = val & 0xF; break;
				case 0x0E: voices[2].FrequencyLo = val; break;
				case 0x0F: voices[2].FrequencyHi = val; break;
				case 0x10: voices[2].PulseWidthLo = val; break;
				case 0x11: voices[2].PulseWidthHi = val; break;
				case 0x12: voices[2].Control = val; envelopes[2].Gate = (val & 0x01) != 0; break;
				case 0x13: envelopes[2].Attack = val >> 4; envelopes[2].Decay = val & 0xF; break;
				case 0x14: envelopes[2].Sustain = val >> 4; envelopes[2].Release = val & 0xF; break;
				case 0x15: filterFrequency &= 0x3FF; filterFrequency |= val & 0x7; break;
				case 0x16: filterFrequency &= 0x7; filterFrequency |= val << 3; break;
				case 0x17:
					filterEnable[0] = (val & 0x1) != 0;
					filterEnable[1] = (val & 0x2) != 0;
					filterEnable[2] = (val & 0x4) != 0;
					filterResonance = val >> 4;
					break;
				case 0x18:
					volume = val & 0xF;
					filterSelectLoPass = (val & 0x10) != 0;
					filterSelectBandPass = (val & 0x20) != 0;
					filterSelectHiPass = (val & 0x40) != 0;
					disableVoice3 = (val & 0x40) != 0;
					break;
				case 0x19:
					potX = val;
					break;
				case 0x1A:
					potY = val;
					break;
			}
		}
	}
}
