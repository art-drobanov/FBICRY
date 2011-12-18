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

			// Создаем ключ на базе двух ключей
			BitmapKey = CryforceUtilities.MergeArrays(key1, key2);
			BitmapKeyIdx = 0;

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
				// Обеспечиваем допустимое значение индекса...
				if(value >= BitmapKey.Length)
				{
					_bitmapKeyIdx = value % BitmapKey.Length;

					// Если режим параноидальный - сразу после "перехода" за размер ключа - обновляем его
					if(ParanoidMode)
					{
						RefreshKey();
					}
				}
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
			byte b1 = BitmapKey[BitmapKeyIdx++];
			byte b2 = BitmapKey[BitmapKeyIdx++];
			BitmapsIdx = (b1 << 8) | b2;
			return Bitmaps[BitmapsIdx];
		}

		/// <summary>
		/// Очистка конфиденциальных данных
		/// </summary>
		public void ClearKey()
		{
			BitmapsIdx = 0;
			BitmapKeyIdx = 0;

			CryforceUtilities.ClearArray(BitmapKey);
		}

		#endregion Public

		#region Private

		/// <summary>
		/// Самообновление ключа
		/// </summary>
		private void RefreshKey()
		{
			// Создаем сущность, позволяющую шифровать потоки
			var streamCryptoWrapper = new StreamCryptoWrapper();

			// Инициализируем сущность для шифрования хешем
			// от текущего ключа (неявно - внутренняя механика)
			streamCryptoWrapper.Initialize(BitmapKey);

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

			// Готовим ключ к обновлению (в смысле размера)
			if(BitmapKey.Length != msOutput.Length)
			{
				CryforceUtilities.ClearArray(BitmapKey);
				BitmapKey = new byte[msOutput.Length];
			}

			// Теперь результаты шифрования нужно перенести в массив ключа
			msOutput.Seek(0, SeekOrigin.Begin);
			msOutput.Read(BitmapKey, 0, BitmapKey.Length);
			csOutput.Close();

			// Обновили ключ - сбросили индекс
			BitmapKeyIdx = 0;
		}

		#endregion Private
	}
}