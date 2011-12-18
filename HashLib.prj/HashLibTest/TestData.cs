using System;
using System.Collections.Generic;
using System.IO;

using HashLib;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HashLibTest
{
	public class TestData
	{
		private readonly List<byte[]> m_datas = new List<byte[]>();
		private readonly IHash m_hash;
		private readonly List<byte[]> m_hashes = new List<byte[]>();

		public TestData(IHash a_hash)
		{
			m_hash = a_hash;
		}

		public int Count
		{
			get { return m_hashes.Count; }
		}

		public string GetFileName()
		{
			string basedir = AppDomain.CurrentDomain.BaseDirectory;
			string pathtemp = Path.Combine(basedir, @"..\..\TestData\" + m_hash.GetType().Name + "Test.txt");
			return Path.GetFullPath(pathtemp);
		}

		public void Load()
		{
			using(var fs = new FileStream(GetFileName(), FileMode.Open))
			{
				var sr = new StreamReader(fs);

				for(;;)
				{
					string line = sr.ReadLine();

					if(line == null)
						break;

					string[] ar = line.Split('|');

					m_datas.Add(Convert.FromBase64String(ar[0]));
					m_hashes.Add(Convert.FromBase64String(ar[1]));
				}
			}

			Assert.IsTrue(Count >= 1);
		}

		public void Save()
		{
			var fs = new FileStream(GetFileName(), FileMode.Create);

			using(var sw = new StreamWriter(fs))
			{
				var random = new MersenneTwister(4563487);

				for(int i = 0; i <= m_hash.BlockSize * 3 + 1; i++)
				{
					byte[] data = random.NextBytes(i);

					sw.Write(Convert.ToBase64String(data));
					sw.Write("|");
					sw.WriteLine(Convert.ToBase64String(m_hash.ComputeBytes(data).GetBytes()));
				}
			}
		}

		public byte[] GetHash(int a_index)
		{
			return (byte[])m_hashes[a_index].Clone();
		}

		public byte[] GetData(int a_index)
		{
			return (byte[])m_datas[a_index].Clone();
		}
	}
}