
#pragma once 

#include "stdafx.h"

namespace Keccak
{
    class Keccak
    {
        private:

            byte m_state[200];
            byte m_buffer[144];
            int BlockSize;
            int m_buffer_pos;

            void TransformBlock(byte* a_data, int a_index);
            void Finish();
            byte* GetResult();

        public:

            int HashSize;

            Keccak(int a_hashSize);

            void Initialize();
            void TransformBytes(byte* a_data, int a_index, int a_length);
            byte* TransformFinal();
            byte* ComputeBytes(byte* a_data, int a_length);
    };

}