
#include "stdafx.h"
#include "Blake.h"

namespace Blake
{
    uint Blake::U8TO32_BE(byte* p)
    {
        return (p[0] << 24) | (p[1] << 16) | (p[2] << 8) | p[3];
    }

    void Blake::U32TO8_BE(byte* p, uint v)
    {
        p[0] = (byte)(v >> 24);  
        p[1] = (byte)(v >> 16); 
        p[2] = (byte)(v >>  8); 
        p[3] = (byte)v; 
    }

    ulong Blake::U8TO64_BE(byte* p) 
    {
        return ((ulong)U8TO32_BE(p) << 32) | (ulong)U8TO32_BE(p + 4);
    }

    void Blake::U64TO8_BE(byte* p, ulong v)
    {
        U32TO8_BE(p, (uint)(v >> 32));
        U32TO8_BE(p + 4, (uint)v);	
    }

    static unsigned char sigma[][16] = 
    {
        {  0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15 } ,
        { 14, 10,  4,  8,  9, 15, 13,  6,  1, 12,  0,  2, 11,  7,  5,  3 } ,
        { 11,  8, 12,  0,  5,  2, 15, 13, 10, 14,  3,  6,  7,  1,  9,  4 } ,
        {  7,  9,  3,  1, 13, 12, 11, 14,  2,  6,  5, 10,  4,  0, 15,  8 } ,
        {  9,  0,  5,  7,  2,  4, 10, 15, 14,  1, 11, 12,  6,  8,  3, 13 } ,
        {  2, 12,  6, 10,  0, 11,  8,  3,  4, 13,  7,  5, 15, 14,  1,  9 } ,
        { 12,  5,  1, 15, 14, 13,  4, 10,  0,  7,  6,  3,  9,  2,  8, 11 } ,
        { 13, 11,  7, 14, 12,  1,  3,  9,  5,  0, 15,  4,  8,  6,  2, 10 } ,
        {  6, 15, 14,  9, 11,  3,  0,  8, 12,  2, 13,  7,  1,  4, 10,  5 } ,
        { 10,  2,  8,  4,  7,  6,  1,  5, 15, 11,  9, 14,  3, 12, 13 , 0 }, 
        {  0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15 } ,
        { 14, 10,  4,  8,  9, 15, 13,  6,  1, 12,  0,  2, 11,  7,  5,  3 } ,
        { 11,  8, 12,  0,  5,  2, 15, 13, 10, 14,  3,  6,  7,  1,  9,  4 } ,
        {  7,  9,  3,  1, 13, 12, 11, 14,  2,  6,  5, 10,  4,  0, 15,  8 } ,
        {  9,  0,  5,  7,  2,  4, 10, 15, 14,  1, 11, 12,  6,  8,  3, 13 } ,
        {  2, 12,  6, 10,  0, 11,  8,  3,  4, 13,  7,  5, 15, 14,  1,  9 } ,
        { 12,  5,  1, 15, 14, 13,  4, 10,  0,  7,  6,  3,  9,  2,  8, 11 } ,
        { 13, 11,  7, 14, 12,  1,  3,  9,  5,  0, 15,  4,  8,  6,  2, 10 } ,
        {  6, 15, 14,  9, 11,  3,  0,  8, 12,  2, 13,  7,  1,  4, 10,  5 } ,
        { 10,  2,  8,  4,  7,  6,  1,  5, 15, 11,  9, 14,  3, 12, 13 , 0 }  
    };

    static uint c32[16] = 
    {
        0x243F6A88, 0x85A308D3, 0x13198A2E, 0x03707344, 0xA4093822, 0x299F31D0, 0x082EFA98, 0xEC4E6C89, 
        0x452821E6, 0x38D01377, 0xBE5466CF, 0x34E90C6C, 0xC0AC29B7, 0xC97C50DD, 0x3F84D5B5, 0xB5470917 
    };

