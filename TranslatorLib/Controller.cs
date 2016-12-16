using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLib
{
    public static class Controller
    {
        static string code = "";
        public static string Code
        {
            get { return code; }
        }

        public static event EventHandler<ErrorOccurredEventArgs> ErrorsOccurred;

        public static void Compile(string fileName)
        {
            Reader.Initialize(fileName);
            LexicalAnalyzer.Initialize();
            SyntaxAnalyzer.Compile();
            if (errors.Length > 0)
                ErrorsOccurred(null, new ErrorOccurredEventArgs(errors.ToString()));
            else
                code = SyntaxAnalyzer.CompiledCode;
        }

        static StringBuilder errors = new StringBuilder();
        static int errorCount = 0;

        internal static void Error(string errorMessage)
        {
            errors.AppendLine(errorMessage);
            errorCount++;
        }

        public static string Errors
        {
            get { return errors.ToString(); }
        }
    }
}
