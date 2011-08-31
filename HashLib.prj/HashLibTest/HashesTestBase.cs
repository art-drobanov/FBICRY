using System;
using HashLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections;

namespace HashLibTest
{
    public class HashesTestBase
    {
        protected MersenneTwister m_random = new MersenneTwister(4563487);
        protected ReadOnlyCollection<Func<object>> m_creators;
        protected const int TEST_BLOCK_SIZE = 64 * 1024;

        public HashesTestBase()
        {
            m_creators = new List<Func<object>>
            {
                () => { return m_random.NextByte(); }, 
                () => { return m_random.NextChar(); },
                () => { return m_random.NextShort(); },
                () => { return m_random.NextUShort(); },
                () => { return m_random.NextInt(); },
                () => { return m_random.NextUInt(); },
                () => { return m_random.NextLong(); },
                () => { return m_random.NextULong(); },
                () => { return m_random.NextFloatFull(); },
                () => { return m_random.NextDoubleFull(); },
                () => { return m_random.NextString(m_random.Next(0, 20)); },
                () => { return m_random.NextBytes(m_random.Next(0, 20)); }, 
                () => { return m_random.NextChars(m_random.Next(0, 20)); },
                () => { return m_random.NextShorts(m_random.Next(0, 20)); },
                () => { return m_random.NextUShorts(m_random.Next(0, 20)); },
                () => { return m_random.NextInts(m_random.Next(0, 20)); },
                () => { return m_random.NextUInts(m_random.Next(0, 20)); },
                () => { return m_random.NextLongs(m_random.Next(0, 20)); },
                () => { return m_random.NextULongs(m_random.Next(0, 20)); },
                () => { return m_random.NextFloatsFull(m_random.Next(0, 20)); },
                () => { return m_random.NextDoublesFull(m_random.Next(0, 20)); },
            }.AsReadOnly();
        }

        protected void Test(IHash a_hash, IHash a_base)
        {
            if (a_base == null)
            {
                TestAgainstTestFile(a_hash, new TestData(a_hash));
            }
            else
            {
                TestAgainstBaseImplementationValueType(a_base, a_hash);
            }

            TestMultipleTransformsValueType(a_hash, a_hash);
            TestHashStreamValueType(a_hash, a_hash);
            TestHashFileValueType(a_hash, a_hash);

            TestHashSize(a_hash);

            if (a_base != null)
                TestCompatibilityWithBaseClass(a_hash, a_base);

            TestInitialization(a_hash);

            TestResult(a_hash);
        }

        protected void TestResult(IHash a_hash)
        {
            a_hash.Initialize();
            a_hash.TransformBytes(m_random.NextBytes(64));
            HashResult r1 = a_hash.TransformFinal();
            byte[] r2 = (byte[])r1.GetBytes().Clone();
            HashResult r3 = a_hash.ComputeBytes(m_random.NextBytes(64));
            byte[] r4 = (byte[])r3.GetBytes().Clone();

            Assert.AreNotSame(r1, r2);
            Assert.AreNotSame(r1.GetBytes(), r3.GetBytes());
            CollectionAssert.AreEqual(r1.GetBytes(), r2);
            CollectionAssert.AreEqual(r3.GetBytes(), r4);

            Assert.AreNotEqual(r1, r3);
            CollectionAssert.AreNotEqual(r2, r4);
            CollectionAssert.AreNotEqual(r1.GetBytes(), r3.GetBytes());
        }

        protected void TestInitialization(IHash a_hash)
        {
            for (int i = 0; i <= (a_hash.BlockSize * 3 + 1); i++)
            {
                IHash hash2 = ((IHash)Activator.CreateInstance(a_hash.GetType()));
                byte[] v = m_random.NextBytes(i);

                HashResult h1 = a_hash.ComputeBytes(v);
                HashResult h2 = hash2.ComputeBytes(v);

                Assert.AreEqual(h1, h2,
                    String.Format("{0}, {1}", a_hash.Name, i));
            }
        }

