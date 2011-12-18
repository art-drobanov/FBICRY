using System;

using HashLib;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HashLibTest
{
	[TestClass]
	public class HashesTest : HashesTestBase
	{
		[TestMethod]
		public void HashLib_Crypto_MD5()
		{
			Test(HashFactory.Crypto.CreateMD5(),
			     HashFactory.Crypto.BuildIn.CreateMD5CryptoServiceProvider());
		}

		[TestMethod]
		public void HashLib_Crypto_Snefru()
		{
			Test(HashFactory.Crypto.CreateSnefru_4_128(), null);
			Test(HashFactory.Crypto.CreateSnefru_4_256(), null);
			Test(HashFactory.Crypto.CreateSnefru_8_128(), null);
			Test(HashFactory.Crypto.CreateSnefru_8_256(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_HAS160()
		{
			Test(HashFactory.Crypto.CreateHAS160(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_BuildIn_MD5CryptoServiceProvider()
		{
			Test(HashFactory.Crypto.BuildIn.CreateMD5CryptoServiceProvider(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_MD2()
		{
			Test(HashFactory.Crypto.CreateMD2(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_MD4()
		{
			Test(HashFactory.Crypto.CreateMD4(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_SHA224()
		{
			Test(HashFactory.Crypto.CreateSHA224(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_RIPEMD128()
		{
			Test(HashFactory.Crypto.CreateRIPEMD128(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_BuildIn_RIPEMD160Managed()
		{
			Test(HashFactory.Crypto.BuildIn.CreateRIPEMD160Managed(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_RIPEMD160()
		{
			Test(HashFactory.Crypto.CreateRIPEMD160(),
			     HashFactory.Crypto.BuildIn.CreateRIPEMD160Managed());
		}

		[TestMethod]
		public void HashLib_Crypto_Haval()
		{
			Test(HashFactory.Crypto.CreateHaval_3_128(), null);
			Test(HashFactory.Crypto.CreateHaval_4_128(), null);
			Test(HashFactory.Crypto.CreateHaval_5_128(), null);

			Test(HashFactory.Crypto.CreateHaval_3_160(), null);
			Test(HashFactory.Crypto.CreateHaval_4_160(), null);
			Test(HashFactory.Crypto.CreateHaval_5_160(), null);

			Test(HashFactory.Crypto.CreateHaval_3_192(), null);
			Test(HashFactory.Crypto.CreateHaval_4_192(), null);
			Test(HashFactory.Crypto.CreateHaval_5_192(), null);

			Test(HashFactory.Crypto.CreateHaval_3_224(), null);
			Test(HashFactory.Crypto.CreateHaval_4_224(), null);
			Test(HashFactory.Crypto.CreateHaval_5_224(), null);

			Test(HashFactory.Crypto.CreateHaval_3_256(), null);
			Test(HashFactory.Crypto.CreateHaval_4_256(), null);
			Test(HashFactory.Crypto.CreateHaval_5_256(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_SHA3_JH()
		{
			Test(HashFactory.Crypto.SHA3.CreateJH224(), null);
			Test(HashFactory.Crypto.SHA3.CreateJH256(), null);
			Test(HashFactory.Crypto.SHA3.CreateJH384(), null);
			Test(HashFactory.Crypto.SHA3.CreateJH512(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_SHA3_Echo()
		{
			Test(HashFactory.Crypto.SHA3.CreateEcho224(), null);
			Test(HashFactory.Crypto.SHA3.CreateEcho256(), null);
			Test(HashFactory.Crypto.SHA3.CreateEcho384(), null);
			Test(HashFactory.Crypto.SHA3.CreateEcho512(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_SHA3_Fugue()
		{
			Test(HashFactory.Crypto.SHA3.CreateFugue224(), null);
			Test(HashFactory.Crypto.SHA3.CreateFugue256(), null);
			Test(HashFactory.Crypto.SHA3.CreateFugue384(), null);
			Test(HashFactory.Crypto.SHA3.CreateFugue512(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_SHA3_Groestl()
		{
			Test(HashFactory.Crypto.SHA3.CreateGroestl224(), null);
			Test(HashFactory.Crypto.SHA3.CreateGroestl256(), null);
			Test(HashFactory.Crypto.SHA3.CreateGroestl384(), null);
			Test(HashFactory.Crypto.SHA3.CreateGroestl512(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_SHA3_Hamsi()
		{
			Test(HashFactory.Crypto.SHA3.CreateHamsi224(), null);
			Test(HashFactory.Crypto.SHA3.CreateHamsi256(), null);
			Test(HashFactory.Crypto.SHA3.CreateHamsi384(), null);
			Test(HashFactory.Crypto.SHA3.CreateHamsi512(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_SHA3_Keccak()
		{
			Test(HashFactory.Crypto.SHA3.CreateKeccak224(), null);
			Test(HashFactory.Crypto.SHA3.CreateKeccak256(), null);
			Test(HashFactory.Crypto.SHA3.CreateKeccak384(), null);
			Test(HashFactory.Crypto.SHA3.CreateKeccak512(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_SHA3_Luffa()
		{
			Test(HashFactory.Crypto.SHA3.CreateLuffa224(), null);
			Test(HashFactory.Crypto.SHA3.CreateLuffa256(), null);
			Test(HashFactory.Crypto.SHA3.CreateLuffa384(), null);
			Test(HashFactory.Crypto.SHA3.CreateLuffa512(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_SHA3_CubeHash()
		{
			Test(HashFactory.Crypto.SHA3.CreateCubeHash224(), null);
			Test(HashFactory.Crypto.SHA3.CreateCubeHash256(), null);
			Test(HashFactory.Crypto.SHA3.CreateCubeHash384(), null);
			Test(HashFactory.Crypto.SHA3.CreateCubeHash512(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_SHA3_Blake()
		{
			Test(HashFactory.Crypto.SHA3.CreateBlake224(), null);
			Test(HashFactory.Crypto.SHA3.CreateBlake256(), null);
			Test(HashFactory.Crypto.SHA3.CreateBlake384(), null);
			Test(HashFactory.Crypto.SHA3.CreateBlake512(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_SHA3_BlueMidnightWish()
		{
			Test(HashFactory.Crypto.SHA3.CreateBlueMidnightWish224(), null);
			Test(HashFactory.Crypto.SHA3.CreateBlueMidnightWish256(), null);
			Test(HashFactory.Crypto.SHA3.CreateBlueMidnightWish384(), null);
			Test(HashFactory.Crypto.SHA3.CreateBlueMidnightWish512(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_RIPEMD256()
		{
			Test(HashFactory.Crypto.CreateRIPEMD256(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_RIPEMD320()
		{
			Test(HashFactory.Crypto.CreateRIPEMD320(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_SHA1()
		{
			Test(HashFactory.Crypto.CreateSHA1(),
			     HashFactory.Crypto.BuildIn.CreateSHA1CryptoServiceProvider());
		}

		[TestMethod]
		public void HashLib_Crypto_BuildIn_SHA1Cng()
		{
			Test(HashFactory.Crypto.BuildIn.CreateSHA1Cng(),
			     HashFactory.Crypto.BuildIn.CreateSHA1CryptoServiceProvider());
		}

		[TestMethod]
		public void HashLib_Crypto_BuildIn_SHA1Managed()
		{
			Test(HashFactory.Crypto.BuildIn.CreateSHA1Managed(),
			     HashFactory.Crypto.BuildIn.CreateSHA1CryptoServiceProvider());
		}

		[TestMethod]
		public void HashLib_Crypto_BuildIn_SHA1CryptoServiceProvider()
		{
			Test(HashFactory.Crypto.BuildIn.CreateSHA1CryptoServiceProvider(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_BuildIn_SHA512Cng()
		{
			Test(HashFactory.Crypto.BuildIn.CreateSHA512Cng(),
			     HashFactory.Crypto.BuildIn.CreateSHA512CryptoServiceProvider());
		}

		[TestMethod]
		public void HashLib_Crypto_BuildIn_SHA512Managed()
		{
			Test(HashFactory.Crypto.BuildIn.CreateSHA512Managed(),
			     HashFactory.Crypto.BuildIn.CreateSHA512CryptoServiceProvider());
		}

		[TestMethod]
		public void HashLib_Crypto_BuildIn_SHA512CryptoServiceProvider()
		{
			Test(HashFactory.Crypto.BuildIn.CreateSHA512CryptoServiceProvider(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_BuildIn_SHA384Cng()
		{
			Test(HashFactory.Crypto.BuildIn.CreateSHA384Cng(),
			     HashFactory.Crypto.BuildIn.CreateSHA384CryptoServiceProvider());
		}

		[TestMethod]
		public void HashLib_Crypto_BuildIn_SHA384Managed()
		{
			Test(HashFactory.Crypto.BuildIn.CreateSHA384Managed(),
			     HashFactory.Crypto.BuildIn.CreateSHA384CryptoServiceProvider());
		}

		[TestMethod]
		public void HashLib_Crypto_BuildIn_SHA384CryptoServiceProvider()
		{
			Test(HashFactory.Crypto.BuildIn.CreateSHA384CryptoServiceProvider(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_BuildIn_SHA256Managed()
		{
			Test(HashFactory.Crypto.BuildIn.CreateSHA256Managed(),
			     HashFactory.Crypto.BuildIn.CreateSHA256CryptoServiceProvider());
		}

		[TestMethod]
		public void HashLib_Crypto_SHA512()
		{
			Test(HashFactory.Crypto.CreateSHA512(),
			     HashFactory.Crypto.BuildIn.CreateSHA512CryptoServiceProvider());
		}

		[TestMethod]
		public void HashLib_Crypto_SHA384()
		{
			Test(HashFactory.Crypto.CreateSHA384(),
			     HashFactory.Crypto.BuildIn.CreateSHA384CryptoServiceProvider());
		}

		[TestMethod]
		public void HashLib_Crypto_BuildIn_SHA256CryptoServiceProvider()
		{
			Test(HashFactory.Crypto.BuildIn.CreateSHA256CryptoServiceProvider(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_BuildIn_SHA256Cng()
		{
			Test(HashFactory.Crypto.BuildIn.CreateSHA256Cng(),
			     HashFactory.Crypto.BuildIn.CreateSHA256CryptoServiceProvider());
		}

		[TestMethod]
		public void HashLib_Crypto_SHA256()
		{
			Test(HashFactory.Crypto.CreateSHA256(),
			     HashFactory.Crypto.BuildIn.CreateSHA256CryptoServiceProvider());
		}

		[TestMethod]
		public void HashLib_Crypto_Whirlpool()
		{
			Test(HashFactory.Crypto.CreateWhirlpool(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_Gost()
		{
			Test(HashFactory.Crypto.CreateGost(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_Tiger()
		{
			Test(HashFactory.Crypto.CreateTiger_3_192(), null);
			Test(HashFactory.Crypto.CreateTiger_4_192(), null);
		}

		[TestMethod]
		public void HashLib_Crypto_Tiger2()
		{
			Test(HashFactory.Crypto.CreateTiger2(), null);
		}

		[TestMethod]
		public void HashLib_32_AP()
		{
			Test(HashFactory.Hash32.CreateAP(), null);
		}

		[TestMethod]
		public void HashLib_Checksum_Adler32()
		{
			Test(HashFactory.Checksum.CreateAdler32(), null);
		}

		[TestMethod]
		public void HashLib_32_Bernstein()
		{
			Test(HashFactory.Hash32.CreateBernstein(), null);
		}

		[TestMethod]
		public void HashLib_32_Bernstein1()
		{
			Test(HashFactory.Hash32.CreateBernstein1(), null);
		}

		[TestMethod]
		public void HashLib_32_BKDR()
		{
			Test(HashFactory.Hash32.CreateBKDR(), null);
		}

		[TestMethod]
		public void HashLib_Checksum_CRC32()
		{
			Test(HashFactory.Checksum.CreateCRC32a(), null);
			Test(HashFactory.Checksum.CreateCRC32b(), null);
			Test(HashFactory.Checksum.CreateCRC32c(), null);
			Test(HashFactory.Checksum.CreateCRC32d(), null);

			HashResult h1 = HashFactory.Checksum.CreateCRC32a().ComputeString("test");
			HashResult h2 = HashFactory.Checksum.CreateCRC32b().ComputeString("test");
			Assert.AreNotEqual(h1, h2);
		}

		[TestMethod]
		public void HashLib_32_DEK()
		{
			Test(HashFactory.Hash32.CreateDEK(), null);
		}

		[TestMethod]
		public void HashLib_32_DJB()
		{
			Test(HashFactory.Hash32.CreateDJB(), null);
		}

		[TestMethod]
		public void HashLib_32_ELF()
		{
			Test(HashFactory.Hash32.CreateELF(), null);
		}

		[TestMethod]
		public void HashLib_Checksum_FNV()
		{
			Test(HashFactory.Hash32.CreateFNV(), null);
		}

		[TestMethod]
		public void HashLib_32_FNV1a()
		{
			Test(HashFactory.Hash32.CreateFNV1a(), null);
		}

		[TestMethod]
		public void HashLib_32_Jenkins3()
		{
			Test(HashFactory.Hash32.CreateJenkins3(), null);
		}

		[TestMethod]
		public void HashLib_32_JS()
		{
			Test(HashFactory.Hash32.CreateJS(), null);
		}

		[TestMethod]
		public void HashLib_32_Murmur2()
		{
			IHash murmur2 = HashFactory.Hash32.CreateMurmur2();

			Test(murmur2, null);

			TestHashImplementationValueType(murmur2, murmur2);
		}

		[TestMethod]
		public void HashLib_32_OneAtTime()
		{
			Test(HashFactory.Hash32.CreateOneAtTime(), null);
		}

		[TestMethod]
		public void HashLib_32_PJW()
		{
			Test(HashFactory.Hash32.CreatePJW(), null);
		}

		[TestMethod]
		public void HashLib_32_Rotating()
		{
			Test(HashFactory.Hash32.CreateRotating(), null);
		}

		[TestMethod]
		public void HashLib_32_RS()
		{
			Test(HashFactory.Hash32.CreateRS(), null);
		}

		[TestMethod]
		public void HashLib_32_SDBM()
		{
			Test(HashFactory.Hash32.CreateSDBM(), null);
		}

		[TestMethod]
		public void HashLib_32_ShiftAndXor()
		{
			Test(HashFactory.Hash32.CreateShiftAndXor(), null);
		}

		[TestMethod]
		public void HashLib_32_SuperFast()
		{
			Test(HashFactory.Hash32.CreateSuperFast(), null);
		}

		[TestMethod]
		public void HashLib_64_FNV()
		{
			Test(HashFactory.Hash64.CreateFNV(), null);
		}

		[TestMethod]
		public void HashLib_64_FNV1a()
		{
			Test(HashFactory.Hash64.CreateFNV1a(), null);
		}

		[TestMethod]
		public void HashLib_Checksum_CRC64()
		{
			Test(HashFactory.Checksum.CreateCRC64a(), null);
			Test(HashFactory.Checksum.CreateCRC64b(), null);

			HashResult h1 = HashFactory.Checksum.CreateCRC64a().ComputeString("test");
			HashResult h2 = HashFactory.Checksum.CreateCRC64b().ComputeString("test");
			Assert.AreNotEqual(h1, h2);
		}

		[TestMethod]
		public void HashLib_64_Murmur2()
		{
			Test(HashFactory.Hash64.CreateMurmur2(), null);
		}

		[TestMethod]
		public void HashResult_Test()
		{
			for(int i = 0; i < 14; i++)
			{
				var h1 = new HashResult(m_random.NextBytes(i));

				try
				{
					uint h2 = h1.GetUInt();

					if(i != 4)
						Assert.Fail(i.ToString());

					Assert.IsTrue(Converters.ConvertBytesToUInts(h1.GetBytes())[0] == h2, i.ToString());
				}
				catch
				{
					if(i == 4)
						Assert.Fail(i.ToString());
				}

				try
				{
					ulong h3 = h1.GetULong();

					if(i != 8)
						Assert.Fail(i.ToString());

					Assert.IsTrue(Converters.ConvertBytesToULongs(h1.GetBytes())[0] == h3, i.ToString());
				}
				catch
				{
					if(i == 8)
						Assert.Fail(i.ToString());
				}
			}
		}

		[TestMethod]
		public void HashesTest_SelfTest()
		{
			{
				IHash dh1 = HashFactory.Crypto.CreateMD5();
				IHash dh2 = HashFactory.Crypto.CreateMD5();
				TestHashImplementationArrayType(dh1, dh2);
			}

			{
				IHash dh1 = HashFactory.Hash32.CreateMurmur2();
				IHash dh2 = HashFactory.Hash32.CreateMurmur2();
				TestHashImplementationValueType(dh1, dh2);
			}

			{
				IHash dh1 = HashFactory.Hash64.CreateMurmur2();
				IHash dh2 = HashFactory.Hash64.CreateMurmur2();
				TestHashImplementationValueType(dh1, dh2);
			}

			{
				IHash dh1 = HashFactory.Crypto.CreateMD5();
				IHash dh2 = HashFactory.Crypto.CreateMD5();
				TestAgainstBaseImplementationArrayType(dh1, dh2);
			}

			{
				IHash dh1 = HashFactory.Hash32.CreateMurmur2();
				IHash dh2 = HashFactory.Hash32.CreateMurmur2();
				TestAgainstBaseImplementationValueType(dh1, dh2);
			}

			{
				IHash dh1 = HashFactory.Hash64.CreateMurmur2();
				IHash dh2 = HashFactory.Hash64.CreateMurmur2();
				TestAgainstBaseImplementationValueType(dh1, dh2);
			}

			{
				IHash md5 = HashFactory.Crypto.CreateMD5();
				TestMultipleTransformsArrayType(md5, md5);
			}

			{
				IHash murmur2 = HashFactory.Hash32.CreateMurmur2();
				TestMultipleTransformsValueType(murmur2, murmur2);
			}

			{
				IHash murmur2 = HashFactory.Hash64.CreateMurmur2();
				TestMultipleTransformsValueType(murmur2, murmur2);
			}

			{
				IHash murmur2 = HashFactory.Hash32.CreateMurmur2();
				TestHashStreamValueType(murmur2, murmur2);
			}

			{
				IHash murmur2 = HashFactory.Hash64.CreateMurmur2();
				TestHashStreamValueType(murmur2, murmur2);
			}

			{
				IHash md5 = HashFactory.Crypto.CreateMD5();
				TestHashStreamArrayType(md5, md5);
			}

			{
				IHash murmur2 = HashFactory.Hash32.CreateMurmur2();
				TestHashFileValueType(murmur2, murmur2);
			}

			{
				IHash murmur2 = HashFactory.Hash64.CreateMurmur2();
				TestHashFileValueType(murmur2, murmur2);
			}

			{
				IHash md5 = HashFactory.Crypto.CreateMD5();
				TestHashFileArrayType(md5, md5);
			}
		}
	}
}