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
				// Очистка строки
				Console.Write("\r");
				for(int i = 0; i < 80; i++)
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
		private static void ConsoleClear()
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
			Console.Write(" FBICRYcmd 1.00 (c) 2012 Дробанов Артём Федорович (DrAF)     ");
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
			Console.WriteLine("\tFBICRYcmd <команда> <входной файл> <выходной файл> [файл-пароль] [итераций хеша]");
			Console.WriteLine();
			Console.WriteLine("\tКоманды: e1 - шифровать (Rijndael-256);");
			Console.WriteLine("\t         e2 - шифровать (двойной Rijndael-256 с перестановкой битов между слоями);");
			Console.WriteLine("\t         e3 - шифровать (e2, параноидальный режим, медленный!);");
			Console.WriteLine();
			Console.WriteLine("\tКоманды: d1 - дешифровать (Rijndael-256);");
			Console.WriteLine("\t         d2 - дешифровать (двойной Rijndael-256 с перестановкой битов между слоями);");
			Console.WriteLine("\t         d3 - дешифровать (d2, параноидальный режим, медленный!);");
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("\tДля шифрования с паролем, вводимым с клавиатуры, укажите несуществующий");
			Console.WriteLine("\tфайл-пароль, либо не указывайте его вообще (приложение запросит ввод).");
			Console.WriteLine("\tПри вводе пароля, при нажатии каждой клавиши можно использовать");
			Console.WriteLine("\tмодификаторы \"Alt\", \"Shift\", \"Control\"...");
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

			ConsoleClear();
			LogoOut();
			VersionOut();

			Console.ForegroundColor = ConsoleColor.DarkGray;

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

			var cryforce = new Cryforce();
			cryforce.ProgressChanged += OnProgressChanged;

			// Атрибуты процесса обработки
			bool single = false;
			bool encryption = false;
			bool paranoid = false;

			switch(args[0].ToLower())
			{
				case "e1":
					{
						single = true;
						encryption = true;
						paranoid = false;
						Console.WriteLine("Режим шифрования (Rijndael-256)...");
						break;
					}
				case "e2":
					{
						single = false;
						encryption = true;
						paranoid = false;
						Console.WriteLine("Режим шифрования (двойной Rijndael-256 с перестановкой битов между слоями)...");
						break;
					}
				case "e3":
					{
						single = false;
						encryption = true;
						paranoid = true;
						Console.WriteLine("Режим шифрования (двойной Rijndael-256 с перестановкой битов между слоями, параноидальный режим, медленный!)...");
						break;
					}
				case "d1":
					{
						single = true;
						encryption = false;
						paranoid = false;
						Console.WriteLine("Режим дешифрования (Rijndael-256)...");
						break;
					}
				case "d2":
					{
						single = false;
						encryption = false;
						paranoid = false;
						Console.WriteLine("Режим дешифрования (двойной Rijndael-256 с перестановкой битов между слоями)...");
						break;
					}
				case "d3":
					{
						single = false;
						encryption = false;
						paranoid = true;
						Console.WriteLine("Режим дешифрования (двойной Rijndael-256 с перестановкой битов между слоями, параноидальный режим, медленный!)...");
						break;
					}
				default:
					{
						Console.WriteLine("Неизвестный режим обработки!");

						Console.ResetColor();
						break;
					}
			}

			Console.WriteLine();

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

			// Читаем файл-пароль...
			if((args.Count() >= 4) && File.Exists(args[3]))
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
					cryforce.DoubleRijndael(inputStream, passwordDataForKey1, passwordDataForKey2, outputStream, encryption, paranoid, iterations);
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