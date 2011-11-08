using System;
using System.IO;
using System.Text;
using System.Linq;

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
			bool workInMemory = false;

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

			//var bitSplitter = new BitSplitter(workInMemory);
			//bitSplitter.ProgressChanged += OnProgressChanged;

			//Stream inputStream = new FileStream("input.txt", FileMode.Open, FileAccess.Read);
			//Stream outputStream = new FileStream("output.txt", FileMode.Create, FileAccess.Write);
			//bitSplitter.SplitToBitstream(inputStream, outputStream);
			//bitSplitter.Clear();
			//inputStream.Close();
			//outputStream.Close();

			//Stream inputStream2 = new FileStream("output.txt", FileMode.Open, FileAccess.Read);
			//Stream outputStream2 = new FileStream("input2.txt", FileMode.Create, FileAccess.Write);
			//bitSplitter.UnsplitFromBitstream(inputStream2, outputStream2);
			//bitSplitter.Clear();
			//inputStream2.Close();
			//outputStream2.Close();

			//bitSplitter.ClearAndClose();

			//

			var cryforce = new Cryforce();
			cryforce.ProgressChanged += OnProgressChanged;

			Stream inputStream = new FileStream("input.txt", FileMode.Open, FileAccess.Read);
			Stream outputStream = new FileStream("input.txt.fbicry", FileMode.Create, FileAccess.Write);

			cryforce.DoubleRijndael(inputStream, Encoding.Unicode.GetBytes("password1"), Encoding.Unicode.GetBytes("password2"), outputStream, true);

			inputStream.Close();
			outputStream.Flush();
			outputStream.Close();

			Stream inputStream2 = new FileStream("input.txt.fbicry", FileMode.Open, FileAccess.Read);
			Stream outputStream2 = new FileStream("input2.txt", FileMode.Create, FileAccess.Write);

			cryforce.DoubleRijndael(inputStream2, Encoding.Unicode.GetBytes("password1"), Encoding.Unicode.GetBytes("password2"), outputStream2, false);

			inputStream2.Close();
			outputStream2.Flush();
			outputStream2.Close();
		}
	}
}