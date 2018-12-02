#include <stdio.h>
#include<stdlib.h>

char * get_if_addr(char *ifname);

int main(int argc, char ** argv)
{

  if (argc<2) {
    fprintf(stderr, "usage: %s <ifname>\nOù `ifname` est le nom de l'interface, par exemple, `eth0`.\n", argv[0]);
    
    return 2;
  }

    char * addr = (char*) get_if_addr(argv[1]);
    if (addr==NULL) {
      printf("no addr found!\n");
      return 1;
    }
    printf(addr);

    free(addr);
    return 0;
}
