#region

using System;
using System.IO;
using System.Security.Cryptography;

#endregion

namespace CRYFORCE.Engine
{
    /// <summary>
    /// Класс, помещающий поток в обертку, работающую с шифрованием по алгоритму Rijndael-256
    /// </summary>
    public sealed class StreamCryptoWrapper : IDisposable
    {
        #region Data

        /// <summary>
        /// Вектор инициализации алгоритма шифрования.
        /// </summary>
        private byte[] _IV;

        /// <summary>
        /// Набор базовых операций криптографического преобразования для расшифровки.
        /// </summary>
        private ICryptoTransform _decryptor;

        /// <summary>
        /// Набор базовых операций криптографического преобразования для шифрования.
        /// </summary>
        private ICryptoTransform _encryptor;

        /// <summary>
        /// Экземпляр класса "SHA256".
        /// </summary>
        private SHA256Cng _hash256;

        /// <summary>
        /// Экземпляр класса "SHA512".
        /// </summary>
        private SHA512Cng _hash512;

        /// <summary>
        /// Ключ.
        /// </summary>
        private byte[] _key;

        /// <summary>
        /// Алгоритм шифрования Rijndael.
        /// </summary>
        /// <remarks>
        /// Алгоритм шифрования Rijndael является прототипом AES, но имеет размер блока 256 бит
        /// (а не 128, как у AES, т.к. для прототипа не задавалось соответствие аппаратным криптопроцессорам,
        /// ориентированным на ограниченную разрядность).
        /// </remarks>
        private RijndaelManaged _rijndael;

        #endregion Data

        #region .ctor

        /// <summary>
        /// Конструктор по-умолчанию
        /// </summary>
        public StreamCryptoWrapper()
        {
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="password"> Пароль в форме строки. </param>
        /// <param name="iterations"> Количество итераций при хешировании пароля. </param>
        public StreamCryptoWrapper(byte[] password, int iterations = 1)
        {
            Initialize(password, iterations);
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

        /// <summary>
        /// Экземпляр класса инициализирован?
        /// </summary>
        public bool IsInitialized { get; private set; }

        #endregion Properties

        #region Private

        /// <summary>
        /// Получение хеша от строки
        /// </summary>
        /// <param name="data"> Данные для хеширования. </param>
        /// <param name="iterations"> Количество итераций. </param>
        /// <param name="key"> Ключ шифрования. </param>
        /// <param name="IV"> Инициализирующий вектор шифрования. </param>
        private void Hash(byte[] data, int iterations, out byte[] key, out byte[] IV)
        {
            // Получаем инициирующий хеш входных данных...
            byte[] hash512Buff = _hash512.ComputeHash(data);
            byte[] hash256Buff = _hash256.ComputeHash(data);

            // Стартуя с единицы мы указываем на то, что хеш уже был вычислен один раз,
            // дорабатываем оставшиеся итерации...
            for(int i = 1; i < iterations; i++)
            {
                hash512Buff = _hash512.ComputeHash(hash512Buff);
                hash256Buff = _hash256.ComputeHash(hash256Buff);
            }

            // Ключ и вектор инициализации будут получены при помощи одной
            // и той же хеш-функции, но с различных хеш-массивов
            key = _hash256.ComputeHash(hash512Buff);
            IV = _hash256.ComputeHash(hash256Buff);
        }

        #endregion Private

        #region Public

        /// <summary>
        /// Инициализация экземпляра класса
        /// </summary>
        /// <param name="password"> Пароль в форме строки. </param>
        /// <param name="iterations"> Количество итераций при хешировании пароля. </param>
        public void Initialize(byte[] password, int iterations = 1)
        {
            // Очистка конфиденциальных данных
            Clear();

            // Хешируем результирующий пароль...
            Hash(password, iterations, out _key, out _IV);

            // Устанавливаем параметры алгоритма шифрования...
            _rijndael.Mode = CipherMode.CBC;
            _rijndael.BlockSize = _rijndael.KeySize = (_key.Length << 3);
            _rijndael.Key = _key;
            _rijndael.IV = _IV;
            _encryptor = _rijndael.CreateEncryptor();
            _decryptor = _rijndael.CreateDecryptor();

            // Указываем, что инициализация прошла успешно
            IsInitialized = true;
        }

        /// <summary>
        /// Очистка конфиденциальных данных
        /// </summary>
        public void Clear()
        {
            // Указываем на деинициализацию
            IsInitialized = false;

            // Чистим массивы...
            CryforceUtilities.ClearArray(_key);
            CryforceUtilities.ClearArray(_IV);

            // Чистим криптографические сущности...
            if(_hash256 != null) _hash256.Clear();
            if(_hash512 != null) _hash512.Clear();
            if(_rijndael != null) _rijndael.Clear();

            // Инициализируем криптографические сущности...
            _hash256 = new SHA256Cng();
            _hash512 = new SHA512Cng();
            _rijndael = new RijndaelManaged();
        }

        /// <summary>
        /// Получение потока-обертки, работающего с шифрованием
        /// </summary>
        /// <remarks>
        ///   При шифровании нужно оборачивать выходной поток, а при расшифровке - входной.
        /// </remarks>
        /// <param name="stream"> Входной поток. </param>
        /// <param name="encryptionMode"> Используется режим шифрования? </param>
        /// <returns> Поток-обертка, работающий с шифрованием. </returns>
        public Stream WrapStream(Stream stream, bool encryptionMode)
        {
            if(!IsInitialized)
            {
                throw new Exception("StreamCryptoWrapper::WrapStream() ==> StreamCryptoWrapper is not initialized!");
            }

            return encryptionMode
                       ? new CryptoStream(stream, _encryptor, CryptoStreamMode.Write)
                       : new CryptoStream(stream, _decryptor, CryptoStreamMode.Read);
        }

        #endregion Public
    }
}