    static ulong c64[16] = 
    {
        0x243F6A8885A308D3,0x13198A2E03707344, 0xA4093822299F31D0,0x082EFA98EC4E6C89,
        0x452821E638D01377,0xBE5466CF34E90C6C, 0xC0AC29B7C97C50DD,0x3F84D5B5B5470917,
        0x9216D5D98979FB1B,0xD1310BA698DFB5AC, 0x2FFD72DBD01ADFB7,0xB8E1AFED6A267E96,
        0xBA7C9045F12C7F99,0x24A19947B3916CF7, 0x0801F2E2858EFC16,0x636920D871574E69
    };

    static byte padding[129] =
    {
        0x80, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };

    static uint m_initial_state_256[8]=
    {
        0x6A09E667, 0xBB67AE85, 0x3C6EF372, 0xA54FF53A, 0x510E527F, 0x9B05688C, 0x1F83D9AB, 0x5BE0CD19
    };

    static uint m_initial_state_224[8]=
    {
        0xC1059ED8, 0x367CD507, 0x3070DD17, 0xF70E5939, 0xFFC00B31, 0x68581511, 0x64F98FA7, 0xBEFA4FA4
    };

    static ulong m_initial_state_384[8]=
    {
        0xCBBB9D5DC1059ED8, 0x629A292A367CD507, 0x9159015A3070DD17, 0x152FECD8F70E5939,
        0x67332667FFC00B31, 0x8EB44A8768581511, 0xDB0C2E0D64F98FA7, 0x47B5481DBEFA4FA4
    };

    static ulong m_initial_state_512[8]=
    {
        0x6A09E667F3BCC908, 0xBB67AE8584CAA73B, 0x3C6EF372FE94F82B, 0xA54FF53A5F1D36F1,
        0x510E527FADE682D1, 0x9B05688C2B3E6C1F, 0x1F83D9ABFB41BD6B, 0x5BE0CD19137E2179
    };

