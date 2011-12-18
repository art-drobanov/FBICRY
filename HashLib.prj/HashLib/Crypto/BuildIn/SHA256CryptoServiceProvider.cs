using System;

namespace HashLib.Crypto.BuildIn
{
	class SHA256CryptoServiceProvider : HashCryptoBuildIn
	{
		public SHA256CryptoServiceProvider()
			: base(new System.Security.Cryptography.SHA256CryptoServiceProvider(), 64)
		{
		}
	}
}