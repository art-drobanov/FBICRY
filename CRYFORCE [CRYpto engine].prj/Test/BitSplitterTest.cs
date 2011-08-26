using System;
using System.Linq;

using NUnit.Framework;

namespace CRYFORCE.Engine.Test
{
	[TestFixture]
	public class BitSplitterTest
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			SetUpIsOK = true; // Указываем, что была произведена установка
		}

		#endregion

		/// <summary>Была произведена установка?</summary>
		public bool SetUpIsOK { get; set; }

		/// <summary>
		/// Тест корректности работы класса разбиения/склеивания файлов на битовые потоки
		/// </summary>
		[Test]
		public void BitSplitterBaseTest()
		{
			if(!SetUpIsOK)
			{
				SetUp();
			}

			try
			{
				// Создаем экземпляр класса для тестирования
				var bitSplitter = new BitSplitter();

				// Метод тестирования - даем 255 в очередной позиции, и после разбиения на биты
				// должны получить в выходном массиве степень двойки, соотв. позиции 255 в массиве
				for(int i = 0; i < 8; i++)
				{
					var b1 = new byte[8];
					var b2 = new byte[8];
					var b3 = new byte[8];

					b1[i] = 0xFF;

					bitSplitter.Split8Bytes(b1, b2);
					
					// Вычисляем ожидаемое значение в позициях байт после разбиения...
					var expectedValue = (byte)(1 << i);

					foreach(byte b in b2)
					{
						if(b != expectedValue)
						{
							throw new Exception("BitSplitterTest: (b != expectedValue)!");
						}
					}

					bitSplitter.Split8Bytes(b2, b3);

					if(!b1.SequenceEqual(b3))
					{
						throw new Exception("BitSplitterTest: b1.SequenceEqual(b3) failed!");
					}
				}
			} // try
			catch(Exception e)
			{
				Console.WriteLine(e.ToString());
				Assert.Fail();
			}
		}
	}
}