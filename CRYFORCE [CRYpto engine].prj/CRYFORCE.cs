using System;
using System.IO;

namespace CRYFORCE.Engine
{
	public class CRYFORCE
	{
		#region Static

		#endregion Static

		#region Constants

		#endregion Constants

		#region Data

		/// <summary>Сущность для разбиения файла на битовые потоки и обратного склеивания.</summary>
		private BitSplitter _bitSplitter;

		/// <summary>Сущность для обеспечения обмена ключами на основе шифрования на основе эллиптических кривых.</summary>
		private EcdhP521 _ecdhP521;

		/// <summary>Сущность для шифрования данных по алгоритму Rijndael (256 bit).</summary>
		private StreamCryptoWrapper _streamCryptoWrapper;

		#endregion Data

		#region Events

		#endregion Events

		#region .ctor

		#endregion .ctor

		#region Properties

		/// <summary>Используется прямое направление преобразования?</summary>
		public bool DirectMode { get; private set; }

		/// <summary>Работаем в ОЗУ?</summary>
		public bool WorkInMemory { get; private set; }

		/// <summary>Размер буфера в ОЗУ под каждый поток.</summary>
		public int BufferSizePerStream { get; private set; }

		/// <summary>Инициализирующее значение генератора случайных чисел.</summary>
		public int RndSeed { get; set; }

		/// <summary>Затирать выходной поток нулями?</summary>
		public bool ZeroOut { get; set; }

		/// <summary>Экземпляр класса инициализирован?</summary>
		public bool IsInitialized { get; private set; }

		#endregion Properties

		#region Private

		#endregion Private

		#region Protected

		#endregion Protected

		#region Public

		/// <summary>
		/// Двойное шифрование по алгоритму Rijndael-256
		/// </summary>
		/// <param name="inputStream">Входной поток.</param>
		/// <param name="password1">Пароль для первого прохода шифрования.</param>
		/// <param name="password2">Пароль для второго прохода шифрования.</param>
		/// <param name="outputStream">Выходной поток.</param>
		public void DoubleEncrypt(Stream inputStream, byte[] password1, byte[] password2, Stream outputStream)
		{
			
		}

		#endregion Public
	}
}