    void Blake32::TransformBlock(byte* a_data, int a_index)
    {
        a_data = a_data + a_index;

        uint v[16];
        uint m[16];

        m[ 0] = U8TO32_BE(a_data + 0);
        m[ 1] = U8TO32_BE(a_data + 4);
        m[ 2] = U8TO32_BE(a_data + 8);
        m[ 3] = U8TO32_BE(a_data +12);
        m[ 4] = U8TO32_BE(a_data +16);
        m[ 5] = U8TO32_BE(a_data +20);
        m[ 6] = U8TO32_BE(a_data +24);
        m[ 7] = U8TO32_BE(a_data +28);
        m[ 8] = U8TO32_BE(a_data +32);
        m[ 9] = U8TO32_BE(a_data +36);
        m[10] = U8TO32_BE(a_data +40);
        m[11] = U8TO32_BE(a_data +44);
        m[12] = U8TO32_BE(a_data +48);
        m[13] = U8TO32_BE(a_data +52);
        m[14] = U8TO32_BE(a_data +56);
        m[15] = U8TO32_BE(a_data +60);

        v[ 0] = m_state[0];
        v[ 1] = m_state[1];
        v[ 2] = m_state[2];
        v[ 3] = m_state[3];
        v[ 4] = m_state[4];
        v[ 5] = m_state[5];
        v[ 6] = m_state[6];
        v[ 7] = m_state[7];
        v[ 8] = 0;
        v[ 8] ^= 0x243F6A88;
        v[ 9] = 0;
        v[ 9] ^= 0x85A308D3;
        v[10] = 0;
        v[10] ^= 0x13198A2E;
        v[11] = 0;
        v[11] ^= 0x03707344;
        v[12] =  0xA4093822;
        v[13] =  0x299F31D0;
        v[14] =  0x082EFA98;
        v[15] =  0xEC4E6C89;

        if (!m_nullt) 
        {
            v[12] ^= (uint)(m_processedBytes * 8);
            v[13] ^= (uint)(m_processedBytes * 8);
            v[14] ^= (uint)((m_processedBytes * 8) >> 32);
            v[15] ^= (uint)((m_processedBytes * 8) >> 32);
        }

        for(int round=0; round<10; ++round) 
        {
            v[0] = (m[sigma[round][0]] ^ c32[sigma[round][0+1]]) + v[0] + v[4];
            v[12] = v[12] ^ v[0];
            v[12] = (v[12] << (32 - 16)) | (v[12] >> 16);
            v[8] = v[8] + v[12];
            v[4] = v[4] ^ v[8];
            v[4] = (v[4] << (32 - 12)) | (v[4] >> 12);
            v[0] = (m[sigma[round][0+1]] ^ c32[sigma[round][0]]) + v[0] + v[4];
            v[12] = v[12] ^ v[0];
            v[12] = (v[12] << (32 - 8)) | (v[12] >> 8);
            v[8] = v[8] + v[12];
            v[4] = v[4] ^ v[8];
            v[4] = (v[4] << (32 - 7)) | (v[4] >> 7);

            v[1] = (m[sigma[round][2]] ^ c32[sigma[round][2+1]]) + v[1] + v[5];
            v[13] = v[13] ^ v[1];
            v[13] = (v[13] << (32 - 16)) | (v[13] >> 16);
            v[9] = v[9] + v[13];
            v[5] = v[5] ^ v[9];
            v[5] = (v[5] << (32 - 12)) | (v[5] >> 12);
            v[1] = (m[sigma[round][2+1]] ^ c32[sigma[round][2]]) + v[1] + v[5];
            v[13] = v[13] ^ v[1];
            v[13] = (v[13] << (32 - 8)) | (v[13] >> 8);
            v[9] = v[9] + v[13];
            v[5] = v[5] ^ v[9];
            v[5] = (v[5] << (32 - 7)) | (v[5] >> 7);

            v[2] = (m[sigma[round][4]] ^ c32[sigma[round][4+1]]) + v[2] + v[6];
            v[14] = v[14] ^ v[2];
            v[14] = (v[14] << (32 - 16)) | (v[14] >> 16);
            v[10] = v[10] + v[14];
            v[6] = v[6] ^ v[10];
            v[6] = (v[6] << (32 - 12)) | (v[6] >> 12);
            v[2] = (m[sigma[round][4+1]] ^ c32[sigma[round][4]]) + v[2] + v[6];
            v[14] = v[14] ^ v[2];
            v[14] = (v[14] << (32 - 8)) | (v[14] >> 8);
            v[10] = v[10] + v[14];
            v[6] = v[6] ^ v[10];
            v[6] = (v[6] << (32 - 7)) | (v[6] >> 7);

            v[3] = (m[sigma[round][6]] ^ c32[sigma[round][6+1]]) + v[3] + v[7];
            v[15] = v[15] ^ v[3];
            v[15] = (v[15] << (32 - 16)) | (v[15] >> 16);
            v[11] = v[11] + v[15];
            v[7] = v[7] ^ v[11];
            v[7] = (v[7] << (32 - 12)) | (v[7] >> 12);
            v[3] = (m[sigma[round][6+1]] ^ c32[sigma[round][6]]) + v[3] + v[7];
            v[15] = v[15] ^ v[3];
            v[15] = (v[15] << (32 - 8)) | (v[15] >> 8);
            v[11] = v[11] + v[15];
            v[7] = v[7] ^ v[11];
            v[7] = (v[7] << (32 - 7)) | (v[7] >> 7);

            v[3] = (m[sigma[round][14]] ^ c32[sigma[round][14+1]]) + v[3] + v[4];
            v[14] = v[14] ^ v[3];
            v[14] = (v[14] << (32 - 16)) | (v[14] >> 16);
            v[9] = v[9] + v[14];
            v[4] = v[4] ^ v[9];
            v[4] = (v[4] << (32 - 12)) | (v[4] >> 12);
            v[3] = (m[sigma[round][14+1]] ^ c32[sigma[round][14]]) + v[3] + v[4];
            v[14] = v[14] ^ v[3];
            v[14] = (v[14] << (32 - 8)) | (v[14] >> 8);
            v[9] = v[9] + v[14];
            v[4] = v[4] ^ v[9];
            v[4] = (v[4] << (32 - 7)) | (v[4] >> 7);

            v[2] = (m[sigma[round][12]] ^ c32[sigma[round][12+1]]) + v[2] + v[7];
            v[13] = v[13] ^ v[2];
            v[13] = (v[13] << (32 - 16)) | (v[13] >> 16);
            v[8] = v[8] + v[13];
            v[7] = v[7] ^ v[8];
            v[7] = (v[7] << (32 - 12)) | (v[7] >> 12);
            v[2] = (m[sigma[round][12+1]] ^ c32[sigma[round][12]]) + v[2] + v[7];
            v[13] = v[13] ^ v[2];
            v[13] = (v[13] << (32 - 8)) | (v[13] >> 8);
            v[8] = v[8] + v[13];
            v[7] = v[7] ^ v[8];
            v[7] = (v[7] << (32 - 7)) | (v[7] >> 7);

            v[0] = (m[sigma[round][8]] ^ c32[sigma[round][8+1]]) + v[0] + v[5];
            v[15] = v[15] ^ v[0];
            v[15] = (v[15] << (32 - 16)) | (v[15] >> 16);
            v[10] = v[10] + v[15];
            v[5] = v[5] ^ v[10];
            v[5] = (v[5] << (32 - 12)) | (v[5] >> 12);
            v[0] = (m[sigma[round][8+1]] ^ c32[sigma[round][8]]) + v[0] + v[5];
            v[15] = v[15] ^ v[0];
            v[15] = (v[15] << (32 - 8)) | (v[15] >> 8);
            v[10] = v[10] + v[15];
            v[5] = v[5] ^ v[10];
            v[5] = (v[5] << (32 - 7)) | (v[5] >> 7);

            v[1] = (m[sigma[round][10]] ^ c32[sigma[round][10+1]]) + v[1] + v[6];
            v[12] = v[12] ^ v[1];
            v[12] = (v[12] << (32 - 16)) | (v[12] >> 16);
            v[11] = v[11] + v[12];
            v[6] = v[6] ^ v[11];
            v[6] = (v[6] << (32 - 12)) | (v[6] >> 12);
            v[1] = (m[sigma[round][10+1]] ^ c32[sigma[round][10]]) + v[1] + v[6];
            v[12] = v[12] ^ v[1];
            v[12] = (v[12] << (32 - 8)) | (v[12] >> 8);
            v[11] = v[11] + v[12];
            v[6] = v[6] ^ v[11];
            v[6] = (v[6] << (32 - 7)) | (v[6] >> 7);
        }

        m_state[0] ^= v[ 0]; 
        m_state[1] ^= v[ 1];    
        m_state[2] ^= v[ 2];    
        m_state[3] ^= v[ 3];    
        m_state[4] ^= v[ 4];    
        m_state[5] ^= v[ 5];    
        m_state[6] ^= v[ 6];    
        m_state[7] ^= v[ 7];
        m_state[0] ^= v[ 8]; 
        m_state[1] ^= v[ 9];    
        m_state[2] ^= v[10];    
        m_state[3] ^= v[11];    
        m_state[4] ^= v[12];    
        m_state[5] ^= v[13];    
        m_state[6] ^= v[14];    
        m_state[7] ^= v[15];    
    }

