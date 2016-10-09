using System;
using System.Runtime.Serialization;

namespace TranslatorLib
{
    [Serializable]
    internal class IdentifierNotDefinedException : Exception
    {
        public IdentifierNotDefinedException()
        {
        }

        public IdentifierNotDefinedException(string message) : base(message)
        {
        }

        public IdentifierNotDefinedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IdentifierNotDefinedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}