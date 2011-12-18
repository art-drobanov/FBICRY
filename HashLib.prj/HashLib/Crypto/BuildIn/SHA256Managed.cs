using System;
using System.Security.Cryptography;

namespace HashLib.Crypto.BuildIn
{
	class SHA256Managed : HashCryptoBuildIn, IHasHMACBuildIn
	{
		public SHA256Managed()
			: base(new System.Security.Cryptography.SHA256Managed(), 64)
		{
		}

		public override HMAC GetBuildHMAC()
		{
			return new HMACSHA256();
		}
	}
}