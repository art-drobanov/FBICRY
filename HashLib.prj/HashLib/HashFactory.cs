using System;
using System.Security.Cryptography;

using HashLib.Checksum;
using HashLib.Crypto;
using HashLib.Crypto.SHA3;
using HashLib.Hash32;
using HashLib.Hash64;

using MD5 = HashLib.Crypto.MD5;
using MD5CryptoServiceProvider = HashLib.Crypto.BuildIn.MD5CryptoServiceProvider;
using RIPEMD160 = HashLib.Crypto.RIPEMD160;
using RIPEMD160Managed = HashLib.Crypto.BuildIn.RIPEMD160Managed;
using SHA1 = HashLib.Crypto.SHA1;
using SHA1Cng = HashLib.Crypto.BuildIn.SHA1Cng;
using SHA1CryptoServiceProvider = HashLib.Crypto.BuildIn.SHA1CryptoServiceProvider;
using SHA1Managed = HashLib.Crypto.BuildIn.SHA1Managed;
using SHA256 = HashLib.Crypto.SHA256;
using SHA256Cng = HashLib.Crypto.BuildIn.SHA256Cng;
using SHA256CryptoServiceProvider = HashLib.Crypto.BuildIn.SHA256CryptoServiceProvider;
using SHA256Managed = HashLib.Crypto.BuildIn.SHA256Managed;
using SHA384 = HashLib.Crypto.SHA384;
using SHA384Cng = HashLib.Crypto.BuildIn.SHA384Cng;
using SHA384CryptoServiceProvider = HashLib.Crypto.BuildIn.SHA384CryptoServiceProvider;
using SHA384Managed = HashLib.Crypto.BuildIn.SHA384Managed;
using SHA512 = HashLib.Crypto.SHA512;
using SHA512Cng = HashLib.Crypto.BuildIn.SHA512Cng;
using SHA512CryptoServiceProvider = HashLib.Crypto.BuildIn.SHA512CryptoServiceProvider;
using SHA512Managed = HashLib.Crypto.BuildIn.SHA512Managed;

namespace HashLib
{
	public static class HashFactory
	{
		#region Nested type: Checksum

		public static class Checksum
		{
			/// <summary>
			/// IEEE 802.3, polynomial = 0xEDB88320
			/// </summary>
			/// <returns></returns>
			public static IHash CreateCRC32a()
			{
				return new CRC32a();
			}

			/// <summary>
			/// Castagnoli, polynomial = 0x82F63B78
			/// </summary>
			/// <returns></returns>
			public static IHash CreateCRC32b()
			{
				return new CRC32b();
			}

			/// <summary>
			/// Koopman, polynomial = 0xEB31D82E
			/// </summary>
			/// <returns></returns>
			public static IHash CreateCRC32c()
			{
				return new CRC32c();
			}


			/// <summary>
			/// Q, polynomial = 0xD5828281
			/// </summary>
			/// <returns></returns>
			public static IHash CreateCRC32d()
			{
				return new CRC32d();
			}

			public static IHash CreateCRC32(uint a_polynomial, uint a_initial_value = uint.MaxValue, uint a_final_xor = uint.MaxValue)
			{
				return new CRC32(a_polynomial, a_initial_value, a_final_xor);
			}

			public static IHash CreateAdler32()
			{
				return new Adler32();
			}

			/// <summary>
			/// ECMA 182, polynomial = 0xC96C5795D7870F42
			/// </summary>
			/// <returns></returns>
			public static IHash CreateCRC64a()
			{
				return new CRC64a();
			}

			/// <summary>
			/// ISO, polynomial = 0xD800000000000000
			/// </summary>
			/// <returns></returns>
			public static IHash CreateCRC64b()
			{
				return new CRC64b();
			}

			public static IHash CreateCRC64(ulong a_polynomial, ulong a_initial_value = ulong.MaxValue, ulong a_final_xor = ulong.MaxValue)
			{
				return new CRC64(a_polynomial, a_initial_value, a_final_xor);
			}
		}

		#endregion

		#region Nested type: Crypto

		public static class Crypto
		{
			public static IHash CreateGost()
			{
				return new Gost();
			}

			public static IHash CreateHAS160()
			{
				return new HAS160();
			}

			public static IHash CreateHaval_3_128()
			{
				return new Haval_3_128();
			}

			public static IHash CreateHaval_4_128()
			{
				return new Haval_4_128();
			}

			public static IHash CreateHaval_5_128()
			{
				return new Haval_5_128();
			}

			public static IHash CreateHaval_3_160()
			{
				return new Haval_3_160();
			}

