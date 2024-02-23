#include <cstdlib>
#include <cstring>
#include "core/emu.hpp"
#include "jaffarCommon/include/file.hpp"
#include "jaffarCommon/include/serializers/contiguous.hpp"
#include "jaffarCommon/include/deserializers/contiguous.hpp"

#ifdef _MSC_VER
#define EXPORT extern "C" __declspec(dllexport)
#elif __MINGW32__
#define EXPORT extern "C" __declspec(dllexport) __attribute__((force_align_arg_pointer))
#else
#define EXPORT extern "C" __attribute__((force_align_arg_pointer))
#endif

EXPORT quickerNES::Emu *qn_new()
{
  // Zero intialized emulator to make super sure no side effects from previous data remains
  auto ptr = calloc(1, sizeof(quickerNES::Emu));
  return new (ptr) quickerNES::Emu();
}

EXPORT void qn_delete(quickerNES::Emu *e)
{
  free(e);
}

EXPORT const char *qn_loadines(quickerNES::Emu *e, const void *data, int length)
{
   e->load_ines((const uint8_t*)data);
   return 0;
}

EXPORT const char *qn_set_sample_rate(quickerNES::Emu *e, int rate)
{
	const char *ret = e->set_sample_rate(rate);
	if (!ret) e->set_equalizer(quickerNES::Emu::nes_eq);
	return ret;
}

EXPORT const char *qn_emulate_frame(quickerNES::Emu *e, int pad1, int pad2)
{
	return e->emulate_frame((uint32_t)pad1, (uint32_t)pad2);
}

EXPORT void qn_blit(quickerNES::Emu *e, int32_t *dest, const int32_t *colors, int cropleft, int croptop, int cropright, int cropbottom)
{
	// what is the point of the 256 color bitmap and the dynamic color allocation to it?
	// why not just render directly to a 512 color bitmap with static palette positions?

	const int srcpitch = e->frame().pitch;
	const unsigned char *src = e->frame().pixels;
	const unsigned char *const srcend = src + (e->image_height - cropbottom) * srcpitch;

	const short *lut = e->frame().palette;

	const int rowlen = 256 - cropleft - cropright;

	src += cropleft;
	src += croptop * srcpitch;

	for (; src < srcend; src += srcpitch)
	{
		for (int i = 0; i < rowlen; i++)
		{
			*dest++ = colors[lut[src[i]]];
		}
	}
}

EXPORT const quickerNES::Emu::rgb_t *qn_get_default_colors()
{
	return quickerNES::Emu::nes_colors;
}

EXPORT int qn_get_joypad_read_count(quickerNES::Emu *e)
{
	return e->frame().joypad_read_count;
}

EXPORT void qn_get_audio_info(quickerNES::Emu *e, int *sample_count, int *chan_count)
{
	if (sample_count)
		*sample_count = e->frame().sample_count;
	if (chan_count)
		*chan_count = e->frame().chan_count;
}

EXPORT int qn_read_audio(quickerNES::Emu *e, short *dest, int max_samples)
{
	return e->read_samples(dest, max_samples);
}

EXPORT void qn_reset(quickerNES::Emu *e, int hard)
{
	e->reset(hard);
}

EXPORT const char *qn_state_size(quickerNES::Emu *e, int *size)
{
	jaffarCommon::serializer::Contiguous s;
	e->serializeState(s);
	*size = s.getOutputSize();
	return 0;
}

EXPORT const char *qn_state_save(quickerNES::Emu *e, void *dest, int size)
{
	jaffarCommon::serializer::Contiguous s(dest, size);
	e->serializeState(s);
	return 0;
}

EXPORT const char *qn_state_load(quickerNES::Emu *e, const void *src, int size)
{
	jaffarCommon::deserializer::Contiguous d(src, size);
	e->deserializeState(d);
	return 0;
}

EXPORT int qn_has_battery_ram(quickerNES::Emu *e)
{
	return e->has_battery_ram();
}

EXPORT const char *qn_battery_ram_size(quickerNES::Emu *e, int *size)
{
	*size = e->get_high_mem_size();
	return 0;
}

EXPORT const char *qn_battery_ram_save(quickerNES::Emu *e, void *dest, int size)
{
	memcpy(dest, e->high_mem(), size);
	return 0;
}

EXPORT const char *qn_battery_ram_load(quickerNES::Emu *e, const void *src, int size)
{
	memcpy(e->high_mem(), src, size);
	return 0;
}

EXPORT const char *qn_battery_ram_clear(quickerNES::Emu *e)
{
	int size = 0;
	qn_battery_ram_size(e, &size);
	std::memset(e->high_mem(), 0xff, size);
	return 0;
}

EXPORT void qn_set_sprite_limit(quickerNES::Emu *e, int n)
{
	e->set_sprite_mode((quickerNES::Emu::sprite_mode_t)n);
}

