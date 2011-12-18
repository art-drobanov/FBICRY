using System;

namespace HashLib.Crypto.BuildIn
{
	class SHA256Cng : HashCryptoBuildIn
	{
		public SHA256Cng()
			: base(new System.Security.Cryptography.SHA256Cng(), 64)
		{
		}
	}
}