#region

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using EventArgsUtilities;

#endregion

namespace CRYFORCE.Engine
{
    /// <summary>
    /// Основной класс ядра шифрования
    /// </summary>
    public class Cryforce
    {
        #region Static

        #endregion Static

        #region Constants

        /// <summary>
        /// Размер буфера в ОЗУ под каждый поток.
        /// </summary>
        private const int DEFAULT_BUFFER_SIZE_PER_STREAM = 16 * 1024 * 1024; // 16 мегабайт

        #endregion Constants

        #region Data

        #endregion Data

        #region Events

        /// <summary>
        /// Событие обновления прогресса обработки.
        /// </summary>
        public event EventHandler<EventArgsGeneric<ProgressChangedArg>> ProgressChanged;

        /// <summary>
        /// Событие обработки полученного в ходе работы сообщения.
        /// </summary>
        public event EventHandler<EventArgsGeneric<MessageReceivedArg>> MessageReceived;

        #endregion Events

        #region .ctor

        /// <summary>
        /// Конструктор с параметрами
        /// </summary>
        public Cryforce()
            : this(false, true, DEFAULT_BUFFER_SIZE_PER_STREAM)
        {
        }

        /// <summary>
        /// Конструктор с параметрами
        /// </summary>
        /// <param name="workInMemory"> Работать в ОЗУ? </param>
        /// <param name="workInTempDir"> Работать в директории для временных файлов? </param>
        public Cryforce(bool workInMemory, bool workInTempDir)
            : this(workInMemory, workInTempDir, DEFAULT_BUFFER_SIZE_PER_STREAM)
        {
        }

        /// <summary>
        /// Конструктор с параметрами
        /// </summary>
        /// <param name="workInMemory"> Работать в ОЗУ? </param>
        /// <param name="workInTempDir"> Работать в директории для временных файлов? </param>
        /// <param name="bufferSizePerStream"> Размер буфера на файловый поток. </param>
        /// <param name="rndSeed"> Инициализирующее значение генератора случайных чисел. </param>
        public Cryforce(bool workInMemory, bool workInTempDir, int bufferSizePerStream, int rndSeed)
            : this(workInMemory, workInTempDir, bufferSizePerStream)
        {
            RndSeed = rndSeed;
        }

        /// <summary>
        /// Конструктор с параметрами
        /// </summary>
        /// <param name="workInMemory"> Работать в ОЗУ? </param>
        /// <param name="workInTempDir"> Работать в директории для временных файлов? </param>
        /// <param name="bufferSizePerStream"> Размер буфера на файловый поток. </param>
        public Cryforce(bool workInMemory, bool workInTempDir, int bufferSizePerStream)
        {
            WorkInMemory = workInMemory;
            WorkInTempDir = workInTempDir;
            BufferSizePerStream = bufferSizePerStream;
            RndSeed = DateTime.Now.Ticks.GetHashCode();
        }

        #endregion .ctor

        #region Properties

        /// <summary>
        /// Работаем в ОЗУ?
        /// </summary>
        public bool WorkInMemory { get; set; }

        /// <summary>
        /// Работаем в директории для временных файлов?
        /// </summary>
        public bool WorkInTempDir { get; set; }

        /// <summary>
        /// Размер буфера в ОЗУ под каждый поток.
        /// </summary>
        public int BufferSizePerStream { get; set; }

        /// <summary>
        /// Инициализирующее значение генератора случайных чисел.
        /// </summary>
        public int RndSeed { get; set; }

        /// <summary>
        /// Затирать выходной поток нулями?
        /// </summary>
        public bool ZeroOut { get; set; }

        #endregion Properties

        #region Private

        #endregion Private

        #region Protected

        #endregion Protected

        #region Public

