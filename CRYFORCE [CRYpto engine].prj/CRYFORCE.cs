using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

using EventArgsUtilities;

namespace CRYFORCE.Engine
{
	/// <summary>
	/// Основной класс ядра шифрования
	/// </summary>
	public class Cryforce
	{
		#region Static

		#endregion Static

		#region Constants

		/// <summary>Размер буфера в ОЗУ под каждый поток.</summary>
		private const int DEFAULT_BUFFER_SIZE_PER_STREAM = 16 * 1024 * 1024; // 16 мегабайт

		#endregion Constants

		#region Data

		#endregion Data

		#region Events

		/// <summary>
		/// Событие обновления прогресса обработки
		/// </summary>
		public event EventHandler<EventArgsGeneric<ProgressChangedArg>> ProgressChanged;

		#endregion Events

		#region .ctor

		/// <summary>
		/// Конструктор с параметрами
		/// </summary>
		public Cryforce()
		{
			WorkInMemory = true;
			WorkInTempDir = true;
			BufferSizePerStream = DEFAULT_BUFFER_SIZE_PER_STREAM;
			RndSeed = DateTime.Now.Ticks.GetHashCode();
		}

		/// <summary>
		/// Конструктор с параметрами
		/// </summary>
		/// <param name="workInMemory">Работать в ОЗУ?</param>
		/// <param name="workInTempDir">Работать в директории для временных файлов?</param>
		public Cryforce(bool workInMemory, bool workInTempDir)
		{
			WorkInMemory = workInMemory;
			WorkInTempDir = workInTempDir;
			BufferSizePerStream = DEFAULT_BUFFER_SIZE_PER_STREAM;
			RndSeed = DateTime.Now.Ticks.GetHashCode();
		}

		/// <summary>
		/// Конструктор с параметрами
		/// </summary>
		/// <param name="workInMemory">Работать в ОЗУ?</param>
		/// <param name="workInTempDir">Работать в директории для временных файлов?</param>
		/// <param name="bufferSizePerStream">Размер буфера на файловый поток.</param>
		public Cryforce(bool workInMemory, bool workInTempDir, int bufferSizePerStream)
		{
			WorkInMemory = workInMemory;
			WorkInTempDir = workInTempDir;
			BufferSizePerStream = bufferSizePerStream;
			RndSeed = DateTime.Now.Ticks.GetHashCode();
		}

		/// <summary>
		/// Конструктор с параметрами
		/// </summary>
		/// <param name="workInMemory">Работать в ОЗУ?</param>
		/// <param name="workInTempDir">Работать в директории для временных файлов?</param>
		/// <param name="bufferSizePerStream">Размер буфера на файловый поток.</param>
		/// <param name="rndSeed">Инициализирующее значение генератора случайных чисел.</param>
		public Cryforce(bool workInMemory, bool workInTempDir, int bufferSizePerStream, int rndSeed)
		{
			WorkInMemory = workInMemory;
			WorkInTempDir = workInTempDir;
			BufferSizePerStream = bufferSizePerStream;
			RndSeed = rndSeed;
		}

		#endregion .ctor

		#region Properties

		/// <summary>Работаем в ОЗУ?</summary>
		public bool WorkInMemory { get; set; }

		/// <summary>Работаем в директории для временных файлов?</summary>
		public bool WorkInTempDir { get; set; }

		/// <summary>Размер буфера в ОЗУ под каждый поток.</summary>
		public int BufferSizePerStream { get; set; }

		/// <summary>Инициализирующее значение генератора случайных чисел.</summary>
		public int RndSeed { get; set; }

		/// <summary>Затирать выходной поток нулями?</summary>
		public bool ZeroOut { get; set; }

		#endregion Properties

		#region Private

		#endregion Private

		#region Protected

		#endregion Protected

		#region Public

