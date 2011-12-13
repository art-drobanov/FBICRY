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

			if(processProgress != 100)
			{
				Console.WriteLine("процесс \"{0}\" / прогресс: {1}", e.TargetObject.ProcessDescription, processProgress);
			}
			else
			{
				Console.WriteLine("процесс \"{0}\" завершен...", e.TargetObject.ProcessDescription, processProgress);
			}
		}

		/// <summary>
		/// Вывод логотипа
		/// </summary>
		private static void LogoOut()
		{
			Console.Clear();
			Console.ForegroundColor = ConsoleColor.Green;
			Console.BackgroundColor = ConsoleColor.Black;
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

			byte[] passwordData = null;
			byte[] passwordDataForKey1 = null;
			byte[] passwordDataForKey2 = null;

			if((args.Count() >= 4) && File.Exists(args[3]))
			{
				try
				{
					passwordData = File.ReadAllBytes(args[3]);
					if(passwordData.Length < 2)
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

				passwordDataForKey1 = new byte[passwordData.Length / 2];
				passwordDataForKey2 = new byte[passwordData.Length - passwordDataForKey1.Length];
			}
			else
			{
				if(single)
				{
					Console.WriteLine("Введите пароль:");
					passwordDataForKey1 = CryforceUtilities.GetPasswordBytesSafely();
					Console.WriteLine();
				}
				else
				{
					Console.WriteLine("Введите пароль №1:");
					passwordDataForKey1 = CryforceUtilities.GetPasswordBytesSafely();
					Console.WriteLine();

					Console.WriteLine("Введите пароль №2:");
					passwordDataForKey2 = CryforceUtilities.GetPasswordBytesSafely();
					Console.WriteLine();
				}
			}

			if(File.Exists(args[2]))
			{
				File.Delete(args[2]);
			}

			Stream inputStream = new FileStream(args[1], FileMode.Open, FileAccess.Read);
			Stream outputStream = new FileStream(args[2], FileMode.Create, FileAccess.Write);

			int iterations;

			if((args.Length < 5) || (!int.TryParse(args[4], out iterations)))
			{
				iterations = 666 * 999;
			}

			Console.WriteLine("Обработка...");

			try
			{
				if(single)
				{
					cryforce.SingleRijndael(inputStream, passwordDataForKey1, outputStream, encryption, iterations);
				}
				else
				{
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

			if(passwordData != null)
			{
				Array.Clear(passwordData, 0, passwordData.Length);
			}

			if(passwordDataForKey1 != null)
			{
				Array.Clear(passwordDataForKey1, 0, passwordDataForKey1.Length);
			}

			if(passwordDataForKey2 != null)
			{
				Array.Clear(passwordDataForKey2, 0, passwordDataForKey2.Length);
			}

			Console.WriteLine();
			Console.WriteLine("Завершено!");

			Console.ResetColor();
		}
	}
}