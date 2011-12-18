using System;
using System.Security.Cryptography;

namespace HashLib.Crypto.BuildIn
{
	class RIPEMD160Managed : HashCryptoBuildIn, IHasHMACBuildIn
	{
		public RIPEMD160Managed()
			: base(new System.Security.Cryptography.RIPEMD160Managed(), 64)
		{
		}

		public override HMAC GetBuildHMAC()
		{
			return new HMACRIPEMD160();
		}
	}
}