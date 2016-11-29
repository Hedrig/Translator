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
            if (SyntaxAnalyzer.ErrorMessages.Length > 0)
                ErrorsOccurred(null, new ErrorOccurredEventArgs(SyntaxAnalyzer.ErrorMessages));
            else
                code = SyntaxAnalyzer.CompiledCode;
        }
    }
}
