using System;
using System.IO;
using System.Linq;

using EventArgsUtilities;

namespace CRYFORCE.Engine
{
	/// <summary>
	/// Класс разбиения/склеивания файлов на битовые потоки
	/// </summary>
	public class BitSplitter : IDisposable
	{
		#region Static

		#endregion Static

		#region Constants

		/// <summary>Количество битов в байте.</summary>
		private const int NBITS = 8;

		#endregion Constants

		#region Data

		/// <summary>Битовые потоки.</summary>
		private Stream[] _bitStreams;

		/// <summary>Имена битовых потоков.</summary>
		private string[] _bitStreamsNames;

		#endregion Data

		#region Events

		/// <summary>
		/// Событие обновления прогресса обработки
		/// </summary>
		public event EventHandler<EventArgs_Generic<ProgressChangedArg>> ProgressChanged;

		#endregion Events

		#region .ctor

		/// <summary>
		/// Конструктор по-умолчанию
		/// </summary>
		public BitSplitter()
		{
			DirectMode = true;
			WorkInMemory = false;
			BufferSizePerStream = 16 * 1024 * 1024; // 16 мегабайт
			RndSeed = DateTime.Now.Ticks.GetHashCode();

			// Работа в ОЗУ
			Initialize(true);
		}

		/// <summary>
		/// Конструктор с параметрами
		/// </summary>
		/// <param name="workInMemory">Работать в ОЗУ?</param>
		public BitSplitter(bool workInMemory = true)
		{
			WorkInMemory = workInMemory;
			BufferSizePerStream = 16 * 1024 * 1024; // 16 мегабайт
			RndSeed = DateTime.Now.Ticks.GetHashCode();

			// Работаем так, как желает пользователь
			Initialize(workInMemory);
		}

		/// <summary>
		/// IDisposable
		/// </summary>
		public void Dispose()
		{
			// Очищаем секретные данные
			Clear();

			// Финализатор для данного объекта не запускать!
			GC.SuppressFinalize(this);
		}

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
		/// Инициализация экземпляра класса
		/// </summary>
		/// <param name="workInMemory">Работаем в ОЗУ?</param>
		public void Initialize(bool workInMemory = true)
		{
			// Генерируем случайные имена файлов...
			_bitStreamsNames = Utilities.GetRandomFilenames(NBITS, NBITS, RndSeed);
			_bitStreamsNames = _bitStreamsNames.Select(item => item + ".jpg").ToArray();

			// Если работаем не в ОЗУ...
			if(!workInMemory)
			{
				// Создаем массив буферизированных потоков...
				_bitStreams = new BufferedStream[NBITS];

				// Для всех потоков...
				for(int i = 0; i < _bitStreams.Length; i++)
				{
					//...если указанный временный файл уже существует...
					if(File.Exists(_bitStreamsNames[i]))
					{
						//...для того, чтобы повысить вероятность успешного удаления файла, устанавливаем нормальные атрибуты...
						File.SetAttributes(_bitStreamsNames[i], FileAttributes.Normal);

						//...стираем его содержимое...
						_bitStreams[i] = new BufferedStream(new FileStream(_bitStreamsNames[i], FileMode.Open, FileAccess.ReadWrite, FileShare.None), BufferSizePerStream);
						Utilities.WipeStream(ProgressChanged, _bitStreams[i], BufferSizePerStream, 0, _bitStreams[i].Length, ZeroOut);

						//...и, затем, удаляем его
						File.Delete(_bitStreamsNames[i]);
					}

					// Создаем файловый поток с требуемым именем...
					_bitStreams[i] = new BufferedStream(new FileStream(_bitStreamsNames[i], FileMode.Create, FileAccess.ReadWrite, FileShare.None), BufferSizePerStream);
				}
			}
			else // Если работаем в ОЗУ...
			{
				// Если работаем в ОЗУ - затираем потоки нулями
				ZeroOut = true;

				// ...создаем массив потоков в памяти
				_bitStreams = new MemoryStream[NBITS];

				// Для всех потоков...
				for(int i = 0; i < _bitStreams.Length; i++)
				{
					_bitStreams[i] = new MemoryStream();
				}
			}

			// Указываем, что инициализация прошла успешно
			IsInitialized = true;
		}

