#ifndef NETWORK_H
#define NETWORK_H

#include <nds.h>
#include <dswifi9.h>
#include <netinet/in.h>

#define PORTNUM 9393

typedef int SOCKET;

typedef enum
{
  Message_Syn,            //ds <-> pc
  Message_Ack,            //ds <-> pc
  Message_Texture,        //pc --> ds
  Message_Register16,     //pc --> ds
  Message_Register32,     //pc --> ds
  Message_DisplayList,    //pc --> ds
  Message_DisplayCapture, //ds --> pc
  Message_Payload,        //ds <-> pc
} MessageType;

typedef struct
{
  MessageType type;
  union
  {
    struct {
      u32 magic;
    } syn;

    struct {
      u32 magic;
    } ack;

    struct {
      void *address;
      int   size;
    } tex;

    struct {
      void     *address;
      uint16_t  value;
    } register16;

    struct {
      void     *address;
      uint32_t  value;
    } register32;

    struct {
      int size;
    } displist;

    struct {
    } dispcap;

    char payload[0];
  };
} Message;

class NetManager {
private:
  SOCKET             listener;
  SOCKET             connection;
  struct in_addr     ip;
  struct sockaddr_in addr;
  int                addr_len;
  Message            msg;

  bool connectWifi();
  bool initSockets();
  bool handshake();
  void shutdown();

public:
  NetManager();
  ~NetManager();

  bool connect();
  void printIP();
};

#endif /* NETWORK_H */

