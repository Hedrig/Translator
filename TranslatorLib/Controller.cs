using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLib
{
    /// <summary>
    /// Класс, связывающий форму с моделью построителя и
    /// генератора кода.
    /// </summary>
    public static class Controller
    {
        static string code = "";

        static StringBuilder output = new StringBuilder();
        static int errorCount = 0;

        public static event EventHandler CodeCompiled;

        public static string Code
        {
            get { return code; }
        }

        public static void Compile(string fileName)
        {
            errorCount = 0;
            output = new StringBuilder();
            code = "";
            Reader.Initialize(fileName);
            LexicalAnalyzer.Initialize();
            NameTable.Initialize();
            CodeGenerator.Initialize();
            SyntaxAnalyzer.Compile();
            // Компиляция завершена успешно, если ошибок нет
            if (errorCount == 0)
            {
                code = SyntaxAnalyzer.CompiledCode;
                Build(fileName);
            }
            Reader.Close();
            CodeCompiled(null, EventArgs.Empty);
        }

        private static void Build(string fileName)
        {
            CodeBuilder.Builder.BuildAll(fileName, code);
            output.AppendLine(CodeBuilder.Builder.Output);
        }


        internal static void Error(string errorMessage)
        {
            output.AppendLine(errorMessage);
            errorCount++;
        }

        public static string Output
        {
            get { return output.ToString(); }
        }
    }
}
