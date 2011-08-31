

#pragma once

using namespace System::Diagnostics;
using namespace System;
using namespace HashLib;

namespace Blake
{
    ref class Blake abstract
    {
        protected:

            static array<byte>^ padding =
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

            static array<byte>^ ending = { 0x00, 0x01, 0x80, 0x81 };

            static array<array<byte>^>^ sigma = 
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

            int m_hashSize;
            bool m_nullt;
            int m_buffer_pos;
            uint64 m_processedBytes;

            virtual void Finish() = 0;
            virtual array<byte>^ GetResult() = 0;
            virtual void TransformBlock(array<byte>^ a_data, int a_index) = 0;

        public:

            Blake(int a_hashSize) : m_hashSize(a_hashSize)
            {
            }

            array<byte>^ TransformFinal()
            {
                Finish();
                array<byte>^ result = GetResult();
                Initialize();
                return result;
            }

            virtual void Initialize()
            {
                m_processedBytes = 0;
                m_buffer_pos = 0;
                m_nullt = false;
            }

            array<byte>^ ComputeBytes(array<byte>^ a_data)
            {
                Initialize();
                TransformBytes(a_data, 0, a_data->Length);
                return TransformFinal();
            }

            virtual void TransformBytes(array<byte>^ a_data, int a_index, int a_length) = 0;
    };

