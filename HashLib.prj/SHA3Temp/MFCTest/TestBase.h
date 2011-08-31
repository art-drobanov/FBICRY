
#pragma once 

#include "stdafx.h"

namespace MFCTest
{
    class TestBase
    {
        protected:

            byte* GetRandomBytes(int length)
            {
                byte* r = new byte[length];

                for (int i=0; i<length; i++)
                    r[i] = rand();

                return r;
            }

            char* BufferToHex(byte* a_buffer, int a_length)
            {
                char t[] = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', 'A', 'B', 'C', 'D', 'E', 'F' };
                char* hex = new char[a_length*2+1];
                hex[a_length*2] = 0;

                for (int i=0; i<a_length; i++)
                {
                    hex[i*2] = t[a_buffer[i] & 0xF0];
                    hex[i*2+1] = t[a_buffer[i] & 0x0F];
                }

                return hex;
            }

            void Split(CString& a_str, CStringArray* a_array)
            {
                int curPos = 0;
                CString s;

                CString token;
                if (a_str.Find("\r\n") != -1)
                    token = "\r\n";
                else
                    token = "\n";

                while ((s = a_str.Tokenize(token, curPos)) && (curPos != -1))
                {
                    if (s.GetLength() > 1)
                        a_array->Add(s);
                };
            }

            char HexToBin(char a_char)
            {
                if ((a_char >= '0') && (a_char <= '9'))
                    return a_char - '0';
                else if ((a_char >= 'a') && (a_char <= 'f'))
                    return a_char - 'a' + 10;
                else if ((a_char >= 'A') && (a_char <= 'H'))
                    return a_char - 'A' + 10;
                else
                    AfxThrowInvalidArgException();
            }

            byte* HexToBuffer(char* a_buffer, int a_length)
            {
                ASSERT(a_length % 2 == 0);
                ASSERT(a_length >= 2);

                byte* res = new byte[a_length/2];

                for (int i=0; i<a_length/2; i++)
                    res[i] = (HexToBin(a_buffer[i*2]) << 4) | HexToBin(a_buffer[i*2+1]);

                return res;
            }

            CString StringFormat(char* a_str, ...)
            {
                va_list argptr;
                va_start(argptr, a_str);
 
                CString str;
                str.FormatV(a_str, argptr);
 
                return str;
            }

            int GetLength(CString& a_str)
            {
                char* buf = a_str.GetBuffer();
                int r = atoi(&buf[6]);
                a_str.ReleaseBuffer();
                return r;
            }

            byte* GetMsg(CString& a_str)
            {
                char* buf = a_str.GetBuffer();
                byte* r = HexToBuffer(&buf[6], a_str.GetLength()-6);
                a_str.ReleaseBuffer();
                return r;
            }

            byte* GetDigest(CString& a_str)
            {
                char* buf = a_str.GetBuffer();
                byte* r = HexToBuffer(&buf[5], a_str.GetLength()-5);
                a_str.ReleaseBuffer();
                return r;
            }

            CStringArray* LoadTestVectors(const char* a_filePath)
            {
                CFile file(a_filePath, CFile::modeRead | CFile::shareDenyNone);

                file.AssertValid();

                CString str;
                file.Read(str.GetBufferSetLength((int)file.GetLength()), (int)file.GetLength());
                str.ReleaseBuffer((int)file.GetLength());

                CStringArray* list = new CStringArray();
                Split(str, list);

                list->RemoveAt(0);
                list->RemoveAt(0);
                list->RemoveAt(0);

                ASSERT(list->GetCount() % 3 == 0);

                return list;
            }