    void Blake64::TransformBlock(byte* a_data, int a_index)
    {
        a_data = a_data + a_index;

        ulong v[16];
        ulong m[16];

        m[ 0] = U8TO64_BE(a_data +  0);
        m[ 1] = U8TO64_BE(a_data +  8);
        m[ 2] = U8TO64_BE(a_data + 16);
        m[ 3] = U8TO64_BE(a_data + 24);
        m[ 4] = U8TO64_BE(a_data + 32);
        m[ 5] = U8TO64_BE(a_data + 40);
        m[ 6] = U8TO64_BE(a_data + 48);
        m[ 7] = U8TO64_BE(a_data + 56);
        m[ 8] = U8TO64_BE(a_data + 64);
        m[ 9] = U8TO64_BE(a_data + 72);
        m[10] = U8TO64_BE(a_data + 80);
        m[11] = U8TO64_BE(a_data + 88);
        m[12] = U8TO64_BE(a_data + 96);
        m[13] = U8TO64_BE(a_data +104);
        m[14] = U8TO64_BE(a_data +112);
        m[15] = U8TO64_BE(a_data +120);

        v[ 0] = m_state[0];
        v[ 1] = m_state[1];
        v[ 2] = m_state[2];
        v[ 3] = m_state[3];
        v[ 4] = m_state[4];
        v[ 5] = m_state[5];
        v[ 6] = m_state[6];
        v[ 7] = m_state[7];
        v[ 8] = 0;
        v[ 8] ^= 0x243F6A8885A308D3;
        v[ 9] = 0;
        v[ 9] ^= 0x13198A2E03707344;
        v[10] = 0;
        v[10] ^= 0xA4093822299F31D0;
        v[11] = 0;
        v[11] ^= 0x082EFA98EC4E6C89;
        v[12] =  0x452821E638D01377;
        v[13] =  0xBE5466CF34E90C6C;
        v[14] =  0xC0AC29B7C97C50DD;
        v[15] =  0x3F84D5B5B5470917;

        if (!m_nullt) 
        { 
            v[12] ^= (m_processedBytes * 8);
            v[13] ^= (m_processedBytes * 8);
            v[14] ^= 0;
            v[15] ^= 0;
        }

        for(int round=0; round<14; ++round) 
        {
            v[0] = v[0] + v[4] + (m[sigma[round][0]] ^ c64[sigma[round][0+1]]);
            v[12] = v[12] ^ v[0];
            v[12] = (v[12] << (64 - 32)) | (v[12] >> 32);
            v[8] = v[8] + v[12];
            v[4] = v[4] ^ v[8];
            v[4] = (v[4] << (64 - 25)) | (v[4] >> 25);
            v[0] = v[0] + v[4] + (m[sigma[round][0+1]] ^ c64[sigma[round][0]]);
            v[12] = v[12] ^ v[0];
            v[12] = (v[12] << (64 - 16)) | (v[12] >> 16); 
            v[8] = v[8] + v[12];
            v[4] = v[4] ^ v[8];
            v[4] = (v[4] << (64 - 11)) | (v[4] >> 11);

            v[1] = v[1] + v[5] + (m[sigma[round][2]] ^ c64[sigma[round][2+1]]);
            v[13] = v[13] ^ v[1];
            v[13] = (v[13] << (64 - 32)) | (v[13] >> 32);
            v[9] = v[9] + v[13];
            v[5] = v[5] ^ v[9];
            v[5] = (v[5] << (64 - 25)) | (v[5] >> 25);
            v[1] = v[1] + v[5] + (m[sigma[round][2+1]] ^ c64[sigma[round][2]]);
            v[13] = v[13] ^ v[1];
            v[13] = (v[13] << (64 - 16)) | (v[13] >> 16); 
            v[9] = v[9] + v[13];
            v[5] = v[5] ^ v[9];
            v[5] = (v[5] << (64 - 11)) | (v[5] >> 11);

            v[2] = v[2] + v[6] + (m[sigma[round][4]] ^ c64[sigma[round][4+1]]);
            v[14] = v[14] ^ v[2];
            v[14] = (v[14] << (64 - 32)) | (v[14] >> 32);
            v[10] = v[10] + v[14];
            v[6] = v[6] ^ v[10];
            v[6] = (v[6] << (64 - 25)) | (v[6] >> 25);
            v[2] = v[2] + v[6] + (m[sigma[round][4+1]] ^ c64[sigma[round][4]]);
            v[14] = v[14] ^ v[2];
            v[14] = (v[14] << (64 - 16)) | (v[14] >> 16); 
            v[10] = v[10] + v[14];
            v[6] = v[6] ^ v[10];
            v[6] = (v[6] << (64 - 11)) | (v[6] >> 11);

            v[3] = v[3] + v[7] + (m[sigma[round][6]] ^ c64[sigma[round][6+1]]);
            v[15] = v[15] ^ v[3];
            v[15] = (v[15] << (64 - 32)) | (v[15] >> 32);
            v[11] = v[11] + v[15];
            v[7] = v[7] ^ v[11];
            v[7] = (v[7] << (64 - 25)) | (v[7] >> 25);
            v[3] = v[3] + v[7] + (m[sigma[round][6+1]] ^ c64[sigma[round][6]]);
            v[15] = v[15] ^ v[3];
            v[15] = (v[15] << (64 - 16)) | (v[15] >> 16); 
            v[11] = v[11] + v[15];
            v[7] = v[7] ^ v[11];
            v[7] = (v[7] << (64 - 11)) | (v[7] >> 11);

            v[3] = v[3] + v[4] + (m[sigma[round][14]] ^ c64[sigma[round][14+1]]);
            v[14] = v[14] ^ v[3];
            v[14] = (v[14] << (64 - 32)) | (v[14] >> 32);
            v[9] = v[9] + v[14];
            v[4] = v[4] ^ v[9];
            v[4] = (v[4] << (64 - 25)) | (v[4] >> 25);
            v[3] = v[3] + v[4] + (m[sigma[round][14+1]] ^ c64[sigma[round][14]]);
            v[14] = v[14] ^ v[3];
            v[14] = (v[14] << (64 - 16)) | (v[14] >> 16); 
            v[9] = v[9] + v[14];
            v[4] = v[4] ^ v[9];
            v[4] = (v[4] << (64 - 11)) | (v[4] >> 11);

            v[2] = v[2] + v[7] + (m[sigma[round][12]] ^ c64[sigma[round][12+1]]);
            v[13] = v[13] ^ v[2];
            v[13] = (v[13] << (64 - 32)) | (v[13] >> 32);
            v[8] = v[8] + v[13];
            v[7] = v[7] ^ v[8];
            v[7] = (v[7] << (64 - 25)) | (v[7] >> 25);
            v[2] = v[2] + v[7] + (m[sigma[round][12+1]] ^ c64[sigma[round][12]]);
            v[13] = v[13] ^ v[2];
            v[13] = (v[13] << (64 - 16)) | (v[13] >> 16); 
            v[8] = v[8] + v[13];
            v[7] = v[7] ^ v[8];
            v[7] = (v[7] << (64 - 11)) | (v[7] >> 11);

            v[0] = v[0] + v[5] + (m[sigma[round][8]] ^ c64[sigma[round][8+1]]);
            v[15] = v[15] ^ v[0];
            v[15] = (v[15] << (64 - 32)) | (v[15] >> 32);
            v[10] = v[10] + v[15];
            v[5] = v[5] ^ v[10];
            v[5] = (v[5] << (64 - 25)) | (v[5] >> 25);
            v[0] = v[0] + v[5] + (m[sigma[round][8+1]] ^ c64[sigma[round][8]]);
            v[15] = v[15] ^ v[0];
            v[15] = (v[15] << (64 - 16)) | (v[15] >> 16); 
            v[10] = v[10] + v[15];
            v[5] = v[5] ^ v[10];
            v[5] = (v[5] << (64 - 11)) | (v[5] >> 11);

            v[1] = v[1] + v[6] + (m[sigma[round][10]] ^ c64[sigma[round][10+1]]);
            v[12] = v[12] ^ v[1];
            v[12] = (v[12] << (64 - 32)) | (v[12] >> 32);
            v[11] = v[11] + v[12];
            v[6] = v[6] ^ v[11];
            v[6] = (v[6] << (64 - 25)) | (v[6] >> 25);
            v[1] = v[1] + v[6] + (m[sigma[round][10+1]] ^ c64[sigma[round][10]]);
            v[12] = v[12] ^ v[1];
            v[12] = (v[12] << (64 - 16)) | (v[12] >> 16); 
            v[11] = v[11] + v[12];
            v[6] = v[6] ^ v[11];
            v[6] = (v[6] << (64 - 11)) | (v[6] >> 11);
        }

        m_state[0] ^= v[ 0]; 
        m_state[1] ^= v[ 1];    
        m_state[2] ^= v[ 2];    
        m_state[3] ^= v[ 3];    
        m_state[4] ^= v[ 4];    
        m_state[5] ^= v[ 5];    
        m_state[6] ^= v[ 6];    
        m_state[7] ^= v[ 7];
        m_state[0] ^= v[ 8]; 
        m_state[1] ^= v[ 9];    
        m_state[2] ^= v[10];    
        m_state[3] ^= v[11];    
        m_state[4] ^= v[12];    
        m_state[5] ^= v[13];    
        m_state[6] ^= v[14];    
        m_state[7] ^= v[15]; 
    }

