

#pragma once

namespace Blake
{
    class Blake
    {
        protected:

            int m_hashSize;
            int m_buffer_pos;
            bool m_nullt;
            ulong m_processedBytes;

            ulong U8TO64_BE(byte* p);
            void U64TO8_BE(byte* p, ulong v);
            uint U8TO32_BE(byte* p);
            void U32TO8_BE(byte* p, uint v);

            virtual void Finish() = 0;
            virtual byte* GetResult() = 0;

        public:

            Blake(int a_hashSize)
            {
                m_hashSize = a_hashSize;
                Initialize();
            }

            byte* TransformFinal();

            virtual void Initialize()
            {
            }

            byte* ComputeBytes(byte* a_data, int a_index, int a_length);

            virtual void TransformBytes(byte* a_data, int a_index, int a_length) = 0;

    };
    class Blake32 : public Blake
    {
        private:

            uint m_state[8];
            byte m_buffer[64];

            void TransformBlock(byte* a_data, int a_index);
            virtual byte* GetResult();
            virtual void Finish();

        public:

            Blake32(int a_hashSize)
                : Blake(a_hashSize)
            {
                ASSERT((a_hashSize == 28) || (a_hashSize == 32));
            }

            virtual void Initialize();
            virtual void TransformBytes(byte* a_data, int a_index, int a_length);
    };

    class Blake64 : public Blake
    {
        private:

            ulong m_state[8];
            byte m_buffer[128];

            void TransformBlock(byte* a_data, int a_index);
            virtual byte* GetResult();
            virtual void Finish();

        public:

            Blake64(int a_hashSize)
                : Blake(a_hashSize)
            {
                ASSERT((a_hashSize == 48) || (a_hashSize == 64));
            }

            virtual void Initialize();
            virtual void TransformBytes(byte* a_data, int a_index, int a_length);
    };
}