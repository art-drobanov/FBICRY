using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

using EventArgsUtilities;

namespace CRYFORCE.Engine
{
	/// <summary>
	/// Класс утилитарного назначения для Cryforce
	/// </summary>
	public static class CryforceUtilities
	{
		/// <summary>
		/// Извлечение массива байт из объекта
		/// </summary>
		/// <param name="obj">Объект.</param>
		/// <returns>Массив байт.</returns>
		public static byte[] ExtractByteArrayFromObject(Object obj)
		{
			byte[] result = null;

			if(obj is byte[])
			{
				// Либо берем байты напрямую...
				if(((byte[])obj).Length > 0) result = (byte[])obj;
			}
			else if(obj is string)
			{
				//...либо извлекаем их из строки
				if(((string)obj).Length > 0) result = Encoding.Unicode.GetBytes(((string)obj));
			}

			return result;
		}

		/// <summary>
		/// Очистка массива
		/// </summary>
		/// <typeparam name="T">Тип элементов массивов.</typeparam>
		/// <param name="array">Массив для очистки.</param>
		public static void ClearArray<T>(T[] array)
		{
			if(array == null)
			{
				return;
			}

			Array.Clear(array, 0, array.Length);
		}

		/// <summary>
		/// Объединение двух массивов в результирующий
		/// </summary>
		/// <typeparam name="T">Тип элементов массивов.</typeparam>
		/// <param name="array1">Массив №1.</param>
		/// <param name="array2">Массив №2.</param>
		/// <returns>Результирующий массив.</returns>
		public static T[] MergeArrays<T>(T[] array1, T[] array2)
		{
			if(array1 == null)
			{
				return (T[])array2.Clone();
			}

			if(array2 == null)
			{
				return (T[])array1.Clone();
			}

			var result = new T[array1.Length + array2.Length];

			Array.Copy(array1, 0, result, 0, array1.Length);
			Array.Copy(array2, 0, result, array1.Length, array2.Length);

			return result;
		}

		/// <summary>
		/// Метод копирования одного потока в другой
		/// </summary>
		/// <param name="source">Исходный поток.</param>
		/// <param name="destination">Целевой поток.</param>
		/// <param name="dataSize">Размер данных, подлежащих копированию.</param>
		/// <param name="bufferSize">Размер буфера для копирования.</param>
		public static void StreamCopy(Stream source, Stream destination, long dataSize, int bufferSize = 4096)
		{
			StreamCopy(null, source, destination, dataSize, bufferSize);
		}

		/// <summary>
		/// Метод копирования одного потока в другой
		/// </summary>
		/// <param name="source">Исходный поток.</param>
		/// <param name="destination">Целевой поток.</param>
		/// <param name="dataSize">Размер данных, подлежащих копированию.</param>
		/// <param name="bufferSize">Размер буфера для копирования.</param>
		public static void StreamCopy(EventHandler<EventArgsGeneric<ProgressChangedArg>> progressChanged,
		                              Stream source, Stream destination, long dataSize, int bufferSize)
		{
			// Обеспечиваем минимальный размер буфера
			bufferSize = bufferSize == 0 ? 4096 : bufferSize;

			// Выделяем буфер для копирования данных потока
			var buffer = new byte[bufferSize];

			long totalCount = 0;
			int count;

			// Пытаемся читать из потока фрагментами по bufferSize, а сколько было прочитано по-факту узнаем из count
			while((count = source.Read(buffer, 0, buffer.Length)) != 0)
			{
				destination.Write(buffer, 0, count);
				totalCount += count;

				// Сообщаем о прогрессе
				if(progressChanged != null)
				{
					double progress = (totalCount / (double)dataSize) * 100;
					progressChanged(null, new EventArgsGeneric<ProgressChangedArg>(new ProgressChangedArg("StreamCopy", progress, "\r")));
				}
			}
		}

