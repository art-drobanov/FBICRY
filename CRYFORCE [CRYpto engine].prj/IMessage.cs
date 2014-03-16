using System;

namespace CRYFORCE.Engine
{
    public interface IMessage
    {
        /// <summary>
        /// Обвязка вызова события "Изменение прогресса процесса"
        /// </summary>
        /// <param name="processDescription"> Описание процесса. </param>
        /// <param name="processProgress"> Прогресс процесса. </param>
        /// <param name="rFlag"> Переводить каретку? </param>
        void Progress(string processDescription, double processProgress, bool rFlag = false);

        /// <summary>
        /// Обвязка вызова обработчика события "Получено сообщение"
        /// </summary>
        /// <param name="messageBody"> Тело сообщения. </param>
        /// <param name="messagePostfix"> Постфикс сообщения. </param>
        void Message(string messageBody, string messagePostfix = "");
    }
}