		/// <summary>
		/// Разбиение на битовые потоки с последующим "склеиванием" в единый поток бит
		/// </summary>
		/// <param name="inputStream">Входной поток.</param>
		/// <param name="outputStream">Выходной поток.</param>
		public void SplitToBitstream(Stream inputStream, Stream outputStream)
		{
			// Исходный,...
			inputStream.Seek(0, SeekOrigin.Begin);

			//...битовые...
			for(int i = 0; i < NBITS; i++)
			{
				_bitStreams[i].Seek(0, SeekOrigin.Begin);
			}

			//...и целевой поток устанавливаем на начало
			outputStream.Seek(0, SeekOrigin.Begin);

			// Устанавливаем начальные параметры для успешного проведения итераций
			long offset = 0;
			long remaining = inputStream.Length;

			// Создаем буфер для работы блоками по 8 байт
			var bytesIn = new byte[NBITS];
			var bytesOut = new byte[NBITS];

			int read = 0; // Счетчик количества считанных байт
			int toRead = 0; // Счетчик количества байт, которые нужно считать

			// Вычисляем выравнивание до 8 байт
			var align8 = (byte)(inputStream.Length % NBITS);

			// Если выравнивание не требуется - писать в выходной поток ничего не будем
			if(align8 != 0)
			{
				// Вычисляем выравнивание входного потока до 8 байт (маскируя его дополнительными битами)...
				var rnd = new Random(RndSeed ^ DateTime.Now.Ticks.GetHashCode());
				var align8rnd = (byte)(align8 | (0xF8 & (rnd.Next(0, 31) << 3)));

				//...и пишем его в выходной поток
				outputStream.WriteByte(align8rnd);

				// Первый блок данных требуется считывать особым образом - так, чтобы учесть невыровненность по границе 8 байт -
				// для этого заполняем его случайными данными и пишем в него реальные данные из исходного потока, начиная с некоторой позиции
				rnd.NextBytes(bytesIn);

				read = 0;
				toRead = align8;
				while((toRead -= (read += inputStream.Read(bytesIn, read, toRead))) != 0) ;
				Split8Bytes(bytesIn, bytesOut); // Bit-splitting...

				// Запись соответствующих бит на свои места в битовые потоки
				for(int i = 0; i < NBITS; i++)
				{
					_bitStreams[i].WriteByte(bytesOut[i]);
				}

				// Учитываем обработанный объем
				remaining -= read;
			}

			// Пока есть объем для обработки...
			while(remaining > 0)
			{
				read = 0;
				toRead = NBITS;
				while((toRead -= (read += inputStream.Read(bytesIn, read, toRead))) != 0) ;
				Split8Bytes(bytesIn, bytesOut); // Bit-splitting...

				// Запись соответствующих бит на свои места в битовые потоки
				for(int i = 0; i < NBITS; i++)
				{
					_bitStreams[i].WriteByte(bytesOut[i]);
				}

				// Учитываем обработанный объем
				remaining -= read;
			}

			// Содержимое всех битовых потоков переносим в выходной поток
			for(int i = 0; i < NBITS; i++)
			{
				_bitStreams[i].Seek(0, SeekOrigin.Begin);
				_bitStreams[i].CopyTo(outputStream);
			}

			// Чистим данные...
			for(int i = 0; i < NBITS; i++)
			{
				bytesIn[i] = bytesOut[i] = 0x00;
			}

			// Синхронизируем буфер с физическим носителем...
			outputStream.Flush();
		}