        /// <summary>
        /// Обвязка вызова события "Изменение прогресса процесса"
        /// </summary>
        /// <param name="processDescription"> Описание процесса. </param>
        /// <param name="processProgress"> Прогресс процесса. </param>
        /// <param name="rFlag"> Переводить каретку? </param>
        public void Progress(string processDescription, double processProgress, bool rFlag = false)
        {
            if(ProgressChanged != null)
            {
                string messagePostfix = rFlag ? "\r" : "";
                ProgressChanged(null, new EventArgsGeneric<ProgressChangedArg>(new ProgressChangedArg(processDescription, processProgress, messagePostfix)));
            }
        }

        /// <summary>
        /// Обвязка вызова обработчика события "Получено сообщение"
        /// </summary>
        /// <param name="messageBody"> Тело сообщения. </param>
        /// <param name="messagePostfix"> Постфикс сообщения. </param>
        public void Message(string messageBody, string messagePostfix = "")
        {
            if(MessageReceived != null)
            {
                MessageReceived(null, new EventArgsGeneric<MessageReceivedArg>(new MessageReceivedArg(messageBody, messagePostfix)));
            }
        }

        /// <summary>
        /// Шифрование по алгоритму Rijndael-256
        /// </summary>
        /// <param name="inputStream"> Входной поток. </param>
        /// <param name="key"> Ключ для первого прохода шифрования. </param>
        /// <param name="outputStream"> Выходной поток. </param>
        /// <param name="encryptionMode"> Используется шифрование? </param>
        /// <param name="iterations"> Количество итераций хеширования пароля. </param>
        public void SingleRijndael(Stream inputStream, byte[] key, Stream outputStream, bool encryptionMode, int iterations = 1)
        {
            if(!inputStream.CanSeek)
            {
                throw new Exception("Cryforce::SingleRijndael() ==> Input stream can't seek!");
            }

            var streamCryptoWrapper = new StreamCryptoWrapper();
            streamCryptoWrapper.Initialize(key, iterations);

            Stream inputStreamAtLevel0 = encryptionMode ? inputStream : streamCryptoWrapper.WrapStream(inputStream, false);
            Stream outputStreamAtLevel0 = encryptionMode ? streamCryptoWrapper.WrapStream(outputStream, true) : outputStream;

            inputStreamAtLevel0.SafeSeekBegin();
            outputStreamAtLevel0.SafeSeekBegin();

            // Процесс шифрования/расшифровки происходит прозрачно, во время чтения из зашифрованного потока или записи в зашифрованный
            // Размер буфера при копировании выбираем таким, чтобы обеспечить вывод каждого процента
            long dataSize = inputStream.Length;
            var bufferSize = (int)(dataSize / 100);
            CryforceUtilities.StreamCopy(this, inputStreamAtLevel0, outputStreamAtLevel0, dataSize, bufferSize);

            if(outputStreamAtLevel0 is CryptoStream)
            {
                ((CryptoStream)outputStreamAtLevel0).FlushFinalBlock();
            }
            outputStreamAtLevel0.Flush();

            // Если выходной поток первого уровня является криптографической оберткой над другим потоком -
            // нужно получить базовый поток для дальнейшей работы
            if(outputStreamAtLevel0 is CryptoStream)
            {
                outputStreamAtLevel0 = outputStream;
            }

            inputStreamAtLevel0.SafeSeekBegin();
            outputStreamAtLevel0.SafeSeekBegin();

            //...и как завершение шифрования - деинициализируем криптовраппер...
            streamCryptoWrapper.Clear();
            //...не забывая про вывод прогресса.
            Progress("Rijndael-256", 100);
        }

