using System;

namespace HashLib.Crypto.BuildIn
{
	class SHA384Cng : HashCryptoBuildIn
	{
		public SHA384Cng()
			: base(new System.Security.Cryptography.SHA384Cng(), 128)
		{
		}
	}
}