		/// <summary>
		/// Сжатие данных, находящихся во входном потоке
		/// </summary>
		/// <param name="stream">Поток с данными для сжатия.</param>
		/// <returns>Поток со сжатыми данными.</returns>
		public static Stream Compress(Stream stream)
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
		/// <param name="stream">Поток со сжатыми данными.</param>
		/// <returns>Поток с исходными данными.</returns>
		public static Stream Uncompress(Stream stream)
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
		/// <param name="input">Входной поток.</param>
		/// <param name="align">Выравнивание.</param>
		/// <returns>Выровненный поток.</returns>
		public static Stream Align(Stream input, int align)
		{
			var output = new MemoryStream();
			input.Seek(0, SeekOrigin.Begin);
			input.CopyTo(output);
			var tail = (byte)(align - output.Length % align);
			output.SetLength(output.Length + tail - 1);
			output.Seek(0, SeekOrigin.End);
			output.WriteByte(tail);
			output.Flush();
			return output;
		}

		/// <summary>
		/// Извлечение выровненного потока
		/// </summary>
		/// <param name="input">Входной поток.</param>
		/// <returns>Выровненный поток.</returns>
		public static Stream DeAlign(Stream input)
		{
			input.Position = input.Length - 1;
			int tail = input.ReadByte();
			input.SetLength(input.Length - tail);
			var output = new MemoryStream();
			input.Seek(0, SeekOrigin.Begin);
			input.CopyTo(output);
			output.Flush();
			return output;
		}

