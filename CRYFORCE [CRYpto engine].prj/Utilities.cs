using System;
using System.IO;

using EventArgsUtilities;

namespace CRYFORCE.Engine
{
	public static class Utilities
	{
		/// <summary>
		/// Заполнение потока данными из переданного массива
		/// </summary>
		/// <param name="stream">Исходный поток.</param>
		/// <param name="offset">Смещение от начала.</param>
		/// <param name="pattern">Массив для заполнения потока.</param>
		/// <param name="patternCount">Количество требуемых записей паттерна.</param>
		public static void WipeStream(Stream stream, long offset, byte[] pattern, long patternCount = long.MaxValue)
		{
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
		public static void WipeStream(EventHandler<EventArgs_Generic<ProgressChangedArg>> progressChanged, Stream stream,
		                              int bufferSizePerStream, long offset, long count, bool zeroOut, int rndSeed = int.MinValue)
		{
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
			long nIters = (int)Math.Ceiling((double)(endPosition + 1) / bufferSizePerStream);
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
					WipeStream(stream, offset, rndPattern);
					// PASS 1

					// PASS 2
					for(int i = 0; i < bufferSizePerStream; i++)
					{
						rndPattern[i] = (byte)(rndPattern[i] ^ 0xFF);
					}
					WipeStream(stream, offset, rndPattern);
					// PASS 2

					// PASS 3
					rndSeed ^= DateTime.Now.Ticks.GetHashCode();
					rnd = new Random(rndSeed);
					rnd.NextBytes(rndPattern);
					WipeStream(stream, offset, rndPattern);
					// PASS 3
				}
				else
				{
					// PASS ZERO
					for(int i = 0; i < bufferSizePerStream; i++)
					{
						rndPattern[i] = 0x00;
					}
					WipeStream(stream, offset, rndPattern);
					// PASS ZERO
				}

				//...шагаем по потоку...
				offset += bufferSizePerStream;

				//...и обновляем прогресс...
				if(progressChanged != null)
				{
					progressChanged(null, new EventArgs_Generic<ProgressChangedArg>(new ProgressChangedArg("WipeStream", (nIter / (double)nIters) * 100)));
				}

				// Учитываем произведенную итерацию
				nIter++;
			}

			// Если не работали с нулями...
			if(!zeroOut)
			{
				//...нужно очистить паттерн в ОЗУ
				for(int i = 0; i < bufferSizePerStream; i++)
				{
					rndPattern[i] = 0x00;
				}
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
		public static void WipeFile(EventHandler<EventArgs_Generic<ProgressChangedArg>> progressChanged, string fileName,
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
		/// <param name="workInMemory"></param>
		/// <param name="rndSeed">Инициализирующее значение генератора случайных чисел.</param>
		public static Stream PrepareOutputStream(EventHandler<EventArgs_Generic<ProgressChangedArg>> progressChanged,
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
			else
			{
				return new MemoryStream();
			}
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

			// Получаем список символов, запрещенных к использованию в именах файлов
			char[] charInvalidFileChars = Path.GetInvalidFileNameChars();

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
				foreach(char charInvalid in charInvalidFileChars)
				{
					fileNames[i] = fileNames[i].Replace(charInvalid, ' ');
				}
				fileNames[i] = fileNames[i].Replace(" ", "");

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