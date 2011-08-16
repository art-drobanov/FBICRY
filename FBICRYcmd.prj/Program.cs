using System;
using System.IO;
using System.Text;

using CRYFORCE.Engine;

using EventArgsUtilities;

namespace FBICRYcmd
{
	class Program
	{
		/// <summary>
		/// Обработчик события "Изменился прогресс обработки"
		/// </summary>
		private static void OnProgressChanged(object sender, EventArgs_Generic<ProgressChangedArg> e)
		{
			Console.WriteLine("Process {0}, Progress: {1}", e.TargetObject.ProcessDescription, (int)e.TargetObject.ProcessProgress);
		}

		private static void Main(string[] args)
		{
			//var ecdhP521 = new EcdhP521();
			//int maxIters = 100000;

			//string HmacKey = null;

			//string personString = "DrAF";
			//string seed = "Seed string";

			//if(ecdhP521.Initialize(personString, maxIters, i =>
			//                                                {
			//                                                    Console.WriteLine(i);
			//                                                    return i;
			//                                                }, seed, null, HmacKey))
			//{
			//    Console.WriteLine("Personalize OK!");
			//    File.WriteAllText("PublicKey.txt", ecdhP521.PublicKey);
			//    File.WriteAllText("PrivateKey.txt", ecdhP521.PrivateKey);
			//}

			//Console.ReadLine();

			//

			bool workInMemory = true;
			var bitSplitter = new BitSplitter(workInMemory);
			bitSplitter.ProgressChanged += OnProgressChanged;

			//// Метод тестирования - даем 255 в очередной позиции, и после разбиения должны получить в выходном массиве
			//// степень двойки, соотв. позиции 255 в массиве
			//var b1 = new byte[8] {0, 0, 255, 0, 0, 0, 0, 0}; //Encoding.Unicode.GetBytes("DrAF");
			//var b2 = new byte[8];
			//var b3 = new byte[8];

			//if(!bitSplitter.Split8Bytes(b1, b2))
			//{
			//    throw new Exception("bitSplitter.Split8Bytes() failed!");
			//}

			//if(!bitSplitter.Split8Bytes(b2, b3))
			//{
			//    throw new Exception("bitSplitter.Glue8Bytes() failed!");
			//}

			//string s1 = Encoding.Unicode.GetString(b1);
			//string s2 = Encoding.Unicode.GetString(b2);
			//string s3 = Encoding.Unicode.GetString(b3);

			//Utilities.DoD_5220_22_E(OnProgressChanged, "storage.rar");

			//

			//StreamCryptoWrapperTest streamCryptoWrapperTest =  new StreamCryptoWrapperTest();
			//streamCryptoWrapperTest.SetUp();
			//streamCryptoWrapperTest.StreamCryptoWrapperBaseTest();

			//int z = 5;

			//

			Stream inputStream = new FileStream("input.txt", FileMode.Open, FileAccess.Read);
			Stream outputStream = new FileStream("output.txt", FileMode.Create, FileAccess.Write);
			bitSplitter.SplitToBitstream(inputStream, outputStream);
			bitSplitter.Dispose();
			inputStream.Close();
			outputStream.Close();

			Stream inputStream2 = new FileStream("output.txt", FileMode.Open, FileAccess.Read);
			Stream outputStream2 = new FileStream("input2.txt", FileMode.Create, FileAccess.Write);
			bitSplitter.UnsplitFromBitstream(inputStream2, outputStream2);
			bitSplitter.Dispose();
			inputStream2.Close();
			outputStream2.Close();
		}
	}
}