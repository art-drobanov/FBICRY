using System;
using System.IO;
using System.Security.Cryptography;

namespace CRYFORCE.Engine
{
	/// <summary>
	/// Класс перестановок битовой карты (для перемешивания битов)
	/// </summary>
	public class BitMaps : IDisposable
	{
		#region Data

		/// <summary>Экземпляр класса "SHA256".</summary>
		private readonly SHA256Cng _hash256;

		/// <summary>Индекс в ключе для работы с битовой картой.</summary>
		private int _bitmapKeyIdx;

		/// <summary>Текущий индекс в перестановках битовой карты.</summary>
		private int _bitmapsIdx;

		#endregion Data

		#region .ctor

		/// <summary>
		/// Конструктор с параметрами
		/// </summary>
		/// <param name="key1">Ключ для первого прохода шифрования.</param>
		/// <param name="key2">Ключ для второго прохода шифрования.</param>
		/// <param name="paranoidMode">Параноидальный режим?</param>
		public BitMaps(byte[] key1, byte[] key2, bool paranoidMode)
		{
			ParanoidMode = paranoidMode;
			Bitmaps = new Permutations<int>(new[] {0, 1, 2, 3, 4, 5, 6, 7}).MakePermutationsSet(); // 40320 перестановок 8 бит
			BitmapsIdx = 0;

			BitmapKey = new byte[key1.Length + key2.Length]; // Создаем ключ на базе двух ключей
			BitmapKeyIdx = 0;

			Array.Copy(key1, 0, BitmapKey, 0, key1.Length); // Сначала копируем первый ключ в результирующий массив...
			Array.Copy(key2, 0, BitmapKey, key1.Length, key2.Length); //...затем к нему добавляем второй

			// Инициализируем хеш-функцию
			_hash256 = new SHA256Cng();

			// Производим самообновление ключа
			RefreshKey();

			if(paranoidMode)
			{
				RefreshKey();
			}
		}

		/// <summary>
		/// IDisposable
		/// </summary>
		public void Dispose()
		{
			// Очищаем секретные данные
			ClearKey();

			// Финализатор для данного объекта не запускать!
			GC.SuppressFinalize(this);
		}

		#endregion .ctor

		#region Properties

		/// <summary>Перестановки битовой карты.</summary>
		public int[][] Bitmaps { get; private set; }

		/// <summary>Текущий индекс в перестановках битовой карты.</summary>
		public int BitmapsIdx
		{
			get { return _bitmapsIdx; }
			set { _bitmapsIdx = value % 40320; } // 40320 = 8!
		}

		/// <summary>Ключ для работы с битовой картой.</summary>
		public byte[] BitmapKey { get; private set; }

		/// <summary>Параноидальный режим?</summary>
		public bool ParanoidMode { get; private set; }

		/// <summary>Индекс в ключе для работы с битовой картой.</summary>
		public int BitmapKeyIdx
		{
			get { return _bitmapKeyIdx; }
			set
			{
				// Если будет переход за пределы последовательности -
				// требуется обновление массива через шифрование
				if((ParanoidMode) && (value >= BitmapKey.Length))
				{
					RefreshKey();
				}
				_bitmapKeyIdx = value % BitmapKey.Length;
			}
		}

		#endregion Properties

		#region Public

		/// <summary>
		/// Метод получения следующей перестановки (на базе ключевых данных)
		/// </summary>
		/// <returns>Битовая перестановка.</returns>
		public int[] GetNextBitmap()
		{
			// Два шага вперед - один шаг назад
			byte b1 = BitmapKey[BitmapKeyIdx++];
			byte b2 = BitmapKey[BitmapKeyIdx++];

			// Если на ++ ключ не был обновлен, можно сделать шаг назад -
			// так ключ будет расходоваться экономнее и получено
			// будет больше комбинаций
			if(BitmapKeyIdx != 0)
			{
				BitmapKeyIdx--;
			}

			BitmapsIdx = (b1 << 8) | b2;
			return Bitmaps[BitmapsIdx];
		}

		/// <summary>
		/// Очистка конфиденциальных данных
		/// </summary>
		public void ClearKey()
		{
			_hash256.Clear();

			BitmapsIdx = 0;
			BitmapKeyIdx = 0;

			for(int i = 0; i < BitmapKey.Length; i++)
			{
				BitmapKey[i] = 0;
			}
		}

		#endregion Public

		#region Private

		/// <summary>
		/// Самообновление ключа
		/// </summary>
		private void RefreshKey()
		{
			var streamCryptoWrapper = new StreamCryptoWrapper();

			// Инициализируем сущность для шифрования хешем от текущего ключа
			streamCryptoWrapper.Initialize(_hash256.ComputeHash(BitmapKey));

			// Входной поток формируем на основе массива ключа...
			Stream msInput = new MemoryStream(BitmapKey);

			//...выходной поток изначально пуст...
			Stream msOutput = new MemoryStream();
			//...но его оснащаем механизмом шифрования...
			Stream csOutput = streamCryptoWrapper.WrapStream(msOutput, true);

			// Процесс шифрования/расшифровки происходит прозрачно
			msInput.CopyTo(csOutput);
			((CryptoStream)csOutput).FlushFinalBlock();

			// После шифрования закрываем поток-источник,...
			msInput.Close();

			//...сбрасываем буфер на зашифрованном потоке...
			csOutput.Flush();
			msOutput.Flush();

			//...и как завершение шифрования - деинициализируем криптовраппер
			streamCryptoWrapper.Clear();

			// Теперь результаты шифрования нужно перенести в массив ключа
			BitmapKey = new byte[msOutput.Length];

			msOutput.Seek(0, SeekOrigin.Begin);
			msOutput.Read(BitmapKey, 0, BitmapKey.Length);
			csOutput.Close();

			// Обновили ключ - сбросили индекс
			BitmapKeyIdx = 0;
		}

		#endregion Private
	}
}