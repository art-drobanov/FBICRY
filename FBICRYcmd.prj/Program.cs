using System;
using System.IO;
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
		private static void OnProgressChanged(object sender, EventArgsGeneric<ProgressChangedArg> e)
		{
			var processProgress = (int)e.TargetObject.ProcessProgress;
			string messagePostfix = e.TargetObject.MessagePostfix;

			if(processProgress != 100)
			{
				Console.Write("процесс \"{0}\" / прогресс: {1}", e.TargetObject.ProcessDescription, processProgress);
			}
			else
			{
				// Очистка строки (при завершении процесса)
				Console.Write("\r");
				for(int i = 0; i < (Console.BufferWidth - 1); i++)
				{
					Console.Write(" ");
				}
				Console.Write("\r");

				Console.Write("процесс \"{0}\" завершен...", e.TargetObject.ProcessDescription);
			}

			if(messagePostfix == "")
			{
				Console.WriteLine();
			}
			else
			{
				Console.Write(messagePostfix);
			}
		}

		/// <summary>
		/// Сброс режима консоли
		/// </summary>
		private static void ConsoleClearAndPrepare()
		{
			Console.Clear();
			Console.ForegroundColor = ConsoleColor.Green;
			Console.BackgroundColor = ConsoleColor.Black;
		}

		/// <summary>
		/// Вывод логотипа
		/// </summary>
		private static void LogoOut()
		{
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("\t▒▒▒▒▒▒▒▒  ░▒▒▒▒▒▒▒    ▒▒░    ░▒▒▒▒     ▒▒▒▒▒▒▒░   ▒▒▒     ░▒▒");
			Console.WriteLine("\t████████▒ █████████▓  ███  ▓████████▒  █████████▒ ▓██▓   ▒███");
			Console.WriteLine("\t██▒       ██▓    ███  ██▓  ██▓    ███  ██▒    ███  ░██▓ ░██▒ ");
			Console.WriteLine("\t███▓▓▓▓▓  █████████   ██▓ ░██░         ██▓▒▒▒▒██▒   ░██▓██▒  ");
			Console.WriteLine("\t████████  ███▒▒▒▒██▓  ██▓ ░██░         █████████░     ███░   ");
			Console.WriteLine("\t██▒       ██▓    ▓██  ██▓ ░██▓    ███  ██▒    ███     ▓██    ");
			Console.WriteLine("\t██▓       █████████▓  ██▓  ▓████████▒  ██▓    ███     ███    ");
			Console.WriteLine("\t░░        ░░░░░░░░    ░░     ░▒▒▒▒░    ░░     ░░░     ░░░    ");
			Console.WriteLine();
			Console.Write("\t");
		}

		/// <summary>
		/// Вывод версии
		/// </summary>
		private static void VersionOut()
		{
			Console.BackgroundColor = ConsoleColor.DarkGreen;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.Write(" FBICRYcmd 1.0.0.1 (c) 2☺12 Дробанов Артём Федорович (DrAF)  ");
			Console.BackgroundColor = ConsoleColor.Black;
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine();
		}

		/// <summary>
		/// Вывод справки
		/// </summary>
		private static void HelpOut()
		{
			Console.WriteLine("\tFBICRYcmd <команда> <входной файл> <выходной файл> [файл-ключ] [итераций хеша]");
			Console.WriteLine();
			Console.WriteLine("\tКоманды: e  - шифровать (на основе открытого ключа другого абонента);");
			Console.WriteLine("\t         e1 - шифровать (Rijndael-256);");
			Console.WriteLine("\t         e2 - шифровать (двойной Rijndael-256 с перестановкой битов между слоями).");
			Console.WriteLine();
			Console.WriteLine("\tКоманды: d  - дешифровать (на основе открытого ключа другого абонента);");
			Console.WriteLine("\t         d1 - дешифровать (Rijndael-256);");
			Console.WriteLine("\t         d2 - дешифровать (двойной Rijndael-256 с перестановкой битов между слоями).");
			Console.WriteLine();
			Console.WriteLine("\tКоманды: g  - сгенерировать пару открытый/закрытый ключ для ECDH521 (вторым аргументом");
			Console.WriteLine("\t              можно передать файл, который будет использован как набор случайных данных);");
			Console.WriteLine("\t         s  - подписать файл своим закрытым ключом (проверка валидности подписи - открытым);");
			Console.WriteLine("\t         c  - проверить валидность указанной подписи (сам файл находится автоматически)");
			Console.WriteLine("\t              для передаваемого следующим аргументом открытого ключа.");
			Console.WriteLine("\t              Пример проверки подписи: FBICRYcmd.exe с input.txt.sig FBICRY.PUB.txt");
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("\tПри вводе пароля, при нажатии каждой клавиши можно использовать модификаторы");
			Console.WriteLine("\t\"Alt\", \"Shift\", \"Control\"...");
			Console.WriteLine();
		}

		/// <summary>
		/// Проверка версии операционной системы
		/// </summary>
		private static bool OSVersionCheck()
		{
			OperatingSystem osInfo = Environment.OSVersion;

			if(osInfo.Platform != PlatformID.Win32NT)
			{
				return false;
			}

			if(osInfo.Version.Major < 6)
			{
				return false;
			}

			// Vista...
			if((osInfo.Version.Major == 6) && (osInfo.Version.Minor == 0))
			{
				//...должна иметь хотя бы SP1
				if((osInfo.ServicePack == "") || (!osInfo.ServicePack.StartsWith("Service Pack")))
				{
					return false;
				}
			}

			return true;
		}

		private static void Main(string[] args)
		{
			// Работаем в ОЗУ
			bool workInMemory = true;

			// Задаем имена открытого и закрытого ключей
			string publicKeyFilename = "FBICRY.PUB.txt";
			string privateKeyFilename = "FBICRY.ECC.txt";

			// Задаем расширение ЭЦП
			string signExt = ".sig";

			// Выполняем очистку консоли, вывод логотипа и версии
			ConsoleClearAndPrepare();
			LogoOut();
			VersionOut();

			// Текст в консоль комфортнее выводить серым
			Console.ForegroundColor = ConsoleColor.DarkGray;

			// Криптографическое ядро
			var cryforce = new Cryforce();
			cryforce.ProgressChanged += OnProgressChanged;

			// Проверка на запрос генерации пары открытый/закрытый ключ...
			if((args.Length != 0) && (args[0].ToLower() == "g"))
			{
				Stream seedStream = null;

				// Если можно открыть поток с данными, которые будут использоваться как случайные...
				if(args.Length >= 2)
				{
					seedStream = new FileStream(args[1], FileMode.Open, FileAccess.Read);
				}

				// Удаляем выходные файлы, если таковые имеются
				if(File.Exists(publicKeyFilename))
				{
					Console.WriteLine("Файл открытого ключа {0} уже существует!", publicKeyFilename);
					return;
				}

				if(File.Exists(privateKeyFilename))
				{
					Console.WriteLine("Файл закрытого ключа {0} уже существует!", privateKeyFilename);
					return;
				}

				// Открываем потоки для генерирования ключей...
				Stream publicKeyStream = new FileStream(publicKeyFilename, FileMode.Create, FileAccess.Write);
				Stream privateKeyStream = new FileStream(privateKeyFilename, FileMode.Create, FileAccess.Write);

				//...и создаем сами ключи
				cryforce.CreateEccKeys(publicKeyStream, privateKeyStream, seedStream);

				// Закрываем файловые потоки
				publicKeyStream.Flush();
				publicKeyStream.Close();

				privateKeyStream.Flush();
				privateKeyStream.Close();

				if(seedStream != null)
				{
					seedStream.Close();
				}

				Console.WriteLine();
				Console.WriteLine("Генерирование открытого и закрытого ключей завершено!");

				return;
			}

			// Проверка на запрос выполнения ЭЦП...
			if((args.Length != 0) && (args[0].ToLower() == "s"))
			{
				if(!File.Exists(privateKeyFilename))
				{
					Console.WriteLine("Файл закрытого ключа {0} не существует!", privateKeyFilename);
					return;
				}

				// Открываем поток для чтения закрытого ключа и исходного файла...
				Stream privateKeyStream = new FileStream(privateKeyFilename, FileMode.Open, FileAccess.Read);
				Stream dataStream = new FileStream(args[1], FileMode.Open, FileAccess.Read);

				// Имя создаваемого файла с ЭЦП
				string signFilename = args[1] + signExt;

				if(File.Exists(signFilename))
				{
					File.SetAttributes(signFilename, FileAttributes.Normal);
				}

				Stream signStream = new FileStream(signFilename, FileMode.Create, FileAccess.Write);

				// Вычисляем ЭЦП
				Console.WriteLine("Начато вычисление электронной цифровой подписи файла {0}...", args[1]);
				cryforce.SignData(privateKeyStream, dataStream, signStream);

				// Закрываем файловые потоки
				privateKeyStream.Close();
				dataStream.Close();

				signStream.Flush();
				signStream.Close();

				Console.WriteLine("Вычисление электронной цифровой подписи завершено! Создан файл {0}", signFilename);

				return;
			}

			// Проверка на запрос проверки ЭЦП...
			if((args.Length != 0) && (args[0].ToLower() == "c"))
			{
				// Проверяем файл открытого ключа на существование
				if(!File.Exists(args[2]))
				{
					Console.WriteLine("Файл открытого ключа другой стороны {0} не существует!", args[2]);
					return;
				}

				// Определяем имя файла данных, ассоциированного с файлом ЭЦП....
				string dataFileName = args[1].Replace(signExt, "");

				if(!File.Exists(dataFileName))
				{
					Console.WriteLine("Файл данных {0}, ассоциируемый с файлом электронной подписи {1}, не существует!", dataFileName, args[1]);
					return;
				}

				// Открываем потоки для чтения данных, подписи и открытого ключа другой стороны...
				Stream dataStream = new FileStream(dataFileName, FileMode.Open, FileAccess.Read);
				Stream signStream = new FileStream(args[1], FileMode.Open, FileAccess.Read);
				Stream publicKeyFromOtherPartyStream = new FileStream(args[2], FileMode.Open, FileAccess.Read);

				// Осуществляем проверку ЭЦП
				Console.WriteLine("Начата проверка электронной цифровой подписи файла {0}...", dataFileName);
				bool result = cryforce.VerifySign(dataStream, signStream, publicKeyFromOtherPartyStream);

				// Закрываем файловые потоки
				dataStream.Close();
				signStream.Close();
				publicKeyFromOtherPartyStream.Close();

				Console.WriteLine();
				if(result)
				{
					Console.WriteLine("Файл {0} СООТВЕТСТВУЕТ предъявленной электронной подписи {1} и открытому ключу {2}...", dataFileName, args[1], args[2]);
				}
				else
				{
					Console.WriteLine("Файл {0} НЕ СООТВЕТСТВУЕТ предъявленной электронной подписи {1} и открытому ключу {2}!", dataFileName, args[1], args[2]);
				}

				return;
			}

			// Если задано слишком малое количество аргументов - выводим справку...
			if(args.Count() < 3)
			{
				HelpOut();
			}

			// Проверка версии ОС
			if(!OSVersionCheck())
			{
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.WriteLine();
				Console.WriteLine("Внимание: текущая версия операционной системы не поддерживается - требуется Vista SP1 или выше!");

				Console.ResetColor();
				return;
			}

			// Если количество аргументов слишком мало - обработка невозможна
			if(args.Count() < 3)
			{
				Console.ResetColor();
				return;
			}

			// Атрибуты процесса обработки
			bool single = false; // Однослойный шифр?
			bool encryption = false; // Режим шифрования (не дешифрование)?
			bool ecdh = false; // Используется EcdhP521?

			switch(args[0].ToLower())
			{
				case "e":
					{
						single = false;
						encryption = true;
						ecdh = true;
						Console.WriteLine("Режим шифрования на основе открытого ключа другого абонента");
						Console.WriteLine("(двойной Rijndael-256 с перестановкой битов между слоями)...");
						break;
					}
				case "e1":
					{
						single = true;
						encryption = true;
						ecdh = false;
						Console.WriteLine("Режим шифрования (Rijndael-256)...");
						break;
					}
				case "e2":
					{
						single = false;
						encryption = true;
						ecdh = false;
						Console.WriteLine("Режим шифрования (двойной Rijndael-256 с перестановкой битов между слоями)...");
						break;
					}
				case "d":
					{
						single = false;
						encryption = false;
						ecdh = true;
						Console.WriteLine("Режим дешифрования на основе открытого ключа другого абонента");
						Console.WriteLine("(двойной Rijndael-256 с перестановкой битов между слоями)...");
						break;
					}
				case "d1":
					{
						single = true;
						encryption = false;
						ecdh = false;
						Console.WriteLine("Режим дешифрования (Rijndael-256)...");
						break;
					}
				case "d2":
					{
						single = false;
						encryption = false;
						ecdh = false;
						Console.WriteLine("Режим дешифрования (двойной Rijndael-256 с перестановкой битов между слоями)...");
						break;
					}
				default:
					{
						Console.WriteLine("Неизвестный режим обработки!");

						Console.ResetColor();
						return;
					}
			}

			Console.WriteLine();

			// Проверяем входной файл
			if(!File.Exists(args[1]))
			{
				Console.WriteLine("Входной файл {0} не существует!", args[1]);

				Console.ResetColor();
				return;
			}

			// Парольные данные, получаемые из файла
			byte[] passwordDataForKeyFromFile = null;
			byte[] passwordDataForKeyFromFile1 = null;
			byte[] passwordDataForKeyFromFile2 = null;

			// Парольные данные, вводимые с клавиатуры
			byte[] passwordDataForKeyFromKeyboard = null;
			byte[] passwordDataForKeyFromKeyboard1 = null;
			byte[] passwordDataForKeyFromKeyboard2 = null;

			// Результирующие блоки парольных данных
			byte[] passwordDataForKey = null;
			byte[] passwordDataForKey1 = null;
			byte[] passwordDataForKey2 = null;

			// Читаем файл-пароль (если не задан режим работы по Диффи-Хеллману)
			if((!ecdh) && (args.Count() >= 4) && File.Exists(args[3]))
			{
				try
				{
					passwordDataForKeyFromFile = File.ReadAllBytes(args[3]);
					if(passwordDataForKeyFromFile.Length < 2)
					{
						Console.WriteLine("Файл-пароль не может быть меньше 2 байт!");
						throw new Exception();
					}
				}
				catch
				{
					Console.WriteLine("Ошибка чтения данных файла-пароля {0}!", args[3]);

					Console.ResetColor();
					return;
				}

				// Если выбран режим шифрования с двумя ключами...
				if(!single)
				{
					//...разбиваем основной блок парольных данных на две компоненты
					passwordDataForKeyFromFile1 = new byte[passwordDataForKeyFromFile.Length / 2];
					passwordDataForKeyFromFile2 = new byte[passwordDataForKeyFromFile.Length - passwordDataForKeyFromFile1.Length];

					// Каждый подмассив берет свой блок
					Array.Copy(passwordDataForKeyFromFile, 0, passwordDataForKeyFromFile1, 0, passwordDataForKeyFromFile1.Length);
					Array.Copy(passwordDataForKeyFromFile, passwordDataForKeyFromFile1.Length, passwordDataForKeyFromFile2, 0, passwordDataForKeyFromFile2.Length);
				}
			}

			// Получаем ключи по Диффи-Хеллману...
			if(ecdh && (args.Count() >= 4) && File.Exists(args[3]))
			{
				// Проверяем файлы ключей на существование
				if(!File.Exists(args[3]))
				{
					Console.WriteLine("Файл открытого ключа другой стороны {0} не существует!", args[3]);
					return;
				}

				if(!File.Exists(privateKeyFilename))
				{
					Console.WriteLine("Файл закрытого ключа {0} не существует!", privateKeyFilename);
					return;
				}

				// Читаем открытый ключ другой стороны и наш закрытый ключ...
				Stream publicKeyFromOtherPartyStream = new FileStream(args[3], FileMode.Open, FileAccess.Read);
				Stream privateKeyStream = new FileStream(privateKeyFilename, FileMode.Open, FileAccess.Read);

				//...а затем формируем симметричные ключи
				cryforce.GetSymmetricKeys(publicKeyFromOtherPartyStream, privateKeyStream,
				                          out passwordDataForKeyFromFile1, out passwordDataForKeyFromFile2);
			}

			// Пароль с клавиатуры считываем в любом случае...
			if(single)
			{
				Console.WriteLine("Введите пароль:");
				passwordDataForKeyFromKeyboard = CryforceUtilities.GetPasswordBytesSafely();
				Console.WriteLine();
			}
			else
			{
				Console.WriteLine("Введите пароль №1:");
				passwordDataForKeyFromKeyboard1 = CryforceUtilities.GetPasswordBytesSafely();
				Console.WriteLine();

				Console.WriteLine("Введите пароль №2:");
				passwordDataForKeyFromKeyboard2 = CryforceUtilities.GetPasswordBytesSafely();
				Console.WriteLine();
			}

			// Удаляем выходной файл, если такой имеется (невосстановимое удаление)
			if(File.Exists(args[2]))
			{
				Console.WriteLine("Уничтожение данных выходного файла, который будет перезаписан...");
				CryforceUtilities.WipeFile(OnProgressChanged, args[2], cryforce.BufferSizePerStream, true);
				File.Delete(args[2]);
			}

			// Формируем входные и выходные потоки...
			Stream inputStream = new FileStream(args[1], FileMode.Open, FileAccess.Read);
			Stream outputStream = new FileStream(args[2], FileMode.Create, FileAccess.Write);

			int iterations; // Количество итераций хеширования пароля
			if((args.Length < 5) || (!int.TryParse(args[4], out iterations)))
			{
				iterations = 666 * 999; // Итерации хеширования нужны для того, чтобы усложнить brute-force attack
			}

			// Подготовка ключей для шифрования...
			Console.WriteLine("Подготовка ключей для шифрования, {0} итер.", iterations.ToString());
			try
			{
				if(single)
				{
					passwordDataForKey = CryforceUtilities.MergeArrays(passwordDataForKeyFromFile, passwordDataForKeyFromKeyboard);
					cryforce.SingleRijndael(inputStream, passwordDataForKey, outputStream, encryption, iterations);
				}
				else
				{
					passwordDataForKey1 = CryforceUtilities.MergeArrays(passwordDataForKeyFromFile1, passwordDataForKeyFromKeyboard1);
					passwordDataForKey2 = CryforceUtilities.MergeArrays(passwordDataForKeyFromFile2, passwordDataForKeyFromKeyboard2);
					cryforce.DoubleRijndael(inputStream, passwordDataForKey1, passwordDataForKey2, outputStream, encryption, iterations);
				}
			}
			catch
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine();
				Console.WriteLine("Ошибка в ходе криптографического преобразования! Неверный ключ?");

				Console.ResetColor();
				return;
			}

			Console.WriteLine("Сброс буферов...");
			inputStream.Close();
			outputStream.Flush();
			outputStream.Close();

			Console.WriteLine("Очистка ключей...");

			// Парольные данные, получаемые из файла
			CryforceUtilities.ClearArray(passwordDataForKeyFromFile);
			CryforceUtilities.ClearArray(passwordDataForKeyFromFile1);
			CryforceUtilities.ClearArray(passwordDataForKeyFromFile2);

			// Парольные данные, вводимые с клавиатуры
			CryforceUtilities.ClearArray(passwordDataForKeyFromKeyboard);
			CryforceUtilities.ClearArray(passwordDataForKeyFromKeyboard1);
			CryforceUtilities.ClearArray(passwordDataForKeyFromKeyboard2);

			// Результирующие блоки парольных данных
			CryforceUtilities.ClearArray(passwordDataForKey);
			CryforceUtilities.ClearArray(passwordDataForKey1);
			CryforceUtilities.ClearArray(passwordDataForKey2);

			Console.WriteLine();
			Console.WriteLine("Завершено!");

			Console.ResetColor();
		}
	}
}