			public static IHash CreateHaval_4_160()
			{
				return new Haval_4_160();
			}

			public static IHash CreateHaval_5_160()
			{
				return new Haval_5_160();
			}

			public static IHash CreateHaval_3_192()
			{
				return new Haval_3_192();
			}

			public static IHash CreateHaval_4_192()
			{
				return new Haval_4_192();
			}

			public static IHash CreateHaval_5_192()
			{
				return new Haval_5_192();
			}

			public static IHash CreateHaval_3_224()
			{
				return new Haval_3_224();
			}

			public static IHash CreateHaval_4_224()
			{
				return new Haval_4_224();
			}

			public static IHash CreateHaval_5_224()
			{
				return new Haval_5_224();
			}

			public static IHash CreateHaval_3_256()
			{
				return new Haval_3_256();
			}

			public static IHash CreateHaval_4_256()
			{
				return new Haval_4_256();
			}

			public static IHash CreateHaval_5_256()
			{
				return new Haval_5_256();
			}

			public static IHash CreateHaval(int a_rounds, HashSize a_hashSize)
			{
				switch(a_rounds)
				{
					case 3:

						switch(a_hashSize)
						{
							case HashSize.HashSize128:
								return CreateHaval_3_128();
							case HashSize.HashSize160:
								return CreateHaval_3_160();
							case HashSize.HashSize192:
								return CreateHaval_3_192();
							case HashSize.HashSize224:
								return CreateHaval_3_224();
							case HashSize.HashSize256:
								return CreateHaval_3_256();
							default:
								throw new ArgumentException();
						}

					case 4:

						switch(a_hashSize)
						{
							case HashSize.HashSize128:
								return CreateHaval_4_128();
							case HashSize.HashSize160:
								return CreateHaval_4_160();
							case HashSize.HashSize192:
								return CreateHaval_4_192();
							case HashSize.HashSize224:
								return CreateHaval_4_224();
							case HashSize.HashSize256:
								return CreateHaval_4_256();
							default:
								throw new ArgumentException();
						}

					case 5:

						switch(a_hashSize)
						{
							case HashSize.HashSize128:
								return CreateHaval_5_128();
							case HashSize.HashSize160:
								return CreateHaval_5_160();
							case HashSize.HashSize192:
								return CreateHaval_5_192();
							case HashSize.HashSize224:
								return CreateHaval_5_224();
							case HashSize.HashSize256:
								return CreateHaval_5_256();
							default:
								throw new ArgumentException();
						}

					default:
						throw new ArgumentException();
				}
			}

			public static IHash CreateMD2()
			{
				return new MD2();
			}

			public static IHash CreateMD4()
			{
				return new MD4();
			}

			public static IHash CreateMD5()
			{
				return new MD5();
			}

			public static IHash CreateRIPEMD128()
			{
				return new RIPEMD128();
			}

			public static IHash CreateRIPEMD160()
			{
				return new RIPEMD160();
			}

			public static IHash CreateRIPEMD256()
			{
				return new RIPEMD256();
			}

			public static IHash CreateRIPEMD320()
			{
				return new RIPEMD320();
			}

			public static IHash CreateSHA1()
			{
				return new SHA1();
			}

			public static IHash CreateSHA224()
			{
				return new SHA224();
			}

			public static IHash CreateSHA256()
			{
				return new SHA256();
			}

			public static IHash CreateSHA384()
			{
				return new SHA384();
			}

			public static IHash CreateSHA512()
			{
				return new SHA512();
			}

			public static IHash CreateSnefru_4_128()
			{
				return new Snefru_4_128();
			}

			public static IHash CreateSnefru_4_256()
			{
				return new Snefru_4_256();
			}

			public static IHash CreateSnefru_8_128()
			{
				return new Snefru_8_128();
			}

			public static IHash CreateSnefru_8_256()
			{
				return new Snefru_8_256();
			}

			public static IHash CreateSnefru(int a_rounds, HashSize a_hashSize)
			{
				switch(a_rounds)
				{
					case 4:

						switch(a_hashSize)
						{
							case HashSize.HashSize128:
								return CreateSnefru_4_128();
							case HashSize.HashSize256:
								return CreateSnefru_4_256();
							default:
								throw new ArgumentException();
						}

					case 8:

						switch(a_hashSize)
						{
							case HashSize.HashSize128:
								return CreateSnefru_8_128();
							case HashSize.HashSize256:
								return CreateSnefru_8_256();
							default:
								throw new ArgumentException();
						}

					default:
						throw new ArgumentException();
				}
			}

