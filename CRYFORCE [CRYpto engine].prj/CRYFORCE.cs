using System;
using System.IO;
using System.Linq;

using EventArgsUtilities;

namespace CRYFORCE.Engine
{
	public class CRYFORCE
	{
		#region Static

		#endregion Static

		#region Constants

		#endregion Constants

		#region Data

		/// <summary>Сущность для разбиения файла на битовые потоки и обратного склеивания.</summary>
		private BitSplitter _bitSplitter;

		/// <summary>Сущность для обеспечения обмена ключами на основе шифрования на основе эллиптических кривых.</summary>
		private EcdhP521 _ecdhP521;

		/// <summary>Сущность для шифрования данных по алгоритму Rijndael (256 bit).</summary>
		private StreamCryptoWrapper _streamCryptoWrapper;

		#endregion Data

		#region Events

		/// <summary>
		/// Событие обновления прогресса обработки
		/// </summary>
		public event EventHandler<EventArgs_Generic<ProgressChangedArg>> ProgressChanged;

		#endregion Events

		#region .ctor

		#endregion .ctor

		#region Properties

		/// <summary>Используется прямое направление преобразования?</summary>
		public bool DirectMode { get; private set; }

		/// <summary>Работаем в ОЗУ?</summary>
		public bool WorkInMemory { get; private set; }

		/// <summary>Размер буфера в ОЗУ под каждый поток.</summary>
		public int BufferSizePerStream { get; private set; }

		/// <summary>Инициализирующее значение генератора случайных чисел.</summary>
		public int RndSeed { get; set; }

		/// <summary>Затирать выходной поток нулями?</summary>
		public bool ZeroOut { get; set; }

		/// <summary>Экземпляр класса инициализирован?</summary>
		public bool IsInitialized { get; private set; }

		#endregion Properties

		#region Private

		#endregion Private

		#region Protected

		#endregion Protected

		#region Public

		/// <summary>
		/// Двойное шифрование по алгоритму Rijndael-256
		/// </summary>
		/// <param name="inputStream">Входной поток.</param>
		/// <param name="password1">Пароль для первого прохода шифрования.</param>
		/// <param name="password2">Пароль для второго прохода шифрования.</param>
		/// <param name="outputStream">Выходной поток.</param>
		/// <param name="encryptionMode">Используется шифрование?.</param>
		public void DoubleRijndael(Stream inputStream, byte[] password1, byte[] password2, Stream outputStream, bool encryptionMode)
		{
			var streamCryptoWrappers = new[] {new StreamCryptoWrapper(), new StreamCryptoWrapper()};
			streamCryptoWrappers[0].Initialize(encryptionMode ? password1 : password2); // При шифровании прямой порядок паролей...
			streamCryptoWrappers[1].Initialize(encryptionMode ? password2 : password1); // ...а при расшифровке - обратный

			// Генерируем два случайных имени файлов, для последующего использования в качестве временных и готовим потоки на их основе
			string[] randomFilenames = Utilities.GetRandomFilenames(2, 8, RndSeed).Select(item => item + ".jpg").ToArray();
			Stream[] randomFilenameStreams = randomFilenames.Select(item => Utilities.PrepareOutputStream(ProgressChanged, item, BufferSizePerStream, ZeroOut, WorkInMemory, RndSeed)).ToArray();

			//////////////////////////////////////
			// Шифрование первого уровня (Level0)
			//////////////////////////////////////
			Stream inputStreamAtLevel0 = encryptionMode ? inputStream : streamCryptoWrappers[0].WrapStream(inputStream, false);
			Stream outputStreamAtLevel0 = encryptionMode ? streamCryptoWrappers[0].WrapStream(randomFilenameStreams[0], true) : randomFilenameStreams[0];

			if(ProgressChanged != null)
			{
				ProgressChanged(null, new EventArgs_Generic<ProgressChangedArg>(new ProgressChangedArg("Rijndael-256 (1/2)", 0)));
			}

			inputStreamAtLevel0.Seek(0, SeekOrigin.Begin);
			outputStreamAtLevel0.Seek(0, SeekOrigin.Begin);

			inputStreamAtLevel0.CopyTo(outputStreamAtLevel0);
			outputStreamAtLevel0.Flush();

			inputStreamAtLevel0.Seek(0, SeekOrigin.Begin);
			outputStreamAtLevel0.Seek(0, SeekOrigin.Begin);

			if(ProgressChanged != null)
			{
				ProgressChanged(null, new EventArgs_Generic<ProgressChangedArg>(new ProgressChangedArg("Rijndael-256 (1/2)", 100)));
			}

			////////////////////////////////////////////////////////
			// Перестановка битов посредством битсплиттера (Level1)
			////////////////////////////////////////////////////////
			Stream inputStreamAtLevel1 = outputStreamAtLevel0;
			Stream outputStreamAtLevel1 = randomFilenameStreams[1];

			var bitSplitter = new BitSplitter(WorkInMemory);
			bitSplitter.RndSeed = RndSeed; // Некритичный параметр, но проброска значения желательна
			bitSplitter.ProgressChanged += ProgressChanged;
			bitSplitter.SplitToBitstream(outputStreamAtLevel1, outputStreamAtLevel1);
			bitSplitter.Dispose();

			//////////////////////////////////////
			// Шифрование второго уровня (Level2)
			//////////////////////////////////////
			Stream inputStreamAtLevel2 = encryptionMode ? outputStreamAtLevel1 : streamCryptoWrappers[1].WrapStream(outputStreamAtLevel1, false);
			Stream outputStreamAtLevel2 = encryptionMode ? streamCryptoWrappers[1].WrapStream(outputStream, true) : outputStream;

			if(ProgressChanged != null)
			{
				ProgressChanged(null, new EventArgs_Generic<ProgressChangedArg>(new ProgressChangedArg("Rijndael-256 (2/2)", 0)));
			}

			inputStreamAtLevel2.Seek(0, SeekOrigin.Begin);
			outputStreamAtLevel2.Seek(0, SeekOrigin.Begin);

			inputStreamAtLevel2.CopyTo(outputStreamAtLevel2);
			outputStreamAtLevel2.Flush();

			inputStreamAtLevel2.Seek(0, SeekOrigin.Begin);
			outputStreamAtLevel2.Seek(0, SeekOrigin.Begin);

			if(ProgressChanged != null)
			{
				ProgressChanged(null, new EventArgs_Generic<ProgressChangedArg>(new ProgressChangedArg("Rijndael-256 (2/2)", 100)));
			}

			// Закрываем все потоки, которые не являлись снинонимами входа или выхода			
			inputStreamAtLevel1.Close();
			inputStreamAtLevel2.Close();

			outputStreamAtLevel0.Close();
			outputStreamAtLevel1.Close();			
		}

		#endregion Public
	}
}