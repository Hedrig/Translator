using System;
using System.Runtime.Serialization;

namespace TranslatorLib
{
    [Serializable]
    internal class UnidentifiedSymbolException : Exception
    {
        public UnidentifiedSymbolException()
        {
        }

        public UnidentifiedSymbolException(string message) : base(message)
        {
        }

        public UnidentifiedSymbolException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnidentifiedSymbolException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}