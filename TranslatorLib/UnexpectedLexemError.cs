using System;
using System.Runtime.Serialization;

namespace TranslatorLib
{
    [Serializable]
    internal class UnexpectedLexemError : Exception
    {
        public UnexpectedLexemError()
        {
        }

        public UnexpectedLexemError(string message) : base(message)
        {
        }

        public UnexpectedLexemError(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnexpectedLexemError(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}