    void Blake64::Initialize() 
    {
        if (m_hashSize == 48) 
            memcpy(m_state, m_initial_state_384, sizeof(m_initial_state_384));      
        else 
            memcpy(m_state, m_initial_state_512, sizeof(m_initial_state_512));

        m_processedBytes = 0;

        for(int i=0; i<64; ++i)
            m_buffer[i] = 0; 

        m_buffer_pos = 0;
        m_nullt = false;
    }

    void Blake32::Initialize() 
    {
        if (m_hashSize == 28) 
            memcpy(m_state, m_initial_state_224, sizeof(m_initial_state_224));      
        else 
            memcpy(m_state, m_initial_state_256, sizeof(m_initial_state_256));

        m_processedBytes = 0;

        for(int i=0; i<64; ++i)
            m_buffer[i] = 0;

        m_buffer_pos = 0;
        m_nullt = false;
    }

    void Blake32::TransformBytes(byte* a_data, int a_index, int a_length) 
    {
        a_data = a_data + a_index;

        if ((a_length == 0) && (m_buffer_pos != 64))
            return;

        int left = m_buffer_pos; 
        int fill = 64 - left;

        /* compress remaining data filled with new bits */
        if (left && ((a_length & 0x3F) >= fill)) 
        {
            memcpy(m_buffer + left, a_data, fill);
            /* update counter */
            m_processedBytes += 64;

            TransformBlock(m_buffer, 0);
            a_data += fill;
            a_length  -= fill; 

            left = 0;
        }

        /* compress data until enough for a block */
        while (a_length >= 64) 
        {
            /* update counter */
            m_processedBytes += 64;

            TransformBlock(a_data, 0);
            a_data += 64;
            a_length  -= 64;
        }

        if(a_length > 0) 
        {
            memcpy(m_buffer + left, a_data, a_length);
            m_buffer_pos = left + a_length;
        }
        else
            m_buffer_pos=0;
    }

