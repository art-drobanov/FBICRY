using System;
using System.IO;
using System.Linq;

using HashLib;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HashLibTest
{
	[TestClass]
	public class AdaptersTest
	{
		[TestMethod]
		public void AdaptersTest_All()
		{
			{
				IHash md5 = HashFactory.Crypto.CreateMD5();

				for(int i = 0; i < 5; i++)
				{
					var v = new byte[16];
					new Random().NextBytes(v);

					Assert.IsFalse(!md5.ComputeBytes(v.Take(1).ToArray()).GetBytes().SequenceEqual(md5.ComputeByte(v[0]).GetBytes()));

					Assert.IsFalse(!md5.ComputeBytes(v.Take(2).ToArray()).GetBytes().SequenceEqual(md5.ComputeChar(BitConverter.ToChar(v, 0)).GetBytes()));

					Assert.IsFalse(!md5.ComputeBytes(v.Take(2).ToArray()).GetBytes().SequenceEqual(md5.ComputeShort(BitConverter.ToInt16(v, 0)).GetBytes()));

					Assert.IsFalse(!md5.ComputeBytes(v.Take(2).ToArray()).GetBytes().SequenceEqual(md5.ComputeUShort(BitConverter.ToUInt16(v, 0)).GetBytes()));

					Assert.IsFalse(!md5.ComputeBytes(v.Take(4).ToArray()).GetBytes().SequenceEqual(md5.ComputeInt(BitConverter.ToInt32(v, 0)).GetBytes()));

					Assert.IsFalse(!md5.ComputeBytes(v.Take(4).ToArray()).GetBytes().SequenceEqual(md5.ComputeUInt(BitConverter.ToUInt32(v, 0)).GetBytes()));

					Assert.IsFalse(!md5.ComputeBytes(v.Take(8).ToArray()).GetBytes().SequenceEqual(md5.ComputeLong(BitConverter.ToInt64(v, 0)).GetBytes()));

					Assert.IsFalse(!md5.ComputeBytes(v.Take(8).ToArray()).GetBytes().SequenceEqual(md5.ComputeULong(BitConverter.ToUInt64(v, 0)).GetBytes()));

					Assert.IsFalse(!md5.ComputeBytes(v.Take(8).ToArray()).GetBytes().SequenceEqual(md5.ComputeDouble(BitConverter.ToDouble(v, 0)).GetBytes()));

					Assert.IsFalse(!md5.ComputeBytes(v.Take(4).ToArray()).GetBytes().SequenceEqual(md5.ComputeFloat(Converters.ConvertBytesToFloat(v, 0)).GetBytes()));

					{
						string s = BitConverter.ToString(v);
						Assert.IsFalse(!md5.ComputeBytes(Converters.ConvertStringToBytes(s)).GetBytes().SequenceEqual(md5.ComputeString(s).GetBytes()));
					}

					Assert.IsFalse(!md5.ComputeBytes(v).GetBytes().SequenceEqual(md5.ComputeChars(Converters.ConvertBytesToChars(v)).GetBytes()));

					Assert.IsFalse(!md5.ComputeBytes(v).GetBytes().SequenceEqual(md5.ComputeShorts(Converters.ConvertBytesToShorts(v)).GetBytes()));

					Assert.IsFalse(!md5.ComputeBytes(v).GetBytes().SequenceEqual(md5.ComputeUShorts(Converters.ConvertBytesToUShorts(v)).GetBytes()));

					Assert.IsFalse(!md5.ComputeBytes(v).GetBytes().SequenceEqual(md5.ComputeInts(Converters.ConvertBytesToInts(v)).GetBytes()));

					Assert.IsFalse(!md5.ComputeBytes(v).GetBytes().SequenceEqual(md5.ComputeUInts(Converters.ConvertBytesToUInts(v)).GetBytes()));

					Assert.IsFalse(!md5.ComputeBytes(v).GetBytes().SequenceEqual(md5.ComputeLongs(Converters.ConvertBytesToLongs(v)).GetBytes()));

					Assert.IsFalse(!md5.ComputeBytes(v).GetBytes().SequenceEqual(md5.ComputeULongs(Converters.ConvertBytesToULongs(v)).GetBytes()));

					Assert.IsFalse(!md5.ComputeBytes(v).GetBytes().SequenceEqual(md5.ComputeFloats(Converters.ConvertBytesToFloats(v)).GetBytes()));

					Assert.IsFalse(!md5.ComputeBytes(v).GetBytes().SequenceEqual(md5.ComputeDoubles(Converters.ConvertBytesToDoubles(v)).GetBytes()));
				}
			}

			{
				IHash murmur2 = HashFactory.Hash32.CreateMurmur2();

				for(int i = 0; i < 5; i++)
				{
					var v = new byte[16];
					new Random().NextBytes(v);

					Assert.IsFalse(murmur2.ComputeBytes(v.Take(1).ToArray()) != murmur2.ComputeByte(v[0]));

					Assert.IsFalse(murmur2.ComputeBytes(v.Take(2).ToArray()) != murmur2.ComputeChar(BitConverter.ToChar(v, 0)));

					Assert.IsFalse(murmur2.ComputeBytes(v.Take(2).ToArray()) != murmur2.ComputeShort(BitConverter.ToInt16(v, 0)));

					Assert.IsFalse(murmur2.ComputeBytes(v.Take(2).ToArray()) != murmur2.ComputeUShort(BitConverter.ToUInt16(v, 0)));

					Assert.IsFalse(murmur2.ComputeBytes(v.Take(4).ToArray()) != murmur2.ComputeInt(BitConverter.ToInt32(v, 0)));

					Assert.IsFalse(murmur2.ComputeBytes(v.Take(4).ToArray()) != murmur2.ComputeUInt(BitConverter.ToUInt32(v, 0)));

					Assert.IsFalse(murmur2.ComputeBytes(v.Take(8).ToArray()) != murmur2.ComputeLong(BitConverter.ToInt64(v, 0)));

					Assert.IsFalse(murmur2.ComputeBytes(v.Take(8).ToArray()) != murmur2.ComputeULong(BitConverter.ToUInt64(v, 0)));

					Assert.IsFalse(murmur2.ComputeBytes(v.Take(8).ToArray()) != murmur2.ComputeDouble(BitConverter.ToDouble(v, 0)));

					Assert.IsFalse(murmur2.ComputeBytes(v.Take(4).ToArray()) != murmur2.ComputeFloat(Converters.ConvertBytesToFloat(v, 0)));

					{
						string s = BitConverter.ToString(v);
						Assert.IsFalse(murmur2.ComputeBytes(Converters.ConvertStringToBytes(s)) != murmur2.ComputeString(s));
					}

					Assert.IsFalse(murmur2.ComputeBytes(v) != murmur2.ComputeChars(Converters.ConvertBytesToChars(v)));

					Assert.IsFalse(murmur2.ComputeBytes(v) != murmur2.ComputeShorts(Converters.ConvertBytesToShorts(v)));

					Assert.IsFalse(murmur2.ComputeBytes(v) != murmur2.ComputeUShorts(Converters.ConvertBytesToUShorts(v)));

					Assert.IsFalse(murmur2.ComputeBytes(v) != murmur2.ComputeInts(Converters.ConvertBytesToInts(v)));

					Assert.IsFalse(murmur2.ComputeBytes(v) != murmur2.ComputeUInts(Converters.ConvertBytesToUInts(v)));

					Assert.IsFalse(murmur2.ComputeBytes(v) != murmur2.ComputeLongs(Converters.ConvertBytesToLongs(v)));

					Assert.IsFalse(murmur2.ComputeBytes(v) != murmur2.ComputeULongs(Converters.ConvertBytesToULongs(v)));

					Assert.IsFalse(murmur2.ComputeBytes(v) != murmur2.ComputeFloats(Converters.ConvertBytesToFloats(v)));

					Assert.IsFalse(murmur2.ComputeBytes(v) != murmur2.ComputeDoubles(Converters.ConvertBytesToDoubles(v)));
				}
			}

			{
				IHash murmur2 = HashFactory.Hash64.CreateMurmur2();

				for(int i = 0; i < 5; i++)
				{
					var v = new byte[16];
					new Random().NextBytes(v);

					Assert.IsFalse(murmur2.ComputeBytes(v.Take(1).ToArray()) != murmur2.ComputeByte(v[0]));

					Assert.IsFalse(murmur2.ComputeBytes(v.Take(2).ToArray()) != murmur2.ComputeChar(BitConverter.ToChar(v, 0)));

					Assert.IsFalse(murmur2.ComputeBytes(v.Take(2).ToArray()) != murmur2.ComputeShort(BitConverter.ToInt16(v, 0)));

					Assert.IsFalse(murmur2.ComputeBytes(v.Take(2).ToArray()) != murmur2.ComputeUShort(BitConverter.ToUInt16(v, 0)));

					Assert.IsFalse(murmur2.ComputeBytes(v.Take(4).ToArray()) != murmur2.ComputeInt(BitConverter.ToInt32(v, 0)));

					Assert.IsFalse(murmur2.ComputeBytes(v.Take(4).ToArray()) != murmur2.ComputeUInt(BitConverter.ToUInt32(v, 0)));

					Assert.IsFalse(murmur2.ComputeBytes(v.Take(8).ToArray()) != murmur2.ComputeLong(BitConverter.ToInt64(v, 0)));

					Assert.IsFalse(murmur2.ComputeBytes(v.Take(8).ToArray()) != murmur2.ComputeULong(BitConverter.ToUInt64(v, 0)));

					Assert.IsFalse(murmur2.ComputeBytes(v.Take(8).ToArray()) != murmur2.ComputeDouble(BitConverter.ToDouble(v, 0)));

					Assert.IsFalse(murmur2.ComputeBytes(v.Take(4).ToArray()) != murmur2.ComputeFloat(Converters.ConvertBytesToFloat(v, 0)));

					{
						string s = BitConverter.ToString(v);
						Assert.IsFalse(murmur2.ComputeBytes(Converters.ConvertStringToBytes(s)) != murmur2.ComputeString(s));
					}

					Assert.IsFalse(murmur2.ComputeBytes(v) != murmur2.ComputeChars(Converters.ConvertBytesToChars(v)));

					Assert.IsFalse(murmur2.ComputeBytes(v) != murmur2.ComputeShorts(Converters.ConvertBytesToShorts(v)));

					Assert.IsFalse(murmur2.ComputeBytes(v) != murmur2.ComputeUShorts(Converters.ConvertBytesToUShorts(v)));

					Assert.IsFalse(murmur2.ComputeBytes(v) != murmur2.ComputeInts(Converters.ConvertBytesToInts(v)));

					Assert.IsFalse(murmur2.ComputeBytes(v) != murmur2.ComputeUInts(Converters.ConvertBytesToUInts(v)));

					Assert.IsFalse(murmur2.ComputeBytes(v) != murmur2.ComputeLongs(Converters.ConvertBytesToLongs(v)));

					Assert.IsFalse(murmur2.ComputeBytes(v) != murmur2.ComputeULongs(Converters.ConvertBytesToULongs(v)));

					Assert.IsFalse(murmur2.ComputeBytes(v) != murmur2.ComputeFloats(Converters.ConvertBytesToFloats(v)));

					Assert.IsFalse(murmur2.ComputeBytes(v) != murmur2.ComputeDoubles(Converters.ConvertBytesToDoubles(v)));
				}
			}

			{
				// Adapters.TransformObjectHash() test

				IHash md5 = HashFactory.Crypto.CreateMD5();

				var ms1 = new MersenneTwister(5566);
				var ms2 = new MersenneTwister(5566);

				Assert.AreEqual(md5.ComputeByte(ms1.NextByte()), md5.ComputeObject(ms2.NextByte()));
				Assert.AreEqual(md5.ComputeShort(ms1.NextShort()), md5.ComputeObject(ms2.NextShort()));
				Assert.AreEqual(md5.ComputeUShort(ms1.NextUShort()), md5.ComputeObject(ms2.NextUShort()));
				Assert.AreEqual(md5.ComputeChar(ms1.NextChar()), md5.ComputeObject(ms2.NextChar()));
				Assert.AreEqual(md5.ComputeInt(ms1.NextInt()), md5.ComputeObject(ms2.NextInt()));
				Assert.AreEqual(md5.ComputeUInt(ms1.NextUInt()), md5.ComputeObject(ms2.NextUInt()));
				Assert.AreEqual(md5.ComputeLong(ms1.NextLong()), md5.ComputeObject(ms2.NextLong()));
				Assert.AreEqual(md5.ComputeULong(ms1.NextULong()), md5.ComputeObject(ms2.NextULong()));
				Assert.AreEqual(md5.ComputeDouble(ms1.NextDoubleFull()), md5.ComputeObject(ms2.NextDoubleFull()));
				Assert.AreEqual(md5.ComputeFloat(ms1.NextFloatFull()), md5.ComputeObject(ms2.NextFloatFull()));

				Assert.AreEqual(md5.ComputeString(ms1.NextString(20)), md5.ComputeObject(ms2.NextString(20)));

				Assert.AreEqual(md5.ComputeBytes(ms1.NextBytes(20)), md5.ComputeObject(ms2.NextBytes(20)));
				Assert.AreEqual(md5.ComputeShorts(ms1.NextShorts(20)), md5.ComputeObject(ms2.NextShorts(20)));
				Assert.AreEqual(md5.ComputeUShorts(ms1.NextUShorts(20)), md5.ComputeObject(ms2.NextUShorts(20)));
				Assert.AreEqual(md5.ComputeChars(ms1.NextChars(20)), md5.ComputeObject(ms2.NextChars(20)));
				Assert.AreEqual(md5.ComputeInts(ms1.NextInts(20)), md5.ComputeObject(ms2.NextInts(20)));
				Assert.AreEqual(md5.ComputeUInts(ms1.NextUInts(20)), md5.ComputeObject(ms2.NextUInts(20)));
				Assert.AreEqual(md5.ComputeLongs(ms1.NextLongs(20)), md5.ComputeObject(ms2.NextLongs(20)));
				Assert.AreEqual(md5.ComputeULongs(ms1.NextULongs(20)), md5.ComputeObject(ms2.NextULongs(20)));
				Assert.AreEqual(md5.ComputeDoubles(ms1.NextDoublesFull(20)), md5.ComputeObject(ms2.NextDoublesFull(20)));
				Assert.AreEqual(md5.ComputeFloats(ms1.NextFloatsFull(20)), md5.ComputeObject(ms2.NextFloatsFull(20)));
			}

			{
				// Adapters.ComputeObjectHash() test

				IHash murmur2 = HashFactory.Hash32.CreateMurmur2();

				var ms1 = new MersenneTwister(5566);
				var ms2 = new MersenneTwister(5566);

				Assert.AreEqual(murmur2.ComputeByte(ms1.NextByte()), murmur2.ComputeObject(ms2.NextByte()));
				Assert.AreEqual(murmur2.ComputeShort(ms1.NextShort()), murmur2.ComputeObject(ms2.NextShort()));
				Assert.AreEqual(murmur2.ComputeUShort(ms1.NextUShort()), murmur2.ComputeObject(ms2.NextUShort()));
				Assert.AreEqual(murmur2.ComputeChar(ms1.NextChar()), murmur2.ComputeObject(ms2.NextChar()));
				Assert.AreEqual(murmur2.ComputeInt(ms1.NextInt()), murmur2.ComputeObject(ms2.NextInt()));
				Assert.AreEqual(murmur2.ComputeUInt(ms1.NextUInt()), murmur2.ComputeObject(ms2.NextUInt()));
				Assert.AreEqual(murmur2.ComputeLong(ms1.NextLong()), murmur2.ComputeObject(ms2.NextLong()));
				Assert.AreEqual(murmur2.ComputeULong(ms1.NextULong()), murmur2.ComputeObject(ms2.NextULong()));
				Assert.AreEqual(murmur2.ComputeDouble(ms1.NextDoubleFull()), murmur2.ComputeObject(ms2.NextDoubleFull()));
				Assert.AreEqual(murmur2.ComputeFloat(ms1.NextFloatFull()), murmur2.ComputeObject(ms2.NextFloatFull()));

				Assert.AreEqual(murmur2.ComputeString(ms1.NextString(20)), murmur2.ComputeObject(ms2.NextString(20)));

				Assert.AreEqual(murmur2.ComputeBytes(ms1.NextBytes(20)), murmur2.ComputeObject(ms2.NextBytes(20)));
				Assert.AreEqual(murmur2.ComputeShorts(ms1.NextShorts(20)), murmur2.ComputeObject(ms2.NextShorts(20)));
				Assert.AreEqual(murmur2.ComputeUShorts(ms1.NextUShorts(20)), murmur2.ComputeObject(ms2.NextUShorts(20)));
				Assert.AreEqual(murmur2.ComputeChars(ms1.NextChars(20)), murmur2.ComputeObject(ms2.NextChars(20)));
				Assert.AreEqual(murmur2.ComputeInts(ms1.NextInts(20)), murmur2.ComputeObject(ms2.NextInts(20)));
				Assert.AreEqual(murmur2.ComputeUInts(ms1.NextUInts(20)), murmur2.ComputeObject(ms2.NextUInts(20)));
				Assert.AreEqual(murmur2.ComputeLongs(ms1.NextLongs(20)), murmur2.ComputeObject(ms2.NextLongs(20)));
				Assert.AreEqual(murmur2.ComputeULongs(ms1.NextULongs(20)), murmur2.ComputeObject(ms2.NextULongs(20)));
				Assert.AreEqual(murmur2.ComputeDoubles(ms1.NextDoublesFull(20)), murmur2.ComputeObject(ms2.NextDoublesFull(20)));
				Assert.AreEqual(murmur2.ComputeFloats(ms1.NextFloatsFull(20)), murmur2.ComputeObject(ms2.NextFloatsFull(20)));
			}

			int old = Hash.BUFFER_SIZE;
			Hash.BUFFER_SIZE = 26;

			{
				IHash md5 = HashFactory.Crypto.CreateMD5();

				for(int i = 0; i < 10; i++)
				{
					var v = new byte[13 * i];
					new Random().NextBytes(v);

					using(var ms = new MemoryStream(v))
					{
						HashResult h1 = md5.ComputeBytes(v);
						HashResult h2 = md5.ComputeStream(ms);

						Assert.AreEqual(h1, h2);

						h1 = md5.ComputeBytes(v.SubArray(i, i * 7));
						ms.Seek(i, SeekOrigin.Begin);
						h2 = md5.ComputeStream(ms, i * 7);

						Assert.AreEqual(h1, h2);

						h1 = md5.ComputeBytes(v);
						md5.Initialize();
						md5.TransformBytes(v, 0, i * 3);
						ms.Seek(i * 3, SeekOrigin.Begin);
						md5.TransformStream(ms, i * 2);
						md5.TransformBytes(v, i * 5);
						h2 = md5.TransformFinal();

						Assert.AreEqual(h1, h2);
					}
				}
			}

			{
				IHash murmur2 = HashFactory.Hash32.CreateMurmur2();

				for(int i = 0; i < 10; i++)
				{
					var v = new byte[13 * i];
					new Random().NextBytes(v);

					using(var ms = new MemoryStream(v))
					{
						HashResult h1 = murmur2.ComputeBytes(v);
						HashResult h2 = murmur2.ComputeStream(ms);

						Assert.AreEqual(h1, h2);

						h1 = murmur2.ComputeBytes(v.SubArray(i, i * 7));
						ms.Seek(i, SeekOrigin.Begin);
						h2 = murmur2.ComputeStream(ms, i * 7);

						Assert.AreEqual(h1, h2);

						h1 = murmur2.ComputeBytes(v);
						murmur2.Initialize();
						murmur2.TransformBytes(v, 0, i * 3);
						ms.Seek(i * 3, SeekOrigin.Begin);
						murmur2.TransformStream(ms, i * 2);
						murmur2.TransformBytes(v, i * 5);
						h2 = murmur2.TransformFinal();

						Assert.AreEqual(h1, h2);
					}
				}
			}

			{
				IHash murmur2 = HashFactory.Hash64.CreateMurmur2();

				for(int i = 0; i < 10; i++)
				{
					var v = new byte[13 * i];
					new Random().NextBytes(v);

					using(var ms = new MemoryStream(v))
					{
						HashResult h1 = murmur2.ComputeBytes(v);
						HashResult h2 = murmur2.ComputeStream(ms);

						Assert.AreEqual(h1, h2);

						h1 = murmur2.ComputeBytes(v.SubArray(i, i * 7));
						ms.Seek(i, SeekOrigin.Begin);
						h2 = murmur2.ComputeStream(ms, i * 7);

						Assert.AreEqual(h1, h2);

						h1 = murmur2.ComputeBytes(v);
						murmur2.Initialize();
						murmur2.TransformBytes(v, 0, i * 3);
						ms.Seek(i * 3, SeekOrigin.Begin);
						murmur2.TransformStream(ms, i * 2);
						murmur2.TransformBytes(v, i * 5);
						h2 = murmur2.TransformFinal();

						Assert.AreEqual(h1, h2);
					}
				}
			}

			Hash.BUFFER_SIZE = old;

			{
				IHash md5 = HashFactory.Crypto.CreateMD5();

				for(int i = 0; i < 10; i++)
				{
					var v = new byte[13 * i];
					new Random().NextBytes(v);

					string file_name = Path.GetTempFileName();

					using(var fs = new FileStream(file_name, FileMode.Truncate))
					{
						var bw = new BinaryWriter(fs);
						bw.Write(v);
					}

					HashResult h1 = md5.ComputeBytes(v);
					HashResult h2 = md5.ComputeFile(file_name);

					Assert.AreEqual(h1, h2);

					h1 = md5.ComputeBytes(v.SubArray(i, i * 7));
					h2 = md5.ComputeFile(file_name, i, i * 7);

					Assert.AreEqual(h1, h2);

					h1 = md5.ComputeBytes(v);
					md5.Initialize();
					md5.TransformBytes(v, 0, i * 3);
					md5.TransformFile(file_name, i * 3, i * 2);
					md5.TransformBytes(v, i * 5);
					h2 = md5.TransformFinal();

					Assert.AreEqual(h1, h2);
				}
			}

			{
				IHash murmur2 = HashFactory.Hash32.CreateMurmur2();

				for(int i = 0; i < 10; i++)
				{
					var v = new byte[13 * i];
					new Random().NextBytes(v);

					string file_name = Path.GetTempFileName();

					using(var fs = new FileStream(file_name, FileMode.Truncate))
					{
						var bw = new BinaryWriter(fs);
						bw.Write(v);
					}

					HashResult h1 = murmur2.ComputeBytes(v);
					HashResult h2 = murmur2.ComputeFile(file_name);

					Assert.AreEqual(h1, h2);

					h1 = murmur2.ComputeBytes(v.SubArray(i, i * 7));
					h2 = murmur2.ComputeFile(file_name, i, i * 7);

					Assert.AreEqual(h1, h2);

					h1 = murmur2.ComputeBytes(v);
					murmur2.Initialize();
					murmur2.TransformBytes(v, 0, i * 3);
					murmur2.TransformFile(file_name, i * 3, i * 2);
					murmur2.TransformBytes(v, i * 5);
					h2 = murmur2.TransformFinal();

					Assert.AreEqual(h1, h2);
				}
			}

			{
				IHash murmur2 = HashFactory.Hash64.CreateMurmur2();

				for(int i = 0; i < 10; i++)
				{
					var v = new byte[13 * i];
					new Random().NextBytes(v);

					string file_name = Path.GetTempFileName();

					using(var fs = new FileStream(file_name, FileMode.Truncate))
					{
						var bw = new BinaryWriter(fs);
						bw.Write(v);
					}

					HashResult h1 = murmur2.ComputeBytes(v);
					HashResult h2 = murmur2.ComputeFile(file_name);

					Assert.AreEqual(h1, h2);

					h1 = murmur2.ComputeBytes(v.SubArray(i, i * 7));
					h2 = murmur2.ComputeFile(file_name, i, i * 7);

					Assert.AreEqual(h1, h2);

					h1 = murmur2.ComputeBytes(v);
					murmur2.Initialize();
					murmur2.TransformBytes(v, 0, i * 3);
					murmur2.TransformFile(file_name, i * 3, i * 2);
					murmur2.TransformBytes(v, i * 5);
					h2 = murmur2.TransformFinal();

					Assert.AreEqual(h1, h2);
				}
			}

			{
				new HashesTestBase().TestAgainstBaseImplementationArrayType(
					HashFactory.Crypto.CreateSHA1(),
					HashFactory.Wrappers.HashAlgorithmToHash(
						HashFactory.Wrappers.HashToHashAlgorithm(
							HashFactory.Crypto.CreateSHA1()),
						HashFactory.Crypto.CreateSHA1().BlockSize
						)
					);
			}
		}
	}
}