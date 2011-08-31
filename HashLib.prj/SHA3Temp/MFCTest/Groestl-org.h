
#pragma once

namespace GroestlOrg
{

    typedef unsigned char uint_8t;
    typedef unsigned int uint_32t;
    #define li_64(h) 0x##h##ull
    typedef unsigned long long uint_64t;
    #define u8 uint_8t
    #define u32 uint_32t
    #define u64 uint_64t

/* some sizes (number of bytes) */
#define ROWS 8
#define LENGTHFIELDLEN ROWS
#define COLS512 8
#define COLS1024 16
#define SIZE512 (ROWS*COLS512)
#define SIZE1024 (ROWS*COLS1024)

#define ROUNDS512 10
#define ROUNDS1024 14

#define ROTL64(a,n) ((((a)<<(n))|((a)>>(64-(n))))&0xffffffffffffffff)

#define EXT_BYTE(var,n) ((u8)((u64)(var) >> (8*n)))
#define U64BIG(a) \
  ((ROTL64(a, 8) & 0x000000FF000000FF) | \
   (ROTL64(a,24) & 0x0000FF000000FF00) | \
   (ROTL64(a,40) & 0x00FF000000FF0000) | \
   (ROTL64(a,56) & 0xFF000000FF000000))

typedef enum { LONG, SHORT } Var;


/* NIST API begin */
typedef unsigned char BitSequence;
typedef unsigned long long DataLength;
typedef enum { SUCCESS = 0, FAIL = 1, BAD_HASHLEN = 2 } HashReturn;
typedef struct H {
  u64 *chaining;            /* actual state */
  u64 block_counter;        /* message block counter */
  int hashbitlen;           /* output length in bits */
  BitSequence *buffer;      /* data buffer */
  int buf_ptr;              /* data buffer pointer */
  int bits_in_last_byte;    /* no. of message bits in last byte of
			       data buffer */
  int columns;              /* no. of columns in state */
  int statesize;            /* total no. of bytes in state */
  Var v;                    /* LONG or SHORT */

  H()
  {
      buffer = nullptr;
      chaining = nullptr;
  }
} hashState;

HashReturn Init(hashState*, int);
HashReturn Update(hashState*, const BitSequence*, DataLength);
HashReturn Final(hashState*, BitSequence*);
HashReturn Hash(int hashbitlen, const BitSequence*, DataLength, BitSequence*);
/* NIST API end   */

/* helper functions */
void PrintHash(const BitSequence*, int);

}