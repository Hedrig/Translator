using System;
using System.Runtime.Serialization;

namespace TranslatorLib
{
    [Serializable]
    internal class UnexpectedLexemException : Exception
    {
        public UnexpectedLexemException()
        {
        }

        public UnexpectedLexemException(string message) : base(message)
        {
        }

        public UnexpectedLexemException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnexpectedLexemException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}