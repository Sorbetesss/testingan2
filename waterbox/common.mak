# common parts of all waterbox cores

WATERBOX_DIR := $(shell dirname $(realpath $(lastword $(MAKEFILE_LIST))))
ROOT_DIR := $(shell dirname $(realpath $(lastword $(filter-out $(lastword $(MAKEFILE_LIST)), $(MAKEFILE_LIST)))))
OUTPUTDLL_DIR := $(realpath $(WATERBOX_DIR)/../output/dll)
OBJ_DIR := $(ROOT_DIR)/obj/release
DOBJ_DIR := $(ROOT_DIR)/obj/debug
EMULIBC_OBJS := $(WATERBOX_DIR)/emulibc/obj/release/emulibc.c.o
EMULIBC_DOBJS := $(WATERBOX_DIR)/emulibc/obj/debug/emulibc.c.o
SYSROOT := $(WATERBOX_DIR)/sysroot
ifdef NEED_LIBCO
EMULIBC_OBJS := $(EMULIBC_OBJS) $(shell find $(WATERBOX_DIR)/libco/obj/release -type f -name '*.o')
EMULIBC_DOBJS := $(EMULIBC_DOBJS) $(shell find $(WATERBOX_DIR)/libco/obj/debug -type f -name '*.o')
endif

print-%: ;
	@echo $* = $($*)

#LD_PLUGIN := $(shell gcc --print-file-name=liblto_plugin.so)

CC := $(SYSROOT)/bin/musl-gcc
CCFLAGS := $(CCFLAGS) -mabi=ms -fvisibility=hidden -I$(WATERBOX_DIR)/emulibc -Wall -mcmodel=large \
	-mstack-protector-guard=global
LDFLAGS := $(LDFLAGS) -fuse-ld=gold -static -Wl,-Ttext,0x0000036f00000000 #-Wl,--plugin,$(LD_PLUGIN)
CCFLAGS_DEBUG := -O0 -g
CCFLAGS_RELEASE := -O3 -flto
LDFLAGS_DEBUG :=
LDFLAGS_RELEASE :=
CXXFLAGS := $(CXXFLAGS) -I$(SYSROOT)/include/c++/v1 -fno-use-cxa-atexit
CXXFLAGS_DEBUG :=
CXXFLAGS_RELEASE :=

EXTRA_LIBS := -L $(SYSROOT)/lib/linux -lclang_rt.builtins-x86_64
ifneq ($(filter %.cpp, $(SRCS)), )
EXTRA_LIBS := -lc++ -lc++abi -lunwind $(EXTRA_LIBS)
endif

_OBJS := $(addsuffix .o, $(realpath $(SRCS)))
OBJS := $(patsubst $(ROOT_DIR)%, $(OBJ_DIR)%, $(_OBJS))
DOBJS := $(patsubst $(ROOT_DIR)%, $(DOBJ_DIR)%, $(_OBJS))

$(OBJ_DIR)/%.c.o: %.c
	@echo cc $<
	@mkdir -p $(@D)
	@$(CC) -c -o $@ $< $(CCFLAGS) $(CCFLAGS_RELEASE)
$(OBJ_DIR)/%.cpp.o: %.cpp
	@echo cxx $<
	@mkdir -p $(@D)
	@$(CC) -c -o $@ $< $(CCFLAGS) $(CXXFLAGS) $(CCFLAGS_RELEASE) 
$(DOBJ_DIR)/%.c.o: %.c
	@echo cc $<
	@mkdir -p $(@D)
	@$(CC) -c -o $@ $< $(CCFLAGS) $(CCFLAGS_DEBUG)
$(DOBJ_DIR)/%.cpp.o: %.cpp
	@echo cxx $<
	@mkdir -p $(@D)
	@$(CC) -c -o $@ $< $(CCFLAGS) $(CXXFLAGS) $(CCFLAGS_DEBUG) $(CXXFLAGS_DEBUG)

ifndef NO_WBX_TARGETS

.DEFAULT_GOAL := release

TARGET_RELEASE := $(OBJ_DIR)/$(TARGET)
TARGET_DEBUG := $(DOBJ_DIR)/$(TARGET)

.PHONY: release debug install install-debug

release: $(TARGET_RELEASE)
debug: $(TARGET_DEBUG)

$(TARGET_RELEASE): $(OBJS) $(EMULIBC_OBJS)
	@echo ld $@
	@$(CC) -o $@ $(LDFLAGS) $(LDFLAGS_RELEASE) $(CCFLAGS) $(CCFLAGS_RELEASE) $(OBJS) $(EMULIBC_OBJS) $(EXTRA_LIBS)
$(TARGET_DEBUG): $(DOBJS) $(EMULIBC_DOBJS)
	@echo ld $@
	@$(CC) -o $@ $(LDFLAGS) $(LDFLAGS_DEBUG) $(CCFLAGS) $(CCFLAGS_DEBUG) $(DOBJS) $(EMULIBC_DOBJS) $(EXTRA_LIBS)

install: $(TARGET_RELEASE)
	@cp -f $< $(OUTPUTDLL_DIR)
	@gzip --stdout --best $< > $(OUTPUTDLL_DIR)/$(TARGET).gz
	@echo Release build of $(TARGET) installed.

install-debug: $(TARGET_DEBUG)
	@cp -f $< $(OUTPUTDLL_DIR)
	@gzip --stdout --best $< > $(OUTPUTDLL_DIR)/$(TARGET).gz
	@echo Debug build of $(TARGET) installed.

else

.DEFAULT_GOAL = all

.PHONY: all

all: $(OBJS) $(DOBJS)

endif

.PHONY: clean clean-release clean-debug
clean:
	rm -rf $(ROOT_DIR)/obj
clean-release:
	rm -rf $(ROOT_DIR)/obj/release
clean-debug:
	rm -rf $(ROOT_DIR)/obj/debug
