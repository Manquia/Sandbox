########################################################################
# Makefile for Linux

ifeq (sol,$(flag))
    SFLAG := -DSOLUTION
    ODIR := sobjs
else
    SFLAG := 
    ODIR := objs
endif

vpath %.o  $(ODIR)

ifneq (,$(wildcard libs))
    LIBDIR := libs
else
    ifneq (,$(wildcard ../libs))
        LIBDIR := ../libs
    else
        ifneq (,$(wildcard ../../libs))
            LIBDIR := ../../libs
        else
            LIBDIR := ../../../libs
        endif
    endif
endif

CXX = g++
CFLAGS = -g $(SFLAG) -I. -I$(LIBDIR)/freeglut -I$(LIBDIR)/glm -I$(LIBDIR) -I/usr/include  -I/usr/include/GL/ 
CXXFLAGS = -std=c++11 $(CFLAGS) -DVK_TAB=9

LIBS =  -L/usr/lib/x86_64-linux-gnu -L../$(LIBDIR) -L/usr/lib -L/usr/local/lib -lglbinding -lglut -lX11 -lGLU -lGL

CPPsrc = framework.cpp interact.cpp transform.cpp scene.cpp texture.cpp shapes.cpp object.cpp shader.cpp simplexnoise.cpp
Csrc = rply.c

headers = framework.h interact.h texture.h shapes.h object.h rply.h scene.h shader.h transform.h simplexnoise.h
shaders = lighting.vert lighting.frag 
shadersSol = lightingSol.vert lightingSol.frag 
srcFiles = $(CPPsrc) $(Csrc) $(shaders) $(headers)
solFiles = $(CPPsrc) $(Csrc) $(shadersSol) $(headers)
extraFiles = framework.vcxproj Makefile box.ply room.ply textures

pkgDir = /home/gherron/packages
pkgName = $(notdir CS251-framework)
solName = $(notdir CS251-solution)
objs = $(patsubst %.cpp,%.o,$(CPPsrc)) $(patsubst %.c,%.o,$(Csrc))
target = $(ODIR)/framework.exe

$(target): $(objs)
	@echo Link $(target)
	cd $(ODIR) && $(CXX) -g  -o ../$@  $(objs) $(LIBS)

run: $(target)
	LD_LIBRARY_PATH="$(LIBDIR);$(LD_LIBRARY_PATH)" ./$(target)

what:
	@echo VPATH = $(VPATH)
	@echo LIBS = $(LIBDIR)
	@echo SFLAG = $(SFLAG)
	@echo objs = $(objs)

clean:
	rm -rf objs sobjs dependencies

%.o: %.cpp
	@echo Compile $<  $(SFLAG)
	@mkdir -p $(ODIR)
	@$(CXX) -c $(CXXFLAGS) $< -o $(ODIR)/$@

%.o: %.c
	@echo Compile $< $(SFLAG)
	@mkdir -p $(ODIR)
	@$(CC) -c $(CFLAGS) $< -o $(ODIR)/$@

zip: $(srcFiles) $(extraFiles)
	rm -rf /$(pkgDir)/$(pkgName) /$(pkgDir)/$(pkgName).zip
	mkdir /$(pkgDir)/$(pkgName)
	../SolutionFilter.py $(srcFiles) /$(pkgDir)/$(pkgName)
	cp -r $(extraFiles) /$(pkgDir)/$(pkgName)
	cp -r ../libs /$(pkgDir)/$(pkgName)
	rm -rf /$(pkgDir)/$(pkgName)/libs/.hg /$(pkgDir)/$(pkgName)/libs/Eigen* /$(pkgDir)/$(pkgName)/libs/assimp
	cd $(pkgDir);  zip -r $(pkgName).zip $(pkgName); rm -rf $(pkgName)

solzip: $(solFiles) $(extraFiles)
	rm -rf /$(pkgDir)/$(solName) /$(pkgDir)/$(solName).zip
	mkdir /$(pkgDir)/$(solName)
	cp $(solFiles) /$(pkgDir)/$(solName)
	cp -r $(extraFiles) /$(pkgDir)/$(solName)
	cp -r ../libs /$(pkgDir)/$(solName)
	rm -rf /$(pkgDir)/$(solName)/libs/.hg /$(pkgDir)/$(solName)/libs/Eigen* /$(pkgDir)/$(solName)/libs/assimp
	cd $(pkgDir);  zip -r $(solName).zip $(solName); rm -rf $(solName)

ws:
	unix2dos $(srcFiles)
	@echo
	@echo ========= TABS:
	@grep -P '\t' $(srcFiles)

dependencies: 
	g++ -MM $(CXXFLAGS) $(CPPsrc) > dependencies

include dependencies
