using System;

namespace CRYFORCE.Engine
{
	/// <summary>
	/// Аргумент обработчика события "Изменение прогресса процесса"
	/// </summary>
	public struct ProgressChangedArg
	{
		/// <summary>Класс сообщения.</summary>
		public readonly long MessageClassId;

		/// <summary>Идентификатор сообщения.</summary>
		public readonly Guid MessageGuid;

		/// <summary>Постфикс сообщения.</summary>
		public readonly string MessagePostfix;

		/// <summary>Описание процесса.</summary>
		public readonly string ProcessDescription;

		/// <summary>Прогресс процесса.</summary>
		public readonly double ProcessProgress;

		/// <summary>
		/// Конструктор аргумента обработчика события "Изменение прогресса процесса"
		/// </summary>
		/// <param name="processDescription">Описание процесса.</param>
		/// <param name="processProgress">Прогресс процесса.</param>
		/// <param name="messagePostfix">Пост-сообщение (например, возврат каретки).</param>
		/// <param name="messageClassId">Класс сообщения.</param>
		public ProgressChangedArg(string processDescription, double processProgress, string messagePostfix = "", long messageClassId = 0)
		{
			ProcessDescription = processDescription;
			ProcessProgress = processProgress;
			MessagePostfix = messagePostfix;
			MessageClassId = messageClassId;
			MessageGuid = Guid.NewGuid();
		}
	}
}