            void SpeedTestPerCycle()
            {
                int bitlens[4] = { 224, 256, 384, 512 };

                printf("\nSpeed Testing (Bytes/cycle)\n\n");
                printf("        1           10          100         1000       10000      100000\n\n");

                byte* msg = GetRandomBytes(100000);

	            for (int i=0; i<4; i++ ) 
	            {
                    printf("%d    ", bitlens[i]);

                    CreateHash(bitlens[i]);

                    Stopwatch sw;
		
		            for(int j=1; j<=100000; j*=10)
		            {
			            volatile ulong cy0 = __rdtsc();
                        ComputeBytes(msg, j);
			            volatile ulong cy1 = __rdtsc();
			            ulong c1 = cy1 - cy0;
                        bool br = false;

                        sw.Restart();

			            for (int ii=0; ii<10000; ii++ ) 
			            {
				            cy0 = __rdtsc();
				            ComputeBytes(msg, j);
				            cy1 = __rdtsc();
				            cy1 -= cy0;
				            c1 = (uint)(c1 > cy1 ? cy1 : c1);
			            }
                        printf("%8.2f%s   ", ((c1) + 1) / (double)j, br ? "*" : "");
		            }

                    printf("\n");
	            }

                delete msg;
            }

            void SpeedTest()
            {
                int bitlens[4] = { 224, 256, 384, 512 };

                int len = 1024*1024*10;
                byte* buf = GetRandomBytes(len);

                if (len % GetMaxBufferSize() != 0)
                    len = ((len / GetMaxBufferSize()) + 1) * GetMaxBufferSize();

                printf("\nSpeed Testing (MB/s)\n\n");

                for (int i=0; i<4; i++)
                {
                    CreateHash(bitlens[i]);

                    Stopwatch sw(false);

                    double time = DBL_MAX;

                    for (int j=0; j<20; j++)
                    {      
                        sw.Start();
                        delete ComputeBytes(buf, len);
                        sw.Stop();
                        if (sw.GetElapsedMilliseconds() < time)
                            time = sw.GetElapsedMilliseconds();
                    }

                    printf("%d = %8.2f MB/s. \n", bitlens[i], (double)len / 1024.0 / 1024.0 / (time / 1000.0));
                }

                delete buf;
            }

            void TestFile(const char* a_filePath, int a_hashLenBits)
            {
                printf("Testing file '%s' \n", CFile(a_filePath, CFile::modeRead | CFile::shareDenyNone).GetFileName());

                CStringArray* list = LoadTestVectors(a_filePath);
                CreateHash(a_hashLenBits);

                for (int i=0; i<list->GetCount()/3; i++)
                {
                    int len = GetLength(list->ElementAt(i*3));
                    byte* msg = GetMsg(list->ElementAt(i*3+1));
                    
                    if (len % 8 == 0)
                    {
                        byte* res = GetDigest(list->ElementAt(i*3+2));

                        byte* out = ComputeBytes(msg, len/8);

                        if (memcmp(out, res, a_hashLenBits/8) != 0)
                        {
                            printf("Test 1 failed for length=%d \n", len);
                            delete out;
                            out = nullptr;
                            delete res;
                            res = nullptr;
                            delete msg;
                            msg = nullptr;
                            break;
                        }

                        delete out;
                        out = nullptr;

                        if (len == 34304)
                        {
                            for (int i=0; i<10; i++)
                            {
                                InitializeHash();

                                int pos = 0;
                                int L = len / 8;

                                while (L > 0)
                                {
                                    int buf_len = rand() % (GetMaxBufferSize() * 3 / 2);
                                    if (buf_len > L)
                                        buf_len = L;

                                    TransformBytes(msg, pos, buf_len);

                                    pos += buf_len;
                                    L -= buf_len;
                                }

                                out = TransformFinal();

                                byte* res2 = ComputeBytes(msg, len/8);

                                if (memcmp(out, res, a_hashLenBits/8) != 0)
                                {
                                    printf("Test 2 failed #1\n");
                                    delete out;
                                    out = nullptr;
                                    delete res;
                                    res = nullptr;
                                    delete msg;
                                    msg = nullptr;
                                    delete res2;
                                    res2 = nullptr;
                                    break;
                                }

                                if (memcmp(out, res2, a_hashLenBits/8) != 0)
                                {
                                    printf("Test 2 failed #2\n");
                                    delete out;
                                    out = nullptr;
                                    delete res;
                                    res = nullptr;
                                    delete msg;
                                    msg = nullptr;
                                    delete res2;
                                    res2 = nullptr;
                                    break;
                                }

                                if (memcmp(res, res2, a_hashLenBits/8) != 0)
                                {
                                    printf("Test 2 failed #3\n");
                                    delete out;
                                    out = nullptr;
                                    delete res;
                                    res = nullptr;
                                    delete msg;
                                    msg = nullptr;
                                    delete res2;
                                    res2 = nullptr;
                                    break;
                                }
                                delete res2;
                                res2 = nullptr;

                                delete out;
                                out = nullptr;
                            }
                        }

                        delete res;
                        res = nullptr;
                    }

                    delete msg;
                    msg = nullptr;
                }

                delete list;
                list = nullptr;
            }

