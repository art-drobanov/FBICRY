
#include "BlakeTest.h"
#include "BlueMidnightWishTest.h"
#include "CubeHashTest.h"
#include "EchoTest.h"
#include "FugueTest.h"
#include "GroestlTest.h"
#include "HamsiTest.h"
#include "JHTest.h"
#include "KeccakTest.h"
#include "LuffaTest.h"

using namespace CLRTest;
using namespace HashLib;

void main()
{
    TestBase::Init();

    //BlakeTest::DoTest();
    //BlakeCSharpTest::DoTest();

    //BlueMidnightWishTest::DoTest();
    //BlueMidnightWishCSharpTest::DoTest();

    //CubeHashTest::Test(16, 32);
    //CubeHashCSharpTest::Test(16, 32);

    //CubeHashCSharpTest::Test(1, 1);
    //CubeHashCSharpTest::Test(1, 8);
    //CubeHashCSharpTest::Test(8, 1);
    //CubeHashCSharpTest::Test(8, 64);

    //CubeHashTest::Test(1, 1);
    //CubeHashTest::Test(1, 8);
    //CubeHashTest::Test(8, 1);
    //CubeHashTest::Test(8, 64);

    //EchoTest::DoTest();
    //EchoCSharpTest::DoTest();

    //FugueTest::DoTest();
    //FugueCSharpTest::DoTest();

    //GroestlTest::DoTest();
    //GroestlCSharpTest::DoTest();

    //HamsiTest::DoTest();
    //HamsiCSharpTest::DoTest();

    //JHTest::DoTest();
    //JHCSharpTest::DoTest();

    //KeccakTest::DoTest();
    //KeccakCSharpTest::DoTest();

    //LuffaTest::DoTest();
    //LuffaCSharpTest::DoTest();

    System::Console::WriteLine("Done \n");
    System::Console::ReadKey();
}