        /// <summary>
        /// Двойное шифрование по алгоритму Rijndael-256
        /// </summary>
        /// <param name="inputStream"> Входной поток. </param>
        /// <param name="key1"> Ключ для первого прохода шифрования. </param>
        /// <param name="key2"> Ключ для второго прохода шифрования. </param>
        /// <param name="outputStream"> Выходной поток. </param>
        /// <param name="encryptionMode"> Используется шифрование? </param>
        /// <param name="iterations"> Количество итераций хеширования пароля. </param>
        public void DoubleRijndael(Stream inputStream, byte[] key1, byte[] key2, Stream outputStream, bool encryptionMode, int iterations = 1)
        {
            if(!inputStream.CanSeek)
            {
                throw new Exception("Cryforce::DoubleRijndael() ==> Input stream can't seek!");
            }

            var streamCryptoWrappers = new[] {new StreamCryptoWrapper(), new StreamCryptoWrapper()};
            streamCryptoWrappers[0].Initialize(encryptionMode ? key1 : key2, iterations); // При шифровании прямой порядок паролей...
            streamCryptoWrappers[1].Initialize(encryptionMode ? key2 : key1, iterations); // ...а при расшифровке - обратный

            // Генерируем 10 случайных имен файлов: два для целей временного хранения данных в пределах данного метода
            // и 8 штук для битсплиттера (генерируем их совместно, чтобы избежать конфликтов)
            string[] tempFilenamesAll = WorkInTempDir ? CryforceUtilities.GetTempFilenames(10) : CryforceUtilities.GetRandomFilenames(10, 8, RndSeed).Select(item => item + ".jpg").ToArray();
            var tempFilenames = new string[2];
            var tempFilenamesToBitSplitter = new string[8];
            Array.Copy(tempFilenamesAll, 0, tempFilenames, 0, 2);
            Array.Copy(tempFilenamesAll, 2, tempFilenamesToBitSplitter, 0, 8);

            Stream[] randomFilenameStreams = tempFilenames.Select(item => CryforceUtilities.PrepareOutputStream(this, item, BufferSizePerStream, ZeroOut, WorkInMemory, RndSeed)).ToArray();

            //////////////////////////////////////
            // Шифрование первого уровня (Level0)
            //////////////////////////////////////
            Stream inputStreamAtLevel0 = encryptionMode ? inputStream : streamCryptoWrappers[0].WrapStream(inputStream, false);
            Stream outputStreamAtLevel0 = encryptionMode ? streamCryptoWrappers[0].WrapStream(randomFilenameStreams[0], true) : randomFilenameStreams[0];

            inputStreamAtLevel0.SafeSeekBegin();
            outputStreamAtLevel0.SafeSeekBegin();

            // Процесс шифрования/расшифровки происходит прозрачно, во время чтения из зашифрованного потока или записи в зашифрованный
            // Размер буфера при копировании выбираем таким, чтобы обеспечить вывод каждого процента
            long dataSize = inputStream.Length;
            var bufferSize = (int)(dataSize / 100);
            CryforceUtilities.StreamCopy(this, inputStreamAtLevel0, outputStreamAtLevel0, dataSize, bufferSize);

            if(outputStreamAtLevel0 is CryptoStream)
            {
                ((CryptoStream)outputStreamAtLevel0).FlushFinalBlock();
            }
            outputStreamAtLevel0.Flush();

            // Если выходной поток первого уровня является криптографической оберткой над другим потоком -
            // нужно получить базовый поток для дальнейшей работы
            if(outputStreamAtLevel0 is CryptoStream)
            {
                outputStreamAtLevel0 = randomFilenameStreams[0];
            }

            inputStreamAtLevel0.SafeSeekBegin();
            outputStreamAtLevel0.SafeSeekBegin();

            // Вывод прогресса...
            Progress("Rijndael-256 (1/2)", 100);

            ////////////////////////////////////////////////////////
            // Перестановка битов посредством битсплиттера (Level1)
            ////////////////////////////////////////////////////////

            // Выходной поток первого уровня обработки является входным потоком для второго
            Stream inputStreamAtLevel1 = outputStreamAtLevel0;

            // Т.к. результат работы битсплиттера не является конечным - работаем с временным потоком
            Stream outputStreamAtLevel1 = randomFilenameStreams[1];

            inputStreamAtLevel1.SafeSeekBegin();
            outputStreamAtLevel1.SafeSeekBegin();

            var bitSplitter = new BitSplitter(tempFilenamesToBitSplitter, key1, key2, WorkInMemory) {RndSeed = RndSeed, Cf = this};
            if(encryptionMode)
            {
                bitSplitter.SplitToBitstream(inputStreamAtLevel1, outputStreamAtLevel1);
            }
            else
            {
                bitSplitter.UnsplitFromBitstream(inputStreamAtLevel1, outputStreamAtLevel1);
            }

            bitSplitter.ClearAndClose();

            inputStreamAtLevel1.SafeSeekBegin();
            outputStreamAtLevel1.SafeSeekBegin();

            //////////////////////////////////////
            // Шифрование второго уровня (Level2)
            //////////////////////////////////////
            Stream inputStreamAtLevel2 = encryptionMode ? outputStreamAtLevel1 : streamCryptoWrappers[1].WrapStream(outputStreamAtLevel1, false);
            Stream outputStreamAtLevel2 = encryptionMode ? streamCryptoWrappers[1].WrapStream(outputStream, true) : outputStream;

            inputStreamAtLevel2.SafeSeekBegin();
            outputStreamAtLevel2.SafeSeekBegin();

            // Процесс шифрования/расшифровки происходит прозрачно, во время чтения из зашифрованного потока или записи в зашифрованный
            // Размер буфера при копировании выбираем таким, чтобы обеспечить вывод каждого процента
            dataSize = outputStreamAtLevel1.Length;
            bufferSize = (int)(dataSize / 100);
            CryforceUtilities.StreamCopy(this, inputStreamAtLevel2, outputStreamAtLevel2, dataSize, bufferSize);

            if(outputStreamAtLevel2 is CryptoStream)
            {
                ((CryptoStream)outputStreamAtLevel2).FlushFinalBlock();
            }
            outputStreamAtLevel2.Flush();

            inputStreamAtLevel2.SafeSeekBegin();
            outputStreamAtLevel2.SafeSeekBegin();

            // Вывод прогресса...
            Progress("Rijndael-256 (2/2)", 100);

            // Уничтожаем данные временных потоков
            foreach(Stream randomFilenameStream in randomFilenameStreams)
            {
                CryforceUtilities.WipeStream(this, randomFilenameStream, BufferSizePerStream, 0, randomFilenameStream.Length, ZeroOut, RndSeed);
                randomFilenameStream.Flush();
                randomFilenameStream.Close();
            }

            // Производим удаление носителей
            foreach(string tempFilename in tempFilenames)
            {
                if(File.Exists(tempFilename))
                {
                    File.SetAttributes(tempFilename, FileAttributes.Normal);
                    File.Delete(tempFilename);
                }
            }

            // Закрываем все потоки, которые не являлись синонимами входа или выхода
            inputStreamAtLevel1.Close();
            inputStreamAtLevel2.Close();

            outputStreamAtLevel0.Close();
            outputStreamAtLevel1.Close();
        }

