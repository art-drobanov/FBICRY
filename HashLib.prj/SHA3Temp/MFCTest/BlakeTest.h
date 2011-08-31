
#pragma once

#include "Blake.h"
#include "Blake-org.h"
#include "TestBase.h"

namespace MFCTest
{
    class BlakeTest : public TestBase
    {
        private:

            Blake::Blake* m_blake;

        protected:

            virtual CString GetTestVectorsDir()
            {
                return "Blake";
            }

            virtual CString GetTestName()
            {
                return "Blake";
            }

            virtual int GetMaxBufferSize()
            {
                return 128;
            }

            virtual void TransformBytes(byte* a_data, int a_index, int a_length)
            {
                m_blake->TransformBytes(a_data, a_index, a_length);
            }

            virtual byte* ComputeBytes(byte* a_data, int a_length)
            {
                return m_blake->ComputeBytes(a_data, 0, a_length);
            }

            virtual byte* TransformFinal()
            {
                return m_blake->TransformFinal();
            }

            virtual void CreateHash(int a_hashLenBits)
            {
                delete m_blake;

                if (a_hashLenBits <= 256)
                    m_blake = new Blake::Blake32(a_hashLenBits/8);
                else
                    m_blake = new Blake::Blake64(a_hashLenBits/8);
            }

            virtual void InitializeHash()
            {
                m_blake->Initialize();
            }

        public:

            BlakeTest()
            {
                m_blake = nullptr;
            }

            ~BlakeTest()
            {
                delete m_blake;
            }
    };

    class BlakeOrgTest : public TestBase
    {
        private:

            BlakeOrg::hashState m_hashState;

        protected:
    
            virtual CString GetTestVectorsDir()
            {
                return "Blake";
            }

            virtual CString GetTestName()
            {
                return "Blake-Org";
            }

            virtual int GetMaxBufferSize()
            {
                return 64;
            }

            virtual void TransformBytes(byte* a_data, int a_index, int a_length)
            {
                BlakeOrg::Update(&m_hashState, a_data + a_index, a_length*8); 
            }

            virtual byte* ComputeBytes(byte* a_data, int a_length)
            {
                byte* out = new byte[m_hashState.hashbitlen/8];
                BlakeOrg::Hash(m_hashState.hashbitlen, a_data, a_length*8, out);
                return out;
            }

            virtual byte* TransformFinal()
            {
                byte* out = new byte[m_hashState.hashbitlen/8];
                BlakeOrg::Final(&m_hashState, out);
                return out;
            }

            virtual void CreateHash(int a_hashLenBits)
            {
                BlakeOrg::Init(&m_hashState, a_hashLenBits);
            }

            virtual void InitializeHash()
            {
                BlakeOrg::Init(&m_hashState, m_hashState.hashbitlen);
            }
    };
}
