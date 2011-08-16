using System;

namespace CRYFORCE.Engine
{
	public struct ProgressChangedArg
	{
		/// <summary>
		/// Конструктор аргумента обработчика события "Изменение прогресса процесса"
		/// </summary>
		/// <param name="processProgress">Прогресс процесса.</param>
		/// <param name="processDescription">Описание процесса.</param>
		/// <param name="messageClassId">Класс сообщения.</param>
		public ProgressChangedArg(double processProgress, string processDescription, long messageClassId = 0)
		{
			ProcessProgress = processProgress;
			ProcessDescription = processDescription;
			MessageClassId = messageClassId;
			MessageGuid = Guid.NewGuid();
		}

		/// <summary>Прогресс процесса.</summary>
		public readonly double ProcessProgress;

		/// <summary>Описание процесса.</summary>
		public readonly string ProcessDescription;

		/// <summary>Класс сообщения.</summary>
		public readonly long MessageClassId;

		/// <summary>Идентификатор сообщения.</summary>
		public readonly Guid MessageGuid;
	}
}