    ref class Blake32 : public Blake
    {
        private:

            static array<uint>^ c32 = 
            {
                0x243F6A88, 0x85A308D3, 0x13198A2E, 0x03707344, 0xA4093822, 0x299F31D0, 0x082EFA98, 0xEC4E6C89, 
                0x452821E6, 0x38D01377, 0xBE5466CF, 0x34E90C6C, 0xC0AC29B7, 0xC97C50DD, 0x3F84D5B5, 0xB5470917 
            };

            static array<uint>^ m_initial_state_256 =
            {
                0x6A09E667, 0xBB67AE85, 0x3C6EF372, 0xA54FF53A, 0x510E527F, 0x9B05688C, 0x1F83D9AB, 0x5BE0CD19
            };

            static array<uint>^ m_initial_state_224 =
            {
                0xC1059ED8, 0x367CD507, 0x3070DD17, 0xF70E5939, 0xFFC00B31, 0x68581511, 0x64F98FA7, 0xBEFA4FA4
            };

        protected:

            array<uint>^ m_state;
            array<byte>^ m_buffer;

            virtual void TransformBlock(array<byte>^ a_data, int a_index) override
            {
                array<uint>^ m = Converters::ConvertBytesToUIntsSwapOrder(a_data, a_index, 64);
                array<uint>^ v = gcnew array<uint>(16);

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

                for (int round=0; round<10; round++) 
                {
                    v[0] = (m[sigma[round][0]] ^ c32[sigma[round][1]]) + v[0] + v[4];
                    v[12] = v[12] ^ v[0];
                    v[12] = (v[12] << (32 - 16)) | (v[12] >> 16);
                    v[8] = v[8] + v[12];
                    v[4] = v[4] ^ v[8];
                    v[4] = (v[4] << (32 - 12)) | (v[4] >> 12);
                    v[0] = (m[sigma[round][1]] ^ c32[sigma[round][0]]) + v[0] + v[4];
                    v[12] = v[12] ^ v[0];
                    v[12] = (v[12] << (32 - 8)) | (v[12] >> 8);
                    v[8] = v[8] + v[12];
                    v[4] = v[4] ^ v[8];
                    v[4] = (v[4] << (32 - 7)) | (v[4] >> 7);

                    v[1] = (m[sigma[round][2]] ^ c32[sigma[round][3]]) + v[1] + v[5];
                    v[13] = v[13] ^ v[1];
                    v[13] = (v[13] << (32 - 16)) | (v[13] >> 16);
                    v[9] = v[9] + v[13];
                    v[5] = v[5] ^ v[9];
                    v[5] = (v[5] << (32 - 12)) | (v[5] >> 12);
                    v[1] = (m[sigma[round][3]] ^ c32[sigma[round][2]]) + v[1] + v[5];
                    v[13] = v[13] ^ v[1];
                    v[13] = (v[13] << (32 - 8)) | (v[13] >> 8);
                    v[9] = v[9] + v[13];
                    v[5] = v[5] ^ v[9];
                    v[5] = (v[5] << (32 - 7)) | (v[5] >> 7);

                    v[2] = (m[sigma[round][4]] ^ c32[sigma[round][5]]) + v[2] + v[6];
                    v[14] = v[14] ^ v[2];
                    v[14] = (v[14] << (32 - 16)) | (v[14] >> 16);
                    v[10] = v[10] + v[14];
                    v[6] = v[6] ^ v[10];
                    v[6] = (v[6] << (32 - 12)) | (v[6] >> 12);
                    v[2] = (m[sigma[round][5]] ^ c32[sigma[round][4]]) + v[2] + v[6];
                    v[14] = v[14] ^ v[2];
                    v[14] = (v[14] << (32 - 8)) | (v[14] >> 8);
                    v[10] = v[10] + v[14];
                    v[6] = v[6] ^ v[10];
                    v[6] = (v[6] << (32 - 7)) | (v[6] >> 7);

                    v[3] = (m[sigma[round][6]] ^ c32[sigma[round][7]]) + v[3] + v[7];
                    v[15] = v[15] ^ v[3];
                    v[15] = (v[15] << (32 - 16)) | (v[15] >> 16);
                    v[11] = v[11] + v[15];
                    v[7] = v[7] ^ v[11];
                    v[7] = (v[7] << (32 - 12)) | (v[7] >> 12);
                    v[3] = (m[sigma[round][7]] ^ c32[sigma[round][6]]) + v[3] + v[7];
                    v[15] = v[15] ^ v[3];
                    v[15] = (v[15] << (32 - 8)) | (v[15] >> 8);
                    v[11] = v[11] + v[15];
                    v[7] = v[7] ^ v[11];
                    v[7] = (v[7] << (32 - 7)) | (v[7] >> 7);

                    v[3] = (m[sigma[round][14]] ^ c32[sigma[round][15]]) + v[3] + v[4];
                    v[14] = v[14] ^ v[3];
                    v[14] = (v[14] << (32 - 16)) | (v[14] >> 16);
                    v[9] = v[9] + v[14];
                    v[4] = v[4] ^ v[9];
                    v[4] = (v[4] << (32 - 12)) | (v[4] >> 12);
                    v[3] = (m[sigma[round][15]] ^ c32[sigma[round][14]]) + v[3] + v[4];
                    v[14] = v[14] ^ v[3];
                    v[14] = (v[14] << (32 - 8)) | (v[14] >> 8);
                    v[9] = v[9] + v[14];
                    v[4] = v[4] ^ v[9];
                    v[4] = (v[4] << (32 - 7)) | (v[4] >> 7);

                    v[2] = (m[sigma[round][12]] ^ c32[sigma[round][13]]) + v[2] + v[7];
                    v[13] = v[13] ^ v[2];
                    v[13] = (v[13] << (32 - 16)) | (v[13] >> 16);
                    v[8] = v[8] + v[13];
                    v[7] = v[7] ^ v[8];
                    v[7] = (v[7] << (32 - 12)) | (v[7] >> 12);
                    v[2] = (m[sigma[round][13]] ^ c32[sigma[round][12]]) + v[2] + v[7];
                    v[13] = v[13] ^ v[2];
                    v[13] = (v[13] << (32 - 8)) | (v[13] >> 8);
                    v[8] = v[8] + v[13];
                    v[7] = v[7] ^ v[8];
                    v[7] = (v[7] << (32 - 7)) | (v[7] >> 7);

                    v[0] = (m[sigma[round][8]] ^ c32[sigma[round][9]]) + v[0] + v[5];
                    v[15] = v[15] ^ v[0];
                    v[15] = (v[15] << (32 - 16)) | (v[15] >> 16);
                    v[10] = v[10] + v[15];
                    v[5] = v[5] ^ v[10];
                    v[5] = (v[5] << (32 - 12)) | (v[5] >> 12);
                    v[0] = (m[sigma[round][9]] ^ c32[sigma[round][8]]) + v[0] + v[5];
                    v[15] = v[15] ^ v[0];
                    v[15] = (v[15] << (32 - 8)) | (v[15] >> 8);
                    v[10] = v[10] + v[15];
                    v[5] = v[5] ^ v[10];
                    v[5] = (v[5] << (32 - 7)) | (v[5] >> 7);

                    v[1] = (m[sigma[round][10]] ^ c32[sigma[round][11]]) + v[1] + v[6];
                    v[12] = v[12] ^ v[1];
                    v[12] = (v[12] << (32 - 16)) | (v[12] >> 16);
                    v[11] = v[11] + v[12];
                    v[6] = v[6] ^ v[11];
                    v[6] = (v[6] << (32 - 12)) | (v[6] >> 12);
                    v[1] = (m[sigma[round][11]] ^ c32[sigma[round][10]]) + v[1] + v[6];
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

            virtual array<byte>^ GetResult() override
            {
                return Converters::ConvertUIntsToBytesSwapOrder(m_state, 0, m_hashSize/4);
            }

            virtual void Finish() override
            {
                uint64 bits = m_processedBytes * 8;

                if (m_buffer_pos == 55) 
                {
                    m_processedBytes -= 1;
                    if (m_hashSize == 28) 
                        TransformBytes(ending, 2, 1);
                    else
                        TransformBytes(ending, 3, 1);
                }
                else 
                {
                    if (m_buffer_pos < 55) 
                    {
                        if (m_buffer_pos == 0) 
                            m_nullt=true;
                        m_processedBytes -= (55 - m_buffer_pos);
                        TransformBytes(padding, 0, 55 - m_buffer_pos);
                    }
                    else 
                    {
                        m_processedBytes -= (64 - m_buffer_pos);
                        TransformBytes(padding, 0, 64 - m_buffer_pos);
                        m_processedBytes -= 55;
                        TransformBytes(padding, 1, 55);
                        m_nullt = true;
                    }
                    if (m_hashSize == 28) 
                        TransformBytes(ending, 0, 1);
                    else
                        TransformBytes(ending, 1, 1);

                    m_processedBytes -= 1;
                }

                m_processedBytes -= 8;

                array<byte>^ msg = gcnew array<byte>(8);
                msg[0] = (byte)(bits >> 56);
                msg[1] = (byte)(bits >> 48);
                msg[2] = (byte)(bits >> 40);
                msg[3] = (byte)(bits >> 32);
                msg[4] = (byte)(bits >> 24);
                msg[5] = (byte)(bits >> 16);
                msg[6] = (byte)(bits >> 8);
                msg[7] = (byte)bits;

                TransformBytes(msg, 0, 8);   
            }

        public:

            Blake32(int a_hashSize)
                : Blake(a_hashSize)
            {
                Debug::Assert((a_hashSize == 28) || (a_hashSize == 32));

                m_state = gcnew array<uint>(8);
                m_buffer = gcnew array<byte>(64);

                Initialize();
            }

            virtual void Initialize() override
            {
                if (m_hashSize == 28) 
                    Buffer::BlockCopy(m_initial_state_224, 0, m_state, 0, 32);
                else 
                    Buffer::BlockCopy(m_initial_state_256, 0, m_state, 0, 32);

                Array::Clear(m_buffer, 0, 64);

                Blake::Initialize();
            }

            virtual void TransformBytes(array<byte>^ a_data, int a_index, int a_length) override
            {
                if ((a_length == 0) && (m_buffer_pos != 64))
                    return;

                int left = m_buffer_pos; 
                int fill = 64 - left;

                if (left && ((a_length & 0x3F) >= fill)) 
                {
                    Array::Copy(a_data, a_index, m_buffer, left, fill);

                    m_processedBytes += fill;

                    TransformBlock(m_buffer, 0);
                    a_index += fill;
                    a_length  -= fill; 

                    left = 0;
                    m_buffer_pos = 0;
                }

                while (a_length >= 64) 
                {
                    m_processedBytes += 64;

                    TransformBlock(a_data, a_index);
                    a_index += 64;
                    a_length  -= 64;
                }

                if(a_length > 0) 
                {
                    Array::Copy(a_data, a_index, m_buffer, left, a_length);
                    m_buffer_pos = left + a_length;
                    m_processedBytes += a_length;
                }
                else
                    m_buffer_pos = 0;
            }
    };

    ref class Blake64 : public Blake
    {
        private:

            static array<uint64>^ c64 = 
            {
                0x243F6A8885A308D3,0x13198A2E03707344, 0xA4093822299F31D0,0x082EFA98EC4E6C89,
                0x452821E638D01377,0xBE5466CF34E90C6C, 0xC0AC29B7C97C50DD,0x3F84D5B5B5470917,
                0x9216D5D98979FB1B,0xD1310BA698DFB5AC, 0x2FFD72DBD01ADFB7,0xB8E1AFED6A267E96,
                0xBA7C9045F12C7F99,0x24A19947B3916CF7, 0x0801F2E2858EFC16,0x636920D871574E69
            };

            static array<uint64>^ m_initial_state_384 =
            {
                0xCBBB9D5DC1059ED8, 0x629A292A367CD507, 0x9159015A3070DD17, 0x152FECD8F70E5939,
                0x67332667FFC00B31, 0x8EB44A8768581511, 0xDB0C2E0D64F98FA7, 0x47B5481DBEFA4FA4
            };

            static array<uint64>^ m_initial_state_512 =
            {
                0x6A09E667F3BCC908, 0xBB67AE8584CAA73B, 0x3C6EF372FE94F82B, 0xA54FF53A5F1D36F1,
                0x510E527FADE682D1, 0x9B05688C2B3E6C1F, 0x1F83D9ABFB41BD6B, 0x5BE0CD19137E2179
            };

        protected:

            array<uint64>^ m_state;
            array<byte>^ m_buffer;

            virtual void TransformBlock(array<byte>^ a_data, int a_index) override
            {
                array<uint64>^ v = gcnew array<uint64>(16);
                array<uint64>^ m = Converters::ConvertBytesToULongsSwapOrder(a_data, a_index, 128);

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

                for(int round=0; round<14; round++) 
                {
                    v[0] = v[0] + v[4] + (m[sigma[round][0]] ^ c64[sigma[round][1]]);
                    v[12] = v[12] ^ v[0];
                    v[12] = (v[12] << (64 - 32)) | (v[12] >> 32);
                    v[8] = v[8] + v[12];
                    v[4] = v[4] ^ v[8];
                    v[4] = (v[4] << (64 - 25)) | (v[4] >> 25);
                    v[0] = v[0] + v[4] + (m[sigma[round][1]] ^ c64[sigma[round][0]]);
                    v[12] = v[12] ^ v[0];
                    v[12] = (v[12] << (64 - 16)) | (v[12] >> 16); 
                    v[8] = v[8] + v[12];
                    v[4] = v[4] ^ v[8];
                    v[4] = (v[4] << (64 - 11)) | (v[4] >> 11);

                    v[1] = v[1] + v[5] + (m[sigma[round][2]] ^ c64[sigma[round][3]]);
                    v[13] = v[13] ^ v[1];
                    v[13] = (v[13] << (64 - 32)) | (v[13] >> 32);
                    v[9] = v[9] + v[13];
                    v[5] = v[5] ^ v[9];
                    v[5] = (v[5] << (64 - 25)) | (v[5] >> 25);
                    v[1] = v[1] + v[5] + (m[sigma[round][3]] ^ c64[sigma[round][2]]);
                    v[13] = v[13] ^ v[1];
                    v[13] = (v[13] << (64 - 16)) | (v[13] >> 16); 
                    v[9] = v[9] + v[13];
                    v[5] = v[5] ^ v[9];
                    v[5] = (v[5] << (64 - 11)) | (v[5] >> 11);

                    v[2] = v[2] + v[6] + (m[sigma[round][4]] ^ c64[sigma[round][5]]);
                    v[14] = v[14] ^ v[2];
                    v[14] = (v[14] << (64 - 32)) | (v[14] >> 32);
                    v[10] = v[10] + v[14];
                    v[6] = v[6] ^ v[10];
                    v[6] = (v[6] << (64 - 25)) | (v[6] >> 25);
                    v[2] = v[2] + v[6] + (m[sigma[round][5]] ^ c64[sigma[round][4]]);
                    v[14] = v[14] ^ v[2];
                    v[14] = (v[14] << (64 - 16)) | (v[14] >> 16); 
                    v[10] = v[10] + v[14];
                    v[6] = v[6] ^ v[10];
                    v[6] = (v[6] << (64 - 11)) | (v[6] >> 11);

                    v[3] = v[3] + v[7] + (m[sigma[round][6]] ^ c64[sigma[round][7]]);
                    v[15] = v[15] ^ v[3];
                    v[15] = (v[15] << (64 - 32)) | (v[15] >> 32);
                    v[11] = v[11] + v[15];
                    v[7] = v[7] ^ v[11];
                    v[7] = (v[7] << (64 - 25)) | (v[7] >> 25);
                    v[3] = v[3] + v[7] + (m[sigma[round][7]] ^ c64[sigma[round][6]]);
                    v[15] = v[15] ^ v[3];
                    v[15] = (v[15] << (64 - 16)) | (v[15] >> 16); 
                    v[11] = v[11] + v[15];
                    v[7] = v[7] ^ v[11];
                    v[7] = (v[7] << (64 - 11)) | (v[7] >> 11);

                    v[3] = v[3] + v[4] + (m[sigma[round][14]] ^ c64[sigma[round][15]]);
                    v[14] = v[14] ^ v[3];
                    v[14] = (v[14] << (64 - 32)) | (v[14] >> 32);
                    v[9] = v[9] + v[14];
                    v[4] = v[4] ^ v[9];
                    v[4] = (v[4] << (64 - 25)) | (v[4] >> 25);
                    v[3] = v[3] + v[4] + (m[sigma[round][15]] ^ c64[sigma[round][14]]);
                    v[14] = v[14] ^ v[3];
                    v[14] = (v[14] << (64 - 16)) | (v[14] >> 16); 
                    v[9] = v[9] + v[14];
                    v[4] = v[4] ^ v[9];
                    v[4] = (v[4] << (64 - 11)) | (v[4] >> 11);

                    v[2] = v[2] + v[7] + (m[sigma[round][12]] ^ c64[sigma[round][13]]);
                    v[13] = v[13] ^ v[2];
                    v[13] = (v[13] << (64 - 32)) | (v[13] >> 32);
                    v[8] = v[8] + v[13];
                    v[7] = v[7] ^ v[8];
                    v[7] = (v[7] << (64 - 25)) | (v[7] >> 25);
                    v[2] = v[2] + v[7] + (m[sigma[round][13]] ^ c64[sigma[round][12]]);
                    v[13] = v[13] ^ v[2];
                    v[13] = (v[13] << (64 - 16)) | (v[13] >> 16); 
                    v[8] = v[8] + v[13];
                    v[7] = v[7] ^ v[8];
                    v[7] = (v[7] << (64 - 11)) | (v[7] >> 11);

                    v[0] = v[0] + v[5] + (m[sigma[round][8]] ^ c64[sigma[round][9]]);
                    v[15] = v[15] ^ v[0];
                    v[15] = (v[15] << (64 - 32)) | (v[15] >> 32);
                    v[10] = v[10] + v[15];
                    v[5] = v[5] ^ v[10];
                    v[5] = (v[5] << (64 - 25)) | (v[5] >> 25);
                    v[0] = v[0] + v[5] + (m[sigma[round][9]] ^ c64[sigma[round][8]]);
                    v[15] = v[15] ^ v[0];
                    v[15] = (v[15] << (64 - 16)) | (v[15] >> 16); 
                    v[10] = v[10] + v[15];
                    v[5] = v[5] ^ v[10];
                    v[5] = (v[5] << (64 - 11)) | (v[5] >> 11);

                    v[1] = v[1] + v[6] + (m[sigma[round][10]] ^ c64[sigma[round][11]]);
                    v[12] = v[12] ^ v[1];
                    v[12] = (v[12] << (64 - 32)) | (v[12] >> 32);
                    v[11] = v[11] + v[12];
                    v[6] = v[6] ^ v[11];
                    v[6] = (v[6] << (64 - 25)) | (v[6] >> 25);
                    v[1] = v[1] + v[6] + (m[sigma[round][11]] ^ c64[sigma[round][10]]);
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

            virtual array<byte>^ GetResult() override
            {
                return Converters::ConvertULongsToBytesSwapOrder(m_state, 0, m_hashSize/8);
            }

            virtual void Finish() override
            {
                uint64 bits = m_processedBytes * 8;

                if (m_buffer_pos == 111) 
                {
                    m_processedBytes -= 1; 
                    if (m_hashSize == 48) 
                        TransformBytes(ending, 2, 1);
                    else
                        TransformBytes(ending, 3, 1);
                }
                else 
                {
                    if (m_buffer_pos < 111) 
                    {
                        if (m_buffer_pos == 0) 
                            m_nullt=true;
                        m_processedBytes -= 111 - m_buffer_pos;
                        TransformBytes(padding, 0, 111 - m_buffer_pos);
                    }
                    else 
                    { 
                        m_processedBytes -= 128 - m_buffer_pos; 
                        TransformBytes(padding, 0, 128 - m_buffer_pos);
                        m_processedBytes -= 111;
                        TransformBytes(padding, 1, 111);
                        m_nullt = true;
                    }
                    if (m_hashSize == 48) 
                        TransformBytes(ending, 0, 1);
                    else
                        TransformBytes(ending, 1, 1);
                    m_processedBytes -= 1;
                }

                m_processedBytes -= 16;

                array<byte>^ msg = gcnew array<byte>(16);
                msg[8] = (byte)(bits >> 56);
                msg[9] = (byte)(bits >> 48);
                msg[10] = (byte)(bits >> 40);
                msg[11] = (byte)(bits >> 32);
                msg[12] = (byte)(bits >> 24);
                msg[13] = (byte)(bits >> 16);
                msg[14] = (byte)(bits >> 8);
                msg[15] = (byte)bits;

                TransformBytes(msg, 0, 16);  
            }

        public:

            Blake64(int a_hashSize)
                : Blake(a_hashSize)
            {
                Debug::Assert((a_hashSize == 48) || (a_hashSize == 64));

                m_state = gcnew array<uint64>(8);
                m_buffer = gcnew array<byte>(128);

                Initialize();
            }

            virtual void Initialize() override
            {
                if (m_hashSize == 48) 
                    Buffer::BlockCopy(m_initial_state_384, 0, m_state, 0, 64);      
                else 
                    Buffer::BlockCopy(m_initial_state_512, 0, m_state, 0, 64);

                Array::Clear(m_buffer, 0, 128);

                Blake::Initialize();
            }

            virtual void TransformBytes(array<byte>^ a_data, int a_index, int a_length) override
            {
                if ((a_length == 0) && (m_buffer_pos != 128))
                    return;

                int left = m_buffer_pos;
                int fill = 128 - left;

                if(left && ((a_length & 0x7F) >= fill)) 
                {
                    Array::Copy(a_data, a_index, m_buffer, left, fill);
                    m_processedBytes += fill;

                    TransformBlock(m_buffer, 0);
                    a_index += fill;
                    a_length  -= fill; 

                    left = 0;
                }

                while(a_length >= 128) 
                {
                    m_processedBytes += 128;
                    TransformBlock(a_data, a_index);
                    a_index += 128;
                    a_length  -= 128;
                }

                if(a_length > 0) 
                {
                    Array::Copy(a_data, a_index, m_buffer, left, a_length);
                    m_buffer_pos = left + a_length;
                    m_processedBytes += a_length;
                }
                else
                    m_buffer_pos = 0;
            }
    };
}