        /// <summary>
        /// Генерация пары открытый/закрытый ключ
        /// </summary>
        /// <param name="publicKeyStream"> Поток для записи открытого ключа. </param>
        /// <param name="privateKeyStream"> Поток для записи закрытого ключа. </param>
        /// <param name="seedStream"> Поток, содержащий случайные данные. </param>
        public void CreateEccKeys(Stream publicKeyStream, Stream privateKeyStream, Stream seedStream = null)
        {
            byte[] seedDataFromStream = null;

            if(!publicKeyStream.CanWrite)
            {
                throw new Exception("Cryforce::CreateEccKeys() ==> Public key stream can't write!");
            }

            if(!privateKeyStream.CanWrite)
            {
                throw new Exception("Cryforce::CreateEccKeys() ==> Private key stream can't write!");
            }

            // Если можем использовать seed-поток...
            if((seedStream != null) && (seedStream.CanSeek))
            {
                seedStream.SafeSeekBegin();
                seedDataFromStream = new byte[seedStream.Length];
                seedStream.Read(seedDataFromStream, 0, seedDataFromStream.Length);
            }

            // ECDH
            Message("Генерирование пары открытый/закрытый ключ (ECDH, шифрование).");
            Message("Нажимайте случайные клавиши на клавиатуре (включая модификаторы");
            Message("\"Alt\", \"Shift\", \"Control\"). Для завершения нажмите Enter...");
            Message("");

            byte[] seedDataFromKeyboard_ECDH = CryforceUtilities.GetPasswordBytesSafely();

            Message("");

            byte[] seedData_ECDH = CryforceUtilities.MergeArrays(seedDataFromStream, seedDataFromKeyboard_ECDH);
            var ecdhP521_ECDH = new EcdhP521(seedData_ECDH, null); // Приватного ключа нет - он будет сгенерирован!

            // ECDSA
            Message("Генерирование пары открытый/закрытый ключ (ECDSA, ЭЦП).");
            Message("Нажимайте случайные клавиши на клавиатуре (включая модификаторы");
            Message("\"Alt\", \"Shift\", \"Control\"). Для завершения нажмите Enter...");
            Message("");
            byte[] seedDataFromKeyboard_ECDSA = CryforceUtilities.GetPasswordBytesSafely();
            byte[] seedData_ECDSA = CryforceUtilities.MergeArrays(seedDataFromStream, seedDataFromKeyboard_ECDSA);
            var ecdhP521_ECDSA = new EcdhP521(seedData_ECDSA, null); // Приватного ключа нет - он будет сгенерирован!

            // Чистим массивы...
            CryforceUtilities.ClearArray(seedDataFromStream);
            CryforceUtilities.ClearArray(seedDataFromKeyboard_ECDH);
            CryforceUtilities.ClearArray(seedDataFromKeyboard_ECDSA);
            CryforceUtilities.ClearArray(seedData_ECDH);
            CryforceUtilities.ClearArray(seedData_ECDSA);

            // Пишем открытые ключи...
            var swPublicKey = new StreamWriter(publicKeyStream, Encoding.ASCII);
            swPublicKey.Write("[ ;|;; ");
            swPublicKey.Write(ecdhP521_ECDH.PublicKeyECDH);
            swPublicKey.Write(" ;|;; ");
            swPublicKey.Write(ecdhP521_ECDSA.PublicKeyECDSA);
            swPublicKey.Write(" ]");
            swPublicKey.Flush();

            // Пишем закрытые ключи...
            var swPrivateKey = new StreamWriter(privateKeyStream, Encoding.ASCII);
            swPrivateKey.Write("[ ( ; ) ");
            swPrivateKey.Write(ecdhP521_ECDH.PrivateKeyECDH);
            swPrivateKey.Write(" ( ; ) ");
            swPrivateKey.Write(ecdhP521_ECDSA.PrivateKeyECDSA);
            swPrivateKey.Write(" ]");
            swPrivateKey.Flush();

            // Уничтожаем секретные данные...
            ecdhP521_ECDH.Clear();
            ecdhP521_ECDSA.Clear();
        }

