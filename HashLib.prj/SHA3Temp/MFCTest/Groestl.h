
#pragma once

namespace Groestl
{
    class GroestlBase
    {
        protected:

            ulong m_state[16];   
            ulong m_processedBytes;
            byte m_buffer[128];
            int m_buffer_pos;
            byte m_pad[136];

            void Finish();
            byte* GetResult();

            virtual void TransformBlock(byte* a_data, int a_index) = 0;
            virtual void OutputTransformation() = 0;

        public:

            int HashSize;
            int BlockSize;

            GroestlBase(int a_hashSize, int a_blockSize)
            {
                HashSize = a_hashSize;
                BlockSize = a_blockSize;

                Initialize();
            }

            void Initialize();
            void TransformBytes(byte* a_data, int a_index, int a_length);
            byte* TransformFinal();
            byte* ComputeBytes(byte* a_data, int a_length);
    };

    class Groestl256Base : public GroestlBase
    {
        protected:

            virtual void TransformBlock(byte* a_data, int a_index);
            virtual void OutputTransformation();

        public:

            Groestl256Base(int a_hashSize)
                : GroestlBase(a_hashSize, 64)
            {
            }
    };

    class Groestl512Base : public GroestlBase
    {
        protected:

            virtual void TransformBlock(byte* a_data, int a_index);
            virtual void OutputTransformation();

        public:

            Groestl512Base(int a_hashSize)
                : GroestlBase(a_hashSize, 128)
            {
            }
    };
}