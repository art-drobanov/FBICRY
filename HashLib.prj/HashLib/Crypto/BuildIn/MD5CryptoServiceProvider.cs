using System;
using System.Security.Cryptography;

namespace HashLib.Crypto.BuildIn
{
	class MD5CryptoServiceProvider : HashCryptoBuildIn, IHasHMACBuildIn
	{
		public MD5CryptoServiceProvider()
			: base(new System.Security.Cryptography.MD5CryptoServiceProvider(), 64)
		{
		}

		public override HMAC GetBuildHMAC()
		{
			return new HMACMD5();
		}
	}
}