            virtual void TestAllFiles()
            {
                char exe_path[MAX_PATH];
                GetModuleFileName(NULL, exe_path, MAX_PATH);
                CString dir = exe_path;
                dir.Replace("MFCTest.exe", "..\\..\\");

                TestFile(dir + StringFormat("TestVectors//%s//ShortMsgKAT_224.txt", GetTestVectorsDir()), 224);
                TestFile(dir + StringFormat("TestVectors//%s//ShortMsgKAT_256.txt", GetTestVectorsDir()), 256);
                TestFile(dir + StringFormat("TestVectors//%s//ShortMsgKAT_384.txt", GetTestVectorsDir()), 384);
                TestFile(dir + StringFormat("TestVectors//%s//ShortMsgKAT_512.txt", GetTestVectorsDir()), 512);

                TestFile(dir + StringFormat("TestVectors//%s//LongMsgKAT_224.txt", GetTestVectorsDir()), 224);
                TestFile(dir + StringFormat("TestVectors//%s//LongMsgKAT_256.txt", GetTestVectorsDir()), 256);
                TestFile(dir + StringFormat("TestVectors//%s//LongMsgKAT_384.txt", GetTestVectorsDir()), 384);
                TestFile(dir + StringFormat("TestVectors//%s//LongMsgKAT_512.txt", GetTestVectorsDir()), 512);
            }

            virtual void TransformBytes(byte* a_data, int a_index, int a_length) = 0;
            virtual byte* ComputeBytes(byte* a_data, int a_length) = 0;
            virtual byte* TransformFinal() = 0;
            virtual void CreateHash(int a_hashLenBits) = 0;
            virtual void InitializeHash() = 0;
            virtual CString GetTestVectorsDir() = 0;
            virtual CString GetTestName() = 0;
            virtual int GetMaxBufferSize() = 0;

        public:

            static void Init()
            {
                srand(::GetTickCount());

                #ifdef NDEBUG

	            DWORD_PTR afp;
	            DWORD_PTR afs;
	            HANDLE ph = GetCurrentProcess();
	            if(GetProcessAffinityMask(ph, &afp, &afs))
	            {
		            afp &= (GetCurrentProcessorNumber() + 1);
		            if (!SetProcessAffinityMask(ph, afp))
			            printf("Couldn't set Process Affinity Mask\n\n"); 
	            }
	            else
		            printf("Couldn't get Process Affinity Mask\n\n"); 

                
                if (!SetPriorityClass(GetCurrentProcess(), REALTIME_PRIORITY_CLASS))
                    printf("Couldn't set process priority\n\n");
                if (!SetThreadPriority(GetCurrentThread(), THREAD_PRIORITY_TIME_CRITICAL))
                    printf("Couldn't set thread priority\n\n");

                #endif
            }

            void Test()
            {
                printf("%s - Starting \n\n", GetTestName());

                TestAllFiles();

                SpeedTest();
                SpeedTestPerCycle();

                printf("\n");
            }

    };
}