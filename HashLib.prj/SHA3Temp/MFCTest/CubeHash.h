
#pragma once

namespace CubeHash
{
    class CubeHash
    {
        private:

            int m_buffer_pos;
            int m_hashSize;
            int m_rounds;
            int m_blockSize;
            uint m_buffer[32];

        protected:

            void TransformBlock();
            byte* GetResult();
            void Finish();

        public:

            CubeHash(int a_hashSize, int a_rounds, int a_blockSize)
            {
                if (a_hashSize < 1) 
                    AfxThrowInvalidArgException();
                if (a_hashSize > 64) 
                    AfxThrowInvalidArgException();
                if (a_blockSize < 1) 
                    AfxThrowInvalidArgException();
                if (a_blockSize > 128) 
                    AfxThrowInvalidArgException();
                if (a_rounds < 1) 
                    AfxThrowInvalidArgException();

                m_blockSize = a_blockSize;
                m_rounds = a_rounds;
                m_hashSize = a_hashSize;

                Initialize();
            }

            void Initialize();
            void TransformBytes(byte* a_data, int a_length);
            byte* TransformFinal();
            byte* ComputeBytes(byte* a_data, int a_length);
    };
}
