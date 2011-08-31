using HashLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace HashLibTest
{
    [TestClass()]
    public class IEnumerableExtensionsTest
    {
        [TestMethod()]
        public void IEnumerableExtensions_Take()
        {
            {
                byte[] ar1 = new byte[23];
                for (byte i = 0; i < 23; i++)
                    ar1[i] = i;

                byte[] ar2 = ar1.Take(5, 8).ToArray();

                for (int i=0; i<ar2.Length; i++)
                    Assert.AreEqual(ar1[5+i], ar2[i]); 
            }

            {
                int[] ar1 = new int[23];
                for (int i = 0; i < 23; i++)
                    ar1[i] = i;

                int[] ar2 = ar1.Take(5, 8).ToArray();

                for (int i = 0; i < ar2.Length; i++)
                    Assert.AreEqual(ar1[5 + i], ar2[i]); 
            }
        }
    }
}
