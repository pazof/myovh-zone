#define _GNU_SOURCE     /* To get defns of NI_MAXSERV and NI_MAXHOST */
#include <ifaddrs.h>
#include <arpa/inet.h>
#include <sys/socket.h>
#include <netdb.h>
#include <ifaddrs.h>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <linux/if_link.h>
#include <string.h>

char * get_if_addr(char *ifname)
{

  struct ifaddrs * list, * plist;
  int family, s;

	char host[NI_MAXHOST];
  char *result;
  if (getifaddrs (&list)==-1)
  {
    fprintf(stderr, "no info");
    return NULL;
  }
  plist = list;
  while (plist!=NULL)
  {
    if (strcmp(plist->ifa_name,ifname)==0) 
		{
      family = plist->ifa_addr->sa_family;
      if (family == AF_INET || family == AF_INET6) {
                   s = getnameinfo(plist->ifa_addr,
                           (family == AF_INET) ? sizeof(struct sockaddr_in) :
                                                 sizeof(struct sockaddr_in6),
                           host, NI_MAXHOST,
                           NULL, 0, NI_NUMERICHOST);
                   if (s != 0) {
                       fprintf(stderr, "getnameinfo() failed: %s\n", gai_strerror(s));
                       exit(EXIT_FAILURE);
                   }
                   result = malloc(strlen(host)+1);
                   strcpy(result,host);
                   freeifaddrs (list);
                   return result;
               }
    }
    plist = plist->ifa_next;
  }
  freeifaddrs (list);
  return NULL;

}