			public static IHash CreateTiger_3_192()
			{
				return new Tiger_3_192();
			}

			public static IHash CreateTiger_4_192()
			{
				return new Tiger_4_192();
			}

			public static IHash CreateTiger(int a_rounds)
			{
				switch(a_rounds)
				{
					case 3:
						return CreateTiger_3_192();
					case 4:
						return CreateTiger_4_192();
					default:
						throw new ArgumentException();
				}
			}

			public static IHash CreateTiger2()
			{
				return new Tiger2();
			}

			public static IHash CreateWhirlpool()
			{
				return new Whirlpool();
			}

			#region Nested type: BuildIn

			public static class BuildIn
			{
				public static IHash CreateMD5CryptoServiceProvider()
				{
					return new MD5CryptoServiceProvider();
				}

				public static IHash CreateRIPEMD160Managed()
				{
					return new RIPEMD160Managed();
				}

				public static IHash CreateSHA1Cng()
				{
					return new SHA1Cng();
				}

				public static IHash CreateSHA1CryptoServiceProvider()
				{
					return new SHA1CryptoServiceProvider();
				}

				public static IHash CreateSHA1Managed()
				{
					return new SHA1Managed();
				}

				public static IHash CreateSHA256Cng()
				{
					return new SHA256Cng();
				}

				public static IHash CreateSHA256CryptoServiceProvider()
				{
					return new SHA256CryptoServiceProvider();
				}

				public static IHash CreateSHA256Managed()
				{
					return new SHA256Managed();
				}

				public static IHash CreateSHA384Cng()
				{
					return new SHA384Cng();
				}

				public static IHash CreateSHA384CryptoServiceProvider()
				{
					return new SHA384CryptoServiceProvider();
				}

				public static IHash CreateSHA384Managed()
				{
					return new SHA384Managed();
				}

				public static IHash CreateSHA512Cng()
				{
					return new SHA512Cng();
				}

				public static IHash CreateSHA512CryptoServiceProvider()
				{
					return new SHA512CryptoServiceProvider();
				}

				public static IHash CreateSHA512Managed()
				{
					return new SHA512Managed();
				}
			}

			#endregion

			#region Nested type: SHA3

			public static class SHA3
			{
				public static IHash CreateJH224()
				{
					return new JH224();
				}

				public static IHash CreateJH256()
				{
					return new JH256();
				}

				public static IHash CreateJH384()
				{
					return new JH384();
				}

				public static IHash CreateJH512()
				{
					return new JH512();
				}

				public static IHash CreateJH(HashSize a_hashSize)
				{
					switch(a_hashSize)
					{
						case HashSize.HashSize224:
							return CreateJH224();
						case HashSize.HashSize256:
							return CreateJH256();
						case HashSize.HashSize384:
							return CreateJH384();
						case HashSize.HashSize512:
							return CreateJH512();
						default:
							throw new ArgumentException();
					}
				}

				public static IHash CreateBlake224()
				{
					return new Blake224();
				}

				public static IHash CreateBlake256()
				{
					return new Blake256();
				}

				public static IHash CreateBlake384()
				{
					return new Blake384();
				}

				public static IHash CreateBlake512()
				{
					return new Blake512();
				}

				public static IHash CreateBlake(HashSize a_hashSize)
				{
					switch(a_hashSize)
					{
						case HashSize.HashSize224:
							return CreateBlake224();
						case HashSize.HashSize256:
							return CreateBlake256();
						case HashSize.HashSize384:
							return CreateBlake384();
						case HashSize.HashSize512:
							return CreateBlake512();
						default:
							throw new ArgumentException();
					}
				}

				public static IHash CreateBlueMidnightWish224()
				{
					return new BlueMidnightWish224();
				}

				public static IHash CreateBlueMidnightWish256()
				{
					return new BlueMidnightWish256();
				}

				public static IHash CreateBlueMidnightWish384()
				{
					return new BlueMidnightWish384();
				}

				public static IHash CreateBlueMidnightWish512()
				{
					return new BlueMidnightWish512();
				}

				public static IHash CreateBlueMidnightWish(HashSize a_hashSize)
				{
					switch(a_hashSize)
					{
						case HashSize.HashSize224:
							return CreateBlueMidnightWish224();
						case HashSize.HashSize256:
							return CreateBlueMidnightWish256();
						case HashSize.HashSize384:
							return CreateBlueMidnightWish384();
						case HashSize.HashSize512:
							return CreateBlueMidnightWish512();
						default:
							throw new ArgumentException();
					}
				}

