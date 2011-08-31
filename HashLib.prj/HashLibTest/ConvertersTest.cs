using HashLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System;
using System.Text;

namespace HashLibTest
{
    [TestClass()]
    public class ConvertersTest
    {
        [TestMethod()]
        public void ConvertersTest_All()
        {
            byte[] ar1 = new byte[] { 12, 234, 35, 167, 5, 66, 255, 23, 9, 10, 200, 12, 100, 
                                      150, 180, 16, 45, 36, 245, 78, 77, 56, 236, 167 };

            {
                // tests for proper array size

                Assert.AreEqual(24 / 2, Converters.ConvertBytesToChars(ar1).Length);
                Assert.AreEqual(24 / 8, Converters.ConvertBytesToDoubles(ar1).Length);
                Assert.AreEqual(24 / 4, Converters.ConvertBytesToFloats(ar1).Length);

                Assert.AreEqual(4, Converters.ConvertFloatToBytes(0).Length);

                Assert.AreEqual(24 / 4, Converters.ConvertBytesToInts(ar1).Length);
                Assert.AreEqual(1, Converters.ConvertBytesToInts(ar1, 1, 4).Length);
                Assert.AreEqual(24 / 4, Converters.ConvertBytesToIntsSwapOrder(ar1).Length);
                Assert.AreEqual(1, Converters.ConvertBytesToIntsSwapOrder(ar1, 1, 4).Length);

                Assert.AreEqual(24 / 4, Converters.ConvertBytesToUInts(ar1).Length);
                Assert.AreEqual(1, Converters.ConvertBytesToUInts(ar1, 1, 4).Length);
                Assert.AreEqual(24 / 4, Converters.ConvertBytesToUIntsSwapOrder(ar1).Length);
                Assert.AreEqual(1, Converters.ConvertBytesToUIntsSwapOrder(ar1, 1, 4).Length);

                Assert.AreEqual(24 / 8, Converters.ConvertBytesToLongs(ar1).Length);
                Assert.AreEqual(1, Converters.ConvertBytesToLongs(ar1, 1, 8).Length);
                Assert.AreEqual(24 / 8, Converters.ConvertBytesToLongsSwapOrder(ar1).Length);
                Assert.AreEqual(1, Converters.ConvertBytesToLongsSwapOrder(ar1, 1, 8).Length);

                Assert.AreEqual(24 / 8, Converters.ConvertBytesToULongs(ar1).Length);
                Assert.AreEqual(1, Converters.ConvertBytesToULongs(ar1, 1, 8).Length);
                Assert.AreEqual(24 / 8, Converters.ConvertBytesToULongsSwapOrder(ar1).Length);
                Assert.AreEqual(1, Converters.ConvertBytesToULongsSwapOrder(ar1, 1, 8).Length);

                Assert.AreEqual(24 / 2, Converters.ConvertBytesToShorts(ar1).Length);
                Assert.AreEqual(53, Converters.ConvertBytesToHexString(ar1, true).Length);
                Assert.AreEqual(ar1.Length * 2, Converters.ConvertBytesToHexString(ar1, false).Length);
                Assert.AreEqual(24 / 2, Converters.ConvertBytesToUShorts(ar1).Length);

                Assert.AreEqual(24,
                    Converters.ConvertHexStringToBytes(Converters.ConvertBytesToHexString(ar1, true)).Length);
                Assert.AreEqual(24,
                    Converters.ConvertHexStringToBytes(Converters.ConvertBytesToHexString(ar1, false)).Length);

                Assert.AreEqual(2, Converters.ConvertCharsToBytes(new char[] { 'd' }).Length);
                Assert.AreEqual(1, Converters.ConvertCharsToBytes(new char[] { 'd' }, Encoding.ASCII).Length);
                Assert.AreEqual(8, Converters.ConvertDoublesToBytes(new double[] { 0 }).Length);
                Assert.AreEqual(4, Converters.ConvertFloatsToBytes(new float[] { 0 }).Length);

                Assert.AreEqual(4, Converters.ConvertIntsToBytes(new int[] { 0 }).Length);
                Assert.AreEqual(4, Converters.ConvertIntsToBytes(new int[] { 0 }, 0, 1).Length);
                Assert.AreEqual(4, Converters.ConvertIntsToBytesSwapOrder(new int[] { 0 }).Length);
                Assert.AreEqual(4, Converters.ConvertIntsToBytesSwapOrder(new int[] { 0 }, 0, 1).Length);

                Assert.AreEqual(4, Converters.ConvertUIntsToBytes(new uint[] { 0 }).Length);
                Assert.AreEqual(4, Converters.ConvertUIntsToBytes(new uint[] { 0 }, 0, 1).Length);
                Assert.AreEqual(4, Converters.ConvertUIntsToBytesSwapOrder(new uint[] { 0 }).Length);
                Assert.AreEqual(4, Converters.ConvertUIntsToBytesSwapOrder(new uint[] { 0 }, 0, 1).Length);

                Assert.AreEqual(8, Converters.ConvertLongsToBytes(new long[] { 0 }).Length);
                Assert.AreEqual(8, Converters.ConvertLongsToBytes(new long[] { 0 }, 0, 1).Length);
                Assert.AreEqual(8, Converters.ConvertLongsToBytesSwapOrder(new long[] { 0 }).Length);
                Assert.AreEqual(8, Converters.ConvertLongsToBytesSwapOrder(new long[] { 0 }, 0, 1).Length);

                Assert.AreEqual(8, Converters.ConvertULongsToBytes(new ulong[] { 0 }).Length);
                Assert.AreEqual(8, Converters.ConvertULongsToBytes(new ulong[] { 0 }, 0, 1).Length);
                Assert.AreEqual(8, Converters.ConvertULongsToBytesSwapOrder(new ulong[] { 0 }).Length);
                Assert.AreEqual(8, Converters.ConvertULongsToBytesSwapOrder(new ulong[] { 0 }, 0, 1).Length);

                Assert.AreEqual(2, Converters.ConvertShortsToBytes(new short[] { 0 }).Length);
                Assert.AreEqual(4, Converters.ConvertStringToBytes("test", Encoding.ASCII).Length);
                Assert.AreEqual(8, Converters.ConvertStringToBytes("test").Length);
                Assert.AreEqual(2, Converters.ConvertUShortsToBytes(new ushort[] { 0 }).Length);
            }

            {
                // test for a=>b=>a convert

                Assert.AreEqual(543646,
                    Converters.ConvertIntToFloat(Converters.ConvertFloatToInt(543646)));

                CollectionAssert.AreEqual(ar1,
                    Converters.ConvertCharsToBytes(Converters.ConvertBytesToChars(ar1)));

                CollectionAssert.AreEqual(ar1,
                    Converters.ConvertDoublesToBytes(Converters.ConvertBytesToDoubles(ar1)));

                CollectionAssert.AreEqual(ar1,
                    Converters.ConvertFloatsToBytes(Converters.ConvertBytesToFloats(ar1)));

                {
                    CollectionAssert.AreEqual(ar1,
                        Converters.ConvertIntsToBytes(Converters.ConvertBytesToInts(ar1)));

                    int[] ar2 = new int[ar1.Length / 4];
                    Converters.ConvertBytesToInts(ar1, ar2);
                    CollectionAssert.AreEqual(ar1, Converters.ConvertIntsToBytes(ar2));

                    CollectionAssert.AreEqual(ar1.Take(6, 4).ToArray(),
                        Converters.ConvertIntsToBytes(Converters.ConvertBytesToInts(ar1, 2, 8), 1, 1));

                    int[] ar3 = new int[2];
                    Converters.ConvertBytesToInts(ar1, 2, 8, ar3);
                    CollectionAssert.AreEqual(ar1.Take(6, 4).ToArray(), Converters.ConvertIntsToBytes(ar3, 1, 1));

                    int[] ar4 = new int[4];
                    Converters.ConvertBytesToInts(ar1, 2, 8, ar4, 2);
                    CollectionAssert.AreEqual(ar1.Take(6, 4).ToArray(), Converters.ConvertIntsToBytes(ar4, 3, 1));
                }

                {
                    CollectionAssert.AreEqual(ar1,
                        Converters.ConvertIntsToBytesSwapOrder(Converters.ConvertBytesToIntsSwapOrder(ar1)));

                    int[] ar2 = new int[ar1.Length / 4];
                    Converters.ConvertBytesToIntsSwapOrder(ar1, ar2);
                    CollectionAssert.AreEqual(ar1, Converters.ConvertIntsToBytesSwapOrder(ar2));

                    CollectionAssert.AreEqual(ar1.Take(6, 4).ToArray(),
                        Converters.ConvertIntsToBytesSwapOrder(Converters.ConvertBytesToIntsSwapOrder(ar1, 2, 8), 1, 1));

                    int[] ar3 = new int[2];
                    Converters.ConvertBytesToIntsSwapOrder(ar1, 2, 8, ar3);
                    CollectionAssert.AreEqual(ar1.Take(6, 4).ToArray(),
                        Converters.ConvertIntsToBytesSwapOrder(ar3, 1, 1));

                    int[] ar4 = new int[4];
                    Converters.ConvertBytesToIntsSwapOrder(ar1, 2, 8, ar4, 2);
                    CollectionAssert.AreEqual(ar1.Take(6, 4).ToArray(), Converters.ConvertIntsToBytesSwapOrder(ar4, 3, 1));
                }

                {
                    CollectionAssert.AreEqual(ar1,
                        Converters.ConvertLongsToBytes(Converters.ConvertBytesToLongs(ar1)));

                    long[] ar2 = new long[ar1.Length / 8];
                    Converters.ConvertBytesToLongs(ar1, ar2);
                    CollectionAssert.AreEqual(
                        Converters.ConvertLongsToBytes(ar2), ar1);

                    CollectionAssert.AreEqual(ar1.Take(10, 8).ToArray(),
                        Converters.ConvertLongsToBytes(Converters.ConvertBytesToLongs(ar1, 2, 16), 1, 1));

                    long[] ar3 = new long[2];
                    Converters.ConvertBytesToLongs(ar1, 2, 16, ar3);
                    CollectionAssert.AreEqual(ar1.Take(10, 8).ToArray(), Converters.ConvertLongsToBytes(ar3, 1, 1));

                    long[] ar4 = new long[4];
                    Converters.ConvertBytesToLongs(ar1, 2, 16, ar4, 2);
                    CollectionAssert.AreEqual(ar1.Take(10, 8).ToArray(), Converters.ConvertLongsToBytes(ar4, 3, 1));
                }

                {

                    CollectionAssert.AreEqual(ar1,
                        Converters.ConvertLongsToBytesSwapOrder(Converters.ConvertBytesToLongsSwapOrder(ar1)));

                    long[] ar2 = new long[ar1.Length / 8];
                    Converters.ConvertBytesToLongsSwapOrder(ar1, ar2);
                    CollectionAssert.AreEqual(ar1, Converters.ConvertLongsToBytesSwapOrder(ar2));

                    CollectionAssert.AreEqual(ar1.Take(10, 8).ToArray(),
                        Converters.ConvertLongsToBytesSwapOrder(Converters.ConvertBytesToLongsSwapOrder(ar1, 2, 16), 1, 1));

                    long[] ar3 = new long[2];
                    Converters.ConvertBytesToLongsSwapOrder(ar1, 2, 16, ar3);
                    CollectionAssert.AreEqual(ar1.Take(10, 8).ToArray(),
                        Converters.ConvertLongsToBytesSwapOrder(ar3, 1, 1));

                    long[] ar4 = new long[4];
                    Converters.ConvertBytesToLongsSwapOrder(ar1, 2, 16, ar4, 2);
                    CollectionAssert.AreEqual(ar1.Take(10, 8).ToArray(), Converters.ConvertLongsToBytesSwapOrder(ar4, 3, 1));
                }

                CollectionAssert.AreEqual(ar1,
                    Converters.ConvertShortsToBytes(Converters.ConvertBytesToShorts(ar1)));

                Assert.AreEqual("0CEA23A7-0542FF17-090AC80C-6496B410-2D24F54E-4D38ECA7",
                    Converters.ConvertBytesToHexString(ar1, true));
                Assert.AreEqual("0CEA23A70542FF17090AC80C6496B4102D24F54E4D38ECA7",
                    Converters.ConvertBytesToHexString(ar1, false));

                CollectionAssert.AreEqual(ar1,
                    Converters.ConvertHexStringToBytes("0CEA23A7-0542FF17-090AC80C-6496B410-2D24F54E-4D38ECA7"));
                CollectionAssert.AreEqual(ar1,
                    Converters.ConvertHexStringToBytes("0CEA23A70542FF17090AC80C6496B4102D24F54E4D38ECA7"));

                {
                    CollectionAssert.AreEqual(ar1,
                        Converters.ConvertUIntsToBytes(Converters.ConvertBytesToUInts(ar1)));

                    uint[] ar2 = new uint[ar1.Length / 4];
                    Converters.ConvertBytesToUInts(ar1, ar2);
                    CollectionAssert.AreEqual(ar1, Converters.ConvertUIntsToBytes(ar2));

                    CollectionAssert.AreEqual(ar1.Take(6, 4).ToArray(),
                        Converters.ConvertUIntsToBytes(Converters.ConvertBytesToUInts(ar1, 2, 8), 1, 1));

                    uint[] ar3 = new uint[2];
                    Converters.ConvertBytesToUInts(ar1, 2, 8, ar3);
                    CollectionAssert.AreEqual(ar1.Take(6, 4).ToArray(), Converters.ConvertUIntsToBytes(ar3, 1, 1));

                    uint[] ar4 = new uint[4];
                    Converters.ConvertBytesToUInts(ar1, 2, 8, ar4, 2);
                    CollectionAssert.AreEqual(ar1.Take(6, 4).ToArray(), Converters.ConvertUIntsToBytes(ar4, 3, 1));
                }

                {
                    CollectionAssert.AreEqual(ar1,
                        Converters.ConvertUIntsToBytesSwapOrder(Converters.ConvertBytesToUIntsSwapOrder(ar1)));

                    uint[] ar2 = new uint[ar1.Length / 4];
                    Converters.ConvertBytesToUIntsSwapOrder(ar1, ar2);
                    CollectionAssert.AreEqual(ar1, Converters.ConvertUIntsToBytesSwapOrder(ar2));

                    CollectionAssert.AreEqual(ar1.Take(6, 4).ToArray(),
                        Converters.ConvertUIntsToBytesSwapOrder(Converters.ConvertBytesToUIntsSwapOrder(ar1, 2, 8), 1, 1));

                    uint[] ar3 = new uint[2];
                    Converters.ConvertBytesToUIntsSwapOrder(ar1, 2, 8, ar3);
                    CollectionAssert.AreEqual(ar1.Take(6, 4).ToArray(),
                        Converters.ConvertUIntsToBytesSwapOrder(ar3, 1, 1));

                    uint[] ar4 = new uint[4];
                    Converters.ConvertBytesToUIntsSwapOrder(ar1, 2, 8, ar4, 2);
                    CollectionAssert.AreEqual(ar1.Take(6, 4).ToArray(), Converters.ConvertUIntsToBytesSwapOrder(ar4, 3, 1));
                }

                {
                    CollectionAssert.AreEqual(ar1,
                        Converters.ConvertULongsToBytes(Converters.ConvertBytesToULongs(ar1)));

                    ulong[] ar2 = new ulong[ar1.Length / 8];
                    Converters.ConvertBytesToULongs(ar1, ar2);
                    CollectionAssert.AreEqual(ar1, Converters.ConvertULongsToBytes(ar2));

                    CollectionAssert.AreEqual(ar1.Take(10, 8).ToArray(),
                        Converters.ConvertULongsToBytes(Converters.ConvertBytesToULongs(ar1, 2, 16), 1, 1));

                    ulong[] ar3 = new ulong[2];
                    Converters.ConvertBytesToULongs(ar1, 2, 16, ar3);
                    CollectionAssert.AreEqual(ar1.Take(10, 8).ToArray(), Converters.ConvertULongsToBytes(ar3, 1, 1));

                    ulong[] ar4 = new ulong[4];
                    Converters.ConvertBytesToULongs(ar1, 2, 16, ar4, 2);
                    CollectionAssert.AreEqual(ar1.Take(10, 8).ToArray(), Converters.ConvertULongsToBytes(ar4, 3, 1));

                    ulong[] ar5 = Converters.ConvertBytesToULongs(ar1, 0, 16);
                    byte[] ar6 = new byte[16];
                    Converters.ConvertULongsToBytes(ar5, 0, 2, ar6);
                    CollectionAssert.AreEqual(ar1.Take(0, 16).ToArray(), ar6);
                }

                {
                    CollectionAssert.AreEqual(ar1,
                        Converters.ConvertULongsToBytesSwapOrder(Converters.ConvertBytesToULongsSwapOrder(ar1)));

                    ulong[] ar2 = new ulong[ar1.Length / 8];
                    Converters.ConvertBytesToULongsSwapOrder(ar1, ar2);
                    CollectionAssert.AreEqual(ar1, Converters.ConvertULongsToBytesSwapOrder(ar2));

                    CollectionAssert.AreEqual(ar1.Take(10, 8).ToArray(),
                        Converters.ConvertULongsToBytesSwapOrder(Converters.ConvertBytesToULongsSwapOrder(ar1, 2, 16), 1, 1));

                    ulong[] ar3 = new ulong[2];
                    Converters.ConvertBytesToULongsSwapOrder(ar1, 2, 16, ar3);
                    CollectionAssert.AreEqual(ar1.Take(10, 8).ToArray(),
                        Converters.ConvertULongsToBytesSwapOrder(ar3, 1, 1));

                    ulong[] ar4 = new ulong[4];
                    Converters.ConvertBytesToULongsSwapOrder(ar1, 2, 16, ar4, 2);
                    CollectionAssert.AreEqual(ar1.Take(10, 8).ToArray(), Converters.ConvertULongsToBytesSwapOrder(ar4, 3, 1));
                }

                CollectionAssert.AreEqual(ar1,
                    Converters.ConvertUShortsToBytes(Converters.ConvertBytesToUShorts(ar1)));

                CollectionAssert.AreEqual(new byte[] { 116, 0, 5, 1, 25, 1, 120, 0, 49, 0, 52, 0 },
                    Converters.ConvertStringToBytes("tąęx14"));

                CollectionAssert.AreEqual(new byte[] { 116, 63, 63, 63, 63, 63, 63, 120, 120, 49, 50, 51, 52 },
                    Converters.ConvertStringToBytes("tąąąęęęxx1234", Encoding.ASCII));

                {
                    CollectionAssert.AreEqual(ar1.Take(4, 4).ToArray(),
                        Converters.ConvertFloatToBytes(Converters.ConvertBytesToFloat(ar1, 4)));

                    byte[] ar2 = new byte[4];
                    Converters.ConvertFloatToBytes(Converters.ConvertBytesToFloat(ar1, 4), ar2, 0);
                    CollectionAssert.AreEqual(ar1.Take(4, 4).ToArray(), ar2);
                }

                Assert.AreEqual(0x45673e56, Converters.ConvertIntToFloat(Converters.ConvertFloatToInt(0x45673e56)));
            }

            {
                // tests against constant set of data.

                CollectionAssert.AreEqual(new char[] { (char)59916, (char)42787, (char)16901, (char)6143, (char)2569, 
                    (char)3272, (char)38500, (char)4276, (char)9261, (char)20213, (char)14413, (char)42988 },
                Converters.ConvertBytesToChars(ar1));

                CollectionAssert.AreEqual(new double[] { 4.2819596702736908E-193, 3.3947371345724108E-228, -2.2381418769276721E-116 },
                    Converters.ConvertBytesToDoubles(ar1));

                CollectionAssert.AreEqual(new float[] { -2.0f, 0.00310765952f, -3.689349E+19f, 50.8261681f },
                    Converters.ConvertBytesToFloats(new byte[] { 0, 0, 0, 192, 224, 169, 75, 59, 0, 0, 0, 224, 255, 77, 75, 66 }));

                {
                    CollectionAssert.AreEqual(new int[] { -1490818548, 402604549, 214436361, 280270436, 1324688429, -1477691315 },
                        Converters.ConvertBytesToInts(ar1));

                    int[] ar2 = new int[ar1.Length / 4];
                    Converters.ConvertBytesToInts(ar1, ar2);
                    CollectionAssert.AreEqual(new int[] { -1490818548, 402604549, 214436361, 280270436, 1324688429, -1477691315 }, ar2);

                    CollectionAssert.AreEqual(new int[] { 1107666723, 168368127 },
                        Converters.ConvertBytesToInts(ar1, 2, 8));

                    int[] ar3 = new int[2];
                    Converters.ConvertBytesToInts(ar1, 2, 8, ar3);
                    CollectionAssert.AreEqual(new int[] { 1107666723, 168368127 }, ar3);

                    int[] ar4 = new int[4];
                    Converters.ConvertBytesToInts(ar1, 2, 8, ar4, 2);
                    CollectionAssert.AreEqual(new int[] { 0, 0, 1107666723, 168368127 }, ar4);
                }

                {
                    CollectionAssert.AreEqual(new int[] { 216671143, 88276759, 151701516, 1687598096, 757396814, 1295576231 },
                        Converters.ConvertBytesToIntsSwapOrder(ar1));

                    int[] ar2 = new int[ar1.Length / 4];
                    Converters.ConvertBytesToIntsSwapOrder(ar1, ar2);
                    CollectionAssert.AreEqual(new int[] { 216671143, 88276759, 151701516, 1687598096, 757396814, 1295576231 }, ar2);

                    CollectionAssert.AreEqual(new int[] { 598148418, -15267574 },
                        Converters.ConvertBytesToIntsSwapOrder(ar1, 2, 8));

                    int[] ar3 = new int[2];
                    Converters.ConvertBytesToIntsSwapOrder(ar1, 2, 8, ar3);
                    CollectionAssert.AreEqual(new int[] { 598148418, -15267574 }, ar3);

                    int[] ar4 = new int[4];
                    Converters.ConvertBytesToIntsSwapOrder(ar1, 2, 8, ar4, 2);
                    CollectionAssert.AreEqual(new int[] { 0, 0, 598148418, -15267574 }, ar4);
                }

                {
                    CollectionAssert.AreEqual(new long[] { 1729173373979978252, 1203752356870097417, -6346635870183545811 },
                        Converters.ConvertBytesToLongs(ar1));

                    long[] ar2 = new long[ar1.Length / 8];
                    Converters.ConvertBytesToLongs(ar1, ar2);
                    CollectionAssert.AreEqual(new long[] { 1729173373979978252, 1203752356870097417, -6346635870183545811 }, ar2);

                    CollectionAssert.AreEqual(new long[] { 723135600261441315, 2606758127120682184 },
                        Converters.ConvertBytesToLongs(ar1, 2, 16));

                    long[] ar3 = new long[2];
                    Converters.ConvertBytesToLongs(ar1, 2, 16, ar3);
                    CollectionAssert.AreEqual(new long[] { 723135600261441315, 2606758127120682184 }, ar3);

                    long[] ar4 = new long[4];
                    Converters.ConvertBytesToLongs(ar1, 2, 16, ar4, 2);
                    CollectionAssert.AreEqual(new long[] { 0, 0, 723135600261441315, 2606758127120682184 }, ar4);
                }

                {
                    CollectionAssert.AreEqual(new long[] { 930595473260216087, 651553051661218832, 3252994547520171175 },
                        Converters.ConvertBytesToLongsSwapOrder(ar1));

                    long[] ar2 = new long[ar1.Length / 8];
                    Converters.ConvertBytesToLongsSwapOrder(ar1, ar2);
                    CollectionAssert.AreEqual(new long[] { 930595473260216087, 651553051661218832, 3252994547520171175 }, ar2);

                    CollectionAssert.AreEqual(new long[] { 2569027897743837450, -4031736967974605532 },
                        Converters.ConvertBytesToLongsSwapOrder(ar1, 2, 16));

                    long[] ar3 = new long[2];
                    Converters.ConvertBytesToLongsSwapOrder(ar1, 2, 16, ar3);
                    CollectionAssert.AreEqual(new long[] { 2569027897743837450, -4031736967974605532 }, ar3);

                    long[] ar4 = new long[4];
                    Converters.ConvertBytesToLongsSwapOrder(ar1, 2, 16, ar4, 2);
                    CollectionAssert.AreEqual(new long[] { 0, 0, 2569027897743837450, -4031736967974605532 }, ar4);
                }

                CollectionAssert.AreEqual(new short[] { -5620, -22749, 16901, 6143, 2569, 3272, -27036, 4276, 9261, 20213, 14413, -22548 },
                    Converters.ConvertBytesToShorts(ar1));

                Assert.AreEqual("0CEA23A7-0542FF17-090AC80C-6496B410-2D24F54E-4D38ECA7",
                    Converters.ConvertBytesToHexString(ar1, true));
                Assert.AreEqual("0CEA23A70542FF17090AC80C6496B4102D24F54E4D38ECA7",
                    Converters.ConvertBytesToHexString(ar1, false));

                {
                    CollectionAssert.AreEqual(new uint[] { 2804148748, 402604549, 214436361, 280270436, 1324688429, 2817275981 },
                        Converters.ConvertBytesToUInts(ar1));

                    uint[] ar2 = new uint[ar1.Length / 4];
                    Converters.ConvertBytesToUInts(ar1, ar2);
                    CollectionAssert.AreEqual(new uint[] { 2804148748, 402604549, 214436361, 280270436, 1324688429, 2817275981 }, ar2);

                    CollectionAssert.AreEqual(new uint[] { 1107666723, 168368127 },
                        Converters.ConvertBytesToUInts(ar1, 2, 8));

                    uint[] ar3 = new uint[2];
                    Converters.ConvertBytesToUInts(ar1, 2, 8, ar3);
                    CollectionAssert.AreEqual(new uint[] { 1107666723, 168368127 }, ar3);

                    uint[] ar4 = new uint[4];
                    Converters.ConvertBytesToUInts(ar1, 2, 8, ar4, 2);
                    CollectionAssert.AreEqual(new uint[] { 0, 0, 1107666723, 168368127 }, ar4);
                }

                {
                    CollectionAssert.AreEqual(new uint[] { 216671143, 88276759, 151701516, 1687598096, 757396814, 1295576231 },
                        Converters.ConvertBytesToUIntsSwapOrder(ar1));

                    uint[] ar2 = new uint[ar1.Length / 4];
                    Converters.ConvertBytesToUIntsSwapOrder(ar1, ar2);
                    CollectionAssert.AreEqual(new uint[] { 216671143, 88276759, 151701516, 1687598096, 757396814, 1295576231 }, ar2);

                    CollectionAssert.AreEqual(new uint[] { 598148418, 4279699722 },
                        Converters.ConvertBytesToUIntsSwapOrder(ar1, 2, 8));

                    uint[] ar3 = new uint[2];
                    Converters.ConvertBytesToUIntsSwapOrder(ar1, 2, 8, ar3);
                    CollectionAssert.AreEqual(new uint[] { 598148418, 4279699722 }, ar3);

                    uint[] ar4 = new uint[4];
                    Converters.ConvertBytesToUIntsSwapOrder(ar1, 2, 8, ar4, 2);
                    CollectionAssert.AreEqual(new uint[] { 0, 0, 598148418, 4279699722 }, ar4);
                }

                {
                    CollectionAssert.AreEqual(new ulong[] { 1729173373979978252, 1203752356870097417, 12100108203526005805 },
                        Converters.ConvertBytesToULongs(ar1));

                    ulong[] ar2 = new ulong[ar1.Length / 8];
                    Converters.ConvertBytesToULongs(ar1, ar2);
                    CollectionAssert.AreEqual(new ulong[] { 1729173373979978252, 1203752356870097417, 12100108203526005805 }, ar2);

                    CollectionAssert.AreEqual(new ulong[] { 723135600261441315, 2606758127120682184 },
                        Converters.ConvertBytesToULongs(ar1, 2, 16));

                    ulong[] ar3 = new ulong[2];
                    Converters.ConvertBytesToULongs(ar1, 2, 16, ar3);
                    CollectionAssert.AreEqual(new ulong[] { 723135600261441315, 2606758127120682184 }, ar3);

                    ulong[] ar4 = new ulong[4];
                    Converters.ConvertBytesToULongs(ar1, 2, 16, ar4, 2);
                    CollectionAssert.AreEqual(new ulong[] { 0, 0, 723135600261441315, 2606758127120682184 }, ar4);
                }

                {
                    CollectionAssert.AreEqual(new ulong[] { 930595473260216087, 651553051661218832, 3252994547520171175 },
                        Converters.ConvertBytesToULongsSwapOrder(ar1));

                    ulong[] ar2 = new ulong[ar1.Length / 8];
                    Converters.ConvertBytesToULongsSwapOrder(ar1, ar2);
                    CollectionAssert.AreEqual(new ulong[] { 930595473260216087, 651553051661218832, 3252994547520171175 }, ar2);

                    CollectionAssert.AreEqual(new ulong[] { 2569027897743837450, 14415007105734946084 },
                        Converters.ConvertBytesToULongsSwapOrder(ar1, 2, 16));

                    ulong[] ar3 = new ulong[2];
                    Converters.ConvertBytesToULongsSwapOrder(ar1, 2, 16, ar3);
                    CollectionAssert.AreEqual(new ulong[] { 2569027897743837450, 14415007105734946084 }, ar3);

                    ulong[] ar4 = new ulong[4];
                    Converters.ConvertBytesToULongsSwapOrder(ar1, 2, 16, ar4, 2);
                    CollectionAssert.AreEqual(new ulong[] { 0, 0, 2569027897743837450, 14415007105734946084 }, ar4);
                }

                CollectionAssert.AreEqual(new ushort[] { 59916, 42787, 16901, 6143, 2569, 3272, 38500, 4276, 9261, 20213, 14413, 42988 },
                    Converters.ConvertBytesToUShorts(ar1));

                CollectionAssert.AreEqual(new byte[] { 116, 0, 5, 1, 25, 1, 120, 0, 49, 0, 52, 0 },
                    Converters.ConvertStringToBytes("tąęx14"));

                CollectionAssert.AreEqual(new byte[] { 116, 63, 63, 63, 63, 63, 63, 120, 120, 49, 50, 51, 52 },
                    Converters.ConvertStringToBytes("tąąąęęęxx1234", Encoding.ASCII));

                {
                    CollectionAssert.AreEqual(new byte[] { 113, 61, 54, 66 },
                        Converters.ConvertFloatToBytes(45.56f));

                    byte[] ar2 = new byte[4];
                    Converters.ConvertFloatToBytes(45.56f, ar2, 0);
                    CollectionAssert.AreEqual(new byte[] { 113, 61, 54, 66 }, ar2);
                }

                Assert.AreEqual(-2.27476712E-15f, Converters.ConvertBytesToFloat(ar1, 0));

                Assert.AreEqual(6.498713E-37f, Converters.ConvertIntToFloat(56435654));

                Assert.AreEqual(1225110633, Converters.ConvertFloatToInt(547654.54f));


                //
                {
                    char c = 'a';
                    byte[] r = new byte[2];
                    Converters.ConvertCharToBytes(c, r, 0);
                    Assert.AreEqual(c, BitConverter.ToChar(r, 0));
                }

                {
                    short c = -30567;
                    byte[] r = new byte[2];
                    Converters.ConvertShortToBytes(c, r, 0);
                    Assert.AreEqual(c, BitConverter.ToInt16(r, 0));
                }

                {
                    ushort c = 10567;
                    byte[] r = new byte[2];
                    Converters.ConvertUShortToBytes(c, r, 0);
                    Assert.AreEqual(c, BitConverter.ToUInt16(r, 0));
                }

                {
                    int c = -45345345;
                    byte[] r = new byte[4];
                    Converters.ConvertIntToBytes(c, r, 0);
                    Assert.AreEqual(c, BitConverter.ToInt32(r, 0));
                }

                {
                    uint c = 45345345;
                    byte[] r = new byte[4];
                    Converters.ConvertUIntToBytes(c, r, 0);
                    Assert.AreEqual(c, BitConverter.ToUInt32(r, 0));
                }

                {
                    long c = 45387687845345;
                    byte[] r = new byte[8];
                    Converters.ConvertLongToBytes(c, r, 0);
                    Assert.AreEqual(c, BitConverter.ToInt64(r, 0));
                }

                {
                    ulong c = 45387687845345;
                    byte[] r = new byte[8];
                    Converters.ConvertULongToBytes(c, r, 0);
                    Assert.AreEqual(c, BitConverter.ToUInt64(r, 0));
                }

                {
                    double c = 45.54654;
                    byte[] r = new byte[8];
                    Converters.ConvertDoubleToBytes(c, r, 0);
                    Assert.AreEqual(c, BitConverter.ToDouble(r, 0));
                }
            }

            {
                // HashUtils.ConvertToBytes()

                MersenneTwister ms1 = new MersenneTwister(5687);
                MersenneTwister ms2 = new MersenneTwister(5687);

                CollectionAssert.AreEqual(new byte[] { ms1.NextByte() }, Converters.ConvertToBytes(ms2.NextByte()));
                CollectionAssert.AreEqual(BitConverter.GetBytes(ms1.NextShort()), Converters.ConvertToBytes(ms2.NextShort()));
                CollectionAssert.AreEqual(BitConverter.GetBytes(ms1.NextUShort()), Converters.ConvertToBytes(ms2.NextUShort()));
                CollectionAssert.AreEqual(BitConverter.GetBytes(ms1.NextChar()), Converters.ConvertToBytes(ms2.NextChar()));
                CollectionAssert.AreEqual(BitConverter.GetBytes(ms1.NextInt()), Converters.ConvertToBytes(ms2.NextInt()));
                CollectionAssert.AreEqual(BitConverter.GetBytes(ms1.NextUInt()), Converters.ConvertToBytes(ms2.NextUInt()));
                CollectionAssert.AreEqual(BitConverter.GetBytes(ms1.NextLong()), Converters.ConvertToBytes(ms2.NextLong()));
                CollectionAssert.AreEqual(BitConverter.GetBytes(ms1.NextULong()), Converters.ConvertToBytes(ms2.NextULong()));
                CollectionAssert.AreEqual(BitConverter.GetBytes(ms1.NextDoubleFull()), Converters.ConvertToBytes(ms2.NextDoubleFull()));
                CollectionAssert.AreEqual(Converters.ConvertFloatToBytes(ms1.NextFloatFull()), Converters.ConvertToBytes(ms2.NextFloatFull()));

                CollectionAssert.AreEqual(Converters.ConvertStringToBytes(ms1.NextString(20)), Converters.ConvertToBytes(ms2.NextString(20)));

                CollectionAssert.AreEqual(ms1.NextBytes(20), Converters.ConvertToBytes(ms2.NextBytes(20)));
                CollectionAssert.AreEqual(Converters.ConvertShortsToBytes(ms1.NextShorts(20)), Converters.ConvertToBytes(ms2.NextShorts(20)));
                CollectionAssert.AreEqual(Converters.ConvertUShortsToBytes(ms1.NextUShorts(20)), Converters.ConvertToBytes(ms2.NextUShorts(20)));
                CollectionAssert.AreEqual(Converters.ConvertCharsToBytes(ms1.NextChars(20)), Converters.ConvertToBytes(ms2.NextChars(20)));
                CollectionAssert.AreEqual(Converters.ConvertIntsToBytes(ms1.NextInts(20)), Converters.ConvertToBytes(ms2.NextInts(20)));
                CollectionAssert.AreEqual(Converters.ConvertUIntsToBytes(ms1.NextUInts(20)), Converters.ConvertToBytes(ms2.NextUInts(20)));
                CollectionAssert.AreEqual(Converters.ConvertLongsToBytes(ms1.NextLongs(20)), Converters.ConvertToBytes(ms2.NextLongs(20)));
                CollectionAssert.AreEqual(Converters.ConvertULongsToBytes(ms1.NextULongs(20)), Converters.ConvertToBytes(ms2.NextULongs(20)));
                CollectionAssert.AreEqual(Converters.ConvertDoublesToBytes(ms1.NextDoublesFull(20)), Converters.ConvertToBytes(ms2.NextDoublesFull(20)));
                CollectionAssert.AreEqual(Converters.ConvertFloatsToBytes(ms1.NextFloatsFull(20)), Converters.ConvertToBytes(ms2.NextFloatsFull(20)));
            }

            {
                // Swap order

                {
                    ulong x1 = 0xFE12345678ABCD09;
                    byte[] b1 = BitConverter.GetBytes(x1);
                    ulong x2 = Converters.SwapOrder(x1);
                    byte[] b2 = BitConverter.GetBytes(x2).Reverse().ToArray();
                    CollectionAssert.AreEqual(b1, b2);
                }

                {
                    long x1 = -0x7E12345608ABCDF9;
                    byte[] b1 = BitConverter.GetBytes(x1);
                    long x2 = Converters.SwapOrder(x1);
                    byte[] b2 = BitConverter.GetBytes(x2).Reverse().ToArray();
                    CollectionAssert.AreEqual(b1, b2);
                }

                {
                    uint x1 = 0x78ABCD09;
                    byte[] b1 = BitConverter.GetBytes(x1);
                    uint x2 = Converters.SwapOrder(x1);
                    byte[] b2 = BitConverter.GetBytes(x2).Reverse().ToArray();
                    CollectionAssert.AreEqual(b1, b2);
                }

                {
                    int x1 = -0x7E123456;
                    byte[] b1 = BitConverter.GetBytes(x1);
                    int x2 = Converters.SwapOrder(x1);
                    byte[] b2 = BitConverter.GetBytes(x2).Reverse().ToArray();
                    CollectionAssert.AreEqual(b1, b2);
                }
            }
        }
    }
}
