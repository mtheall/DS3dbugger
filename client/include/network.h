#ifndef NETWORK_H
#define NETWORK_H

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
      int magic;
    } syn;

    struct {
      int magic;
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

#endif /* NETWORK_H */