		/// <summary>
		/// Шифрование по алгоритму Rijndael-256
		/// </summary>
		/// <param name="inputStream">Входной поток.</param>
		/// <param name="key">Ключ для первого прохода шифрования.</param>
		/// <param name="outputStream">Выходной поток.</param>
		/// <param name="encryptionMode">Используется шифрование?</param>
		/// <param name="iterations">Количество итераций хеширования пароля.</param>
		public void SingleRijndael(Stream inputStream, byte[] key, Stream outputStream, bool encryptionMode, int iterations = 1)
		{
			if(!inputStream.CanSeek)
			{
				throw new Exception("Cryforce::SingleRijndael() ==> Input stream can't seek!");
			}

			var streamCryptoWrapper = new StreamCryptoWrapper();
			streamCryptoWrapper.Initialize(key, iterations);

			Stream inputStreamAtLevel0 = encryptionMode ? inputStream : streamCryptoWrapper.WrapStream(inputStream, false);
			Stream outputStreamAtLevel0 = encryptionMode ? streamCryptoWrapper.WrapStream(outputStream, true) : outputStream;

			CryforceUtilities.SafeSeekBegin(inputStreamAtLevel0);
			CryforceUtilities.SafeSeekBegin(outputStreamAtLevel0);

			// Процесс шифрования/расшифровки происходит прозрачно, во время чтения из зашифрованного потока или записи в зашифрованный
			// Размер буфера при копировании выбираем таким, чтобы обеспечить вывод каждого процента
			long dataSize = inputStream.Length;
			var bufferSize = (int)(dataSize / 100);
			CryforceUtilities.StreamCopy(ProgressChanged, inputStreamAtLevel0, outputStreamAtLevel0, dataSize, bufferSize);

			if(outputStreamAtLevel0 is CryptoStream)
			{
				((CryptoStream)outputStreamAtLevel0).FlushFinalBlock();
			}
			outputStreamAtLevel0.Flush();

			// Если выходной поток первого уровня является криптографической оберткой над другим потоком -
			// нужно получить базовый поток для дальнейшей работы
			if(outputStreamAtLevel0 is CryptoStream)
			{
				outputStreamAtLevel0 = outputStream;
			}

			CryforceUtilities.SafeSeekBegin(inputStreamAtLevel0);
			CryforceUtilities.SafeSeekBegin(outputStreamAtLevel0);

			//...и как завершение шифрования - деинициализируем криптовраппер
			streamCryptoWrapper.Clear();

			if(ProgressChanged != null)
			{
				ProgressChanged(null, new EventArgsGeneric<ProgressChangedArg>(new ProgressChangedArg("Rijndael-256", 100)));
			}
		}