        protected void TestCompatibilityWithBaseClass(IHash a_hash, IHash a_base)
        {
            Assert.AreEqual(a_base.HashSize, a_hash.HashSize, a_hash.Name);
            Assert.AreEqual(a_base.BlockSize, a_hash.BlockSize, a_hash.Name);
        }

        protected void TestHashSize(IHash a_hash)
        {
            Assert.AreEqual(a_hash.HashSize, a_hash.ComputeBytes(new byte[] {}).GetBytes().Length);
        }

        protected void TestAgainstTestFile(IHash a_hash, TestData a_testData)
        {
            a_testData.Load();

            for (int i = 0; i < a_testData.Count; i++)
            {
                byte[] output_array = a_testData.GetHash(i);

                CollectionAssert.AreEqual(output_array,
                    a_hash.ComputeBytes(a_testData.GetData(i)).GetBytes(), String.Format("{0}, {1}", a_hash.Name, i));

                CollectionAssert.AreEqual(output_array, a_testData.GetHash(i), a_hash.Name);
            }
        }

        protected void TestHashImplementationValueType(IHash a_baseHashFunction, IHash a_hash)
        {
            for (int j = 0; j < 8; j++)
            {
                {
                    byte v = m_random.NextByte();
                    Assert.AreEqual(a_baseHashFunction.ComputeByte(v), a_hash.ComputeByte(v),
                        a_hash.Name);
                }

                {
                    char v = m_random.NextChar();
                    Assert.AreEqual(a_baseHashFunction.ComputeChar(v), a_hash.ComputeChar(v),
                        a_hash.Name);
                }

                {
                    short v = m_random.NextShort();
                    Assert.AreEqual(a_baseHashFunction.ComputeShort(v), a_hash.ComputeShort(v),
                        a_hash.Name);
                }

                {
                    ushort v = m_random.NextUShort();
                    Assert.AreEqual(a_baseHashFunction.ComputeUShort(v), a_hash.ComputeUShort(v),
                        a_hash.Name);
                }

                {
                    int v = m_random.Next();
                    Assert.AreEqual(a_baseHashFunction.ComputeInt(v), a_hash.ComputeInt(v),
                        a_hash.Name);
                }

                {
                    uint v = m_random.NextUInt();
                    Assert.AreEqual(a_baseHashFunction.ComputeUInt(v), a_hash.ComputeUInt(v),
                        a_hash.Name);
                }

                {
                    long v = m_random.NextLong();
                    Assert.AreEqual(a_baseHashFunction.ComputeLong(v), a_hash.ComputeLong(v),
                        a_hash.Name);
                }

                {
                    ulong v = m_random.NextULong();
                    Assert.AreEqual(a_baseHashFunction.ComputeULong(v), a_hash.ComputeULong(v),
                        a_hash.Name);
                }

                {
                    double v = m_random.NextDoubleFull();
                    Assert.AreEqual(a_baseHashFunction.ComputeDouble(v), a_hash.ComputeDouble(v),
                        a_hash.Name);
                }

                {
                    float v = m_random.NextFloatFull();
                    Assert.AreEqual(a_baseHashFunction.ComputeFloat(v), a_hash.ComputeFloat(v),
                        a_hash.Name);
                }
            }

            for (int j = 0; j < 2; j++)
            {
                for (int i = 2050; i <= (a_hash.BlockSize * 3 + 1); i++)
                {
                    byte[] v = m_random.NextBytes(i);
                    Assert.AreEqual(a_baseHashFunction.ComputeBytes(v), a_hash.ComputeBytes(v),
                        String.Format("{0}, {1}", a_hash.Name, i));
                }

                for (int i = 0; i <= (a_hash.BlockSize * 3 + 1) / 2; i++)
                {
                    char[] v = m_random.NextChars(i);
                    Assert.AreEqual(a_baseHashFunction.ComputeChars(v), a_hash.ComputeChars(v),
                        String.Format("{0}, {1}", a_hash.Name, i));
                }

                for (int i = 0; i <= (a_hash.BlockSize * 3 + 1) / 2; i++)
                {
                    short[] v = m_random.NextShorts(i);
                    Assert.AreEqual(a_baseHashFunction.ComputeShorts(v), a_hash.ComputeShorts(v),
                        String.Format("{0}, {1}", a_hash.Name, i));
                }

                for (int i = 0; i <= (a_hash.BlockSize * 3 + 1) / 2; i++)
                {
                    ushort[] v = (from n in Enumerable.Range(0, i) select m_random.NextUShort()).ToArray();
                    Assert.AreEqual(a_baseHashFunction.ComputeUShorts(v), a_hash.ComputeUShorts(v),
                        String.Format("{0}, {1}", a_hash.Name, i));
                }

                for (int i = 0; i <= (a_hash.BlockSize * 3 + 1) / 4; i++)
                {
                    int[] v = m_random.NextInts(i);
                    Assert.AreEqual(a_baseHashFunction.ComputeInts(v), a_hash.ComputeInts(v),
                        String.Format("{0}, {1}", a_hash.Name, i));
                }

                for (int i = 0; i <= (a_hash.BlockSize * 3 + 1) / 4; i++)
                {
                    uint[] v = m_random.NextUInts(i);
                    Assert.AreEqual(a_baseHashFunction.ComputeUInts(v), a_hash.ComputeUInts(v),
                        String.Format("{0}, {1}", a_hash.Name, i));
                }

                for (int i = 1; i <= (a_hash.BlockSize * 3 + 1) / 8; i++)
                {
                    long[] v = m_random.NextLongs(i);
                    Assert.AreEqual(a_baseHashFunction.ComputeLongs(v), a_hash.ComputeLongs(v),
                        String.Format("{0}, {1}", a_hash.Name, i));
                }

                for (int i = 0; i <= (a_hash.BlockSize * 3 + 1) / 8; i++)
                {
                    ulong[] v = m_random.NextULongs(i);
                    Assert.AreEqual(a_baseHashFunction.ComputeULongs(v), a_hash.ComputeULongs(v),
                        String.Format("{0}, {1}", a_hash.Name, i));
                }

                for (int i = 0; i <= (a_hash.BlockSize * 3 + 1) / 8; i++)
                {
                    double[] v = m_random.NextDoublesFull(i);
                    Assert.AreEqual(a_baseHashFunction.ComputeDoubles(v), a_hash.ComputeDoubles(v),
                        String.Format("{0}, {1}", a_hash.Name, i));
                }

                for (int i = 0; i <= (a_hash.BlockSize * 3 + 1) / 4; i++)
                {
                    float[] v = m_random.NextFloatsFull(i);
                    Assert.AreEqual(a_baseHashFunction.ComputeFloats(v), a_hash.ComputeFloats(v),
                        String.Format("{0}, {1}", a_hash.Name, i));
                }

                for (int i = 0; i <= (a_hash.BlockSize * 3 + 1) / 2; i++)
                {
                    string v = m_random.NextString(i);
                    Assert.AreEqual(a_baseHashFunction.ComputeString(v), a_hash.ComputeString(v),
                        String.Format("{0}, {1}", a_hash.Name, i));
                }
            }
        }

