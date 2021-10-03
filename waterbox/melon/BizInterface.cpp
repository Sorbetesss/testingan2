#include "melonds/src/NDS.h"
#include "melonds/src/GPU.h"
#include "melonds/src/Platform.h"
#include "melonds/src/Config.h"
#include "melonds/src/types.h"

#include "../emulibc/emulibc.h"
#include "../emulibc/waterboxcore.h"

#define EXPORT extern "C" ECL_EXPORT

static GPU::RenderSettings biz_render_settings { false, 1, false };

EXPORT bool Init(Platform::FileStruct **filesToLoad, bool dsi, bool direct, bool sd)
{
	NDS::SetConsoleType(dsi);
	for (int i = 0; i < Platform::NUM_FILES; i++)
	{
		if ((*filesToLoad)[i].FileData)
		{
			Platform::files[i].FileData = alloc_sealed((*filesToLoad)[i].FileLength);
			Platform::files[i].FileLength = (*filesToLoad)[i].FileLength;
			memcpy(Platform::files[i].FileData, (*filesToLoad)[i].FileData, Platform::files[i].FileLength);
		}
		else
		{
			Platform::files[i].FileData = NULL;
			Platform::files[i].FileLength = 0;
		}
	}
	NDS::Init();
	GPU::InitRenderer(false);
	GPU::SetRenderSettings(false, biz_render_settings);
	NDS::LoadROM(direct);
	NDS::LoadGBAROM();
	return true;
}

EXPORT void GetMemoryAreas(MemoryArea *m)
{
	/*m[0].Data = ;
	m[0].Name = "RAM";
	m[0].Size = ;
	m[0].Flags = MEMORYAREA_FLAGS_WORDSIZE4 | MEMORYAREA_FLAGS_WRITABLE | MEMORYAREA_FLAGS_PRIMARY;

	/*m[1].Data = ;
	m[1].Name = "SRAM";
	m[1].Size = ;
	m[1].Flags = MEMORYAREA_FLAGS_WORDSIZE4 | MEMORYAREA_FLAGS_WRITABLE | MEMORYAREA_FLAGS_SAVERAMMABLE;

	m[2].Data = ;
	m[2].Name = "ROM";
	m[2].Size = ;
	m[2].Flags = MEMORYAREA_FLAGS_WORDSIZE4;

	m[3].Data = ;
	m[3].Name = "System Bus";
	m[3].Size = ;
	m[3].Flags = MEMORYAREA_FLAGS_WORDSIZE4 | MEMORYAREA_FLAGS_WRITABLE;*/
}

struct MyFrameInfo : public FrameInfo
{
};

EXPORT void FrameAdvance(MyFrameInfo* f)
{
}

void (*InputCallback)();

EXPORT void SetInputCallback(void (*callback)())
{
	InputCallback = callback;
}