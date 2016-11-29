namespace TranslatorLib
{
    public class ErrorOccurredEventArgs: System.EventArgs
    {
        internal ErrorOccurredEventArgs(string messages)
        {
            Messages = messages;
        }
        public string Messages { get; }
    }
}