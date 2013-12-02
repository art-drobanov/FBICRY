#region

using System;
using System.IO;
using System.Text;

using NUnit.Framework;

#endregion

namespace CRYFORCE.Engine.Test
{
    [TestFixture]
    public class EcdhP521Test
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            SetUpIsOK = true; // Указываем, что была произведена установка
        }

        #endregion

        /// <summary>
        /// Была произведена установка?
        /// </summary>
        public bool SetUpIsOK { get; set; }

        /// <summary>
        /// Тест корректности работы класса шифрования по эллиптическим кривым + ЭЦП
        /// </summary>
        [Test]
        public void EcdhP521BaseTest()
        {
            if(!SetUpIsOK)
            {
                SetUp();
            }

            try
            {
                string HmacKey = null;

                string data = "This is a sample string to digital signature test! Это тестовая строка для проверки ЭЦП!";

                Random rnd = new Random(DateTime.Now.Ticks.GetHashCode());

                var seedArr = new byte[64]; // 512 bit
                rnd.NextBytes(seedArr);

                var hmacKeyArr = new byte[64]; // 512 bit
                rnd.NextBytes(hmacKeyArr);

                string aliceSeed = Encoding.Unicode.GetString(seedArr);

                rnd.NextBytes(seedArr);
                string bobSeed = Encoding.Unicode.GetString(seedArr);

                // Два прогона - один проход без Hmac, другой с ним...
                for(int m = 0; m < 2; m++)
                {
                    EcdhP521 alice = new EcdhP521(aliceSeed, null, HmacKey); // Приватного ключа нет - он будет сгенерирован!
                    EcdhP521 bob = new EcdhP521(bobSeed, null, HmacKey);     // Приватного ключа нет - он будет сгенерирован!

                    File.WriteAllText("alicePublicKey.txt", alice.PublicKeyECDH);
                    File.WriteAllText("alicePrivateKey.txt", alice.PrivateKeyECDH);
                    File.WriteAllText("aliceDataSign.txt", alice.SignData(data));

                    File.WriteAllText("bobPublicKey.txt", bob.PublicKeyECDH);
                    File.WriteAllText("bobPrivateKey.txt", bob.PrivateKeyECDH);
                    File.WriteAllText("bobDataSign.txt", bob.SignData(data));

                    //

                    alice = new EcdhP521(null, File.ReadAllText("alicePrivateKey.txt")); // Читаем СВОЙ приватный ключ!
                    bob = new EcdhP521(null, File.ReadAllText("bobPrivateKey.txt"));     // Читаем СВОЙ приватный ключ!

                    alice.PublicKeyFromOtherParty = File.ReadAllText("bobPublicKey.txt"); // Импортируем ЧУЖОЙ открытый ключ!
                    bob.PublicKeyFromOtherParty = File.ReadAllText("alicePublicKey.txt"); // Импортируем ЧУЖОЙ открытый ключ!

                    //

                    if(!alice.VerifyData(data, File.ReadAllText("bobDataSign.txt"), File.ReadAllText("bobPublicKey.txt")))
                    {
                        throw new Exception("EcdhP521Test: alice: sign of bob failed!");
                    }

                    if(!bob.VerifyData(data, File.ReadAllText("aliceDataSign.txt"), File.ReadAllText("alicePublicKey.txt")))
                    {
                        throw new Exception("EcdhP521Test: bob: sign of alice failed!");
                    }

                    //

                    if(!alice.CreateSymmetricKey())
                    {
                        throw new Exception("EcdhP521Test: DeriveKeyMaterial() failed!");
                    }

                    if(!bob.CreateSymmetricKey())
                    {
                        throw new Exception("EcdhP521Test: DeriveKeyMaterial() failed!");
                    }

                    //

                    var aliceSymmetricKeys256 = alice.Keys256;
                    var aliceSymmetricKey512 = alice.Key512;

                    var bobSymmetricKeys256 = bob.Keys256;
                    var bobSymmetricKey512 = bob.Key512;

                    for(int k = 0; k < 2; k++)
                    {
                        for(int i = 0; i < (256 >> 3); i++)
                        {
                            if(aliceSymmetricKeys256[k][i] != bobSymmetricKeys256[k][i])
                            {
                                throw new InvalidDataException("EcdhP521Test: Wrong Symmetric keys (256 bit)!");
                            }
                            if(aliceSymmetricKeys256[k][i] != aliceSymmetricKey512[(k * (256 >> 3)) + i])
                            {
                                throw new InvalidDataException("EcdhP521Test: Wrong Symmetric keys (256 bit) conversion from 512 bit key!");
                            }
                            if(bobSymmetricKeys256[k][i] != bobSymmetricKey512[(k * (256 >> 3)) + i])
                            {
                                throw new InvalidDataException("EcdhP521Test: Wrong simmetric keys (256 bit) conversion from 512 bit key!");
                            }
                        }
                    }

                    for(int i = 0; i < (512 >> 3); i++)
                    {
                        if(aliceSymmetricKey512[i] != bobSymmetricKey512[i])
                        {
                            throw new InvalidDataException("EcdhP521Test: Wrong simmetric keys (512 bit)!");
                        }
                    }

                    HmacKey = Encoding.Unicode.GetString(hmacKeyArr);
                } // for(int m = 0; m < 2; m++)
            } // try
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                Assert.Fail();
            }
        }
    }
}