		/// <summary>
		/// Двойное шифрование по алгоритму Rijndael-256
		/// </summary>
		/// <param name="inputStream">Входной поток.</param>
		/// <param name="key1">Ключ для первого прохода шифрования.</param>
		/// <param name="key2">Ключ для второго прохода шифрования.</param>
		/// <param name="outputStream">Выходной поток.</param>
		/// <param name="encryptionMode">Используется шифрование?</param>
		/// <param name="iterations">Количество итераций хеширования пароля.</param>
		public void DoubleRijndael(Stream inputStream, byte[] key1, byte[] key2, Stream outputStream, bool encryptionMode, int iterations = 1)
		{
			if(!inputStream.CanSeek)
			{
				throw new Exception("Cryforce::DoubleRijndael() ==> Input stream can't seek!");
			}

			var streamCryptoWrappers = new[] {new StreamCryptoWrapper(), new StreamCryptoWrapper()};
			streamCryptoWrappers[0].Initialize(encryptionMode ? key1 : key2, iterations); // При шифровании прямой порядок паролей...
			streamCryptoWrappers[1].Initialize(encryptionMode ? key2 : key1, iterations); // ...а при расшифровке - обратный

			// Генерируем 10 случайных имен файлов: два для целей временного хранения данных в пределах данного метода
			// и 8 штук для битсплиттера (генерируем их совместно, чтобы избежать конфликтов)			
			string[] tempFilenamesAll = WorkInTempDir ? CryforceUtilities.GetTempFilenames(10) : CryforceUtilities.GetRandomFilenames(10, 8, RndSeed).Select(item => item + ".jpg").ToArray();
			var tempFilenames = new string[2];
			var tempFilenamesToBitSplitter = new string[8];
			Array.Copy(tempFilenamesAll, 0, tempFilenames, 0, 2);
			Array.Copy(tempFilenamesAll, 2, tempFilenamesToBitSplitter, 0, 8);

			Stream[] randomFilenameStreams = tempFilenames.Select(item => CryforceUtilities.PrepareOutputStream(ProgressChanged, item, BufferSizePerStream, ZeroOut, WorkInMemory, RndSeed)).ToArray();

			//////////////////////////////////////
			// Шифрование первого уровня (Level0)
			//////////////////////////////////////
			Stream inputStreamAtLevel0 = encryptionMode ? inputStream : streamCryptoWrappers[0].WrapStream(inputStream, false);
			Stream outputStreamAtLevel0 = encryptionMode ? streamCryptoWrappers[0].WrapStream(randomFilenameStreams[0], true) : randomFilenameStreams[0];

			CryforceUtilities.SafeSeekBegin(inputStreamAtLevel0);
			CryforceUtilities.SafeSeekBegin(outputStreamAtLevel0);

			// Процесс шифрования/расшифровки происходит прозрачно, во время чтения из зашифрованного потока или записи в зашифрованный
			// Размер буфера при копировании выбираем таким, чтобы обеспечить вывод каждого процента
			long dataSize = inputStream.Length;
			var bufferSize = (int)(dataSize / 100);
			CryforceUtilities.StreamCopy(ProgressChanged, inputStreamAtLevel0, outputStreamAtLevel0, dataSize, bufferSize);

			if(outputStreamAtLevel0 is CryptoStream)
			{
				((CryptoStream)outputStreamAtLevel0).FlushFinalBlock();
			}
			outputStreamAtLevel0.Flush();

			// Если выходной поток первого уровня является криптографической оберткой над другим потоком -
			// нужно получить базовый поток для дальнейшей работы
			if(outputStreamAtLevel0 is CryptoStream)
			{
				outputStreamAtLevel0 = randomFilenameStreams[0];
			}

			CryforceUtilities.SafeSeekBegin(inputStreamAtLevel0);
			CryforceUtilities.SafeSeekBegin(outputStreamAtLevel0);

			if(ProgressChanged != null)
			{
				ProgressChanged(null, new EventArgsGeneric<ProgressChangedArg>(new ProgressChangedArg("Rijndael-256 (1/2)", 100)));
			}

			////////////////////////////////////////////////////////
			// Перестановка битов посредством битсплиттера (Level1)
			////////////////////////////////////////////////////////

			// Выходной поток первого уровня обработки является входным потоком для второго
			Stream inputStreamAtLevel1 = outputStreamAtLevel0;

			// Т.к. результат работы битсплиттера не является конечным - работаем с временным потоком
			Stream outputStreamAtLevel1 = randomFilenameStreams[1];

			CryforceUtilities.SafeSeekBegin(inputStreamAtLevel1);
			CryforceUtilities.SafeSeekBegin(outputStreamAtLevel1);

			var bitSplitter = new BitSplitter(tempFilenamesToBitSplitter, key1, key2, WorkInMemory);
			bitSplitter.RndSeed = RndSeed; // Некритичный параметр, но проброска значения желательна
			bitSplitter.ProgressChanged += ProgressChanged;
			if(encryptionMode)
			{
				bitSplitter.SplitToBitstream(inputStreamAtLevel1, outputStreamAtLevel1);
			}
			else
			{
				bitSplitter.UnsplitFromBitstream(inputStreamAtLevel1, outputStreamAtLevel1);
			}

			if(ProgressChanged != null)
			{
				ProgressChanged(null, new EventArgsGeneric<ProgressChangedArg>(new ProgressChangedArg("BitSplitter", 100)));
			}

			bitSplitter.ClearAndClose();

			CryforceUtilities.SafeSeekBegin(inputStreamAtLevel1);
			CryforceUtilities.SafeSeekBegin(outputStreamAtLevel1);

			//////////////////////////////////////
			// Шифрование второго уровня (Level2)
			//////////////////////////////////////
			Stream inputStreamAtLevel2 = encryptionMode ? outputStreamAtLevel1 : streamCryptoWrappers[1].WrapStream(outputStreamAtLevel1, false);
			Stream outputStreamAtLevel2 = encryptionMode ? streamCryptoWrappers[1].WrapStream(outputStream, true) : outputStream;

			CryforceUtilities.SafeSeekBegin(inputStreamAtLevel2);
			CryforceUtilities.SafeSeekBegin(outputStreamAtLevel2);

			// Процесс шифрования/расшифровки происходит прозрачно, во время чтения из зашифрованного потока или записи в зашифрованный
			// Размер буфера при копировании выбираем таким, чтобы обеспечить вывод каждого процента
			dataSize = outputStreamAtLevel1.Length;
			bufferSize = (int)(dataSize / 100);
			CryforceUtilities.StreamCopy(ProgressChanged, inputStreamAtLevel2, outputStreamAtLevel2, dataSize, bufferSize);

			if(outputStreamAtLevel2 is CryptoStream)
			{
				((CryptoStream)outputStreamAtLevel2).FlushFinalBlock();
			}
			outputStreamAtLevel2.Flush();

			CryforceUtilities.SafeSeekBegin(inputStreamAtLevel2);
			CryforceUtilities.SafeSeekBegin(outputStreamAtLevel2);

			if(ProgressChanged != null)
			{
				ProgressChanged(null, new EventArgsGeneric<ProgressChangedArg>(new ProgressChangedArg("Rijndael-256 (2/2)", 100)));
			}

			// Уничтожаем данные временных потоков
			foreach(Stream randomFilenameStream in randomFilenameStreams)
			{
				CryforceUtilities.WipeStream(ProgressChanged, randomFilenameStream, BufferSizePerStream, 0, randomFilenameStream.Length, ZeroOut, RndSeed);
				randomFilenameStream.Flush();
				randomFilenameStream.Close();
			}

			// Производим удаление носителей
			foreach(string tempFilename in tempFilenames)
			{
				if(File.Exists(tempFilename))
				{
					File.SetAttributes(tempFilename, FileAttributes.Normal);
					File.Delete(tempFilename);
				}
			}

			// Закрываем все потоки, которые не являлись синонимами входа или выхода
			inputStreamAtLevel1.Close();
			inputStreamAtLevel2.Close();

			outputStreamAtLevel0.Close();
			outputStreamAtLevel1.Close();
		}

