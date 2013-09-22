using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CRYFORCE.Engine
{
    /// <summary>
    /// Класс-обвязка для алгоритмов RSA
    /// </summary>
    public static class RSA
    {
        /// <summary>Открывающий маркер RSA-ключа: размер.</summary>
        private static string _bitStrengthOpeningMarker = "<BitStrength>";

        /// <summary>Закрывающий маркер RSA-ключа: размер.</summary>
        private static string _bitStrengthClosingMarker = "</BitStrength>";

        /// <summary>
        /// Генерирование пары открытый / закрытый ключ
        /// </summary>
        /// <param name="bitStrength">Стойкость ключа.</param>
        /// <param name="publicKey">Открытый ключ.</param>
        /// <param name="privateKey">Закрытый ключ.</param>
        public static void GenerateRsaKeyPair(int bitStrength, out string publicKey, out string privateKey)
        {
            var RSAProvider = new RSACryptoServiceProvider(bitStrength);
            publicKey = _bitStrengthOpeningMarker + bitStrength.ToString() + _bitStrengthClosingMarker + RSAProvider.ToXmlString(false);
            privateKey = _bitStrengthOpeningMarker + bitStrength.ToString() + _bitStrengthClosingMarker + RSAProvider.ToXmlString(true);
        }

        /// <summary>
        /// Шифрование строки алгоритмом RSA
        /// </summary>
        /// <param name="inputString">Входная строка.</param>
        /// <param name="publicKey">Открытый ключ.</param>
        /// <returns>Зашифрованная строка в XML.</returns>
        public static string EncryptString(string inputString, string publicKey)
        {
            // Определяем размер ключа RSA...
            string bitStrengthString = publicKey.Substring(0, publicKey.IndexOf(_bitStrengthClosingMarker) + _bitStrengthClosingMarker.Length);
            int keySize = Convert.ToInt32(bitStrengthString.Replace(_bitStrengthOpeningMarker, string.Empty).Replace(_bitStrengthClosingMarker, string.Empty));
            //...и убираем блок данных с размером ключа
            publicKey = publicKey.Replace(bitStrengthString, string.Empty);

            // Алгоритм RSA
            var rsaCryptoServiceProvider = new RSACryptoServiceProvider(keySize);
            rsaCryptoServiceProvider.FromXmlString(publicKey);
            byte[] bytesInput = Encoding.UTF8.GetBytes(inputString);

            // Сжатие GZip...
            var input = new MemoryStream(bytesInput);
            var output = (MemoryStream)CryforceUtilities.Compress(input);
            output.Seek(0, SeekOrigin.Begin);
            byte[] bytes = output.ToArray();
            input.Close();
            output.Close();

            //...и выравнивание по границе 4 байта (важно для последующего деления на блоки!)
            var ms = new MemoryStream(bytes);
            var output2 = (MemoryStream)CryforceUtilities.AlignStream(ms, 4);
            output2.Seek(0, SeekOrigin.Begin);
            byte[] alignedBytes = output2.ToArray();
            ms.Close();
            output2.Close();

            // Хеш-функция, используемая сущностью ".NET RSACryptoServiceProvider" - это SHA1
            // int maxLength = ( keySize ) - 2 - ( 2 * SHA1.Create().ComputeHash( rawBytes ).Length );
            keySize = keySize >> 3; // В байте 8 бит, вычисляем количество байт, осуществляя деление на 8
            int maxLength = keySize - 42;
            int dataLength = alignedBytes.Length;
            int iterations = dataLength / maxLength;
            var stringBuilder = new StringBuilder();
            for(int i = 0; i <= iterations; i++)
            {
                var tempBytes = new byte[(dataLength - maxLength * i > maxLength) ? maxLength : dataLength - maxLength * i];
                Buffer.BlockCopy(alignedBytes, maxLength * i, tempBytes, 0, tempBytes.Length);
                byte[] encryptedBytes = rsaCryptoServiceProvider.Encrypt(tempBytes, true);
                stringBuilder.Append(Convert.ToBase64String(encryptedBytes));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Расшифровка строки, зашифрованной посредством RSA
        /// </summary>
        /// <param name="inputString">Входная строка.</param>
        /// <param name="privateKey">Закрытый ключ.</param>
        /// <returns>Расшифрованная строка.</returns>
        public static string DecryptString(string inputString, string privateKey)
        {
            // Определяем размер ключа RSA...
            string bitStrengthString = privateKey.Substring(0, privateKey.IndexOf(_bitStrengthClosingMarker) + _bitStrengthClosingMarker.Length);
            int keySize = Convert.ToInt32(bitStrengthString.Replace(_bitStrengthOpeningMarker, string.Empty).Replace(_bitStrengthClosingMarker, string.Empty));
            //...и убираем блок данных с размером ключа
            privateKey = privateKey.Replace(bitStrengthString, string.Empty);

            // Алгоритм RSA
            var rsaCryptoServiceProvider = new RSACryptoServiceProvider(keySize);
            rsaCryptoServiceProvider.FromXmlString(privateKey);
            keySize = keySize >> 3;
            int base64BlockSize = CryforceUtilities.GetBase64BlockSize(keySize);
            int iterations = inputString.Length / base64BlockSize;
            var arrayList = new ArrayList();
            for(int i = 0; i < iterations; i++)
            {
                byte[] encryptedBytes = Convert.FromBase64String(inputString.Substring(base64BlockSize * i, base64BlockSize));
                arrayList.AddRange(rsaCryptoServiceProvider.Decrypt(encryptedBytes, true));
            }

            // Снятие выравнивания с потока
            var bytesInput = arrayList.ToArray(Type.GetType("System.Byte")) as byte[];
            var ms = new MemoryStream(bytesInput);
            var output2 = (MemoryStream)CryforceUtilities.DeAlignStream(ms);
            output2.Seek(0, SeekOrigin.Begin);
            byte[] bytesInput2 = output2.ToArray();
            ms.Close();
            output2.Close();

            // Декомпрессия GZip
            var input = new MemoryStream(bytesInput2);
            var output = (MemoryStream)CryforceUtilities.Uncompress(input);
            output.Seek(0, SeekOrigin.Begin);
            byte[] bytes = output.ToArray();
            input.Close();
            output.Close();
            return Encoding.UTF8.GetString(bytes);
        }
    }
}