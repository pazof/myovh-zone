
IFADDRBIN=ifaddr/build/bin/ifaddr

all: myovh-zone

myovh-zone: myovh-zone/ifaddr
	make -C myovh-zone

myovh-zone/ifaddr: $(IFADDRBIN)
	cp -a $^ $@

$(IFADDRBIN):
	make -C ifaddr/src

dist-clean:
	make -C ifaddr/src dist-clean
	make -C myovh-zone dist-clean

