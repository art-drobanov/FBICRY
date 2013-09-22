#region

using System;

#endregion

namespace CRYFORCE.Engine
{
    /// <summary>
    /// Аргумент обработчика события "Получено сообщение"
    /// </summary>
    public class MessageReceivedArg
    {
        /// <summary>
        /// Тело сообщения.
        /// </summary>
        public readonly string MessageBody;

        /// <summary>
        /// Класс сообщения.
        /// </summary>
        public readonly long MessageClassId;

        /// <summary>
        /// Идентификатор сообщения.
        /// </summary>
        public readonly Guid MessageGuid;

        /// <summary>
        /// Постфикс сообщения.
        /// </summary>
        public readonly string MessagePostfix;

        /// <summary>
        /// Конструктор аргумента обработчика события "Получено сообщение"
        /// </summary>
        /// <param name="messageBody"> Тело сообщения. </param>
        /// <param name="messagePostfix"> Постфикс сообщение (например, возврат каретки). </param>
        /// <param name="messageClassId"> Класс сообщения. </param>
        public MessageReceivedArg(string messageBody, string messagePostfix = "", long messageClassId = 0)
        {
            MessageBody = messageBody;
            MessagePostfix = messagePostfix;
            MessageClassId = messageClassId;
            MessageGuid = Guid.NewGuid();
        }
    }
}