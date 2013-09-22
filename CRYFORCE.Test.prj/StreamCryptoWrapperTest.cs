using System;
using System.IO;
using System.Linq;

using NUnit.Framework;

namespace CRYFORCE.Engine.Test
{
    [TestFixture]
    public class StreamCryptoWrapperTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            SetUpIsOK = true; // Указываем, что была произведена установка
        }

        #endregion

        /// <summary>Была произведена установка?</summary>
        public bool SetUpIsOK { get; set; }

        /// <summary>
        /// Тест корректности работы класса шифрования по алгоритму Rijndael
        /// </summary>
        [Test]
        public void StreamCryptoWrapperBaseTest()
        {
            if(!SetUpIsOK)
            {
                SetUp();
            }

            try
            {
                // Инициализируем генератор случайных чисел
                var rnd = new Random(DateTime.Now.Ticks.GetHashCode());

                var password = new byte[255];
                var inputData = new byte[(1024 * 1024)]; // Размер тестовых данных - 1 Мб
                var inputData2 = new byte[(1024 * 1024)]; // Размер тестовых данных - 1 Мб
                var outputData = new byte[(1024 * 1024) + 32]; // В выходном потоке даем запас на выравнивание при шифровании

                // Генерируем случайный пароль...
                rnd.NextBytes(password);

                //...и случайные входные данные...
                rnd.NextBytes(inputData);

                //...затем выбираем случайное количество итераций при хешировании пароля
                int iterations = rnd.Next(1, 100);

                // Шифрование
                var streamCryptoWrapper = new StreamCryptoWrapper();
                streamCryptoWrapper.Initialize(password, iterations);
                var inputStream = new MemoryStream(inputData);
                Stream outputStream = streamCryptoWrapper.WrapStream(new MemoryStream(outputData), true); // Шифрование
                inputStream.CopyTo(outputStream);
                inputStream.Close();
                outputStream.Flush();
                outputStream.Close();

                // Расшифровка
                streamCryptoWrapper = new StreamCryptoWrapper();
                streamCryptoWrapper.Initialize(password, iterations);
                Stream inputStream2 = streamCryptoWrapper.WrapStream(new MemoryStream(outputData), false); // Расшифровка
                var outputStream2 = new MemoryStream(inputData2);
                inputStream2.CopyTo(outputStream2);
                inputStream2.Close();
                outputStream2.Flush();
                outputStream2.Close();

                // Проверка содержимого исходного массива и массива после расшифровки
                if(!inputData.SequenceEqual(inputData2))
                {
                    throw new InvalidDataException("StreamCryptoWrapperTest: Wrong decrypted data!");
                }
            } // try
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                Assert.Fail();
            }
        }
    }
}