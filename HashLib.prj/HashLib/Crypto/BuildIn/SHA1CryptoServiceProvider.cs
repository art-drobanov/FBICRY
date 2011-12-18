using System;
using System.Security.Cryptography;

namespace HashLib.Crypto.BuildIn
{
	class SHA1CryptoServiceProvider : HashCryptoBuildIn, IHasHMACBuildIn
	{
		public SHA1CryptoServiceProvider()
			: base(new System.Security.Cryptography.SHA1CryptoServiceProvider(), 64)
		{
		}

		public override HMAC GetBuildHMAC()
		{
			return new HMACSHA1(new byte[0], false);
		}
	}
}