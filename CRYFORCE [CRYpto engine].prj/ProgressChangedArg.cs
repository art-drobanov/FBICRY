using System;

namespace CRYFORCE.Engine
{
	public struct ProgressChangedArg
	{
		/// <summary>
		/// Конструктор аргумента обработчика события "Изменение прогресса процесса"
		/// </summary>
		/// <param name="processDescription">Описание процесса.</param>
		/// <param name="processProgress">Прогресс процесса.</param>
		/// <param name="messageClassId">Класс сообщения.</param>
		public ProgressChangedArg(string processDescription, double processProgress, long messageClassId = 0)
		{			
			ProcessDescription = processDescription;
			ProcessProgress = processProgress;
			MessageClassId = messageClassId;
			MessageGuid = Guid.NewGuid();
		}

		/// <summary>Описание процесса.</summary>
		public readonly string ProcessDescription;

		/// <summary>Прогресс процесса.</summary>
		public readonly double ProcessProgress;
		
		/// <summary>Класс сообщения.</summary>
		public readonly long MessageClassId;

		/// <summary>Идентификатор сообщения.</summary>
		public readonly Guid MessageGuid;
	}
}