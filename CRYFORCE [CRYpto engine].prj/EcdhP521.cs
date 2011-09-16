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
					var keys256 = new byte[2][] {new byte[256 >> 3], new byte[256 >> 3]};
					Array.Copy(Key512, 0, keys256[0], 0, (256 >> 3));
					Array.Copy(Key512, (256 >> 3), keys256[1], 0, (256 >> 3));
					return keys256;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Открытый ключ
		/// </summary>
		public string PublicKey
		{
			get { return Convert.ToBase64String(ExportKeyBinData(_ECDiffieHellmanCng.Key, true)); }
		}

		/// <summary>
		/// Открытый ключ от второй стороны
		/// </summary>
		public string PublicKeyFromOtherParty { get; set; }

		/// <summary>
		/// Закрытый ключ
		/// </summary>
		public string PrivateKey
		{
			get { return Convert.ToBase64String(ExportKeyBinData(_ECDiffieHellmanCng.Key, false)); }
		}

		/// <summary>Экземпляр класса инициализирован?</summary>
		public bool IsInitialized { get; private set; }

		#endregion Properties

		#region Private

		/// <summary>
		/// Извлечение массива байт из объекта
		/// </summary>
		/// <param name="obj">Объект.</param>
		/// <returns>Массив байт.</returns>
		private byte[] ExtractByteArrayFromObject(Object obj)
		{
			byte[] result = null;

			if(obj is byte[])
			{
				// Либо берем байты напрямую...
				if(((byte[])obj).Length > 0) result = (byte[])obj;
			}
			else if(obj is string)
			{
				//...либо извлекаем их из строки
				if(((string)obj).Length > 0) result = Encoding.Unicode.GetBytes(((string)obj));
			}

			return result;
		}

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
			CngKey cngKeyDH;

			// Ключ для алгоритма ECDsa
			CngKey cngKeyDS;

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
				cngKeyDH = CngKey.Create(CngAlgorithm.ECDiffieHellmanP521, "CngKey", _cngKeyCreationParameters);

				//...и экспортируя в двоичные данные подготовленный ключ DH, задаем на его основе ключ для ЭЦП
				cngKeyDS = ImportKeyBinData(ExportKeyBinData(cngKeyDH, false), false, true);
			}
			else //...а иначе используем предоставленный...
			{
				//...импортируя его из строки в формате Base64
				cngKeyDH = ImportKeyBinData(Convert.FromBase64String(privateKey), false, false); // Не ЭЦП
				cngKeyDS = ImportKeyBinData(Convert.FromBase64String(privateKey), false, true); // ЭЦП
			}

			// Инициализируем криптографические сущности ключом
			_ECDiffieHellmanCng = new ECDiffieHellmanCng(cngKeyDH);
			_ECDsaCng = new ECDsaCng(cngKeyDS);
			if(seed != null)
			{
				_ECDiffieHellmanCng.Seed = ExtractByteArrayFromObject(seed);
			}

			// Если требуется использование кода аутентичности сообщения на основе хеша
			if(hmacKey != null)
			{
				_ECDiffieHellmanCng.HmacKey = ExtractByteArrayFromObject(hmacKey);
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
				seedBuffer = ExtractByteArrayFromObject(seed);
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
				Key512 = _ECDiffieHellmanCng.DeriveKeyMaterial(ImportKeyBinData(Convert.FromBase64String(PublicKeyFromOtherParty), true, false));
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
			if(!IsInitialized)
			{
				throw new Exception("EcdhP521::VerifyData() ==> EcdhP521 is not initialized!");
			}

			using(var eECDsaCng = new ECDsaCng(ImportKeyBinData(Convert.FromBase64String(publicKey), true, true))) // public, DS
			{
				eECDsaCng.HashAlgorithm = CngAlgorithm.Sha512;
				return eECDsaCng.VerifyData(data, offset, count, Convert.FromBase64String(signature));
			}
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
			if(!IsInitialized)
			{
				throw new Exception("EcdhP521::VerifyData() ==> EcdhP521 is not initialized!");
			}

			using(var eECDsaCng = new ECDsaCng(ImportKeyBinData(Convert.FromBase64String(publicKey), true, true))) // public, DS
			{
				eECDsaCng.HashAlgorithm = CngAlgorithm.Sha512;
				return eECDsaCng.VerifyData(data, offset, (data.Length - offset), Convert.FromBase64String(signature));
			}
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
			if(!IsInitialized)
			{
				throw new Exception("EcdhP521::VerifyData() ==> EcdhP521 is not initialized!");
			}

			using(var eECDsaCng = new ECDsaCng(ImportKeyBinData(Convert.FromBase64String(publicKey), true, true))) // public, DS
			{
				eECDsaCng.HashAlgorithm = CngAlgorithm.Sha512;
				return eECDsaCng.VerifyData(data, Convert.FromBase64String(signature));
			}
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
			if(!IsInitialized)
			{
				throw new Exception("EcdhP521::VerifyData() ==> EcdhP521 is not initialized!");
			}

			using(var eECDsaCng = new ECDsaCng(ImportKeyBinData(Convert.FromBase64String(publicKey), true, true))) // public, DS
			{
				eECDsaCng.HashAlgorithm = CngAlgorithm.Sha512;
				return eECDsaCng.VerifyData(data, Convert.FromBase64String(signature));
			}
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
			if(!IsInitialized)
			{
				throw new Exception("EcdhP521::VerifyData() ==> EcdhP521 is not initialized!");
			}

			using(var eECDsaCng = new ECDsaCng(ImportKeyBinData(Convert.FromBase64String(publicKey), true, true))) // public, DS
			{
				eECDsaCng.HashAlgorithm = CngAlgorithm.Sha512;
				return eECDsaCng.VerifyData(Encoding.Unicode.GetBytes(data), Convert.FromBase64String(signature));
			}
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
			if(!IsInitialized)
			{
				throw new Exception("EcdhP521::VerifyHash() ==> EcdhP521 is not initialized!");
			}

			using(var eECDsaCng = new ECDsaCng(ImportKeyBinData(Convert.FromBase64String(publicKey), true, true))) // public, DS
			{
				eECDsaCng.HashAlgorithm = CngAlgorithm.Sha512;
				return eECDsaCng.VerifyHash(data, Convert.FromBase64String(signature));
			}
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

			if(Key512 != null)
			{
				for(int i = 0; i < Key512.Length; i++)
				{
					Key512[i] = 0x00;
				}
			}
		}

		#endregion Public
	}
}