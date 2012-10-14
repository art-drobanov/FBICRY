using System;

namespace CRYFORCE.Engine
{
    /// <summary>
    /// Аргумент обработчика события "Изменение прогресса процесса"
    /// </summary>
    public class ProgressChangedArg : MessageReceivedArg
    {
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
            : base(processDescription, messagePostfix, messageClassId)
        {
            ProcessProgress = processProgress;
        }
    }
}