		/// <summary>
		/// Считывание из единого битового потока с последующим восстановлением порядка следования бит
		/// </summary>
		/// <param name="inputStream">Входной поток.</param>
		/// <param name="outputStream">Выходной поток.</param>
		public void UnsplitFromBitstream(Stream inputStream, Stream outputStream)
		{
			// Исходный,...
			inputStream.Seek(0, SeekOrigin.Begin);

			//...битовые...
			for(int i = 0; i < NBITS; i++)
			{
				_bitStreams[i].Seek(0, SeekOrigin.Begin);
			}

			//...и целевые потоки устанавливаем на начало
			outputStream.Seek(0, SeekOrigin.Begin);

			// Выделяем временный буфер
			var buffer = new byte[BufferSizePerStream];

			// Создаем буфер для работы блоками по 8 байт
			var bytesIn = new byte[NBITS];
			var bytesOut = new byte[NBITS];

			// Вычисляем выравнивание входного потока до 8 байт (отбрасывая маскирующие биты)...
			byte align8;

			// Количество байт для копирования в соотв. битстрим
			long remaining;

			// Автодетект выравнивания
			if(inputStream.Length % NBITS != 0)
			{
				align8 = (byte)(inputStream.ReadByte() & 0x07);
				remaining = inputStream.Length - 1;
			}
			else
			{
				align8 = 0;
				remaining = inputStream.Length;
			}

			// Вычисляем размер битстрима
			long bitstreamSize = remaining / NBITS;

			// Разбиваем исходный поток на 8 битстримов...
			for(int i = 0; i < NBITS; i++)
			{
				// Количество байт для копирования в соотв. битстрим
				long remaining2 = bitstreamSize;

				// Пока есть что копировать...
				while(remaining2 > 0)
				{
					//...вычисляем количество байт для считывания
					int toRead = (remaining2 < buffer.Length) ? (int)remaining2 : buffer.Length;
					int read = 0;
					while((toRead -= (read += inputStream.Read(buffer, read, toRead))) != 0) ;

					// Учитываем отработанный объем
					remaining2 -= read;
				}

				// Считали блок байт принадлежащий некоторому битстриму - помещаем его на свое место...
				_bitStreams[i].Write(buffer, 0, (int)bitstreamSize);
			}

			// Чистим временный буфер
			for(int i = 0; i < buffer.Length; i++)
			{
				buffer[i] = 0x00;
			}

			// После того, как в битовых потоках восстановлены все байты - можно производить их разбор...

			// Устанавливаем исходные потоки на начало...
			for(int i = 0; i < NBITS; i++)
			{
				_bitStreams[i].Seek(0, SeekOrigin.Begin);
			}

			// Автодетект выравнивания
			if(inputStream.Length % NBITS != 0)
			{
				// Читаем данные из битовых потоков...
				for(int i = 0; i < NBITS; i++)
				{
					bytesIn[i] = (byte)_bitStreams[i].ReadByte();
				}
				//...восстанавливаем порядок бит...
				Split8Bytes(bytesIn, bytesOut); // Bit-splitting...

				//...и пишем данные на свои места...
				//...параллельно "сливаем отстой" генератора случайных чисел записывая лишь align8
				outputStream.Write(bytesOut, 0, align8);

				// Учитываем обработанный объем
				remaining -= NBITS;
			}

			// Пока есть объем для обработки...
			while(remaining > 0)
			{
				// Читаем данные из битовых потоков...
				for(int i = 0; i < NBITS; i++)
				{
					bytesIn[i] = (byte)_bitStreams[i].ReadByte();
				}
				//...восстанавливаем порядок бит...
				Split8Bytes(bytesIn, bytesOut); // Bit-splitting...

				//...и пишем данные на свои места...
				outputStream.Write(bytesOut, 0, NBITS);

				// Учитываем обработанный объем
				remaining -= NBITS;
			}

			// Чистим данные...
			for(int i = 0; i < NBITS; i++)
			{
				bytesIn[i] = bytesOut[i] = 0x00;
			}

			// Синхронизируем буфер с физическим носителем...
			outputStream.Flush();
		}

		/// <summary>
		/// Метод разбиения группы из 8 байт на набор "битовых" байт
		/// </summary>
		/// <param name="bytesIn">Исходный набор байт.</param>
		/// <param name="bytesOut">Массив "битовых" байт.</param>
		/// <returns>Массив "битовых" байт.</returns>
		public void Split8Bytes(byte[] bytesIn, byte[] bytesOut)
		{
			// Каждый i-ый байт выходного потока формируется из i-ых бит
			for(int i = 0; i < NBITS; i++)
			{
				int b = 0x00;
				for(int j = 0; j < NBITS; j++)
				{
					b |= ((bytesIn[j] >> i) & 0x01) << j;
				}
				bytesOut[i] = (byte)b;
			}
		}

		/// <summary>
		/// Очистка конфиденциальных данных
		/// </summary>
		public void Clear()
		{
			Clear(RndSeed, ZeroOut);
		}

		/// <summary>
		/// Очистка конфиденциальных данных
		/// </summary>
		/// <param name="rndSeed">Инициализирующее значение генератора случайных чисел.</param>
		/// <param name="zeroOut">Затирать выходной поток нулями?</param>
		public void Clear(int rndSeed, bool zeroOut)
		{
			// Указываем на деинициализацию
			IsInitialized = false;

			// Производим стирание данных потоков, чтобы было невозможным восстановление) при помощи программных средств
			foreach(Stream bitStream in _bitStreams)
			{
				Utilities.WipeStream(ProgressChanged, bitStream, BufferSizePerStream, 0, bitStream.Length, zeroOut, rndSeed);
			}
		}

		#endregion Public
	}
}