        protected void TestHashImplementationArrayType(IHash a_baseHashFunction, IHash a_hash)
        {
            for (int j = 0; j < 8; j++)
            {
                {
                    byte v = m_random.NextByte();
                    Assert.AreEqual(a_baseHashFunction.ComputeByte(v), a_hash.ComputeByte(v), a_hash.Name);
                }

                {
                    char v = m_random.NextChar();
                    Assert.AreEqual(a_baseHashFunction.ComputeChar(v), a_hash.ComputeChar(v), a_hash.Name);
                }

                {
                    short v = m_random.NextShort();
                    Assert.AreEqual(a_baseHashFunction.ComputeShort(v), a_hash.ComputeShort(v), a_hash.Name);
                }

                {
                    ushort v = m_random.NextUShort();
                    Assert.AreEqual(a_baseHashFunction.ComputeUShort(v), a_hash.ComputeUShort(v), a_hash.Name);
                }

                {
                    int v = m_random.Next();
                    Assert.AreEqual(a_baseHashFunction.ComputeInt(v), a_hash.ComputeInt(v), a_hash.Name);
                }

                {
                    uint v = m_random.NextUInt();
                    Assert.AreEqual(a_baseHashFunction.ComputeUInt(v), a_hash.ComputeUInt(v), a_hash.Name);
                }

                {
                    long v = m_random.NextLong();
                    Assert.AreEqual(a_baseHashFunction.ComputeLong(v), a_hash.ComputeLong(v), a_hash.Name);
                }

                {
                    ulong v = m_random.NextULong();
                    Assert.AreEqual(a_baseHashFunction.ComputeULong(v), a_hash.ComputeULong(v), a_hash.Name);
                }

                {
                    double v = m_random.NextDoubleFull();
                    Assert.AreEqual(a_baseHashFunction.ComputeDouble(v), a_hash.ComputeDouble(v), a_hash.Name);
                }

                {
                    float v = m_random.NextFloatFull();
                    Assert.AreEqual(a_baseHashFunction.ComputeFloat(v), a_hash.ComputeFloat(v), a_hash.Name);
                }
            }

            for (int j = 0; j < 2; j++)
            {
                for (int i = 128; i <= (a_hash.BlockSize * 3 + 1); i++)
                {
                    byte[] v = m_random.NextBytes(i);
                    Assert.AreEqual(a_baseHashFunction.ComputeBytes(v), a_hash.ComputeBytes(v),
                        String.Format("{0}, {1}", a_hash.Name, i));
                }

                for (int i = 0; i <= (a_hash.BlockSize * 3 + 1) / 2; i++)
                {
                    char[] v = m_random.NextChars(i);
                    Assert.AreEqual(a_baseHashFunction.ComputeChars(v), a_hash.ComputeChars(v),
                        String.Format("{0}, {1}", a_hash.Name, i));
                }

                for (int i = 0; i <= (a_hash.BlockSize * 3 + 1) / 2; i++)
                {
                    short[] v = m_random.NextShorts(i);
                    Assert.AreEqual(a_baseHashFunction.ComputeShorts(v), a_hash.ComputeShorts(v),
                        String.Format("{0}, {1}", a_hash.Name, i));
                }

                for (int i = 0; i <= (a_hash.BlockSize * 3 + 1) / 2; i++)
                {
                    ushort[] v = m_random.NextUShorts(i);
                    Assert.AreEqual(a_baseHashFunction.ComputeUShorts(v), a_hash.ComputeUShorts(v),
                        String.Format("{0}, {1}", a_hash.Name, i));
                }

                for (int i = 0; i <= (a_hash.BlockSize * 3 + 1) / 4; i++)
                {
                    int[] v = m_random.NextInts(i);
                    Assert.AreEqual(a_baseHashFunction.ComputeInts(v), a_hash.ComputeInts(v),
                        String.Format("{0}, {1}", a_hash.Name, i));
                }

                for (int i = 0; i <= (a_hash.BlockSize * 3 + 1) / 4; i++)
                {
                    uint[] v = m_random.NextUInts(i);
                    Assert.AreEqual(a_baseHashFunction.ComputeUInts(v), a_hash.ComputeUInts(v),
                        String.Format("{0}, {1}", a_hash.Name, i));
                }

                for (int i = 0; i <= (a_hash.BlockSize * 3 + 1) / 8; i++)
                {
                    long[] v = m_random.NextLongs(i);
                    Assert.AreEqual(a_baseHashFunction.ComputeLongs(v), a_hash.ComputeLongs(v),
                        String.Format("{0}, {1}", a_hash.Name, i));
                }

                for (int i = 0; i <= (a_hash.BlockSize * 3 + 1) / 8; i++)
                {
                    ulong[] v = m_random.NextULongs(i);
                    Assert.AreEqual(a_baseHashFunction.ComputeULongs(v), a_hash.ComputeULongs(v),
                        String.Format("{0}, {1}", a_hash.Name, i));
                }

                for (int i = 0; i <= (a_hash.BlockSize * 3 + 1) / 8; i++)
                {
                    double[] v = m_random.NextDoublesFull(i);
                    Assert.AreEqual(a_baseHashFunction.ComputeDoubles(v), a_hash.ComputeDoubles(v),
                        String.Format("{0}, {1}", a_hash.Name, i));
                }

                for (int i = 0; i <= (a_hash.BlockSize * 3 + 1) / 4; i++)
                {
                    float[] v = m_random.NextFloatsFull(i);
                    Assert.AreEqual(a_baseHashFunction.ComputeFloats(v), a_hash.ComputeFloats(v),
                        String.Format("{0}, {1}", a_hash.Name, i));
                }

                for (int i = 0; i <= (a_hash.BlockSize * 3 + 1) / 2; i++)
                {
                    string v = m_random.NextString(i);
                    Assert.AreEqual(a_baseHashFunction.ComputeString(v), a_hash.ComputeString(v),
                        String.Format("{0}, {1}", a_hash.Name, i));
                }
            }
        }

