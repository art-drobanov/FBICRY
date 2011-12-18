using System;
using System.Security.Cryptography;

namespace HashLib.Crypto.BuildIn
{
	class SHA1Managed : HashCryptoBuildIn, IHasHMACBuildIn
	{
		public SHA1Managed()
			: base(new System.Security.Cryptography.SHA1Managed(), 64)
		{
		}

		public override HMAC GetBuildHMAC()
		{
			return new HMACSHA1(new byte[0], true);
		}
	}
}