        /// <summary>
        /// Генерирование симметричных ключей
        /// </summary>
        /// <param name="publicKeyFromOtherPartyStream"> Поток открытого ключа другой стороны. </param>
        /// <param name="privateKeyStream"> Поток своего закрытого ключа. </param>
        /// <param name="symmetricKey1"> Симметричный ключ №1 (256 bit). </param>
        /// <param name="symmetricKey2"> Симметричный ключ №2 (256 bit). </param>
        public void GetSymmetricKeys(Stream publicKeyFromOtherPartyStream, Stream privateKeyStream,
                                     out byte[] symmetricKey1, out byte[] symmetricKey2)
        {
            if(!publicKeyFromOtherPartyStream.CanSeek)
            {
                throw new Exception("Cryforce::GetSymmetricKeys() ==> Public key from other party stream can't seek!");
            }

            if(!privateKeyStream.CanSeek)
            {
                throw new Exception("Cryforce::GetSymmetricKeys() ==> Private key stream can't seek!");
            }

            // Ищем начало потоков...
            publicKeyFromOtherPartyStream.SafeSeekBegin();
            privateKeyStream.SafeSeekBegin();

            // Создаем сущность для работы с эллиптическими кривыми
            var ecdhP521 = new EcdhP521(null, new StreamReader(privateKeyStream, Encoding.UTF8).ReadToEnd()) {PublicKeyFromOtherParty = new StreamReader(publicKeyFromOtherPartyStream, Encoding.UTF8).ReadToEnd()};

            // Если не получилось сгенерировать симметричные ключи...
            if(!ecdhP521.CreateSymmetricKey())
            {
                //...уничтожаем секретные данные...
                ecdhP521.Clear();

                throw new Exception("EcdhP521::CreateSymmetricKey() failed!");
            }

            // Получаем симметричные ключи
            symmetricKey1 = (byte[])ecdhP521.Keys256[0].Clone();
            symmetricKey2 = (byte[])ecdhP521.Keys256[1].Clone();

            // Уничтожаем секретные данные...
            ecdhP521.Clear();
        }