		/// <summary>
		/// Генерация пары открытый/закрытый ключ
		/// </summary>
		/// <param name="publicKeyStream">Поток для записи открытого ключа.</param>
		/// <param name="privateKeyStream">Поток для записи закрытого ключа.</param>
		/// <param name="seedStream">Поток, содержащий случайные данные.</param>
		public void CreateEccKeys(Stream publicKeyStream, Stream privateKeyStream, Stream seedStream = null)
		{
			byte[] seedDataFromStream = null;

			if(!publicKeyStream.CanWrite)
			{
				throw new Exception("Cryforce::CreateEccKeys() ==> Public key stream can't write!");
			}

			if(!privateKeyStream.CanWrite)
			{
				throw new Exception("Cryforce::CreateEccKeys() ==> Private key stream can't write!");
			}

			// Если можем использовать seed-поток...
			if((seedStream != null) && (seedStream.CanSeek))
			{
				CryforceUtilities.SafeSeekBegin(seedStream);
				seedDataFromStream = new byte[seedStream.Length];
				seedStream.Read(seedDataFromStream, 0, seedDataFromStream.Length);
			}

			Console.WriteLine("Генерирование пары открытый/закрытый ключ.");
			Console.WriteLine("Нажимайте случайные клавиши на клавиатуре (включая модификаторы");
			Console.WriteLine("\"Alt\", \"Shift\", \"Control\"). Для завершения нажмите Enter...");
			Console.WriteLine();
			byte[] seedDataFromKeyboard = CryforceUtilities.GetPasswordBytesSafely();
			byte[] seedData = CryforceUtilities.MergeArrays(seedDataFromStream, seedDataFromKeyboard);

			// Создаем сущность для работы с эллиптическими кривыми
			var ecdhP521 = new EcdhP521(seedData, null); // Приватного ключа нет - он будет сгенерирован!

			// Чистим массивы...
			CryforceUtilities.ClearArray(seedDataFromStream);
			CryforceUtilities.ClearArray(seedDataFromKeyboard);
			CryforceUtilities.ClearArray(seedData);

			// Пишем ключи в потоки
			publicKeyStream.Write(ecdhP521.PublicKey.Select(item => (byte)item).ToArray(), 0, ecdhP521.PublicKey.Length);
			privateKeyStream.Write(ecdhP521.PrivateKey.Select(item => (byte)item).ToArray(), 0, ecdhP521.PrivateKey.Length);

			// Уничтожаем секретные данные...
			ecdhP521.Clear();
		}