    void Blake64::TransformBytes(byte* a_data, int a_index, int a_length) 
    {
        if ((a_length == 0) && (m_buffer_pos != 128))
            return;

        int left = m_buffer_pos;
        int fill = 128 - left;

        /* compress remaining data filled with new bits */
        if(left && ((a_length & 0x7F) >= fill)) 
        {
            memcpy(m_buffer + left, a_data, fill);
            /* update counter  */
            m_processedBytes += 128;

            TransformBlock(m_buffer, 0);
            a_data += fill;
            a_length  -= fill; 

            left = 0;
        }

        /* compress data until enough for a block */
        while(a_length >= 128) 
        {
            /* update counter */
            m_processedBytes += 128;
            TransformBlock(a_data, 0);
            a_data += 128;
            a_length  -= 128;
        }

        if(a_length > 0) 
        {
            memcpy(m_buffer + left, a_data, a_length & 0x7F);
            m_buffer_pos = left + a_length;
        }
        else
            m_buffer_pos=0;
    }

    void Blake32::Finish()
    {
        unsigned char msglen[8];
        byte zz=0x00;
        byte zo=0x01;
        byte oz=0x80;
        byte oo=0x81;

        /* 
        copy nb. bits hash in total as a 64-bit BE word
        */
        uint low  = (uint)(m_processedBytes + m_buffer_pos) * 8;
        uint high = (uint)((m_processedBytes * 8) >> 32);
        if (low < (uint)m_buffer_pos*8)
            high++;
        U32TO8_BE(msglen + 0, high);
        U32TO8_BE(msglen + 4, low);

        /* message bitlength multiple of 8 */

        if (m_buffer_pos == 55) 
        {
            /* special case of one padding byte */
            m_processedBytes -= 1;
            if (m_hashSize == 28) 
                TransformBytes(&oz, 0, 1);
            else
                TransformBytes(&oo, 0, 1);
        }
        else 
        {
            if (m_buffer_pos < 55) 
            {
                /* use t=0 if no remaining data */
                if (m_buffer_pos == 0) 
                    m_nullt=true;
                /* enough space to fill the block  */
                m_processedBytes -= (55 - m_buffer_pos);
                TransformBytes(padding, 0, 55 - m_buffer_pos);
            }
            else 
            {
                /* NOT enough space, need 2 compressions */
                m_processedBytes -= (64 - m_buffer_pos);
                TransformBytes(padding, 0, 64 - m_buffer_pos);
                m_processedBytes -= 55;
                TransformBytes(padding+1, 0, 55);  /* padd with zeroes */
                m_nullt = true; /* raise flag to set t=0 at the next compress */
            }
            if (m_hashSize == 28) 
                TransformBytes(&zz, 0, 1);
            else
                TransformBytes(&zo, 0, 1);

            m_processedBytes -= 1;
        }
        m_processedBytes -= 8;
        TransformBytes(msglen, 0, 8);   
    }

