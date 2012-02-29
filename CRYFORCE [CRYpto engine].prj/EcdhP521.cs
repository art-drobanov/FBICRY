using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CRYFORCE.Engine
{
	/// <summary>
	/// Класс-обвязка реализации обмена ключами на основе шифрования по эллиптическим кривым + ЭЦП
	/// </summary>
	public class EcdhP521 : IDisposable
	{
		#region Static

		#endregion Static

		#region Constants

		/// <summary>Длина префикса ключа Cng.</summary>
		private const int CNG_KEY_PREFIX_LEN = 8;

		/// <summary>Размер открытого ключа.</summary>
		private const int PUBLIC_KEY_SIZE = 176;

		/// <summary>Размер закрытого ключа.</summary>
		private const int PRIVATE_KEY_SIZE = 264;

		#endregion Constants

		#region Data

		/// <summary>Экземпляр класса, реализующего обмен ключами на основе шифрования по эллиптическим кривым.</summary>
		private ECDiffieHellmanCng _ECDiffieHellmanCng;

		/// <summary>Экземпляр класса, реализующего ЭЦП на основе шифрования по эллиптическим кривым.</summary>
		private ECDsaCng _ECDsaCng;

		/// <summary>Параметры создания ключа.</summary>
		private CngKeyCreationParameters _cngKeyCreationParameters;

		/// <summary>Хеш-функция для целей формирования персонализированного ключа.</summary>
		private SHA512Cng _hash512;

		#endregion Data

		#region Events

		#endregion Events

		#region .ctor

		/// <summary>
		/// Конструктор по-умолчанию
		/// </summary>
		public EcdhP521()
		{
		}

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="seed">Инициализирующее случайное значение.</param>
		/// <param name="privateKey">Закрытый ключ.</param>
		/// <param name="hmacKey">Код аутентичности сообщения на базе хеш-функции.</param>
		public EcdhP521(Object seed, string privateKey, Object hmacKey = null)
		{
			// Инициализатор
			Initialize(seed, privateKey, hmacKey);
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
		/// Основной симметричный ключ (512 бит)
		/// </summary>
		public byte[] Key512 { get; private set; }

		/// <summary>
		/// Два симметричных ключа (256 бит) на основе основного ключа (512 бит)
		/// </summary>
		public byte[][] Keys256
		{
			get
			{
				if(Key512 != null)
				{
					var keys256 = new[] {new byte[256 >> 3], new byte[256 >> 3]};
					Array.Copy(Key512, 0, keys256[0], 0, (256 >> 3));
					Array.Copy(Key512, (256 >> 3), keys256[1], 0, (256 >> 3));

					return keys256;
				}

				return null;
			}
		}

		/// <summary>
		/// Открытый ключ
		/// </summary>
		public byte[] PublicKeyBin
		{
			get { return ExportKeyBinData(_ECDiffieHellmanCng.Key, true); }
		}

		/// <summary>
		/// Открытый ключ (в формате Base64)
		/// </summary>
		public string PublicKey
		{
			get { return Convert.ToBase64String(PublicKeyBin); }
		}

		/// <summary>
		/// Открытый ключ от второй стороны
		/// </summary>
		public byte[] PublicKeyFromOtherPartyBin { get; set; }

		/// <summary>
		/// Открытый ключ от второй стороны (в формате Base64)
		/// </summary>
		public string PublicKeyFromOtherParty
		{
			get { return Convert.ToBase64String(PublicKeyFromOtherPartyBin); }
			set { PublicKeyFromOtherPartyBin = Convert.FromBase64String(value.Base64String()); }
		}

		/// <summary>
		/// Закрытый ключ
		/// </summary>
		public byte[] PrivateKeyBin
		{
			get { return ExportKeyBinData(_ECDiffieHellmanCng.Key, false); }
		}

		/// <summary>
		/// Закрытый ключ (в формате Base64)
		/// </summary>
		public string PrivateKey
		{
			get { return Convert.ToBase64String(PrivateKeyBin); }
		}

		/// <summary>Экземпляр класса инициализирован?</summary>
		public bool IsInitialized { get; private set; }

		#endregion Properties

		#region Private

		/// <summary>
		/// Экспорт двоичных данных ключа
		/// </summary>
		/// <param name="cngKey">Оригинальный ключ.</param>
		/// <param name="isPublic">Ключ является публичным?</param>
		/// <returns>Двоичные данные ключа.</returns>
		private byte[] ExportKeyBinData(CngKey cngKey, bool isPublic)
		{
			// Экспортируем все доступные данные ключа
			byte[] cngKeyLongBinData = cngKey.Export(isPublic ? CngKeyBlobFormat.EccPublicBlob : CngKeyBlobFormat.EccPrivateBlob);

			// Вычисляем длину ключа, укороченного на префикс
			int cngKeyShortBinLength = (cngKeyLongBinData.Length - CNG_KEY_PREFIX_LEN);

			// Наполняем укороченный ключ...
			var cngKeyShortBin = new byte[cngKeyShortBinLength];
			Array.Copy(cngKeyLongBinData, CNG_KEY_PREFIX_LEN, cngKeyShortBin, 0, cngKeyShortBinLength);

			//...и возвращаем его
			return cngKeyShortBin;
		}

		/// <summary>
		/// Импорт двоичных данных ключа
		/// </summary>
		/// <param name="cngKeyShortBinData">Двоичные данные ключа.</param>
		/// <param name="isPublic">Ключ является публичным?</param>
		/// <param name="isDigitalSignature">Ключ предназначен для ЭЦП?</param>
		/// <returns>Оригинальный ключ.</returns>
		private CngKey ImportKeyBinData(byte[] cngKeyShortBinData, bool isPublic, bool isDigitalSignature)
		{
			// Создаем эталонный ключ для извлечения заголовка (на тот случай, если в очередной реализации будет изменен формат)
			CngKey cngKeyEtalon = CngKey.Create(isDigitalSignature ? CngAlgorithm.ECDsaP521 : CngAlgorithm.ECDiffieHellmanP521, "CngKey", _cngKeyCreationParameters);
			byte[] cngKeyEtalonBinData = cngKeyEtalon.Export(isPublic ? CngKeyBlobFormat.EccPublicBlob : CngKeyBlobFormat.EccPrivateBlob);

			// Вычисляем длину оригинального ключа
			int cngKeyLongBinLength = (cngKeyShortBinData.Length + CNG_KEY_PREFIX_LEN);

			// Создаем массив данных оригинального ключа...
			var cngKeyLongBinData = new byte[cngKeyLongBinLength];

			//...и восстанавливаем его заголовок...
			Array.Copy(cngKeyEtalonBinData, 0, cngKeyLongBinData, 0, CNG_KEY_PREFIX_LEN);

			//...а затем и тело ключа
			Array.Copy(cngKeyShortBinData, 0, cngKeyLongBinData, CNG_KEY_PREFIX_LEN, (cngKeyLongBinLength - CNG_KEY_PREFIX_LEN));

			// Создаем восстановленный ключ и возвращаем его
			CngKey cngKey = CngKey.Import(cngKeyLongBinData, isPublic ? CngKeyBlobFormat.EccPublicBlob : CngKeyBlobFormat.EccPrivateBlob);

			return cngKey;
		}

		/// <summary>
		/// Получение предпочтительной части открытого ключа абонента
		/// </summary>
		/// <param name="publicKey">Открытый ключ ЭЦП.</param>
		/// <returns>Предпочтительная часть ключа.</returns>
		private string GetEcdsaPrefferedKeyPart(string publicKey)
		{
			// Строка с ключом для ЭЦП
			string stringECDSA;

			try
			{
				// Пытаемся грузить расширенную часть ключа...
				stringECDSA = publicKey.Base64String().Substring(PUBLIC_KEY_SIZE, PUBLIC_KEY_SIZE);
			}
			catch
			{
				//...если не получилось - формируем ключ для ECDSA на основе ключа для ECDH
				stringECDSA = publicKey.Base64String().Substring(0, PUBLIC_KEY_SIZE);
			}

			return stringECDSA;
		}

		/// <summary>
		/// Проверка ЭЦП
		/// </summary>
		/// <param name="data">Массив данных.</param>
		/// <param name="offset">Смещение до участка интереса.</param>
		/// <param name="count">Длина байт участка интереса.</param>
		/// <param name="signature">ЭЦП.</param>
		/// <param name="publicKey">Открытый ключ для проверки ЭЦП.</param>
		/// <returns>Булевский флаг проверки ЭЦП.</returns>
		private bool VerifyData(byte[] data, int offset, int count, byte[] signature, byte[] publicKey)
		{
			if(!IsInitialized)
			{
				throw new Exception("EcdhP521::VerifyData() ==> EcdhP521 is not initialized!");
			}

			using(var eECDsaCng = new ECDsaCng(ImportKeyBinData(publicKey, true, true))) // public, DS
			{
				eECDsaCng.HashAlgorithm = CngAlgorithm.Sha512;
				return eECDsaCng.VerifyData(data, offset, count, signature);
			}
		}

		/// <summary>
		/// Проверка ЭЦП
		/// </summary>
		/// <param name="data">Поток данных.</param>
		/// <param name="signature">ЭЦП.</param>
		/// <param name="publicKey">Открытый ключ для проверки ЭЦП.</param>
		/// <returns>Булевский флаг проверки ЭЦП.</returns>
		private bool VerifyData(Stream data, byte[] signature, byte[] publicKey)
		{
			if(!IsInitialized)
			{
				throw new Exception("EcdhP521::VerifyData() ==> EcdhP521 is not initialized!");
			}

			using(var eECDsaCng = new ECDsaCng(ImportKeyBinData(publicKey, true, true))) // public, DS
			{
				eECDsaCng.HashAlgorithm = CngAlgorithm.Sha512;
				return eECDsaCng.VerifyData(data, signature);
			}
		}

		/// <summary>
		/// Проверка ЭЦП
		/// </summary>
		/// <param name="data">Массив данных.</param>
		/// <param name="signature">ЭЦП.</param>
		/// <param name="publicKey">Открытый ключ для проверки ЭЦП.</param>
		/// <returns>Булевский флаг проверки ЭЦП.</returns>
		private bool VerifyHash(byte[] data, byte[] signature, byte[] publicKey)
		{
			if(!IsInitialized)
			{
				throw new Exception("EcdhP521::VerifyHash() ==> EcdhP521 is not initialized!");
			}

			using(var eECDsaCng = new ECDsaCng(ImportKeyBinData(publicKey, true, true))) // public, DS
			{
				eECDsaCng.HashAlgorithm = CngAlgorithm.Sha512;
				return eECDsaCng.VerifyHash(data, signature);
			}
		}

		#endregion Private

		#region Protected

		#endregion Protected

		#region Public

		/// <summary>
		/// Инициализация экземпляра класса
		/// </summary>
		/// <param name="seed">Инициализирующее случайное значение.</param>
		/// <param name="privateKey">Закрытый ключ.</param>
		/// <param name="hmacKey">Код аутентичности сообщения на базе хеш-функции.</param>
		public void Initialize(Object seed, string privateKey, Object hmacKey = null)
		{
			// Защита от потери конфиденциальных данных при многократном вызове Initialize
			Clear();

			// Ключ для алгоритма ECDH
			CngKey cngKeyECDH;

			// Ключ для алгоритма ECDSA
			CngKey cngKeyECDSA;

			// Задаем параметры создания ключа
			_cngKeyCreationParameters = new CngKeyCreationParameters();
			_cngKeyCreationParameters.ExportPolicy = CngExportPolicies.AllowPlaintextExport;
			_cngKeyCreationParameters.KeyCreationOptions = CngKeyCreationOptions.OverwriteExistingKey;
			_cngKeyCreationParameters.KeyUsage = CngKeyUsages.AllUsages;
			_cngKeyCreationParameters.Provider = CngProvider.MicrosoftSoftwareKeyStorageProvider;

			// Если закрытый ключ не задан...
			if(privateKey == null)
			{
				// - создаем его...
				cngKeyECDH = CngKey.Create(CngAlgorithm.ECDiffieHellmanP521, "CngKey", _cngKeyCreationParameters);

				//...и экспортируя в двоичные данные подготовленный ключ DH, задаем на его основе ключ для ЭЦП
				cngKeyECDSA = ImportKeyBinData(ExportKeyBinData(cngKeyECDH, false), false, true);
			}
			else //...а иначе используем предоставленный...
			{
				//...импортируя его из строки в формате Base64
				cngKeyECDH = ImportKeyBinData(Convert.FromBase64String(privateKey.Base64String().Substring(0, PRIVATE_KEY_SIZE)), false, false); // Не ЭЦП

				// Строка с ключом для ЭЦП
				string stringECDSA;

				try
				{
					// Пытаемся грузить расширенную часть ключа...
					stringECDSA = privateKey.Base64String().Substring(PRIVATE_KEY_SIZE, PRIVATE_KEY_SIZE);
				}
				catch
				{
					//...если не получилось - формируем ключ для ECDSA на основе ключа для ECDH
					stringECDSA = privateKey.Base64String().Substring(0, PRIVATE_KEY_SIZE);
				}

				// Формируем ключ для ЭЦП на основе тех данных, что удалось добыть
				cngKeyECDSA = ImportKeyBinData(Convert.FromBase64String(stringECDSA), false, true); // ЭЦП
			}

			// Инициализируем криптографические сущности ключом
			_ECDiffieHellmanCng = new ECDiffieHellmanCng(cngKeyECDH);
			_ECDsaCng = new ECDsaCng(cngKeyECDSA);
			if(seed != null)
			{
				_ECDiffieHellmanCng.Seed = CryforceUtilities.ExtractByteArrayFromObject(seed);
			}

			// Если требуется использование кода аутентичности сообщения на основе хеша
			if(hmacKey != null)
			{
				_ECDiffieHellmanCng.HmacKey = CryforceUtilities.ExtractByteArrayFromObject(hmacKey);
				_ECDiffieHellmanCng.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hmac;
			}
			else
			{
				_ECDiffieHellmanCng.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
			}

			// Задаем функцию хеширования
			_ECDiffieHellmanCng.HashAlgorithm = _ECDsaCng.HashAlgorithm = CngAlgorithm.Sha512;

			// Указываем, что инициализация прошла успешно
			IsInitialized = true;
		}

		/// <summary>
		/// Инициализация экземпляра класса c персонализацией
		/// </summary>
		/// <param name="personString">Персонализирующая строка.</param>
		/// <param name="maxIters">Персонализирующая строка.</param>
		/// <param name="iterHandler">Обработчик события "прошедшая итерация".</param>
		/// <param name="seed">Инициализирующее случайное значение.</param>
		/// <param name="privateKey">Закрытый ключ.</param>
		/// <param name="hmacKey">Код аутентичности сообщения на базе хеш-функции.</param>
		/// <returns>Булевский флаг операции.</returns>
		public bool Initialize(string personString, int maxIters, Func<int, int> iterHandler, Object seed, string privateKey, Object hmacKey = null)
		{
			byte[] seedBuffer;
			if(seed != null)
			{
				seedBuffer = CryforceUtilities.ExtractByteArrayFromObject(seed);
			}
			else
			{
				return false;
			}

			// Отрабатываем все заданные итерации -...
			for(int i = 0; i < maxIters; i++)
			{
				//...если инициализация...
				Initialize(seed, privateKey, hmacKey);

				//...дала требуемую персонализацию...
				if(PublicKey.Contains(personString))
				{
					//...выходим с "успешным" флагом
					return true;
				}

				//...а иначе используем хеш-функцию для формирования очередного шанса на персонализацию...
				seedBuffer = _hash512.ComputeHash(seedBuffer);
				seed = Encoding.Unicode.GetString(seedBuffer);

				//...если подписаны на обработчик...
				if(iterHandler != null)
				{
					i = iterHandler(i); // Через данный обработчик можно управлять процессом - затянуть или ускорить вплоть до выхода					
				}
			}

			// Указываем, что инициализация прошла неуспешно
			return IsInitialized = false;
		}

		/// <summary>
		/// Генерация симметричного ключа на основе двух открытых и закрытого
		/// </summary>
		/// <returns>Булевский флаг операции.</returns>
		public bool CreateSymmetricKey()
		{
			if(!IsInitialized)
			{
				throw new Exception("EcdhP521::CreateSymmetricKey() ==> EcdhP521 is not initialized!");
			}

			// Если не задан открытый ключ от другой стороны - выполнение метода невозможно!
			if(PublicKeyFromOtherParty == null)
			{
				return false;
			}

			try
			{
				Key512 = _ECDiffieHellmanCng.DeriveKeyMaterial(ImportKeyBinData(Convert.FromBase64String(PublicKeyFromOtherParty.Base64String().Substring(0, PUBLIC_KEY_SIZE)), true, false));
			}
			catch
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Вычисление ЭЦП
		/// </summary>
		/// <param name="data">Массив данных.</param>
		/// <param name="offset">Смещение до участка интереса.</param>
		/// <param name="count">Длина байт участка интереса.</param>
		/// <returns>ЭЦП.</returns>
		public string SignData(byte[] data, int offset, int count)
		{
			if(!IsInitialized)
			{
				throw new Exception("EcdhP521::SignData() ==> EcdhP521 is not initialized!");
			}

			return Convert.ToBase64String(_ECDsaCng.SignData(data, offset, count));
		}

		/// <summary>
		/// Вычисление ЭЦП
		/// </summary>
		/// <param name="data">Массив данных.</param>
		/// <param name="offset">Смещение до участка интереса.</param>
		/// <returns>ЭЦП.</returns>
		public string SignData(byte[] data, int offset)
		{
			if(!IsInitialized)
			{
				throw new Exception("EcdhP521::SignData() ==> EcdhP521 is not initialized!");
			}

			return Convert.ToBase64String(_ECDsaCng.SignData(data, offset, (data.Length - offset)));
		}

		/// <summary>
		/// Вычисление ЭЦП
		/// </summary>
		/// <param name="data">Массив данных.</param>
		/// <returns>ЭЦП.</returns>
		public string SignData(byte[] data)
		{
			if(!IsInitialized)
			{
				throw new Exception("EcdhP521::SignData() ==> EcdhP521 is not initialized!");
			}

			return Convert.ToBase64String(_ECDsaCng.SignData(data));
		}

		/// <summary>
		/// Вычисление ЭЦП
		/// </summary>
		/// <param name="data">Поток данных.</param>
		/// <returns>ЭЦП.</returns>
		public string SignData(Stream data)
		{
			if(!IsInitialized)
			{
				throw new Exception("EcdhP521::SignData() ==> EcdhP521 is not initialized!");
			}

			return Convert.ToBase64String(_ECDsaCng.SignData(data));
		}

		/// <summary>
		/// Вычисление ЭЦП
		/// </summary>
		/// <param name="data">Строка данных.</param>
		/// <returns>ЭЦП.</returns>
		public string SignData(string data)
		{
			if(!IsInitialized)
			{
				throw new Exception("EcdhP521::SignData() ==> EcdhP521 is not initialized!");
			}

			return Convert.ToBase64String(_ECDsaCng.SignData(Encoding.Unicode.GetBytes(data)));
		}

		/// <summary>
		/// Вычисление ЭЦП
		/// </summary>
		/// <param name="data">Массив данных.</param>
		/// <returns>ЭЦП.</returns>
		public string SignHash(byte[] data)
		{
			if(!IsInitialized)
			{
				throw new Exception("EcdhP521::SignHash() ==> EcdhP521 is not initialized!");
			}

			return Convert.ToBase64String(_ECDsaCng.SignHash(data));
		}

		/// <summary>
		/// Проверка ЭЦП
		/// </summary>
		/// <param name="data">Массив данных.</param>
		/// <param name="offset">Смещение до участка интереса.</param>
		/// <param name="count">Длина байт участка интереса.</param>
		/// <param name="signature">ЭЦП.</param>
		/// <param name="publicKey">Открытый ключ для проверки ЭЦП.</param>
		/// <returns>Булевский флаг проверки ЭЦП.</returns>
		public bool VerifyData(byte[] data, int offset, int count, string signature, string publicKey)
		{
			publicKey = GetEcdsaPrefferedKeyPart(publicKey); // Берем предпочтительную часть расширенного ключа
			return VerifyData(data, offset, count, Convert.FromBase64String(signature.Base64String()), Convert.FromBase64String(publicKey));
		}

		/// <summary>
		/// Проверка ЭЦП
		/// </summary>
		/// <param name="data">Массив данных.</param>
		/// <param name="offset">Смещение до участка интереса.</param>
		/// <param name="signature">ЭЦП.</param>
		/// <param name="publicKey">Открытый ключ для проверки ЭЦП.</param>
		/// <returns>Булевский флаг проверки ЭЦП.</returns>
		public bool VerifyData(byte[] data, int offset, string signature, string publicKey)
		{
			byte[] signatureByteArr = Convert.FromBase64String(signature.Base64String());
			publicKey = GetEcdsaPrefferedKeyPart(publicKey); // Берем предпочтительную часть расширенного ключа
			return VerifyData(data, offset, signature.Length, signatureByteArr, Convert.FromBase64String(publicKey.Base64String()));
		}

		/// <summary>
		/// Проверка ЭЦП
		/// </summary>
		/// <param name="data">Массив данных.</param>
		/// <param name="signature">ЭЦП.</param>
		/// <param name="publicKey">Открытый ключ для проверки ЭЦП.</param>
		/// <returns>Булевский флаг проверки ЭЦП.</returns>
		public bool VerifyData(byte[] data, string signature, string publicKey)
		{
			byte[] signatureByteArr = Convert.FromBase64String(signature.Base64String());
			publicKey = GetEcdsaPrefferedKeyPart(publicKey); // Берем предпочтительную часть расширенного ключа
			return VerifyData(data, 0, signature.Length, signatureByteArr, Convert.FromBase64String(publicKey.Base64String()));
		}

		/// <summary>
		/// Проверка ЭЦП
		/// </summary>
		/// <param name="data">Строка данных.</param>
		/// <param name="signature">ЭЦП.</param>
		/// <param name="publicKey">Открытый ключ для проверки ЭЦП.</param>
		/// <returns>Булевский флаг проверки ЭЦП.</returns>
		public bool VerifyData(string data, string signature, string publicKey)
		{
			byte[] signatureByteArr = Convert.FromBase64String(signature.Base64String());
			publicKey = GetEcdsaPrefferedKeyPart(publicKey); // Берем предпочтительную часть расширенного ключа
			return VerifyData(Encoding.Unicode.GetBytes(data), 0, signature.Length, signatureByteArr, Convert.FromBase64String(publicKey.Base64String()));
		}

		/// <summary>
		/// Проверка ЭЦП
		/// </summary>
		/// <param name="data">Поток данных.</param>
		/// <param name="signature">ЭЦП.</param>
		/// <param name="publicKey">Открытый ключ для проверки ЭЦП.</param>
		/// <returns>Булевский флаг проверки ЭЦП.</returns>
		public bool VerifyData(Stream data, string signature, string publicKey)
		{
			publicKey = GetEcdsaPrefferedKeyPart(publicKey); // Берем предпочтительную часть расширенного ключа
			return VerifyData(data, Convert.FromBase64String(signature.Base64String()), Convert.FromBase64String(publicKey.Base64String()));
		}

		/// <summary>
		/// Проверка ЭЦП
		/// </summary>
		/// <param name="data">Массив данных.</param>
		/// <param name="signature">ЭЦП.</param>
		/// <param name="publicKey">Открытый ключ для проверки ЭЦП.</param>
		/// <returns>Булевский флаг проверки ЭЦП.</returns>
		public bool VerifyHash(byte[] data, string signature, string publicKey)
		{
			publicKey = GetEcdsaPrefferedKeyPart(publicKey); // Берем предпочтительную часть расширенного ключа
			return VerifyHash(data, Convert.FromBase64String(signature.Base64String()), Convert.FromBase64String(publicKey.Base64String()));
		}

		/// <summary>
		/// Очистка конфиденциальных данных
		/// </summary>
		public void Clear()
		{
			// Указываем на деинициализацию
			IsInitialized = false;

			if(_ECDiffieHellmanCng != null)
			{
				_ECDiffieHellmanCng.Clear();
			}
			_ECDiffieHellmanCng = new ECDiffieHellmanCng();

			if(_ECDsaCng != null)
			{
				_ECDsaCng.Clear();
			}
			_ECDsaCng = new ECDsaCng();

			if(_hash512 != null)
			{
				_hash512.Clear();
			}
			_hash512 = new SHA512Cng();

			CryforceUtilities.ClearArray(Key512);
		}

		#endregion Public
	}
}