		/// <summary>
		/// Шифрование строки алгоритмом RSA
		/// </summary>
		/// <param name="inputString">Входная строка.</param>
		/// <param name="rsaXmlKey">RSA-ключ.</param>
		/// <returns>Зашифрованная строка в XML.</returns>
		public static string EncryptStringByRSA(string inputString, string rsaXmlKey)
		{
			// Определяем размер ключа RSA...
			string bitStrengthString = rsaXmlKey.Substring(0, rsaXmlKey.IndexOf("</BitStrength>") + 14);
			int keySize = Convert.ToInt32(bitStrengthString.Replace("<BitStrength>", "").Replace("</BitStrength>", ""));
			//...и убираем блок данных с размером ключа
			rsaXmlKey = rsaXmlKey.Replace(bitStrengthString, "");

			// Алгоритм RSA
			var rsaCryptoServiceProvider = new RSACryptoServiceProvider(keySize);
			rsaCryptoServiceProvider.FromXmlString(rsaXmlKey);
			byte[] bytesInput = Encoding.UTF8.GetBytes(inputString);

			// Сжатие GZip...
			var input = new MemoryStream(bytesInput);
			var output = (MemoryStream)CryforceUtilities.Compress(input);
			output.Seek(0, SeekOrigin.Begin);
			byte[] bytes = output.ToArray();
			input.Close();
			output.Close();

			//...и выравнивание по границе 4 байта (важно для последующего деления!)
			var ms = new MemoryStream(bytes);
			var output2 = (MemoryStream)CryforceUtilities.Align(ms, 4);
			output2.Seek(0, SeekOrigin.Begin);
			byte[] alignedBytes = output2.ToArray();
			ms.Close();
			output2.Close();

			// The hash function in use by the .NET RSACryptoServiceProvider here is SHA1
			// int maxLength = ( keySize ) - 2 - ( 2 * SHA1.Create().ComputeHash( rawBytes ).Length );
			keySize = keySize >> 3;
			int maxLength = keySize - 42;
			int dataLength = alignedBytes.Length;
			int iterations = dataLength / maxLength;
			var stringBuilder = new StringBuilder();
			for(int i = 0; i <= iterations; i++)
			{
				var tempBytes = new byte[(dataLength - maxLength * i > maxLength) ? maxLength : dataLength - maxLength * i];
				Buffer.BlockCopy(alignedBytes, maxLength * i, tempBytes, 0, tempBytes.Length);
				byte[] encryptedBytes = rsaCryptoServiceProvider.Encrypt(tempBytes, true);
				stringBuilder.Append(Convert.ToBase64String(encryptedBytes));
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		/// Расшифровка строки, зашифрованной посредством RSA
		/// </summary>
		/// <param name="inputString">Входная строка.</param>
		/// <param name="rsaXmlKey">RSA-ключ.</param>
		/// <returns>Расшифрованная строка.</returns>
		public static string DecryptStringByRSA(string inputString, string rsaXmlKey)
		{
			// Определяем размер ключа RSA...
			string bitStrengthString = rsaXmlKey.Substring(0, rsaXmlKey.IndexOf("</BitStrength>") + 14);
			int keySize = Convert.ToInt32(bitStrengthString.Replace("<BitStrength>", "").Replace("</BitStrength>", ""));
			//...и убираем блок данных с размером ключа
			rsaXmlKey = rsaXmlKey.Replace(bitStrengthString, "");

			// Алгоритм RSA
			var rsaCryptoServiceProvider = new RSACryptoServiceProvider(keySize);
			rsaCryptoServiceProvider.FromXmlString(rsaXmlKey);
			keySize = keySize >> 3;
			int base64BlockSize = (keySize % 3 != 0) ? ((keySize / 3) * 4) + 4 : (keySize / 3) * 4;
			int iterations = inputString.Length / base64BlockSize;
			var arrayList = new ArrayList();
			for(int i = 0; i < iterations; i++)
			{
				byte[] encryptedBytes = Convert.FromBase64String(inputString.Substring(base64BlockSize * i, base64BlockSize));
				arrayList.AddRange(rsaCryptoServiceProvider.Decrypt(encryptedBytes, true));
			}

			// Снятие выравнивания с потока
			var bytesInput = arrayList.ToArray(Type.GetType("System.Byte")) as byte[];
			var ms = new MemoryStream(bytesInput);
			var output2 = (MemoryStream)CryforceUtilities.DeAlign(ms);
			output2.Seek(0, SeekOrigin.Begin);
			byte[] bytesInput2 = output2.ToArray();
			ms.Close();
			output2.Close();

			// Декомпрессия GZip
			var input = new MemoryStream(bytesInput2);
			var output = (MemoryStream)CryforceUtilities.Uncompress(input);
			output.Seek(0, SeekOrigin.Begin);
			byte[] bytes = output.ToArray();
			input.Close();
			output.Close();
			return Encoding.UTF8.GetString(bytes);
		}

		/// <summary>
		/// Безопасная установка начальной позиции в потоке
		/// </summary>
		/// <param name="stream">Входной поток.</param>
		/// <returns>Булевский флаг операции.</returns>
		public static bool SafeSeekBegin(this Stream stream)
		{
			if(stream.CanSeek)
			{
				stream.Seek(0, SeekOrigin.Begin);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Безопасная установка начальной позиции в потоке
		/// </summary>
		/// <param name="stream">Входной поток.</param>
		/// <param name="offset">Смещение от начала во входном потоке.</param>
		/// <param name="dataLength">Размер блока данных, который должен быть считан.</param>
		/// <returns>Булевский флаг операции.</returns>
		public static bool SafeSeekData(this Stream stream, long offset, long dataLength)
		{
			if(stream.CanSeek)
			{
				// Проверка на наличие требуемого объема данных
				if((offset + dataLength) <= stream.Length)
				{
					stream.Seek(offset, SeekOrigin.Begin);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Заполнение потока данными из переданного массива
		/// </summary>
		/// <param name="stream">Исходный поток.</param>
		/// <param name="offset">Смещение от начала.</param>
		/// <param name="pattern">Массив для заполнения потока.</param>
		/// <param name="patternCount">Количество требуемых записей паттерна.</param>
		public static void WipeStreamByPattern(Stream stream, long offset, byte[] pattern, long patternCount = long.MaxValue)
		{
			// Проверка на нулевой размер паттерна
			if(pattern.Length == 0)
			{
				throw new Exception("CryforceUtilities::WipeStreamByPattern() ==> Pattern to wipe can't be empty!");
			}

			// Перемещаемся на заданную позицию от начала
			stream.Seek(offset, SeekOrigin.Begin);

			// До тех пор, пока все данные не обработаны...
			while(stream.Position < (stream.Length - 1))
			{
				//...узнаем остаток байт в потоке...
				long bytesToProcess = (stream.Length - stream.Position);

				//...и пишем столько байт, сколько можно записать на данной итерации
				stream.Write(pattern, 0, (bytesToProcess < pattern.Length) ? (int)bytesToProcess : pattern.Length);

				// Уменьшаем количество оставшихся паттернов
				patternCount--;

				// Если выработаны все ресурсы задачи - выходим...
				if(patternCount <= 0)
				{
					break;
				}
			}

			// Синхронизация буфера с физическим носителем данных
			stream.Flush(); // ЗАПРЕЩАЕТСЯ ИСКЛЮЧАТЬ ИЗ КОДА! Иначе стирание данных будет происходить некорректно
		}

		/// <summary>
		/// Заполнение потока случайным паттерном по стандарту DoD_5220_22_E
		/// </summary>
		/// <param name="progressChanged">Событие обновления прогресса обработки.</param>
		/// <param name="stream">Исходный поток.</param>
		/// <param name="bufferSizePerStream">Размер буфера на файловый поток.</param>
		/// <param name="offset">Смещение от начала потока.</param>
		/// <param name="count">Количество байт, подлежащих стиранию.</param>
		/// <param name="zeroOut">Затирать выходной поток нулями?</param>
		/// <param name="rndSeed">Инициализирующее значение генератора случайных чисел.</param>
		public static void WipeStream(EventHandler<EventArgsGeneric<ProgressChangedArg>> progressChanged, Stream stream,
		                              int bufferSizePerStream, long offset, long count, bool zeroOut, int rndSeed = int.MinValue)
		{
			// Проверка на нулевой размер буфера
			if(bufferSizePerStream == 0)
			{
				throw new Exception("CryforceUtilities::WipeStream() ==> bufferSizePerStream can't be \"0\"!");
			}

			// Перемещаемся на заданную позицию от начала
			stream.Seek(offset, SeekOrigin.Begin);

			// Уточняем инициализирующее значение генератора случайных чисел
			rndSeed = (rndSeed == int.MinValue) ? DateTime.Now.Ticks.GetHashCode() : rndSeed ^ DateTime.Now.Ticks.GetHashCode();

			// Создаем массив для затирания данных в потоке
			var rndPattern = new byte[bufferSizePerStream];

			// Вычисляем конечную позицию для обработки в потоке...
			long endPosition = (offset + count);

			// Уточняем конечную позицию в потоке. Она должна быть не далее конца потока.
			endPosition = (endPosition > (stream.Length - 1)) ? (stream.Length - 1) : endPosition;

			// Параметры, необходимые для работы с итерациями
			long nIters = (int)Math.Ceiling((double)(count) / bufferSizePerStream);

			// Общее количество итераций не может быть равно нулю, минимум - одна
			nIters = (nIters == 0) ? 1 : nIters;

			// Устанавливаем текущую итерацию
			long nIter = 1;

			// До тех пор, пока не обработаны все данные...
			while(stream.Position < endPosition)
			{
				if(!zeroOut)
				{
					// Инициализируем генератор случайных чисел
					var rnd = new Random(rndSeed);

					// PASS 1
					rnd.NextBytes(rndPattern);
					// Прописываем паттерн в поток ОДИН РАЗ по указанному смещению...
					WipeStreamByPattern(stream, offset, rndPattern, 1);
					// PASS 1

					// PASS 2
					for(int i = 0; i < bufferSizePerStream; i++)
					{
						rndPattern[i] = (byte)(rndPattern[i] ^ 0xFF);
					}
					// Прописываем паттерн в поток ОДИН РАЗ по указанному смещению...
					WipeStreamByPattern(stream, offset, rndPattern, 1);
					// PASS 2

					// PASS 3
					rndSeed ^= DateTime.Now.Ticks.GetHashCode();
					rnd = new Random(rndSeed);
					rnd.NextBytes(rndPattern);
					// Прописываем паттерн в поток ОДИН РАЗ по указанному смещению...
					WipeStreamByPattern(stream, offset, rndPattern, 1);
					// PASS 3
				}
				else
				{
					// PASS ZERO
					for(int i = 0; i < bufferSizePerStream; i++)
					{
						rndPattern[i] = 0x00;
					}

					// Прописываем паттерн в поток ОДИН РАЗ по указанному смещению...
					WipeStreamByPattern(stream, offset, rndPattern, 1);
					// PASS ZERO
				}

				//...шагаем по потоку...
				offset += bufferSizePerStream;

				// Учитываем произведенную итерацию
				nIter++;
			}

			// Если не работали с нулями...
			if(!zeroOut)
			{
				//...нужно очистить паттерн в ОЗУ				
				ClearArray(rndPattern);
			}

			// Сообщаем о прогрессе
			if(progressChanged != null)
			{
				progressChanged(null, new EventArgsGeneric<ProgressChangedArg>(new ProgressChangedArg("WipeStream", 100)));
			}
		}

		/// <summary>
		/// Стирание данных файла по алгоритму DoD 5220.22-E
		/// </summary>
		/// <param name="progressChanged">Событие обновления прогресса обработки.</param>
		/// <param name="fileName">Имя файла.</param>
		/// <param name="bufferSizePerStream">Размер буфера на файловый поток.</param>
		/// <param name="zeroOut">Затирать выходной поток нулями?</param>
		/// <param name="rndSeed">Инициализирующее значение генератора случайных чисел.</param>
		public static void WipeFile(EventHandler<EventArgsGeneric<ProgressChangedArg>> progressChanged, string fileName,
		                            int bufferSizePerStream, bool zeroOut, int rndSeed = int.MinValue)
		{
			// Если указанный временный файл уже существует...
			if(File.Exists(fileName))
			{
				//...для того, чтобы повысить вероятность успешного удаления файла, устанавливаем нормальные атрибуты...
				File.SetAttributes(fileName, FileAttributes.Normal);

				//...затем создаем файловый поток...
				var bs = new BufferedStream(new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None), bufferSizePerStream);

				//...и стираем данные файла,...
				WipeStream(progressChanged, bs, bufferSizePerStream, 0, bs.Length, zeroOut, rndSeed);

				//...закрывая затем файловый поток
				bs.Close();
			}
		}

		/// <summary>
		/// Подготовка выходного потока к работе
		/// </summary>
		/// <param name="progressChanged">Событие обновления прогресса обработки.</param>
		/// <param name="fileName">Имя файла.</param>
		/// <param name="bufferSizePerStream">Размер буфера на файловый поток.</param>
		/// <param name="zeroOut">Затирать выходной поток нулями?</param>
		/// <param name="workInMemory">Работать в ОЗУ?</param>
		/// <param name="rndSeed">Инициализирующее значение генератора случайных чисел.</param>
		public static Stream PrepareOutputStream(EventHandler<EventArgsGeneric<ProgressChangedArg>> progressChanged,
		                                         string fileName, int bufferSizePerStream, bool zeroOut, bool workInMemory, int rndSeed = int.MinValue)
		{
			// Если работаем не в ОЗУ...
			if(!workInMemory)
			{
				//...если указанный временный файл уже существует...
				if(File.Exists(fileName))
				{
					//...затираем файл, с которым планируется работать...
					WipeFile(progressChanged, fileName, bufferSizePerStream, zeroOut, rndSeed);
				}

				//...и создаем файловый поток с требуемым именем
				return new BufferedStream(new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None), bufferSizePerStream);
			}

			return new MemoryStream();
		}

		/// <summary>
		/// Генерирование временных имен файлов
		/// </summary>
		/// <param name="count">Количество имен файлов.</param>
		/// <returns>Массив имен файлов.</returns>
		public static string[] GetTempFilenames(int count)
		{
			// Создаем набор имен файлов
			var fileNames = new string[count];

			for(int i = 0; i < count; i++)
			{
				fileNames[i] = Path.GetTempFileName();
			}

			// Возвращаем результат
			return fileNames;
		}

		/// <summary>
		/// Стирание строки в ОЗУ
		/// </summary>
		/// <param name="str">Строка для стирания.</param>
		public static unsafe void WipeString(ref string str)
		{
			fixed(char* ch = str)
			{
				for(Int32 i = 0; i < str.Length; i++)
				{
					ch[i] = '*';
				}
			}
		}

		/// <summary>
		/// Получение хеш-множества символов, принадлежащих Base64
		/// </summary>
		/// <returns>Хеш-множество символов, принадлежащих Base64</returns>
		public static HashSet<byte> GetBase64HashSet()
		{
			var base64HashSet = new HashSet<byte>();
			foreach(char c in "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/")
			{
				base64HashSet.Add((byte)c);
			}
			return base64HashSet;
		}

		/// <summary>
		/// Очистка входной строки от символов, не принадлежащих Base64
		/// </summary>
		public static string Base64String(this string source)
		{
			// Хеш-множество кодировки Base64
			HashSet<byte> base64 = GetBase64HashSet();

			// Здесь будем хранить формируемую строку
			var sb = new StringBuilder();

			// Обрабатываем каждый символ входной последовательности...
			foreach(char c in source)
			{
				if(base64.Contains((byte)c))
				{
					sb.Append((char)((byte)c));
				}
			}

			// Шлем отфильтрованную по стандарту Base64 строку...
			return sb.ToString();
		}

		/// <summary>
		/// Безопасное считывание пароля с клавиатуры с учетом нажатия клавиш "Alt", "Shift", "Control"
		/// </summary>
		/// <returns>Набор байт в кодировке Unicode для введенной строки.</returns>
		public static byte[] GetPasswordBytesSafely()
		{
			// Целевой список для накопления символов вводимого пароля
			var passCharsInBytes = new List<byte>();

			ConsoleKeyInfo keypress;
			do
			{
				// Считываем Scan-код...
				keypress = Console.ReadKey(true);

				//...проверяем его на модификаторы...
				if((ConsoleModifiers.Alt & keypress.Modifiers) != 0)
				{
					passCharsInBytes.Add(Encoding.Unicode.GetBytes("A")[0]);
					passCharsInBytes.Add(Encoding.Unicode.GetBytes("l")[0]);
					passCharsInBytes.Add(Encoding.Unicode.GetBytes("t")[0]);
				}

				if((ConsoleModifiers.Shift & keypress.Modifiers) != 0)
				{
					passCharsInBytes.Add(Encoding.Unicode.GetBytes("S")[0]);
					passCharsInBytes.Add(Encoding.Unicode.GetBytes("h")[0]);
					passCharsInBytes.Add(Encoding.Unicode.GetBytes("i")[0]);
					passCharsInBytes.Add(Encoding.Unicode.GetBytes("f")[0]);
					passCharsInBytes.Add(Encoding.Unicode.GetBytes("t")[0]);
				}

				if((ConsoleModifiers.Control & keypress.Modifiers) != 0)
				{
					passCharsInBytes.Add(Encoding.Unicode.GetBytes("C")[0]);
					passCharsInBytes.Add(Encoding.Unicode.GetBytes("o")[0]);
					passCharsInBytes.Add(Encoding.Unicode.GetBytes("n")[0]);
					passCharsInBytes.Add(Encoding.Unicode.GetBytes("t")[0]);
					passCharsInBytes.Add(Encoding.Unicode.GetBytes("r")[0]);
					passCharsInBytes.Add(Encoding.Unicode.GetBytes("o")[0]);
					passCharsInBytes.Add(Encoding.Unicode.GetBytes("l")[0]);
				}

				//...затем читаем сам символ...
				var chArr = new[] {keypress.KeyChar};
				var keypressKeyCharStr = new string(chArr);
				chArr[0] = '*';
				passCharsInBytes.Add(Encoding.Unicode.GetBytes(keypressKeyCharStr)[0]);
				WipeString(ref keypressKeyCharStr);

				// Ввод пароля завершается по нажатию "Enter"
				if(keypress.Key == ConsoleKey.Enter)
				{
					break;
				}

				Console.Write("*");
			} while(true);

			// Если массив байт пароля пуст - создаем пароль по-умолчанию...
			if(passCharsInBytes.Count == 0)
			{
				passCharsInBytes.Add(Encoding.Unicode.GetBytes("p")[0]);
				passCharsInBytes.Add(Encoding.Unicode.GetBytes("a")[0]);
				passCharsInBytes.Add(Encoding.Unicode.GetBytes("s")[0]);
				passCharsInBytes.Add(Encoding.Unicode.GetBytes("s")[0]);
				passCharsInBytes.Add(Encoding.Unicode.GetBytes("w")[0]);
				passCharsInBytes.Add(Encoding.Unicode.GetBytes("o")[0]);
				passCharsInBytes.Add(Encoding.Unicode.GetBytes("r")[0]);
				passCharsInBytes.Add(Encoding.Unicode.GetBytes("d")[0]);
			}

			// Перенос результатов ввода пароля из списка в целевой массив
			var result = new byte[passCharsInBytes.Count];
			for(int i = 0; i < passCharsInBytes.Count; i++)
			{
				result[i] = passCharsInBytes[i];
				passCharsInBytes[i] = 0x00;
			}

			// После окончания ввода переводим каретку на новую строку
			Console.WriteLine();

			return result;
		}

		/// <summary>
		/// Устранение символов, запрещенных в именах файлов
		/// </summary>
		/// <param name="inputString">Входная строка.</param>
		/// <returns>Результат элиминации.</returns>
		public static string EliminateInvalidFileNameChars(this string inputString)
		{
			// Получаем список символов, запрещенных к использованию в именах файлов
			char[] invalidFileNameChars = Path.GetInvalidFileNameChars();

			foreach(char invalidFileNameChar in invalidFileNameChars)
			{
				inputString = inputString.Replace(invalidFileNameChar, ' ');
			}

			// Возвращаем результат
			return inputString.Replace(" ", "");
		}

		/// <summary>
		/// Генерирование случайных имен файлов
		/// </summary>
		/// <param name="count">Количество имен файлов.</param>
		/// <param name="maxLen">Максимальная длина имени файла.</param>
		/// <param name="rndSeed">Инициализирующее значение генератора случайных чисел.</param>
		/// <returns>Массив имен файлов.</returns>
		public static string[] GetRandomFilenames(int count, int maxLen, int rndSeed)
		{
			// Создаем набор имен файлов
			var fileNames = new string[count];

			// Защищаем rndSeed значение через ^ DateTime.Now.Ticks.GetHashCode()
			var rnd = new Random(rndSeed ^ DateTime.Now.Ticks.GetHashCode());

			bool failed = false;
			for(int i = 0; i < count; i++)
			{
				// Дубликаты имен запрещены! Если был обнаружен таковой - делаем сброс генерации
				if(failed)
				{
					i = 0;
					failed = false;
				}

				// Получаем случайную длину будущего имени файла...
				int bufferSize = rnd.Next(1, maxLen);

				//...получаем случайное имя...
				var fileNameBuffer = new byte[bufferSize];
				rnd.NextBytes(fileNameBuffer);

				//...устраняем запрещенные символы в имени файла
				fileNames[i] = Convert.ToBase64String(fileNameBuffer).Replace("=", "\0");
				fileNames[i] = EliminateInvalidFileNameChars(fileNames[i]);

				// Если получили пустую строку в результате элиминирования пробелов, то требуется начать генерацию сначала.
				// Т.к. процесс будет начать снова, это вносит большую неопределенность в общий процесс и имена получаются
				// более случайные, чем если бы просто перегенерировать "неудачное" имя
				if(fileNames[i] == "")
				{
					failed = true;
					continue;
				}

				// Проверка на совпадение имен. Новое имя не должно совпадать с имеющимися
				for(int j = 0; j < i; j++)
				{
					if(fileNames[j] == fileNames[i])
					{
						failed = true;
						break;
					}
				}
			}

			// Возвращаем результат
			return fileNames;
		}

		/// <summary>
		/// Получение бита из байта в байтовой форме
		/// </summary>
		/// <param name="data">Байт данных.</param>
		/// <param name="bitIdx">Индекс выделяемого бита.</param>
		/// <returns>Бит в байтовой форме.</returns>
		public static byte GetBit(byte data, int bitIdx)
		{
			return (byte)((data >> bitIdx) & 0x01);
		}

		/// <summary>
		/// Установка бита в байте
		/// </summary>
		/// <param name="data">Байт данных.</param>
		/// <param name="bitIdx">Индекс устанавливаемого бита.</param>
		/// <param name="bitValue">Значение устанавливаемого бита.</param>
		public static void SetBit(ref byte data, int bitIdx, byte bitValue)
		{
			data = (byte)((((0x01 << bitIdx) ^ 0xFF) & data) | ((bitValue & 0x01) << bitIdx));
		}
	}
}