				public static IHash CreateCubeHash224()
				{
					return new CubeHash224();
				}

				public static IHash CreateCubeHash256()
				{
					return new CubeHash256();
				}

				public static IHash CreateCubeHash384()
				{
					return new CubeHash384();
				}

				public static IHash CreateCubeHash512()
				{
					return new CubeHash512();
				}

				public static IHash CreateCubeHash(HashSize a_hashSize, int a_rounds = 16, int a_blockSize = 32)
				{
					if(a_blockSize < 1)
						throw new ArgumentOutOfRangeException();
					if(a_blockSize > 128)
						throw new ArgumentOutOfRangeException();
					if(a_rounds < 1)
						throw new ArgumentOutOfRangeException();

					if((a_rounds == 16) && (a_blockSize == 32))
					{
						switch(a_hashSize)
						{
							case HashSize.HashSize224:
								return CreateCubeHash224();
							case HashSize.HashSize256:
								return CreateCubeHash256();
							case HashSize.HashSize384:
								return CreateCubeHash384();
							case HashSize.HashSize512:
								return CreateCubeHash512();
							default:
								throw new ArgumentException();
						}
					}
					else
					{
						switch(a_hashSize)
						{
							case HashSize.HashSize224:
								return new CubeHashCustom(a_hashSize, a_rounds, a_blockSize);
							case HashSize.HashSize256:
								return new CubeHashCustom(a_hashSize, a_rounds, a_blockSize);
							case HashSize.HashSize384:
								return new CubeHashCustom(a_hashSize, a_rounds, a_blockSize);
							case HashSize.HashSize512:
								return new CubeHashCustom(a_hashSize, a_rounds, a_blockSize);
							default:
								throw new ArgumentException();
						}
					}
				}

				public static IHash CreateEcho224()
				{
					return new Echo224();
				}

				public static IHash CreateEcho256()
				{
					return new Echo256();
				}

				public static IHash CreateEcho384()
				{
					return new Echo384();
				}

				public static IHash CreateEcho512()
				{
					return new Echo512();
				}

				public static IHash CreateEcho(HashSize a_hashSize)
				{
					switch(a_hashSize)
					{
						case HashSize.HashSize224:
							return CreateEcho224();
						case HashSize.HashSize256:
							return CreateEcho256();
						case HashSize.HashSize384:
							return CreateEcho384();
						case HashSize.HashSize512:
							return CreateEcho512();
						default:
							throw new ArgumentException();
					}
				}

				public static IHash CreateFugue224()
				{
					return new Fugue224();
				}

				public static IHash CreateFugue256()
				{
					return new Fugue256();
				}

				public static IHash CreateFugue384()
				{
					return new Fugue384();
				}

				public static IHash CreateFugue512()
				{
					return new Fugue512();
				}

				public static IHash CreateFugue(HashSize a_hashSize)
				{
					switch(a_hashSize)
					{
						case HashSize.HashSize224:
							return CreateFugue224();
						case HashSize.HashSize256:
							return CreateFugue256();
						case HashSize.HashSize384:
							return CreateFugue384();
						case HashSize.HashSize512:
							return CreateFugue512();
						default:
							throw new ArgumentException();
					}
				}

				public static IHash CreateGroestl224()
				{
					return new Groestl224();
				}

				public static IHash CreateGroestl256()
				{
					return new Groestl256();
				}

				public static IHash CreateGroestl384()
				{
					return new Groestl384();
				}

				public static IHash CreateGroestl512()
				{
					return new Groestl512();
				}

				public static IHash CreateGroestl(HashSize a_hashSize)
				{
					switch(a_hashSize)
					{
						case HashSize.HashSize224:
							return CreateGroestl224();
						case HashSize.HashSize256:
							return CreateGroestl256();
						case HashSize.HashSize384:
							return CreateGroestl384();
						case HashSize.HashSize512:
							return CreateGroestl512();
						default:
							throw new ArgumentException();
					}
				}

				public static IHash CreateHamsi224()
				{
					return new Hamsi224();
				}

				public static IHash CreateHamsi256()
				{
					return new Hamsi256();
				}

				public static IHash CreateHamsi384()
				{
					return new Hamsi384();
				}

				public static IHash CreateHamsi512()
				{
					return new Hamsi512();
				}

				public static IHash CreateHamsi(HashSize a_hashSize)
				{
					switch(a_hashSize)
					{
						case HashSize.HashSize224:
							return CreateHamsi224();
						case HashSize.HashSize256:
							return CreateHamsi256();
						case HashSize.HashSize384:
							return CreateHamsi384();
						case HashSize.HashSize512:
							return CreateHamsi512();
						default:
							throw new ArgumentException();
					}
				}

