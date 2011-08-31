
#include "TestBase.h"
#include "CubeHash.h"

#pragma once 

namespace CLRTest
{
    ref class CubeHashTest : TestBase
    {
        private:

            CubeHash::CubeHash^ m_hash;
            int m_blockSize;
            int m_rounds;

        protected:

            virtual void TransformBytes(array<byte>^ a_data, int a_index, int a_length) override
            {
                m_hash->TransformBytes(a_data, a_index, a_length);
            }

            virtual array<byte>^ ComputeBytes(array<byte>^ a_data) override
            {
                return m_hash->ComputeBytes(a_data);
            }

            virtual array<byte>^ TransformFinal() override
            {
                return m_hash->TransformFinal();
            }

            virtual void CreateHash(int a_hashLenBits) override
            {
                m_hash = gcnew CubeHash::CubeHash(a_hashLenBits/8, m_rounds, m_blockSize);
            }

            virtual void InitializeHash() override
            {
                m_hash->Initialize();
            }

            virtual String^ GetTestVectorsDir() override
            {
                return String::Format("CubeHash-{0}-{1}", m_rounds, m_blockSize);
            }

            virtual String^ GetTestName() override
            {
                return String::Format("CubeHash-CLR-{0}-{1}", m_rounds, m_blockSize);
            }

            virtual int GetMaxBufferSize() override
            {
                return 128;
            }

        public: 

            CubeHashTest(int a_rounds, int a_blockSize)
            {
                m_blockSize = a_blockSize;
                m_rounds = a_rounds;
            }

            static void Test(int a_rounds, int a_blockSize)
            {
                CubeHashTest^ test = gcnew CubeHashTest(a_rounds, a_blockSize);
                test->Test();
            }
    };

    ref class CubeHashCSharpTest : TestBase
    {
        private:

            IHash^ m_hash;
            int m_blockSize;
            int m_rounds;

        protected:

            virtual void TransformBytes(array<byte>^ a_data, int a_index, int a_length) override
            {
                m_hash->TransformBytes(a_data, a_index, a_length);
            }

            virtual array<byte>^ ComputeBytes(array<byte>^ a_data) override
            {
                return m_hash->ComputeBytes(a_data)->GetBytes();
            }

            virtual array<byte>^ TransformFinal() override
            {
                return m_hash->TransformFinal()->GetBytes();
            }

            virtual void CreateHash(int a_hashLenBits) override
            {
                m_hash = HashLib::HashFactory::Crypto::SHA3::CreateCubeHash(HashLib::HashSize(a_hashLenBits/8), m_rounds, m_blockSize);
            }

            virtual void InitializeHash() override
            {
                m_hash->Initialize();
            }

            virtual String^ GetTestVectorsDir() override
            {
                return String::Format("CubeHash-{0}-{1}", m_rounds, m_blockSize);
            }

            virtual String^ GetTestName() override
            {
                return String::Format("CubeHash-CSharp-{0}-{1}", m_rounds, m_blockSize);
            }

            virtual int GetMaxBufferSize() override
            {
                return 128;
            }

        public:

            CubeHashCSharpTest(int a_rounds, int a_blockSize)
            {
                m_blockSize = a_blockSize;
                m_rounds = a_rounds;
            }

            static void Test(int a_rounds, int a_blockSize)
            {
                CubeHashCSharpTest^ test = gcnew CubeHashCSharpTest(a_rounds, a_blockSize);
                test->Test();
            }

    };
}
