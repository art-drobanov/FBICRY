using System;
using System.IO;
using System.Text;
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
		private static void OnProgressChanged(object sender, EventArgs_Generic<ProgressChangedArg> e)
		{
			Console.WriteLine("процесс {0} / прогресс: {1}", e.TargetObject.ProcessDescription, (int)e.TargetObject.ProcessProgress);
		}

		private static void Main(string[] args)
		{
			// Работаем в ОЗУ
			bool workInMemory = true;

			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine();
			Console.WriteLine("\t***************************************************************************************************");
			Console.WriteLine("\t*                                                                                                 *");
			Console.Write("\t*");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write("   FBICRYcmd 0.01 (c) 2011 DrAF, г. Череповец                                                    ");
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine("*");			
			Console.WriteLine("\t*                                                                                                 *");			
			Console.Write("\t*");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write("   Утилита для шифрования файлов двойным Rijndael-256 с битовым транспонированием между слоями.  ");
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine("*");
			Console.WriteLine("\t*                                                                                                 *");			
			Console.WriteLine("\t***************************************************************************************************");
			Console.WriteLine();
			Console.ResetColor();

			if(args.Count() < 3)
			{
				Console.WriteLine("\tИспользование: FBICRYcmd <команда> <входной файл> <выходной файл> [файл-пароль] [итераций хеша]\n");
				Console.WriteLine("\tКоманды: e - шифровать");
				Console.WriteLine("\t         d - расшифровать\n");
				Console.WriteLine();
				Console.WriteLine("\tДля шифрования с паролем, вводимым с клавиатуры, укажите несуществующий файл-пароль,");
				Console.WriteLine("\tлибо не указывайте его вообще. Приложение запросит ввод.");
				Console.WriteLine("\tПри нажатии каждой клавиши можно использовать модификаторы \"Alt\", \"Shift\", \"Control\"...");
				Console.WriteLine();

				return;
			}
		
			var cryforce = new Cryforce();
			cryforce.ProgressChanged += OnProgressChanged;

			if(args[0].ToLower() == "e")
			{
				Console.WriteLine("Режим шифрования...");
			}
			else if(args[0].ToLower() == "d")
			{
				Console.WriteLine("Режим расшифровки...");
			}
			else
			{
				Console.WriteLine("Неизвестный режим обработки!");
				return;
			}

			Console.WriteLine();

			if(!File.Exists(args[1]))
			{
				Console.WriteLine("Входной файл {0} не существует!", args[1]);
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
					return;
				}

				passwordDataForKey1 = new byte[passwordData.Length / 2];
				passwordDataForKey2 = new byte[passwordData.Length - passwordDataForKey1.Length];
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

			if(File.Exists(args[2]))
			{
				File.Delete(args[2]);
			}

			Stream inputStream = new FileStream(args[1], FileMode.Open, FileAccess.Read);
			Stream outputStream = new FileStream(args[2], FileMode.Create, FileAccess.Write);

			int iterations;
			try
			{
				iterations = int.Parse(args[4]);
			}
			catch
			{
				iterations = 666 * 999;
			}

			Console.WriteLine("Обработка...");

			try
			{
				cryforce.DoubleRijndael(inputStream, passwordDataForKey1, passwordDataForKey2, outputStream, args[0].ToLower() == "e", iterations);
			}
			catch
			{
				Console.WriteLine();
				Console.WriteLine("Ошибка в ходе криптографического преобразования!");
				return;
			}

			Console.WriteLine("Сброс буферов...");
			inputStream.Close();
			outputStream.Flush();
			outputStream.Close();

			Console.WriteLine("Очистка ключей...");

			if(passwordData != null)
			{
				for(int i = 0; i < passwordData.Length; i++)
				{
					passwordData[i] = 0x00;
				}
			}

			for(int i = 0; i < passwordDataForKey1.Length; i++)
			{
				passwordDataForKey1[i] = 0x00;
			}

			for(int i = 0; i < passwordDataForKey2.Length; i++)
			{
				passwordDataForKey2[i] = 0x00;
			}
		
			Console.WriteLine("Завершено!");
		}
	}
}