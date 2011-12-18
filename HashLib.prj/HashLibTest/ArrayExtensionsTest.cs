using System;
using System.Linq;

using HashLib;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HashLibTest
{
	[TestClass]
	public class ArrayExtensionsTest
	{
		[TestMethod]
		public void ArrayExtensions_Clear()
		{
			{
				var ar1 = new byte[23];
				new Random().NextBytes(ar1);
				ar1.Clear();
				byte[] ar2 = Enumerable.Repeat((byte)0, ar1.Length).ToArray();
				CollectionAssert.AreEqual(ar2, ar1);
			}

			{
				var ar2 = new byte[4 * 23];
				new Random().NextBytes(ar2);
				int[] ar1 = Converters.ConvertBytesToInts(ar2);
				ar1.Clear();
				int[] ar3 = Enumerable.Repeat(0, ar1.Length).ToArray();
				CollectionAssert.AreEqual(ar3, ar1);
			}
		}

		[TestMethod]
		public void ArrayExtensions_SubArray()
		{
			{
				var ar1 = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9};
				byte[] ar2 = ar1.SubArray(0, 9);
				byte[] ar3 = ar1.SubArray(0, 1);
				byte[] ar4 = ar1.SubArray(8, 1);
				byte[] ar5 = ar1.SubArray(5, 2);

				byte[] ar6 = ar1.SubArray(9, 0);
				byte[] ar7 = ar1.SubArray(8, 0);
				byte[] ar8 = ar1.SubArray(0, 0);
				byte[] ar9 = ar1.SubArray(4, 0);

				byte[] ar10 = ar1.SubArray(0, -1);
				byte[] ar11 = ar1.SubArray(5, -1);
				byte[] ar12 = ar1.SubArray(8, -1);
				byte[] ar13 = ar1.SubArray(9, -1);

				CollectionAssert.AreEqual(ar2, new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9});
				CollectionAssert.AreEqual(ar3, new byte[] {1});
				CollectionAssert.AreEqual(ar4, new byte[] {9});
				CollectionAssert.AreEqual(ar5, new byte[] {6, 7});

				CollectionAssert.AreEqual(ar6, new byte[] {});
				CollectionAssert.AreEqual(ar7, new byte[] {});
				CollectionAssert.AreEqual(ar8, new byte[] {});
				CollectionAssert.AreEqual(ar9, new byte[] {});

				CollectionAssert.AreEqual(ar10, new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9});
				CollectionAssert.AreEqual(ar11, new byte[] {6, 7, 8, 9});
				CollectionAssert.AreEqual(ar12, new byte[] {9});
				CollectionAssert.AreEqual(ar13, new byte[] {});
			}
		}
	}
}