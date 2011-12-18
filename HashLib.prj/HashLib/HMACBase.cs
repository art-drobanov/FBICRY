using System;

namespace HashLib
{
	abstract class HMACBase : Hash, IHMAC
	{
		protected bool m_bTransforming;
		private byte[] m_key;

		public HMACBase(int a_hashSize, int a_blockSize)
			: base(a_hashSize, a_blockSize)
		{
			m_key = new byte[0];
		}

		#region IHMAC Members

		public virtual byte[] Key
		{
			get { return m_key; }
			set { m_key = value; }
		}

		#endregion
	}
}