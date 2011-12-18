using System;
using System.Diagnostics;
using System.Security.Cryptography;

namespace HashLib
{
	class HashAlgorithmWrapper : HashAlgorithm
	{
		private readonly IHash m_hash;

		public HashAlgorithmWrapper(IHash a_hash)
		{
			Debug.Assert(a_hash != null);

			m_hash = a_hash;
			HashSizeValue = a_hash.HashSize * 8;
		}

		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			Debug.Assert(array != null);
			Debug.Assert(cbSize >= 0);
			Debug.Assert(ibStart >= 0);
			Debug.Assert(ibStart + cbSize <= array.Length);

			m_hash.TransformBytes(array, ibStart, cbSize);
		}

		protected override byte[] HashFinal()
		{
			HashValue = m_hash.TransformFinal().GetBytes();
			return HashValue;
		}

		public override void Initialize()
		{
			m_hash.Initialize();
		}
	}
}