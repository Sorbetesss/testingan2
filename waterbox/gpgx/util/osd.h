#pragma once

#ifdef _MSC_VER
#include <stdio.h>
typedef unsigned char bool;
#define strncasecmp _strnicmp
#endif

#include <stdint.h>
#include <stdlib.h>
#include <string.h>

#define MAX_INPUTS 8
#define MAX_KEYS 8
#define MAXPATHLEN 1024

#ifndef TRUE
#define TRUE 1
#endif

#ifndef FALSE
#define FALSE 0
#endif

#ifndef M_PI
#define M_PI 3.1415926535897932385
#endif

typedef struct
{
  int8_t device;
  uint8_t port;
  uint8_t padtype;
} t_input_config;

struct config_t
{
  uint8_t hq_fm;
  uint8_t filter;
  uint8_t hq_psg;
  uint8_t ym2612;
  uint8_t ym2413;
  uint8_t ym3438;
  uint8_t opll;
  uint8_t cd_latency;
  int16_t psg_preamp;
  int16_t fm_preamp;
  int16_t cdda_volume;
  int16_t pcm_volume;
  uint32_t lp_range;
  int16_t low_freq;
  int16_t high_freq;
  int16_t lg;
  int16_t mg;
  int16_t hg;
  uint8_t mono;
  uint8_t system;
  uint8_t region_detect;
  uint8_t vdp_mode;
  uint8_t master_clock;
  uint8_t force_dtack;
  uint8_t addr_error;
  uint8_t bios;
  uint8_t lock_on;
  uint8_t add_on;
  uint8_t hot_swap;
  uint8_t invert_mouse;
  uint8_t gun_cursor[2];
  uint8_t overscan;
  uint8_t gg_extra;
  uint8_t ntsc;
  uint8_t lcd;
  uint8_t render;
  uint8_t enhanced_vscroll;
  uint8_t enhanced_vscroll_limit;
  t_input_config input[MAX_INPUTS];
  uint8_t no_sprite_limit;
  uint8_t sprites_always_on_top;
};

extern struct config_t config;

extern char GG_ROM[256];
extern char AR_ROM[256];
extern char SK_ROM[256];
extern char SK_UPMEM[256];
extern char GG_BIOS[256];
extern char CD_BIOS_EU[256];
extern char CD_BIOS_US[256];
extern char CD_BIOS_JP[256];
extern char MD_BIOS[256];
extern char MS_BIOS_US[256];
extern char MS_BIOS_EU[256];
extern char MS_BIOS_JP[256];

void osd_input_update(void);
int load_archive(const char *filename, unsigned char *buffer, int maxsize, char *extension);
void real_input_callback(void);

