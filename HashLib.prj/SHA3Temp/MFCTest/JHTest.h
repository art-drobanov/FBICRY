
#pragma once

#include "JH.h"
#include "JH-org.h"
#include "TestBase.h"

namespace MFCTest
{
    class JHTest : public TestBase
    {
        private:

            JH::JH* m_jh;

        protected:

            virtual CString GetTestVectorsDir()
            {
                return "JH";
            }

            virtual CString GetTestName()
            {
                return "JH";
            }

            virtual int GetMaxBufferSize()
            {
                return 64;
            }

            virtual void TransformBytes(byte* a_data, int a_index, int a_length)
            {
                m_jh->TransformBytes(a_data, a_index, a_length);
            }

            virtual byte* ComputeBytes(byte* a_data, int a_length)
            {
                return m_jh->ComputeBytes(a_data, a_length);
            }

            virtual byte* TransformFinal()
            {
                return m_jh->TransformFinal();
            }

            virtual void CreateHash(int a_hashLenBits)
            {
                delete m_jh;
                m_jh = new JH::JH(a_hashLenBits/8);
            }

            virtual void InitializeHash()
            {
                m_jh->Initialize();
            }

        public:

            JHTest()
            {
                m_jh = nullptr;
            }

            ~JHTest()
            {
                delete m_jh;
            }
    };

    class JHOrgTest : public TestBase
    {
        private:

            JHORG::hashState m_hashState;

        protected:
    
            virtual CString GetTestVectorsDir()
            {
                return "JH";
            }

            virtual CString GetTestName()
            {
                return "JH-Org";
            }

            virtual int GetMaxBufferSize()
            {
                return 64;
            }

            virtual void TransformBytes(byte* a_data, int a_index, int a_length)
            {
                JHORG::Update(&m_hashState, a_data + a_index, a_length*8); 
            }

            virtual byte* ComputeBytes(byte* a_data, int a_length)
            {
                byte* out = new byte[m_hashState.hashbitlen/8];
                JHORG::Hash(m_hashState.hashbitlen, a_data, a_length*8, out);
                return out;
            }

            virtual byte* TransformFinal()
            {
                byte* out = new byte[m_hashState.hashbitlen/8];
                JHORG::Final(&m_hashState, out);
                return out;
            }

            virtual void CreateHash(int a_hashLenBits)
            {
                JHORG::Init(&m_hashState, a_hashLenBits);
            }

            virtual void InitializeHash()
            {
                JHORG::Init(&m_hashState, m_hashState.hashbitlen);
            }
    };
}