EXPORT int qn_get_memory_area(quickerNES::Emu *e, int which, const void **data, int *size, int *writable, const char **name)
{
	if (!data || !size || !writable || !name)
		return 0;
	switch (which)
	{
	default:
		return 0;
	case 0:
		*data = e->get_low_mem();
		*size = e->low_mem_size;
		*writable = 1;
		*name = "RAM";
		return 1;
	case 1:
		*data = e->high_mem();
		*size = e->high_mem_size;
		*writable = 1;
		*name = "SRAM";
		return 1;
	case 2:
		*data = e->chr_mem();
		*size = e->chr_size();
		*writable = 1;
		*name = "CHR";
		return 1;
	case 3:
		*data = e->nametable_mem();
		*size = e->nametable_size();
		*writable = 1;
		*name = "CIRAM (nametables)";
		return 1;
	case 4:
		*data = e->cart()->prg();
		*size = e->cart()->prg_size();
		*writable = 1;
		*name = "PRG ROM";
		return 1;
	case 5:
		*data = e->cart()->chr();
		*size = e->cart()->chr_size();
		*writable = 1;
		*name = "CHR VROM";
		return 1;
	case 6:
		*data = e->pal_mem();
		*size = e->pal_mem_size();
		*writable = 1;
		*name = "Palette RAM";
		return 1;
	case 7:
	    *data = e->spr_mem();
		*size = e->spr_mem_size();
		*writable = 1;
		*name = "Sprite RAM";
		return 1;
	}
}

EXPORT unsigned char qn_peek_prgbus(quickerNES::Emu *e, int addr)
{
	return e->peek_prg(addr & 0xffff);
}

EXPORT void qn_poke_prgbus(quickerNES::Emu *e, int addr, unsigned char val)
{
	e->poke_prg(addr & 0xffff, val);
}

EXPORT void qn_get_cpuregs(quickerNES::Emu *e, unsigned int *dest)
{
	e->get_regs(dest);
}

EXPORT const char *qn_get_mapper(quickerNES::Emu *e, int *number)
{
	int m = e->cart()->mapper_code();
	if (number)
		*number = m;
	switch (m)
	{
	default: return "unknown";
	case   0: return "nrom";
	case   1: return "mmc1";
	case   2: return "unrom";
	case   3: return "cnrom";
	case   4: return "mmc3";
	case   5: return "mmc5";
	case   7: return "aorom";
	case   9: return "mmc2";
	case  10: return "mmc4";
	case  11: return "color_dreams";
	case  15: return "k1029/30P";
	case  19: return "namco106";
	case  21: return "vrc2,vrc4(21)";
	case  22: return "vrc2,vrc4(22)";
	case  23: return "vrc2,vrc4(23)";
	case  24: return "vrc6a";
	case  25: return "vrc2,vrc4(25)";
	case  26: return "vrc6b";
	case  30: return "Unrom512";
	case  32: return "Irem_G101";
	case  33: return "TaitoTC0190";
	case  34: return "nina1";
	case  60: return "NROM-128";
	case  66: return "gnrom";
	case  69: return "fme7";
	case  70: return "74x161x162x32(70)";
	case  71: return "camerica";
	case  73: return "vrc3";
	case  75: return "vrc1";
	case  78: return "mapper_78";
	case  79: return "nina03,nina06(79)";
	case  85: return "vrc7";
	case  86: return "mapper_86";
	case  87: return "mapper_87";
	case  88: return "namco34(88)";
	case  89: return "sunsoft2b";
	case  93: return "sunsoft2a";
	case  94: return "Un1rom";
    case  97: return "irem_tam_s1";
	case 113: return "nina03,nina06(113)";
	case 140: return "jaleco_jf11";
	case 152: return "74x161x162x32(152)";
	case 154: return "namco34(154)";
	case 156: return "dis23c01_daou";
	case 180: return "uxrom(inverted)";
	case 184: return "sunsoft1";
	case 190: return "magickidgoogoo";
	case 193: return "tc112";
	case 206: return "namco34(206)";
	case 207: return "taitox1005";
	case 232: return "quattro";
	case 240: return "mapper_240";
	case 241: return "mapper_241";
	case 246: return "mapper_246";
	}
}

EXPORT uint8_t qn_get_reg2000(quickerNES::Emu *e)
{
	return e->get_ppu2000();
}

EXPORT uint8_t *qn_get_palmem(quickerNES::Emu *e)
{
	return e->pal_mem();
}

EXPORT uint8_t *qn_get_oammem(quickerNES::Emu *e)
{
	return e->pal_mem();
}

EXPORT uint8_t qn_peek_ppu(quickerNES::Emu *e, int addr)
{
	return e->peek_ppu(addr);
}

EXPORT void qn_peek_ppubus(quickerNES::Emu *e, uint8_t *dest)
{
	for (int i = 0; i < 0x3000; i++)
		dest[i] = e->peek_ppu(i);
}

EXPORT void qn_set_tracecb(quickerNES::Emu *e, void (*cb)(unsigned int *dest))
{
	// In spirit of performance, this function is no longer supported
}