    byte* Blake32::GetResult()
    {
        byte* result = new byte[m_hashSize];

        U32TO8_BE(result + 0, m_state[0]);
        U32TO8_BE(result + 4, m_state[1]);
        U32TO8_BE(result + 8, m_state[2]);
        U32TO8_BE(result +12, m_state[3]);
        U32TO8_BE(result +16, m_state[4]);
        U32TO8_BE(result +20, m_state[5]);
        U32TO8_BE(result +24, m_state[6]);

        if (m_hashSize == 32) 
            U32TO8_BE(result + 28, m_state[7]);

        return result;
    }

    byte* Blake64::GetResult()
    {
        byte* result = new byte[m_hashSize];

        U64TO8_BE(result + 0, m_state[0]);
        U64TO8_BE(result + 8, m_state[1]);
        U64TO8_BE(result +16, m_state[2]);
        U64TO8_BE(result +24, m_state[3]);
        U64TO8_BE(result +32, m_state[4]);
        U64TO8_BE(result +40, m_state[5]);

        if (m_hashSize == 64) 
        {
            U64TO8_BE(result +48, m_state[6]);
            U64TO8_BE(result +56, m_state[7]);
        }

        return result;
    }

    void Blake64::Finish()
    {
        unsigned char msglen[16];
        byte zz=0x00;
        byte zo=0x01;
        byte oz=0x80;
        byte oo=0x81;

        /* copy nb. bits hash in total as a 128-bit BE word */
        ulong low  = (m_processedBytes + m_buffer_pos) * 8;
        ulong high = 0;
        if (low < m_buffer_pos*8)
            high++;
        U64TO8_BE(msglen + 0, high);
        U64TO8_BE(msglen + 8, low);

        /* message bitlength multiple of 8 */

        if (m_buffer_pos == 111) 
        {
            /* special case of one padding byte */
            m_processedBytes -= 1; 
            if (m_hashSize == 48) 
                TransformBytes(&oz, 0, 1);
            else
                TransformBytes(&oo, 0, 1);
        }
        else 
        {
            if (m_buffer_pos < 111) 
            {
                /* use t=0 if no remaining data */
                if (m_buffer_pos == 0) 
                    m_nullt=true;
                /* enough space to fill the block */
                m_processedBytes -= 111 - m_buffer_pos;
                TransformBytes(padding, 0, 111 - m_buffer_pos);
            }
            else 
            { 
                /* NOT enough space, need 2 compressions */
                m_processedBytes -= 128 - m_buffer_pos; 
                TransformBytes(padding, 0, 128 - m_buffer_pos);
                m_processedBytes -= 111;
                TransformBytes(padding+1, 0, 111);  /* padd with zeros */
                m_nullt = true; /* raise flag to set t=0 at the next compress */
            }
            if (m_hashSize == 48) 
                TransformBytes(&zz, 0, 1);
            else
                TransformBytes(&zo, 0, 1);
            m_processedBytes -= 1;
        }
        m_processedBytes -= 16;
        TransformBytes(msglen, 0, 16);  
    }

    byte* Blake::TransformFinal() 
    {
        Finish();
        byte* result = GetResult();
        Initialize();
        return result;
    }

    byte* Blake::ComputeBytes(byte* a_data, int a_index, int a_length) 
    {
        Initialize();
        TransformBytes(a_data, a_index, a_length);
        return TransformFinal();
    }
}