using HashLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HashLibTest
{
    [TestClass()]
    public class TypeExtensionsTest
    {
        private interface IFalse
        {
        }

        [TestMethod()]
        public void TypeExtensionsTest_IsImplementInterface()
        {
            Assert.IsTrue(HashFactory.Crypto.CreateMD5().GetType().IsImplementingInterface(typeof(IHash)));
            Assert.IsFalse(HashFactory.Crypto.CreateMD5().GetType().IsImplementingInterface(typeof(IFalse)));
            Assert.IsTrue(typeof(IHMAC).IsImplementingInterface(typeof(IHash)));
            Assert.IsFalse(typeof(IHash).IsImplementingInterface(typeof(IFalse))); 
            Assert.IsTrue(typeof(IHash).IsImplementingInterface(typeof(IHash)));
            Assert.IsFalse(typeof(IHash).IsImplementingInterface(typeof(IHMAC)));
            Assert.IsFalse(typeof(IFalse).IsImplementingInterface(typeof(IHash)));
        }

        [TestMethod()]
        public void TypeExtensionsTest_IsDerivedFrom()
        {
            Assert.IsTrue(HashFactory.Crypto.CreateMD5().
                GetType().IsDerivedFrom(typeof(Object)));
            Assert.IsFalse(HashFactory.Crypto.CreateMD5().
                GetType().IsDerivedFrom(typeof(string)));
            Assert.IsTrue(HashFactory.Crypto.CreateMD5().
                GetType().IsDerivedFrom(typeof(object)));

            Assert.IsTrue(!typeof(Object).IsDerivedFrom(
                HashFactory.Crypto.CreateMD5().GetType()));
            Assert.IsFalse(typeof(string).IsDerivedFrom(
                HashFactory.Crypto.CreateMD5().GetType()));
            Assert.IsTrue(!typeof(object).IsDerivedFrom(
                HashFactory.Crypto.CreateMD5().GetType()));
        }
    }
}