        protected void TestAgainstBaseImplementationValueType(IHash a_baseHashFunction, IHash a_hash)
        {
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i <= (a_hash.BlockSize * 3 + 1); i++)
                {
                    byte[] v = m_random.NextBytes(i);
                    Assert.AreEqual(a_baseHashFunction.ComputeBytes(v), a_hash.ComputeBytes(v),
                        String.Format("{0}, {1}", a_hash.Name, i));
                }
            }
        }

        internal void TestAgainstBaseImplementationArrayType(IHash a_baseHashFunction, IHash a_hash)
        {
            for (int j = 0; j < 2; j++)
            {
                for (int i = 9; i <= (a_hash.BlockSize * 3 + 1); i++)
                {
                    byte[] v = m_random.NextBytes(i);

                    HashResult h1 = a_baseHashFunction.ComputeBytes(v);
                    HashResult h2 = a_hash.ComputeBytes(v);

                    Assert.AreEqual(h1, h2,
                        String.Format("{0}, {1}", a_hash.Name, i));
                }
            }
        }

        protected void TestMultipleTransformsValueType(IHash a_multi, IHash a_hash, IList<int> a_list)
        {
            a_multi.Initialize();
            a_hash.Initialize();

            List<byte[]> v1 = new List<byte[]>(a_list.Count);

            foreach (int length in a_list)
            {
                byte[] ar = new byte[length];
                for (int i = 0; i < ar.Length; i++)
                    ar[i] = (byte)m_random.Next(Byte.MaxValue);
                v1.Add(ar);
            }

            int len = 0;
            foreach (byte[] ar in v1)
                len += ar.Length;

            byte[] v2 = new byte[len];

            int index = 0;
            foreach (byte[] ar in v1)
            {
                Array.Copy(ar, 0, v2, index, ar.Length);
                index += ar.Length;
            }

            for (int i = 0; i < v1.Count; i++)
                a_multi.TransformBytes(v1[i]);

            HashResult h1 = a_multi.TransformFinal();
            HashResult h2 = a_hash.ComputeBytes(v2);

            Assert.AreEqual(h2, h1);

            index = 0;
            a_multi.Initialize();
            for (int i = 0; i < v1.Count; i++)
            {
                a_multi.TransformBytes(v2, index, a_list[i]);
                index += a_list[i];
            }

            h1 = a_multi.TransformFinal();

            Assert.AreEqual(h2, h1);
        }

        protected void TestMultipleTransformsArrayType(IHash a_multi, IHash a_hash, IList<int> a_list)
        {
            a_multi.TransformByte(0x55);
            a_hash.TransformByte(0x55);

            a_multi.Initialize();

            List<byte[]> v1 = new List<byte[]>(a_list.Count);

            foreach (int length in a_list)
            {
                byte[] ar = new byte[length];
                for (int i = 0; i < ar.Length; i++)
                    ar[i] = (byte)m_random.Next(Byte.MaxValue);
                v1.Add(ar);
            }

            int len = 0;
            foreach (byte[] ar in v1)
                len += ar.Length;

            byte[] v2 = new byte[len];

            int index = 0;
            foreach (byte[] ar in v1)
            {
                Array.Copy(ar, 0, v2, index, ar.Length);
                index += ar.Length;
            }

            for (int i = 0; i < v1.Count; i++)
                a_multi.TransformBytes(v1[i]);

            HashResult h1 = a_multi.TransformFinal();
            HashResult h2 = a_hash.ComputeBytes(v2);

            Assert.AreEqual(h2, h1, a_hash.Name);
        }

        protected void TestMultipleTransformsValueType(IHash a_multi, IHash a_hash)
        {
            TestMultipleTransformsValueType(a_multi, a_hash, new List<int>() { 0, 0 });
            TestMultipleTransformsValueType(a_multi, a_hash, new List<int>() { 1, 0 });
            TestMultipleTransformsValueType(a_multi, a_hash, new List<int>() { 0, 1 });

            for (int tries = 0; tries < 10; tries++)
            {
                int parts = m_random.Next(20) + 1;

                List<int> list = new List<int>(parts);

                for (int i = 0; i < parts; i++)
                    list.Add(m_random.Next(a_multi.BlockSize * 3 + 1));

                TestMultipleTransformsValueType(a_multi, a_hash, list);
            }

            List<object> objects;
            byte[] bytes;

            for (int i = 0; i < 50; i++)
            {
                CreateListForDataTest(i, out objects, out bytes);

                a_hash.TransformByte(0x55);
                HashResult h1 = a_hash.ComputeBytes(bytes);

                a_multi.TransformByte(0x55);
                a_multi.Initialize();
                foreach (object o in objects)
                    a_multi.TransformObject(o);
                HashResult h2 = a_multi.TransformFinal();

                Assert.AreEqual(h1, h2, String.Format("{0}, {1}", a_hash.Name, i));
            }
        }

        protected void TestMultipleTransformsArrayType(IHash a_multi, IHash a_hash)
        {
            TestMultipleTransformsArrayType(a_multi, a_hash, new List<int>() { 0, 0 });
            TestMultipleTransformsArrayType(a_multi, a_hash, new List<int>() { 1, 0 });
            TestMultipleTransformsArrayType(a_multi, a_hash, new List<int>() { 0, 1 });

            for (int tries = 0; tries < 10; tries++)
            {
                int parts = m_random.Next(20) + 1;

                List<int> list = new List<int>(parts);

                for (int i = 0; i < parts; i++)
                    list.Add(m_random.Next(a_multi.BlockSize * 3 + 1));

                TestMultipleTransformsArrayType(a_multi, a_hash, list);
            }

            List<object> objects;
            byte[] bytes;

            for (int i = 0; i < 50; i++)
            {
                CreateListForDataTest(i, out objects, out bytes);

                HashResult h1 = a_hash.ComputeBytes(bytes);

                a_multi.Initialize();
                foreach (object o in objects)
                    a_multi.TransformObject(o);
                HashResult h2 = a_multi.TransformFinal();

                Assert.AreEqual(h1, h2, String.Format("{0}, {1}", a_hash.Name, i));
            }
        }

        private void CreateListForDataTest(int a_count, out List<object> a_objects, out byte[] a_bytes)
        {
            a_objects = new List<object>();

            for (int i = 0; i < a_count; i++)
                a_objects.Add( m_creators[m_random.Next(0, m_creators.Count)]() );

            List<byte[]> bytes = new List<byte[]>();

            foreach (object o in a_objects)
                bytes.Add(Converters.ConvertToBytes(o));

            int count = 0;
            foreach (byte[] b in bytes)
                count += b.Length;

            a_bytes = new byte[count];

            int index = 0;
            foreach (byte[] b in bytes)
            {
                Array.Copy(b, 0, a_bytes, index, b.Length);
                index += b.Length;
            }  
        }

        protected void TestHashFileArrayType(IHash a_hash_file, IHash a_hash)
        {
            string file_name = System.IO.Path.GetTempFileName();
            int ratio = (TEST_BLOCK_SIZE * 3 / 2) / 9;
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    byte[] data = new byte[ratio * i];
                    m_random.NextBytes(data);

                    using (FileStream stream = new FileStream(file_name, FileMode.Truncate))
                        new System.IO.BinaryWriter(stream).Write(data);

                    HashResult h1 = a_hash_file.ComputeFile(file_name);
                    HashResult h2 = a_hash.ComputeBytes(data);

                    Assert.AreEqual(h2, h1, String.Format("{0}, {1}", a_hash.Name, data.Length));
                }
            }
            finally
            {
                new FileInfo(file_name).Delete();
            }
        }

        protected void TestHashFileValueType(IHash a_hash_file, IHash a_hash)
        {
            string file_name = System.IO.Path.GetTempFileName();
            int ratio = (TEST_BLOCK_SIZE * 3 / 2) / 9;
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    byte[] data = new byte[ratio * i];
                    m_random.NextBytes(data);

                    using (FileStream stream = new FileStream(file_name, FileMode.Truncate))
                        new System.IO.BinaryWriter(stream).Write(data);

                    HashResult h1 = a_hash_file.ComputeFile(file_name);
                    HashResult h2 = a_hash.ComputeBytes(data);

                    Assert.AreEqual(h2, h1, String.Format("{0}, {1}", a_hash.Name, data.Length));
                }
            }
            finally
            {
                new FileInfo(file_name).Delete();
            }
        }

        protected void TestHashStreamArrayType(IHash a_hash_stream, IHash a_hash)
        {
            string file_name = System.IO.Path.GetTempFileName();
            int ratio = (TEST_BLOCK_SIZE * 3 / 2) / 9;

            try
            {
                for (int i = 0; i < 10; i++)
                {
                    byte[] data = new byte[ratio * i];
                    m_random.NextBytes(data);

                    using (FileStream stream = new FileStream(file_name, FileMode.Truncate))
                    {
                        new System.IO.BinaryWriter(stream).Write(data);

                        stream.Seek(0, SeekOrigin.Begin);

                        HashResult h1 = a_hash_stream.ComputeStream(stream);
                        HashResult h2 = a_hash.ComputeBytes(data);

                       Assert.AreEqual(h2, h1, String.Format("{0}, {1}", a_hash.Name, data.Length));
                    }
                }
            }
            finally
            {
                new FileInfo(file_name).Delete();
            }
        }

        protected void TestHashStreamValueType(IHash a_hash_stream, IHash a_hash)
        {
            string file_name = System.IO.Path.GetTempFileName();
            int ratio = (TEST_BLOCK_SIZE * 3 / 2) / 9;

            try
            {
                for (int i = 0; i < 10; i++)
                {
                    byte[] data = new byte[ratio * i];
                    m_random.NextBytes(data);

                    using (FileStream stream = new FileStream(file_name, FileMode.Truncate))
                    {
                        new System.IO.BinaryWriter(stream).Write(data);

                        stream.Seek(0, SeekOrigin.Begin);

                        HashResult h1 = a_hash_stream.ComputeStream(stream);
                        HashResult h2 = a_hash.ComputeBytes(data);

                        Assert.AreEqual(h2, h1, String.Format("{0}, {1}", a_hash.Name, data.Length));
                    }
                }
            }
            finally
            {
                new FileInfo(file_name).Delete();
            }
        }
    }
}
