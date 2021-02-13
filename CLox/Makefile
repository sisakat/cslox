PROGRAM = clox
ARGUMENTS =

CC = gcc # use C++ compiler
CPPFLAGS = -pedantic -Wall -Wextra -std=c11 -ggdb # use C++ standard

LD = gcc # use C++ linker
LDFLAGS = -lm

CFILES = $(wildcard *.c) # collect c files
OFILES = $(CFILES:.c=.o)

all: clean depend $(PROGRAM)

$(PROGRAM): $(OFILES)
	$(LD) -o $@ $(OFILES) $(LDFLAGS)

.PHONY: all clean

clean:
	rm -f $(OFILES) $(PROGRAM) .depend

run: $(PROGRAM)
	./$(PROGRAM) $(ARGUMENTS)

debug: $(PROGRAM)
	gdb $(PROGRAM) $(ARGUMENTS)

DEPENDFILE = .depend

depend:
	$(CC) $(CCFLAGS) -MM $(CFILES) > $(DEPENDFILE)

-include $(DEPENDFILE)