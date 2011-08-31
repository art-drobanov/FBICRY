using System;
using HashLibQualityTest.DataSourceRows;
using HashLib;
using System.Diagnostics;
using System.Linq;

namespace HashLibQualityTest
{
    // TODO: test na ciagi dluzsze niz 2 gb, jednorazowo, dluzsze nic 4 gb, 

    public class SpeedTest
    {
        public double Measure(int a_bytes, Action a_action)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();

            for (int i = 0; i < 10; i++) 
                a_action();

            sw.Stop();

            return (double)a_bytes / 1024 / 1024 * 10 / ((double)sw.ElapsedMilliseconds / 1000);
        }

        public void Test(SpeedTestDataSourceRow a_row)
        {    
            Test(a_row.HashFunction, a_row, a_row.HashFunction is IFastHashCodes);
        }

        public void Test(IHash a_hash, SpeedTestDataSourceRow a_row, bool a_bFast)
        {
            Random r = new Random();

            int length = 10000000;

            {
                byte[] ar = new byte[length];
                r.NextBytes(ar);

                if (a_bFast)
                {
                    a_hash.ComputeByte(ar[0]);

                    a_row.ByteSpeed = Measure(ar.Length, () =>
                    {
                        for (int i = 0; i < ar.Length; i++)
                            a_hash.ComputeByte(ar[i]);
                    });
                }

                a_hash.ComputeBytes(ar.Take(100).ToArray());

                a_row.BytesSpeed = Measure(ar.Length, () =>
                {
                    a_hash.ComputeBytes(ar);
                });
            }

            {
                if (a_bFast)
                {
                    char[] ar = (from n in Enumerable.Range(0, length / 2) select (char)r.Next()).ToArray();

                    a_hash.ComputeChar(ar[0]);

                    a_row.CharSpeed = Measure(ar.Length * 2, () =>
                    {
                        for (int i = 0; i < ar.Length; i++)
                            a_hash.ComputeChar(ar[i]);
                    });

                    a_hash.ComputeChars(ar.Take(100).ToArray());

                    a_row.CharsSpeed = Measure(ar.Length * 2, () =>
                    {
                        a_hash.ComputeChars(ar);
                    });
                }
            }

            {
                if (a_bFast)
                {
                    short[] ar = (from n in Enumerable.Range(0, length / 2) select (short)r.Next()).ToArray();

                    a_hash.ComputeShort(ar[0]);

                    a_row.ShortSpeed = Measure(ar.Length * 2, () =>
                    {
                        for (int i = 0; i < ar.Length; i++)
                            a_hash.ComputeShort(ar[i]);
                    });

                    a_hash.ComputeShorts(ar.Take(100).ToArray());

                    a_row.ShortsSpeed = Measure(ar.Length * 2, () =>
                    {
                        a_hash.ComputeShorts(ar);
                    });
                }
            }

            {
                if (a_bFast)
                {
                    ushort[] ar = (from n in Enumerable.Range(0, length / 2) select (ushort)r.Next()).ToArray();

                    a_hash.ComputeUShort(ar[0]);

                    a_row.UShortSpeed = Measure(ar.Length * 2, () =>
                    {
                        for (int i = 0; i < ar.Length; i++)
                            a_hash.ComputeUShort(ar[i]);
                    });

                    a_hash.ComputeUShorts(ar.Take(100).ToArray());

                    a_row.UShortsSpeed = Measure(ar.Length * 2, () =>
                    {
                        a_hash.ComputeUShorts(ar);
                    });
                }
            }

            {
                if (a_bFast)
                {
                    int[] ar = (from n in Enumerable.Range(0, length / 4) select r.Next()).ToArray();

                    a_hash.ComputeInt(ar[0]);

                    a_row.IntSpeed = Measure(ar.Length * 4, () =>
                    {
                        for (int i = 0; i < ar.Length; i++)
                            a_hash.ComputeInt(ar[i]);
                    });

                    a_hash.ComputeInts(ar.Take(100).ToArray());

                    a_row.IntsSpeed = Measure(ar.Length * 4, () =>
                    {
                        a_hash.ComputeInts(ar);
                    });
                }
            }

            {
                if (a_bFast)
                {
                    uint[] ar = (from n in Enumerable.Range(0, length / 4) select (uint)r.Next()).ToArray();

                    a_hash.ComputeUInt(ar[0]);

                    a_row.UIntSpeed = Measure(ar.Length * 4, () =>
                    {
                        for (int i = 0; i < ar.Length; i++)
                            a_hash.ComputeUInt(ar[i]);
                    });

                    a_hash.ComputeUInts(ar.Take(100).ToArray());

                    a_row.UIntsSpeed = Measure(ar.Length * 4, () =>
                    {
                        a_hash.ComputeUInts(ar);
                    });
                }
            }

            {
                if (a_bFast)
                {
                    long[] ar = (from n in Enumerable.Range(0, length / 8) select (long)(((ulong)r.Next() + (ulong)r.Next() * (ulong)Math.Pow(2, 32)))).ToArray();

                    a_hash.ComputeLong(ar[0]);

                    a_row.LongSpeed = Measure(ar.Length * 8, () =>
                    {
                        for (int i = 0; i < ar.Length; i++)
                            a_hash.ComputeLong(ar[i]);
                    });

                    a_hash.ComputeLongs(ar.Take(100).ToArray());

                    a_row.LongsSpeed = Measure(ar.Length * 8, () =>
                    {
                        a_hash.ComputeLongs(ar);
                    });
                }
            }

            {
                if (a_bFast)
                {
                    ulong[] ar = (from n in Enumerable.Range(0, length / 8) select ((ulong)r.Next() + (ulong)r.Next() * (ulong)Math.Pow(2, 32))).ToArray();

                    a_hash.ComputeULong(ar[0]);

                    a_row.ULongSpeed = Measure(ar.Length * 8, () =>
                    {
                        for (int i = 0; i < ar.Length; i++)
                            a_hash.ComputeULong(ar[i]);
                    });

                    a_row.ULongsSpeed = Measure(ar.Length * 8, () =>
                    {
                        a_hash.ComputeULongs(ar);
                    });
                }
            }

            {
                if (a_bFast)
                {
                    float[] ar = (from n in Enumerable.Range(0, length / 8) select (float)(r.NextDouble() * r.Next())).ToArray();

                    a_hash.ComputeFloat(ar[0]);

                    a_row.FloatSpeed = Measure(ar.Length * 4, () =>
                    {
                        for (int i = 0; i < ar.Length; i++)
                            a_hash.ComputeFloat(ar[i]);
                    });

                    a_hash.ComputeFloats(ar.Take(100).ToArray());

                    a_row.FloatsSpeed = Measure(ar.Length * 4, () =>
                    {
                        a_hash.ComputeFloats(ar);
                    });
                }
            }

            {
                if (a_bFast)
                {
                    double[] ar = (from n in Enumerable.Range(0, length / 8) select r.NextDouble() * r.Next()).ToArray();

                    a_hash.ComputeDouble(ar[0]);

                    a_row.DoubleSpeed = Measure(ar.Length * 8, () =>
                    {
                        for (int i = 0; i < ar.Length; i++)
                            a_hash.ComputeDouble(ar[i]);
                    });

                    a_hash.ComputeDoubles(ar.Take(100).ToArray());

                    a_row.DoublesSpeed = Measure(ar.Length * 8, () =>
                    {
                        a_hash.ComputeDoubles(ar);
                    });
                }
            }

            {
                if (a_bFast)
                {
                    byte[] a = new byte[length];
                    r.NextBytes(a);
                    string ar = new String((from b in a select (char)(b / 2 + 32)).ToArray());

                    a_hash.ComputeString(ar.Substring(0, 100));

                    a_row.StringSpeed = Measure(ar.Length * 2, () =>
                    {
                        a_hash.ComputeString(ar);
                    });
                }
            }
        }
    }
}