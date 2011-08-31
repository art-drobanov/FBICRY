
#pragma once

using namespace System;
using namespace System::Diagnostics;
using namespace System::Collections::Generic;
using namespace System::IO;
using namespace HashLib;
using namespace System::Threading;

#include <intrin.h>
#include "Types.h"
#include "Trans.h"

namespace CLRTest
{
    #pragma unmanaged

    class RDTSC
    {
        public:

            static __inline uint64 rdtsc()
            {
                return __rdtsc();
            }
    };

    #pragma managed

    ref class TestBase abstract
    {
        private:

            static array<byte>^ GetRandomBytes(int a_length)
            {
                array<byte>^ r = gcnew array<byte>(a_length);

                Random^ random = gcnew Random();
                random->NextBytes(r);

                return r;
            }

            int GetLength(String^ a_str)
            {
                return Int32::Parse(a_str->Substring(6));
            }

            array<byte>^ GetMsg(String^ a_str)
            {
                return Converters::ConvertHexStringToBytes(a_str->Substring(6)->ToUpper());
            }

            array<byte>^ GetDigest(String^ a_str)
            {
                return Converters::ConvertHexStringToBytes(a_str->Substring(5)->ToUpper());
            }

            List<String^>^ LoadTestVectors(String^ a_filePath)
            {
                List<String^>^ list = gcnew List<String^>();

                for each (String^ str in File::ReadAllLines(a_filePath))
                    list->Add(str);

                for (int i=list->Count-1; i>=0; i--)
                {
                    if (String::IsNullOrWhiteSpace(list[i]))
                        list->RemoveAt(i);
                }

                list->RemoveAt(0);
                list->RemoveAt(0);
                list->RemoveAt(0);

                Debug::Assert(list->Count % 3 == 0);

                return list;
            }

            void SpeedTest()
            {
                int bitlens[4] = { 224, 256, 384, 512 };

                System::Console::WriteLine("\nSpeed Testing (MB/s)");

                int len = 1024*1024;

                if (len % GetMaxBufferSize() != 0)
                    len = ((len / GetMaxBufferSize()) + 1) * GetMaxBufferSize();

                array<byte>^ buf = GetRandomBytes(len);

                for (int i=0; i<4; i++)
                {
                    CreateHash(bitlens[i]);

                    Stopwatch^ sw = gcnew Stopwatch();

                    Stopwatch^ total = gcnew Stopwatch();
                    total->Start();

                    InitializeHash();

                    double time = Double::MaxValue;
                    for (int j=0; j<200; j++)
                    {      
                        sw->Start();
                        TransformBytes(buf, 0, len);
                        sw->Stop();
                        if (sw->ElapsedMilliseconds < time)
                            time = (double)sw->ElapsedMilliseconds;
                        if (total->ElapsedMilliseconds > 5000)
                            break;
                    }

                    TransformFinal();

                    System::Console::WriteLine("{0} - {1:0.00} MB/s", bitlens[i], (double)len / 1024.0 / 1024.0 / (time / 1000.0));
                }
            }

            void SpeedTestPerCycle()
            {
                int bitlens[4] = { 224, 256, 384, 512 };

                System::Console::WriteLine("\nSpeed Testing (Bytes/cycle)");
                System::Console::WriteLine("       1        10       100      1000     10000    100000\n");

	            for (int i=0; i<4; i++ ) 
	            {
                    System::Console::Write("{0}    ", bitlens[i]);

                    CreateHash(bitlens[i]);
		
                    Stopwatch^ sw = gcnew Stopwatch();

		            for(int j=1; j<=100000; j*=10)
		            {
                        array<byte>^ msg = GetRandomBytes(j);

			            volatile uint64 cy0 = RDTSC::rdtsc();
                        ComputeBytes(msg);
			            volatile uint64 cy1 = RDTSC::rdtsc();
			            uint64 c1 = cy1 - cy0;

                        bool br = false;
                        sw->Restart();

			            for (int ii=0; ii<10000; ii++ ) 
			            {
				            cy0 = RDTSC::rdtsc();
				            ComputeBytes(msg);
				            cy1 = RDTSC::rdtsc();
				            cy1 -= cy0;
				            c1 = (uint)(c1 > cy1 ? cy1 : c1);

                            /*if (sw->ElapsedMilliseconds > 10*1000)
                            {
                                br = true;
                                break;
                            }*/
			            }
                        System::Console::Write("{0,-6:00.##}{1}   ", ((c1) + 1) / (double)j, br ? "*" : "");
		            }

                    System::Console::WriteLine("");
	            }
            }

            void TestFile(String^ a_filePath, int a_hashLenBits)
            {
                System::Console::WriteLine("Testing file '{0}'", Path::GetFileName(a_filePath));

                List<String^>^ list = LoadTestVectors(a_filePath);
                CreateHash(a_hashLenBits);

                for (int i=0; i<list->Count/3; i++)
                {
                    int len = GetLength(list[i*3]);

                    array<byte>^ msg;
                    if (len == 0)
                        msg = gcnew array<byte>(0);
                    else
                        msg = GetMsg(list[i*3+1]);

                    if (len % 8 == 0)
                    {
                        array<byte>^ res = GetDigest(list[i*3+2]);

                        array<byte>^ out = ComputeBytes(msg);

                        if (Trans::memcmp(out, res) != 0)
                        {
                            System::Console::WriteLine("Test 1 failed for length={0}", len);
                            break;
                        }

                        if ((a_hashLenBits == 512) && (len == 34304))
                        {
                            Random^ random = gcnew Random();
                            for (int i=0; i<10; i++)
                            {
                                InitializeHash();

                                int pos = 0;
                                int L = len / 8;
                                
                                while (L > 0)
                                {
                                    int buf_len = random->Next(GetMaxBufferSize() * 3 / 2);
                                    if (buf_len > L)
                                        buf_len = L;

                                    TransformBytes(msg, pos, buf_len);

                                    pos += buf_len;
                                    L -= buf_len;
                                }

                                array<byte>^ out = TransformFinal();

                                array<byte>^ x1 = ComputeBytes(msg);

                                if (Trans::memcmp(out, res) != 0)
                                {
                                    System::Console::WriteLine("Test 2 failed #1");
                                    break;
                                }

                                if (Trans::memcmp(x1, res) != 0)
                                {
                                    System::Console::WriteLine("Test 2 failed #2");
                                    break;
                                }

                                if (Trans::memcmp(out, x1) != 0)
                                {
                                    System::Console::WriteLine("Test 2 failed #3");
                                    break;
                                }
                            }
                        }
                    }
                }
            }

        protected:

            virtual void TestAllFiles()
            {
                DirectoryInfo^ dir_info = 
                    gcnew DirectoryInfo(
                    Path::GetDirectoryName(System::Reflection::Assembly::GetExecutingAssembly()->CodeBase->Remove(0, 8)));
                String^ dir = dir_info->Parent->Parent->Parent->FullName;

                TestFile(dir + String::Format("//MFCTest//TestVectors//{0}//ShortMsgKAT_224.txt", GetTestVectorsDir()), 224);
                TestFile(dir + String::Format("//MFCTest//TestVectors//{0}//ShortMsgKAT_256.txt", GetTestVectorsDir()), 256);
                TestFile(dir + String::Format("//MFCTest//TestVectors//{0}//ShortMsgKAT_384.txt", GetTestVectorsDir()), 384);
                TestFile(dir + String::Format("//MFCTest//TestVectors//{0}//ShortMsgKAT_512.txt", GetTestVectorsDir()), 512);

                TestFile(dir + String::Format("//MFCTest//TestVectors//{0}//LongMsgKAT_224.txt", GetTestVectorsDir()), 224);
                TestFile(dir + String::Format("//MFCTest//TestVectors//{0}//LongMsgKAT_256.txt", GetTestVectorsDir()), 256);
                TestFile(dir + String::Format("//MFCTest//TestVectors//{0}//LongMsgKAT_384.txt", GetTestVectorsDir()), 384);
                TestFile(dir + String::Format("//MFCTest//TestVectors//{0}//LongMsgKAT_512.txt", GetTestVectorsDir()), 512);
            }

            virtual void TransformBytes(array<byte>^ a_data, int a_index, int a_length) = 0;
            virtual array<byte>^ ComputeBytes(array<byte>^ a_data) = 0;
            virtual array<byte>^ TransformFinal() = 0;
            virtual void CreateHash(int a_hashLenBits) = 0;
            virtual void InitializeHash() = 0;
            virtual String^ GetTestVectorsDir() = 0;
            virtual String^ GetTestName() = 0;
            virtual int GetMaxBufferSize() = 0;

        public:

            static void Init()
            {
                #ifdef NDEBUG
                Process::GetCurrentProcess()->ProcessorAffinity = (IntPtr)1;
                Process::GetCurrentProcess()->PriorityClass = ProcessPriorityClass::RealTime;
                Thread::CurrentThread->Priority = ThreadPriority::Highest;
                #endif
            }

            void Test()
            {
                System::Console::WriteLine("{0}\n", GetTestName());

                TestAllFiles();
                SpeedTest(); 
                SpeedTestPerCycle();

                System::Console::WriteLine("");
            }
    };
}

