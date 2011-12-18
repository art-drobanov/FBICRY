using System;
using System.Security.Cryptography;

namespace HashLib.Crypto.BuildIn
{
	class SHA512Managed : HashCryptoBuildIn, IHasHMACBuildIn
	{
		public SHA512Managed()
			: base(new System.Security.Cryptography.SHA512Managed(), 128)
		{
		}

		public override HMAC GetBuildHMAC()
		{
			return new HMACSHA512();
		}
	}
}