				public static IHash CreateKeccak224()
				{
					return new Keccak224();
				}

				public static IHash CreateKeccak256()
				{
					return new Keccak256();
				}

				public static IHash CreateKeccak384()
				{
					return new Keccak384();
				}

				public static IHash CreateKeccak512()
				{
					return new Keccak512();
				}

				public static IHash CreateKeccak(HashSize a_hashSize)
				{
					switch(a_hashSize)
					{
						case HashSize.HashSize224:
							return CreateKeccak224();
						case HashSize.HashSize256:
							return CreateKeccak256();
						case HashSize.HashSize384:
							return CreateKeccak384();
						case HashSize.HashSize512:
							return CreateKeccak512();
						default:
							throw new ArgumentException();
					}
				}

				public static IHash CreateLuffa224()
				{
					return new Luffa224();
				}

				public static IHash CreateLuffa256()
				{
					return new Luffa256();
				}

				public static IHash CreateLuffa384()
				{
					return new Luffa384();
				}

				public static IHash CreateLuffa512()
				{
					return new Luffa512();
				}

				public static IHash CreateLuffa(HashSize a_hashSize)
				{
					switch(a_hashSize)
					{
						case HashSize.HashSize224:
							return CreateLuffa224();
						case HashSize.HashSize256:
							return CreateLuffa256();
						case HashSize.HashSize384:
							return CreateLuffa384();
						case HashSize.HashSize512:
							return CreateLuffa512();
						default:
							throw new ArgumentException();
					}
				}
			}

			#endregion
		}

		#endregion

		#region Nested type: HMAC

		public static class HMAC
		{
			public static IHMAC CreateHMAC(IHash a_hash)
			{
				if(a_hash is IHMAC)
					return (IHMAC)a_hash;
				else if(a_hash is IHasHMACBuildIn)
				{
					var h = (HashCryptoBuildIn)a_hash;
					return new HMACBuildInAdapter(h.GetBuildHMAC(), h.BlockSize);
				}

				return new HMACNotBuildInAdapter(a_hash);
			}
		}

		#endregion

		#region Nested type: Hash32

		public static class Hash32
		{
			public static IHash CreateAP()
			{
				return new AP();
			}

			public static IHash CreateBernstein()
			{
				return new Bernstein();
			}

			public static IHash CreateBernstein1()
			{
				return new Bernstein1();
			}

			public static IHash CreateBKDR()
			{
				return new BKDR();
			}

			public static IHash CreateDEK()
			{
				return new DEK();
			}

			public static IHash CreateDJB()
			{
				return new DJB();
			}

			public static IHash CreateDotNet()
			{
				return new DotNet();
			}

			public static IHash CreateELF()
			{
				return new ELF();
			}

			public static IHash CreateFNV()
			{
				return new FNV();
			}

			public static IHash CreateFNV1a()
			{
				return new FNV1a();
			}

			public static IHash CreateJenkins3()
			{
				return new Jenkins3();
			}

			public static IHash CreateJS()
			{
				return new JS();
			}

			public static IHash CreateMurmur2()
			{
				return new Murmur2();
			}

			public static IHash CreateOneAtTime()
			{
				return new OneAtTime();
			}

			public static IHash CreatePJW()
			{
				return new PJW();
			}

			public static IHash CreateRotating()
			{
				return new Rotating();
			}

			public static IHash CreateRS()
			{
				return new RS();
			}

			public static IHash CreateSDBM()
			{
				return new SDBM();
			}

			public static IHash CreateShiftAndXor()
			{
				return new ShiftAndXor();
			}

			public static IHash CreateSuperFast()
			{
				return new SuperFast();
			}
		}

		#endregion

		#region Nested type: Hash64

		public static class Hash64
		{
			public static IHash CreateFNV1a()
			{
				return new FNV1a64();
			}

			public static IHash CreateFNV()
			{
				return new FNV64();
			}

			public static IHash CreateMurmur2()
			{
				return new Murmur2_64();
			}
		}

		#endregion

		#region Nested type: Wrappers

		public static class Wrappers
		{
			public static HashAlgorithm HashToHashAlgorithm(IHash a_hash)
			{
				return new HashAlgorithmWrapper(a_hash);
			}

			public static IHash HashAlgorithmToHash(HashAlgorithm a_hash, int a_blockSize = -1)
			{
				return new HashCryptoBuildIn(a_hash, a_blockSize);
			}
		}

		#endregion
	}
}