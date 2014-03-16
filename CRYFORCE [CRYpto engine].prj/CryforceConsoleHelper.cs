#region

using System;
using System.IO;
using System.Linq;

using EventArgsUtilities;

#endregion

namespace CRYFORCE.Engine
{
    public static class CryforceConsoleHelper
    {
        #region Private

        /// <summary>
        /// Версия ПО.
        /// </summary>
        private static string buildVersion = "1.0.7.15";

        /// <summary>
        /// Год.
        /// </summary>
        private static string buildYear = "2013";

        /// <summary>
        /// Стандартное имя открытого ключа.
        /// </summary>
        private static string publicKeyFilename = "FBICRY.PUB.txt";

        /// <summary>
        /// Стандартное имя закрытого ключа.
        /// </summary>
        private static string privateKeyFilename = "FBICRY.ECC.txt";

        /// <summary>
        /// Стандартное имя лога ошибок.
        /// </summary>
        private static string errLogFilename = "!ErrLog.txt";

        /// <summary>
        /// Задаем расширение ЭЦП.
        /// </summary>
        private static string signExt = ".sig";

        /// <summary>
        /// Обработчик события "Изменился прогресс обработки"
        /// </summary>
        private static void OnProgressChanged(object sender, EventArgsGeneric<ProgressChangedArg> e)
        {
            var processProgress = (int)e.TargetObject.ProcessProgress;
            string messagePostfix = e.TargetObject.MessagePostfix;

            if(processProgress != 100)
            {
                Console.Write("процесс \"{0}\" / прогресс: {1} % ", e.TargetObject.MessageBody, processProgress);
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

                Console.Write("процесс \"{0}\" завершен...", e.TargetObject.MessageBody);
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
        /// Обработчик события "Получено сообщение"
        /// </summary>
        private static void OnMessageReceived(object sender, EventArgsGeneric<MessageReceivedArg> e)
        {
            Console.Write(e.TargetObject.MessageBody);

            string messagePostfix = e.TargetObject.MessagePostfix;
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
        /// <param name="msg"> Ссылка на интерфейс вывода сообщений. </param>
        private static void LogoOut(IMessage msg)
        {
            msg.Message("");
            msg.Message("");
            msg.Message("\t▒▒▒▒▒▒▒▒  ░▒▒▒▒▒▒▒    ▒▒░    ░▒▒▒▒     ▒▒▒▒▒▒▒░   ▒▒▒     ░▒▒");
            msg.Message("\t████████▒ █████████▓  ███  ▓████████▒  █████████▒ ▓██▓   ▒███");
            msg.Message("\t██▒       ██▓    ███  ██▓  ██▓    ███  ██▒    ███  ░██▓ ░██▒ ");
            msg.Message("\t███▓▓▓▓▓  █████████   ██▓ ░██░         ██▓▒▒▒▒██▒   ░██▓██▒  ");
            msg.Message("\t████████  ███▒▒▒▒██▓  ██▓ ░██░         █████████░     ███░   ");
            msg.Message("\t██▒       ██▓    ▓██  ██▓ ░██▓    ███  ██▒    ███     ▓██    ");
            msg.Message("\t██▓       █████████▓  ██▓  ▓████████▒  ██▓    ███     ███    ");
            msg.Message("\t░░        ░░░░░░░░    ░░     ░▒▒▒▒░    ░░     ░░░     ░░░    ");
            msg.Message("");
        }

        /// <summary>
        /// Вывод версии
        /// </summary>
        /// <param name="msg"> Ссылка на интерфейс вывода сообщений. </param>
        private static void VersionOut(IMessage msg)
        {
            msg.Message("", "\t");
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.Black;
            msg.Message(string.Format(" FBICRYcmd {0} (c) {1} Дробанов Артём Федорович (DrAF) ", buildVersion, buildYear));
            Console.BackgroundColor = ConsoleColor.Black;
            msg.Message("");
            msg.Message("");
        }

        /// <summary>
        /// Вывод справки
        /// </summary>
        /// <param name="msg"> Ссылка на интерфейс вывода сообщений. </param>
        private static void HelpOut(IMessage msg)
        {
            msg.Message("\tFBICRYcmd <команда> <входной файл> <выходной файл> [файл-ключ] [итераций хеша]");
            msg.Message("");
            msg.Message("\tКоманды: e  - шифровать (на основе открытого ключа другого абонента);");
            msg.Message("\t         e1 - шифровать (Rijndael-256);");
            msg.Message("\t         e2 - шифровать (двойной Rijndael-256 с перестановкой битов между слоями).");
            msg.Message("");
            msg.Message("\tКоманды: d  - дешифровать (на основе открытого ключа другого абонента);");
            msg.Message("\t         d1 - дешифровать (Rijndael-256);");
            msg.Message("\t         d2 - дешифровать (двойной Rijndael-256 с перестановкой битов между слоями).");
            msg.Message("");
            msg.Message("\tКоманды: g  - сгенерировать пару открытый/закрытый ключ для ECDH521 (вторым аргументом");
            msg.Message("\t              можно передать файл, который будет использован как набор случайных данных);");
            msg.Message("\t         s  - подписать файл своим закрытым ключом (проверка валидности подписи - открытым);");
            msg.Message("\t              Пример вычисления подписи: FBICRYcmd.exe s input.txt");
            msg.Message("\t         c  - проверить валидность указанной подписи (сам файл находится автоматически)");
            msg.Message("\t              для передаваемого следующим аргументом открытого ключа.");
            msg.Message("\t              Пример проверки подписи: FBICRYcmd.exe с input.txt.sig FBICRY.PUB.txt");
            msg.Message("");
            msg.Message("\tПри вводе пароля, при нажатии каждой клавиши можно использовать модификаторы");
            msg.Message("\t\"Alt\", \"Shift\", \"Control\"...");
        }

        /// <summary>
        /// Запись исключения в лог ошибок
        /// </summary>
        /// <param name="errLogFilename"> Имя перезаписываемого лог-файла. </param>
        /// <param name="e"> Исключение, текст которого нужно сохранить. </param>
        private static void ErrLogOut(string errLogFilename, Exception e)
        {
            if(File.Exists(errLogFilename))
            {
                File.SetAttributes(errLogFilename, FileAttributes.Normal);
                File.Delete(errLogFilename);
            }
            File.WriteAllText(errLogFilename, e.ToString());
        }

        #endregion Private

        #region Public

        /// <summary>
        /// Основной метод обработки, ориентированный на имена файлов
        /// </summary>
        public static void Main(string[] args)
        {
            // Криптографическое ядро...
            var cf = new Cryforce();
            //...должно быть подписано на события обновления прогресса...
            cf.ProgressChanged += OnProgressChanged;
            //...и вывода сообщений
            cf.MessageReceived += OnMessageReceived;

            // Выполняем очистку консоли, вывод логотипа и версии...
            try
            {
                ConsoleClearAndPrepare();
                LogoOut(cf);
                VersionOut(cf);
            }
            catch
            {
                //...при возникновении проблем - сообщаем об этом
                cf.Message("Ошибка вывода в консоль!");
                return;
            }

            // Текст в консоль комфортнее выводить серым
            Console.ForegroundColor = ConsoleColor.DarkGray;

            // Проверка на запрос генерации пары открытый/закрытый ключ...
            if((args.Length != 0) && (args[0].ToLower() == "g"))
            {
                Stream seedStream = null;

                // Если можно открыть поток с данными, которые будут использоваться как случайные...
                if(args.Length >= 2)
                {
                    seedStream = new FileStream(args[1], FileMode.Open, FileAccess.Read);
                }

                // Удаляем выходные файлы, если таковые имеются...
                if(File.Exists(publicKeyFilename))
                {
                    cf.Message(string.Format("Файл открытого ключа \"{0}\" уже существует!", publicKeyFilename));
                    return;
                }

                if(File.Exists(privateKeyFilename))
                {
                    cf.Message(string.Format("Файл закрытого ключа \"{0}\" уже существует!", privateKeyFilename));
                    return;
                }

                // Открываем потоки для генерирования ключей...
                Stream publicKeyStream = new FileStream(publicKeyFilename, FileMode.Create, FileAccess.Write);
                Stream privateKeyStream = new FileStream(privateKeyFilename, FileMode.Create, FileAccess.Write);

                //...и создаем сами ключи
                try
                {
                    cf.CreateEccKeys(publicKeyStream, privateKeyStream, seedStream);
                }
                catch(Exception e)
                {
                    cf.Message("Ошибка при создании ключей!");
                    ErrLogOut(errLogFilename, e);
                    Console.ResetColor();
                    return;
                }
                finally
                {
                    // Закрываем файловые потоки
                    publicKeyStream.Flush();
                    publicKeyStream.Close();
                    privateKeyStream.Flush();
                    privateKeyStream.Close();
                    if(seedStream != null) seedStream.Close();
                }

                cf.Message("");
                cf.Message(string.Format("Генерирование открытого и закрытого ключей завершено (\"{0}\" и \"{1}\")!", publicKeyFilename, privateKeyFilename));

                return;
            }

            // Проверка на запрос выполнения ЭЦП...
            if((args.Length == 2) && (args[0].ToLower() == "s"))
            {
                if(!File.Exists(privateKeyFilename))
                {
                    cf.Message(string.Format("Файл закрытого ключа \"{0}\" не существует!", privateKeyFilename));
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

                // Поток формирования подписи
                Stream signStream = new FileStream(signFilename, FileMode.Create, FileAccess.Write);

                // Вычисляем ЭЦП
                cf.Message(string.Format("Начато вычисление ЭЦП файла \"{0}\"...", args[1]));

                try
                {
                    cf.SignData(privateKeyStream, dataStream, signStream);
                }
                catch(Exception e)
                {
                    cf.Message("Ошибка при вычислении ЭЦП!");
                    ErrLogOut(errLogFilename, e);
                    Console.ResetColor();
                    return;
                }
                finally
                {
                    // Закрываем файловые потоки
                    privateKeyStream.Close();
                    dataStream.Close();
                    signStream.Flush();
                    signStream.Close();
                }

                cf.Message("Вычисление ЭЦП завершено!");
                cf.Message(string.Format("Создан файл \"{0}\"", signFilename));

                return;
            }

            // Проверка на запрос проверки ЭЦП...
            if((args.Length == 3) && (args[0].ToLower() == "c"))
            {
                // Проверяем файл открытого ключа на существование
                if(!File.Exists(args[2]))
                {
                    cf.Message(string.Format("Файл открытого ключа другой стороны \"{0}\" не существует!", args[2]));
                    return;
                }

                // Определяем имя файла данных, ассоциированного с файлом ЭЦП....
                string dataFileName = args[1].Replace(signExt, "");

                if(!File.Exists(dataFileName))
                {
                    cf.Message(string.Format("Файл данных \"{0}\", ассоциируемый с файлом ЭЦП \"{1}\", не существует!", dataFileName, args[1]));
                    return;
                }

                // Открываем потоки для чтения данных, подписи и открытого ключа другой стороны...
                Stream dataStream = new FileStream(dataFileName, FileMode.Open, FileAccess.Read);
                Stream signStream = new FileStream(args[1], FileMode.Open, FileAccess.Read);
                Stream publicKeyFromOtherPartyStream = new FileStream(args[2], FileMode.Open, FileAccess.Read);

                // Осуществляем проверку ЭЦП
                cf.Message(string.Format("Начата проверка ЭЦП файла \"{0}\"...", dataFileName));
                bool result = cf.VerifySign(dataStream, signStream, publicKeyFromOtherPartyStream);

                // Закрываем файловые потоки
                dataStream.Close();
                signStream.Close();
                publicKeyFromOtherPartyStream.Close();

                if(result)
                {
                    cf.Message(string.Format("Файл \"{0}\" соответствует предъявленной ЭЦП \"{1}\" и открытому ключу \"{2}\"...", dataFileName, args[1], args[2]));
                    cf.Message("All OK!");
                }
                else
                {
                    cf.Message(string.Format("ФАЙЛ \"{0}\" НЕ СООТВЕТСТВУЕТ ПРЕДЪЯВЛЕННОЙ ЭЦП \"{1}\" и открытому ключу \"{2}\"!", dataFileName, args[1], args[2]));
                    cf.Message("ERROR!");
                }

                return;
            }

            // Если задано слишком малое количество аргументов - выводим справку...
            if(args.Count() < 3)
            {
                HelpOut(cf);
            }

            // Проверка версии ОС
            if(!CryforceUtilities.OSVersionCheck())
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                cf.Message("");
                cf.Message("Внимание: текущая версия операционной системы не поддерживается - требуется Vista SP1 или выше!");
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
            bool single; // Однослойный шифр?
            bool encryption; // Режим шифрования (не дешифрование)?
            bool ecdh; // Используется EcdhP521?

            switch(args[0].ToLower())
            {
                case "e":
                    {
                        single = false;
                        encryption = true;
                        ecdh = true;
                        cf.Message("Режим шифрования на основе открытого ключа другого абонента");
                        cf.Message("(двойной Rijndael-256 с перестановкой битов между слоями)...");
                        break;
                    }
                case "e1":
                    {
                        single = true;
                        encryption = true;
                        ecdh = false;
                        cf.Message("Режим шифрования (Rijndael-256)...");
                        break;
                    }
                case "e2":
                    {
                        single = false;
                        encryption = true;
                        ecdh = false;
                        cf.Message("Режим шифрования (двойной Rijndael-256 с перестановкой битов между слоями)...");
                        break;
                    }
                case "d":
                    {
                        single = false;
                        encryption = false;
                        ecdh = true;
                        cf.Message("Режим дешифрования на основе открытого ключа другого абонента");
                        cf.Message("(двойной Rijndael-256 с перестановкой битов между слоями)...");
                        break;
                    }
                case "d1":
                    {
                        single = true;
                        encryption = false;
                        ecdh = false;
                        cf.Message("Режим дешифрования (Rijndael-256)...");
                        break;
                    }
                case "d2":
                    {
                        single = false;
                        encryption = false;
                        ecdh = false;
                        cf.Message("Режим дешифрования (двойной Rijndael-256 с перестановкой битов между слоями)...");
                        break;
                    }
                default:
                    {
                        cf.Message("Неизвестный режим обработки!");
                        Console.ResetColor();
                        return;
                    }
            }

            cf.Message("");

            // Проверяем входной файл
            if(!File.Exists(args[1]))
            {
                cf.Message(string.Format("Входной файл \"{0}\" не существует!", args[1]));

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
                        cf.Message("Файл-пароль не может быть меньше 2 байт!");
                        throw new Exception();
                    }
                }
                catch
                {
                    cf.Message(string.Format("Ошибка чтения данных файла-пароля \"{0}\"!", args[3]));
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
                    cf.Message(string.Format("Файл открытого ключа другой стороны \"{0}\" не существует!", args[3]));
                    return;
                }

                if(!File.Exists(privateKeyFilename))
                {
                    cf.Message(string.Format("Файл закрытого ключа \"{0}\" не существует!", privateKeyFilename));
                    return;
                }

                // Читаем открытый ключ другой стороны и наш закрытый ключ...
                Stream publicKeyFromOtherPartyStream = new FileStream(args[3], FileMode.Open, FileAccess.Read);
                Stream privateKeyStream = new FileStream(privateKeyFilename, FileMode.Open, FileAccess.Read);

                //...а затем формируем симметричные ключи
                try
                {
                    cf.GetSymmetricKeys(publicKeyFromOtherPartyStream, privateKeyStream,
                                        out passwordDataForKeyFromFile1, out passwordDataForKeyFromFile2);
                }
                catch(Exception e)
                {
                    cf.Message("Ошибка при вычислении симметричных ключей ECDH!");
                    ErrLogOut(errLogFilename, e);
                    Console.ResetColor();
                    return;
                }
            }

            // Пароль с клавиатуры считываем в любом случае...
            if(single)
            {
                cf.Message("Введите пароль:");
                passwordDataForKeyFromKeyboard = CryforceUtilities.GetPasswordBytesSafely();
                cf.Message("");
            }
            else
            {
                cf.Message("Введите пароль №1:");
                passwordDataForKeyFromKeyboard1 = CryforceUtilities.GetPasswordBytesSafely();
                cf.Message("");

                cf.Message("Введите пароль №2:");
                passwordDataForKeyFromKeyboard2 = CryforceUtilities.GetPasswordBytesSafely();
                cf.Message("");
            }

            // Удаляем выходной файл, если такой имеется (невосстановимое удаление)
            if(File.Exists(args[2]))
            {
                cf.Message("Предварительное затирание данных выходного файла, который будет перезаписан...");
                CryforceUtilities.WipeFile(cf, args[2], cf.BufferSizePerStream, true);
                File.Delete(args[2]);
            }

            // Формируем входные и выходные потоки...
            Stream inputStream = new FileStream(args[1], FileMode.Open, FileAccess.Read);
            Stream outputStream = new FileStream(args[2], FileMode.Create, FileAccess.Write);

            int iterations; // Количество итераций хеширования пароля
            if((args.Length < 5) || (!int.TryParse(args[4], out iterations)))
            {
                iterations = 666 * 999; // Итерации хеширования нужны для того, чтобы усложнить "brute-force attack"
            }

            // Подготовка ключей для шифрования...
            cf.Message(string.Format("Подготовка ключей для шифрования, {0} итер.", iterations.ToString()));
            try
            {
                if(single)
                {
                    passwordDataForKey = CryforceUtilities.MergeArrays(passwordDataForKeyFromFile, passwordDataForKeyFromKeyboard);
                    cf.SingleRijndael(inputStream, passwordDataForKey, outputStream, encryption, iterations);
                }
                else
                {
                    passwordDataForKey1 = CryforceUtilities.MergeArrays(passwordDataForKeyFromFile1, passwordDataForKeyFromKeyboard1);
                    passwordDataForKey2 = CryforceUtilities.MergeArrays(passwordDataForKeyFromFile2, passwordDataForKeyFromKeyboard2);
                    cf.DoubleRijndael(inputStream, passwordDataForKey1, passwordDataForKey2, outputStream, encryption, iterations);
                }
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                cf.Message("");
                cf.Message("Ошибка в ходе криптографического преобразования (неверный ключ?).");
                Console.ResetColor();
                return;
            }

            // Сброс буферов...
            cf.Message("Сброс буферов...");
            inputStream.Close();
            outputStream.Flush();
            outputStream.Close();

            cf.Message("Очистка ключей...");

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

            cf.Message("");
            cf.Message("Завершено!");

            // Сбрасываем цветовой режим
            Console.ResetColor();
        }

        #endregion Public
    }
}