		/// <summary>
		/// Генерирование симметричных ключей
		/// </summary>
		/// <param name="publicKeyFromOtherPartyStream">Поток открытого ключа другой стороны.</param>
		/// <param name="privateKeyStream">Поток своего закрытого ключа.</param>
		/// <param name="symmetricKey1">Симметричный ключ №1 (256 bit).</param>
		/// <param name="symmetricKey2">Симметричный ключ №2 (256 bit).</param>
		public void GetSymmetricKeys(Stream publicKeyFromOtherPartyStream, Stream privateKeyStream,
		                             out byte[] symmetricKey1, out byte[] symmetricKey2)
		{
			if(!publicKeyFromOtherPartyStream.CanSeek)
			{
				throw new Exception("Cryforce::GetSymmetricKeys() ==> Public key from other party stream can't seek!");
			}

			if(!privateKeyStream.CanSeek)
			{
				throw new Exception("Cryforce::GetSymmetricKeys() ==> Private key stream can't seek!");
			}

			// Читаем открытый ключ
			var publicKeyFromOtherParty = new byte[publicKeyFromOtherPartyStream.Length];
			publicKeyFromOtherPartyStream.Read(publicKeyFromOtherParty, 0, publicKeyFromOtherParty.Length);

			// Читаем закрытый ключ
			var privateKey = new byte[privateKeyStream.Length];
			privateKeyStream.Read(privateKey, 0, privateKey.Length);

			// Создаем сущность для работы с эллиптическими кривыми
			var ecdhP521 = new EcdhP521(null, new string(privateKey.Select(item => (char)item).ToArray())) {PublicKeyFromOtherParty = new string(publicKeyFromOtherParty.Select(item => (char)item).ToArray())};

			// Если не получилось сгенерировать симметричные ключи...
			if(!ecdhP521.CreateSymmetricKey())
			{
				// Чистим массивы...
				CryforceUtilities.ClearArray(publicKeyFromOtherParty);
				CryforceUtilities.ClearArray(privateKey);

				//...уничтожаем секретные данные...
				ecdhP521.Clear();

				throw new Exception("Cryforce::CreateSymmetricKey() failed!");
			}

			// Получаем симметричные ключи
			symmetricKey1 = (byte[])ecdhP521.Keys256[0].Clone();
			symmetricKey2 = (byte[])ecdhP521.Keys256[1].Clone();

			// Чистим массивы...
			CryforceUtilities.ClearArray(publicKeyFromOtherParty);
			CryforceUtilities.ClearArray(privateKey);

			// Уничтожаем секретные данные...
			ecdhP521.Clear();
		}

		/// <summary>
		/// Вычисление ЭЦП потока данных
		/// </summary>
		/// <param name="privateKeyStream">Приватный ключ.</param>
		/// <param name="dataStream">Поток данных.</param>
		/// <param name="signStream">ЭЦП для потока данных.</param>
		public void SignData(Stream privateKeyStream, Stream dataStream, Stream signStream)
		{
			if(!privateKeyStream.CanSeek)
			{
				throw new Exception("Cryforce::SignData() ==> Private key stream can't seek!");
			}

			// Читаем закрытый ключ
			var privateKey = new byte[privateKeyStream.Length];
			privateKeyStream.Read(privateKey, 0, privateKey.Length);

			// Создаем сущность для работы с эллиптическими кривыми
			var ecdhP521 = new EcdhP521(null, new string(privateKey.Select(item => (char)item).ToArray()));

			string sign = ecdhP521.SignData(dataStream);
			signStream.Write(sign.Select(item => (byte)item).ToArray(), 0, sign.Length);

			// Чистим массивы...
			CryforceUtilities.ClearArray(privateKey);

			// Уничтожаем секретные данные...
			ecdhP521.Clear();
		}

		/// <summary>
		/// Проверка ЭЦП на базе открытого ключа другой стороны
		/// </summary>
		/// <param name="dataStream">Поток данных.</param>
		/// <param name="signStream">ЭЦП.</param>
		/// <param name="publicKeyFromOtherPartyStream">Поток открытого ключа другой стороны.</param>
		/// <returns>Булевский флаг операции.</returns>
		public bool VerifySign(Stream dataStream, Stream signStream, Stream publicKeyFromOtherPartyStream)
		{
			if(!publicKeyFromOtherPartyStream.CanSeek)
			{
				throw new Exception("Cryforce::VerifySign() ==> Public key from other party stream can't seek!");
			}

			// Читаем открытый ключ
			var publicKeyFromOtherParty = new byte[publicKeyFromOtherPartyStream.Length];
			publicKeyFromOtherPartyStream.Read(publicKeyFromOtherParty, 0, publicKeyFromOtherParty.Length);

			// Читаем ЭЦП
			var sign = new byte[signStream.Length];
			signStream.Read(sign, 0, sign.Length);

			// Создаем сущность для работы с эллиптическими кривыми
			var ecdhP521 = new EcdhP521(null, null);

			// Возвращаем результат проверки ЭЦП
			bool result;

			try
			{
				// Осуществляем проверку ЭЦП...
				result = ecdhP521.VerifyData(dataStream,
				                             Convert.FromBase64String(new string(sign.Select(item => (char)item).ToArray())),
				                             Convert.FromBase64String(new string(publicKeyFromOtherParty.Select(item => (char)item).ToArray())));
			}
			catch
			{
				result = false;
			}

			// Чистим массивы...
			CryforceUtilities.ClearArray(sign);

			// Уничтожаем секретные данные...
			ecdhP521.Clear();

			// Возвращаем результат проверки ЭЦП
			return result;
		}

		#endregion Public
	}
}