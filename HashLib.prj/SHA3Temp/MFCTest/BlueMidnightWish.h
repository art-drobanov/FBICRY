
#pragma once 

namespace BMW
{    
    class BMW
    {
        protected:

            int m_hashSize;
	        ulong m_processedBytes;
            
            virtual byte* GetResult() = 0;
            virtual void Finish() = 0;

        public:

            BMW(int a_hashSize) : m_hashSize(a_hashSize)
            {
            }

            byte* ComputeBytes(byte* a_data, int a_length);

            virtual void Initialize() = 0;
            virtual void TransformBytes(byte* a_data, int a_index, int a_length) = 0;

            byte* TransformFinal() 
            {
                Finish();
                byte* result = GetResult();
                Initialize();
                return result;
            }
    };

    class BMW32 : public BMW
    {
        protected:

            uint m_state32[32];
	        byte m_buffer32[64*2];

            void TransformBlock(byte* a_data, int a_index);
            void FinalCompression();

            virtual byte* GetResult();
            virtual void Finish();

        public:

            BMW32(int a_hashSize)
                : BMW(a_hashSize)
            {
                Initialize();
            }

            virtual void Initialize();
            virtual void TransformBytes(byte* a_data, int a_index, int a_length);
    };

    class BMW64 : public BMW
    {
        protected:

            ulong m_state64[32];
	        byte m_buffer64[128*2];

            void TransformBlock(byte* a_data, int a_index);
            void FinalCompression();

            virtual byte* GetResult();
            virtual void Finish();

        public:

            BMW64(int a_hashSize)
                : BMW(a_hashSize)
            {
                Initialize();
            }

            virtual void Initialize();
            virtual void TransformBytes(byte* a_data, int a_index, int a_length);
    };
}