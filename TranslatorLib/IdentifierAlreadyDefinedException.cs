using System;
using System.Runtime.Serialization;

namespace TranslatorLib
{
    [Serializable]
    internal class IdentifierAlreadyDefinedException : Exception
    {
        public IdentifierAlreadyDefinedException()
        {
        }

        public IdentifierAlreadyDefinedException(string message) : base(message)
        {
        }

        public IdentifierAlreadyDefinedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IdentifierAlreadyDefinedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}