
#include "stdafx.h"
#include "JHTest.h"
#include "BlakeTest.h"
#include "BlueMidnightWishTest.h"
#include "CubeHashTest.h"
#include "EchoTest.h"
#include "FugueTest.h"
#include "GroestlTest.h"
#include "HamsiTest.h"
#include "KeccakTest.h"
#include "LuffaTest.h"

using namespace MFCTest;

int _tmain(int argc, TCHAR* argv[], TCHAR* envp[])
{
    TestBase::Init();
	
    //JHTest().Test();
    //JHOrgTest().Test();

    //BlakeTest().Test();
    //BlakeOrgTest().Test();
    
    //BlueMidnightWishTest().Test();
    //BlueMidnightWishOrgTest().Test();

    //CubeHashTest(16,32).Test();
    //CubeHashTest(1,1).Test();
    //CubeHashTest(1,8).Test();
    //CubeHashTest(8,1).Test();
    //CubeHashTest(8,64).Test();

    //CubeHashOrgTest().Test();
    
    //EchoTest().Test();
    //EchoOrgTest().Test();

    //FugueTest().Test();
    //FugueOrgTest().Test();

    //GroestlOrgTest().Test();
    //GroestlTest().Test();

    //HamsiOrgTest().Test();
    //HamsiTest().Test();

    //KeccakOrgTest().Test();
    //KeccakTest().Test();

    //LuffaOrgTest().Test();
    //LuffaTest().Test();

    printf("Done \n");
    getchar();

	return 0;
}