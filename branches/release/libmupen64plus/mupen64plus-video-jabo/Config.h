#ifndef CONFIG_H
#define CONFIG_H

#include "m64p.h"

BOOL Config_Open();
int Config_ReadInt(const char *itemname, const char *desc, int def_value, BOOL create=TRUE, BOOL isBoolean=TRUE);


#endif /* CONFIG_H */