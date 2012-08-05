using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace RSACryptoPad
{
	public class EncryptionThread
	{
		private ContainerControl containerControl = null;
		private Delegate finishedProcessDelegate = null;
		private Delegate updateTextDelegate = null;

		public void Encrypt(object inputObject)
		{
			object[] inputObjects = (object[])inputObject;
			containerControl = (Form)inputObjects[0];
			finishedProcessDelegate = (Delegate)inputObjects[1];
			updateTextDelegate = (Delegate)inputObjects[2];
			string encryptedString = EncryptString((string)inputObjects[3], (int)inputObjects[4], (string)inputObjects[5]);
			containerControl.Invoke(updateTextDelegate, new object[] {encryptedString});
			containerControl.Invoke(finishedProcessDelegate);
		}

		public void Decrypt(object inputObject)
		{
			object[] inputObjects = (object[])inputObject;
			containerControl = (Form)inputObjects[0];
			finishedProcessDelegate = (Delegate)inputObjects[1];
			updateTextDelegate = (Delegate)inputObjects[2];
			string decryptedString = DecryptString((string)inputObjects[3], (int)inputObjects[4], (string)inputObjects[5]);
			containerControl.Invoke(updateTextDelegate, new object[] {decryptedString});
			containerControl.Invoke(finishedProcessDelegate);
		}

		/// <summary>
		/// Сжатие данных, находящихся во входном потоке
		/// </summary>
		/// FBICRY
		/// <param name="stream">Поток с данными для сжатия</param>
		/// <returns>Поток со сжатыми данными</returns>
		public Stream Compress(Stream stream)
		{
			byte[] compressed;
			stream.Position = 0;
			using(var outStream = new MemoryStream())
			{
				using(var tinyStream = new GZipStream(outStream, CompressionMode.Compress, true))
				{
					stream.CopyTo(tinyStream);
					tinyStream.Flush();
				}
				outStream.Position = 0;
				compressed = outStream.ToArray();
			}
			return new MemoryStream(compressed);
		}

		/// <summary>
		/// Получение данных из сжатого состояния
		/// </summary>
		/// FBICRY
		/// <param name="stream">Поток со сжатыми данными</param>
		/// <returns>Поток с исходными данными</returns>
		public Stream Uncompress(Stream stream)
		{
			byte[] output;
			stream.Position = 0;
			using(var bigStream = new GZipStream(stream, CompressionMode.Decompress, true))
			using(var bigStreamOut = new MemoryStream())
			{
				bigStream.CopyTo(bigStreamOut);
				bigStreamOut.Position = 0;
				output = bigStreamOut.ToArray();
			}
			return new MemoryStream(output);
		}

		/// <summary>
		/// Выравнивание потока по указанной границе
		/// </summary>
		/// <param name="input">Входной поток</param>
		/// <param name="align">Выравнивание</param>
		/// <returns>Выровненный поток</returns>
		public Stream Align(Stream input, int align)
		{
			MemoryStream output = new MemoryStream();
			input.Seek(0, SeekOrigin.Begin);
			input.CopyTo(output);
			output.SetLength(output.Length + (align - output.Length % align));
			output.Seek(0, SeekOrigin.End);
			uint dataSize = (uint)input.Length;
			BinaryWriter bw = new BinaryWriter(output);
			bw.Write(dataSize);
			bw.Flush();
			return output;
		}

		/// <summary>
		/// Извлечение выровненного потока
		/// </summary>
		/// <param name="input">Входной поток</param>
		/// <returns>Выровненный поток</returns>
		public Stream DeAlign(Stream input)
		{
			input.Position = input.Length - sizeof(uint);
			BinaryReader br = new BinaryReader(input);
			uint dataSize = br.ReadUInt32();
			input.SetLength(dataSize);
			MemoryStream output = new MemoryStream();
			input.Seek(0, SeekOrigin.Begin);
			input.CopyTo(output);
			output.Flush();
			return output;
		}

		public string EncryptString(string inputString, int dwKeySize, string xmlString)
		{
			RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider(dwKeySize);
			rsaCryptoServiceProvider.FromXmlString(xmlString);
			int keySize = dwKeySize / 8;
			byte[] bytesInput = Encoding.UTF8.GetBytes(inputString);

			// Сжатие GZip...
			MemoryStream input = new MemoryStream(bytesInput);
			MemoryStream output = (MemoryStream)Compress(input);
			output.Seek(0, SeekOrigin.Begin);
			byte[] bytes = output.ToArray();
			input.Close();
			output.Close();

			//...и выравнивание по границе 4 байта
			MemoryStream ms = new MemoryStream(bytes);
			MemoryStream output2 = (MemoryStream)Align(ms, 4);
			output2.Seek(0, SeekOrigin.Begin);
			var alignedBytes = output2.ToArray();
			ms.Close();
			output2.Close();

			// The hash function in use by the .NET RSACryptoServiceProvider here is SHA1
			// int maxLength = ( keySize ) - 2 - ( 2 * SHA1.Create().ComputeHash( rawBytes ).Length );
			int maxLength = keySize - 42;
			int dataLength = alignedBytes.Length;
			int iterations = dataLength / maxLength;
			StringBuilder stringBuilder = new StringBuilder();
			for(int i = 0; i <= iterations; i++)
			{
				byte[] tempBytes = new byte[(dataLength - maxLength * i > maxLength) ? maxLength : dataLength - maxLength * i];
				Buffer.BlockCopy(alignedBytes, maxLength * i, tempBytes, 0, tempBytes.Length);
				byte[] encryptedBytes = rsaCryptoServiceProvider.Encrypt(tempBytes, true);
				stringBuilder.Append(Convert.ToBase64String(encryptedBytes));
			}
			return stringBuilder.ToString();
		}

		public string DecryptString(string inputString, int dwKeySize, string xmlString)
		{
			RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider(dwKeySize);
			rsaCryptoServiceProvider.FromXmlString(xmlString);
			int base64BlockSize = ((dwKeySize / 8) % 3 != 0) ? (((dwKeySize / 8) / 3) * 4) + 4 : ((dwKeySize / 8) / 3) * 4;
			int iterations = inputString.Length / base64BlockSize;
			ArrayList arrayList = new ArrayList();
			for(int i = 0; i < iterations; i++)
			{
				byte[] encryptedBytes = Convert.FromBase64String(inputString.Substring(base64BlockSize * i, base64BlockSize));
				arrayList.AddRange(rsaCryptoServiceProvider.Decrypt(encryptedBytes, true));
			}

			byte[] bytesInput = arrayList.ToArray(Type.GetType("System.Byte")) as byte[];

			MemoryStream ms = new MemoryStream(bytesInput);
			MemoryStream output2 = (MemoryStream)DeAlign(ms);
			output2.Seek(0, SeekOrigin.Begin);
			var bytesInput2 = output2.ToArray();
			ms.Close();
			output2.Close();

			// Декомпрессия GZip и снятие выравнивания потока по границе байт
			MemoryStream input = new MemoryStream(bytesInput2);
			MemoryStream output = (MemoryStream)Uncompress(input);
			output.Seek(0, SeekOrigin.Begin);
			byte[] bytes = output.ToArray();
			input.Close();
			output.Close();
			return Encoding.UTF8.GetString(bytes);
		}
	}
}