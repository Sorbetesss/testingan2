#ifndef BSNESCORE_HPP
#define BSNESCORE_HPP

#include <stdint.h>
#include <stdlib.h>

#define EXPORT ECL_EXPORT

#define SAMPLE_RATE 32040

enum SNES_MEMORY {
    CARTRIDGE_RAM,
    BSX_RAM,
    BSX_PRAM,
    SUFAMI_TURBO_A_RAM,
    SUFAMI_TURBO_B_RAM,

    WRAM,
    APURAM,
    VRAM,
    CGRAM,

    CARTRIDGE_ROM
};

typedef void (*snes_trace_t)(uint32_t which, const char *msg);

// todo: put to platform->allocSharedMemory or smth
// void* snes_allocSharedMemory(const char* memtype, size_t amt);
// void snes_freeSharedMemory(void* ptr);


bool snes_load_cartridge_bsx_slotted(
    const char *rom_xml, const uint8_t *rom_data, unsigned rom_size,
    const char *bsx_xml, const uint8_t *bsx_data, unsigned bsx_size
);

bool snes_load_cartridge_bsx(
    const char *rom_xml, const uint8_t *rom_data, unsigned rom_size,
    const char *bsx_xml, const uint8_t *bsx_data, unsigned bsx_size
);

bool snes_load_cartridge_sufami_turbo(
    const char *rom_xml, const uint8_t *rom_data, unsigned rom_size,
    const char *sta_xml, const uint8_t *sta_data, unsigned sta_size,
    const char *stb_xml, const uint8_t *stb_data, unsigned stb_size
);


void snes_unload_cartridge(void);

struct LayerEnablesComm
{
    bool BG1_Prio0, BG1_Prio1;
    bool BG2_Prio0, BG2_Prio1;
    bool BG3_Prio0, BG3_Prio1;
    bool BG4_Prio0, BG4_Prio1;
    bool Obj_Prio0, Obj_Prio1, Obj_Prio2, Obj_Prio3;
};


//zeromus additions
// typedef void (*snes_scanlineStart_t)(int);
// void snes_set_scanlineStart(snes_scanlineStart_t);
extern uint16_t backdropColor;

// void snes_set_trace_callback(uint32_t mask, void (*callback)(uint32_t mask, const char *));

// system bus implementation
uint8_t bus_read(unsigned addr);
void bus_write(unsigned addr, uint8_t val);


//$2105
#define SNES_REG_BG_MODE 0
#define SNES_REG_BG3_PRIORITY 1
#define SNES_REG_BG1_TILESIZE 2
#define SNES_REG_BG2_TILESIZE 3
#define SNES_REG_BG3_TILESIZE 4
#define SNES_REG_BG4_TILESIZE 5
//$2107
#define SNES_REG_BG1_SCADDR 10
#define SNES_REG_BG1_SCSIZE 11
//$2108
#define SNES_REG_BG2_SCADDR 12
#define SNES_REG_BG2_SCSIZE 13
//$2109
#define SNES_REG_BG3_SCADDR 14
#define SNES_REG_BG3_SCSIZE 15
//$210A
#define SNES_REG_BG4_SCADDR 16
#define SNES_REG_BG4_SCSIZE 17
//$210B
#define SNES_REG_BG1_TDADDR 20
#define SNES_REG_BG2_TDADDR 21
//$210C
#define SNES_REG_BG3_TDADDR 22
#define SNES_REG_BG4_TDADDR 23
//$2133 SETINI
#define SNES_REG_SETINI_MODE7_EXTBG 30
#define SNES_REG_SETINI_HIRES 31
#define SNES_REG_SETINI_OVERSCAN 32
#define SNES_REG_SETINI_OBJ_INTERLACE 33
#define SNES_REG_SETINI_SCREEN_INTERLACE 34
//$2130 CGWSEL
#define SNES_REG_CGWSEL_COLORMASK 40
#define SNES_REG_CGWSEL_COLORSUBMASK 41
#define SNES_REG_CGWSEL_ADDSUBMODE 42
#define SNES_REG_CGWSEL_DIRECTCOLOR 43
//$2101 OBSEL
#define SNES_REG_OBSEL_NAMEBASE 50
#define SNES_REG_OBSEL_NAMESEL 51
#define SNES_REG_OBSEL_SIZE 52
//$2131 CGADSUB
#define SNES_REG_CGADDSUB_MODE 60
#define SNES_REG_CGADDSUB_HALF 61
#define SNES_REG_CGADDSUB_BG4 62
#define SNES_REG_CGADDSUB_BG3 63
#define SNES_REG_CGADDSUB_BG2 64
#define SNES_REG_CGADDSUB_BG1 65
#define SNES_REG_CGADDSUB_OBJ 66
#define SNES_REG_CGADDSUB_BACKDROP 67
//$212C TM
#define SNES_REG_TM_BG1 70
#define SNES_REG_TM_BG2 71
#define SNES_REG_TM_BG3 72
#define SNES_REG_TM_BG4 73
#define SNES_REG_TM_OBJ 74
//$212D TM
#define SNES_REG_TS_BG1 80
#define SNES_REG_TS_BG2 81
#define SNES_REG_TS_BG3 82
#define SNES_REG_TS_BG4 83
#define SNES_REG_TS_OBJ 84
//Mode7 regs
#define SNES_REG_M7SEL_REPEAT 90
#define SNES_REG_M7SEL_HFLIP 91
#define SNES_REG_M7SEL_VFLIP 92
#define SNES_REG_M7A 93
#define SNES_REG_M7B 94
#define SNES_REG_M7C 95
#define SNES_REG_M7D 96
#define SNES_REG_M7X 97
#define SNES_REG_M7Y 98
//BG scroll regs
#define SNES_REG_BG1HOFS 100
#define SNES_REG_BG1VOFS 101
#define SNES_REG_BG2HOFS 102
#define SNES_REG_BG2VOFS 103
#define SNES_REG_BG3HOFS 104
#define SNES_REG_BG3VOFS 105
#define SNES_REG_BG4HOFS 106
#define SNES_REG_BG4VOFS 107
#define SNES_REG_M7HOFS 108
#define SNES_REG_M7VOFS 109


int snes_peek_logical_register(int reg);


#endif
