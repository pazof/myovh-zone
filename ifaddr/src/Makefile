DESTDIR=../build
BINDIR=bin
BINARY=$(DESTDIR)/$(BINDIR)/ifaddr

all: $(BINARY)
	
$(BINARY): main.c getifaddr.c
	mkdir -p $(DESTDIR)/$(BINDIR)
	gcc  $^ -o $@

dist-clean:
	rm -f $(BINARY)