        /// <summary>
        /// Вычисление ЭЦП потока данных
        /// </summary>
        /// <param name="privateKeyStream"> Приватный ключ. </param>
        /// <param name="dataStream"> Поток данных. </param>
        /// <param name="signStream"> ЭЦП для потока данных. </param>
        public void SignData(Stream privateKeyStream, Stream dataStream, Stream signStream)
        {
            if(!privateKeyStream.CanSeek)
            {
                throw new Exception("Cryforce::SignData() ==> Private key stream can't seek!");
            }

            // Ищем начало потоков...
            privateKeyStream.SafeSeekBegin();
            dataStream.SafeSeekBegin();

            // Создаем сущность для работы с эллиптическими кривыми
            var ecdhP521 = new EcdhP521(null, new StreamReader(privateKeyStream, Encoding.UTF8).ReadToEnd());

            // Пишем ЭЦП...
            var sw = new StreamWriter(signStream, Encoding.ASCII);
            sw.Write(ecdhP521.SignData(dataStream));
            sw.Flush();

            // Уничтожаем секретные данные...
            ecdhP521.Clear();
        }

        /// <summary>
        /// Проверка ЭЦП на базе открытого ключа другой стороны
        /// </summary>
        /// <param name="dataStream"> Поток данных. </param>
        /// <param name="signStream"> ЭЦП. </param>
        /// <param name="publicKeyFromOtherPartyStream"> Поток открытого ключа другой стороны. </param>
        /// <returns> Булевский флаг операции. </returns>
        public bool VerifySign(Stream dataStream, Stream signStream, Stream publicKeyFromOtherPartyStream)
        {
            if(!publicKeyFromOtherPartyStream.CanSeek)
            {
                throw new Exception("Cryforce::VerifySign() ==> Public key from other party stream can't seek!");
            }

            // Ищем начало потоков...
            dataStream.SafeSeekBegin();
            signStream.SafeSeekBegin();
            publicKeyFromOtherPartyStream.SafeSeekBegin();

            // Создаем сущность для работы с эллиптическими кривыми
            var ecdhP521 = new EcdhP521(null, null);

            // Возвращаем результат проверки ЭЦП
            bool result;

            try
            {
                // Осуществляем проверку ЭЦП...
                result = ecdhP521.VerifyData(dataStream,
                                             new StreamReader(signStream, Encoding.UTF8).ReadToEnd(),
                                             new StreamReader(publicKeyFromOtherPartyStream, Encoding.UTF8).ReadToEnd());
            }
            catch
            {
                result = false;
            }

            // Уничтожаем секретные данные...
            ecdhP521.Clear();

            // Возвращаем результат проверки ЭЦП
            return result;
        }

        #endregion Public
    }
}