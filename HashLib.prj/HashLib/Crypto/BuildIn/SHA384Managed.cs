using System;
using System.Security.Cryptography;

namespace HashLib.Crypto.BuildIn
{
	class SHA384Managed : HashCryptoBuildIn, IHasHMACBuildIn
	{
		public SHA384Managed()
			: base(new System.Security.Cryptography.SHA384Managed(), 128)
		{
		}

		public override HMAC GetBuildHMAC()
		{
			return new HMACSHA384();
		}
	}
}