using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLib
{
    static class SyntaxAnalyzer
    {

        public static void Compile()
        {
            LexicalAnalyzer.Initialize();
            DecodeVariableDeclaring();
            CheckLexem(Lexems.Separator);
        }

        static void CheckLexem(Lexems expectedLexem)
        {
            if (LexicalAnalyzer.CurrentLexem != expectedLexem)
                throw new UnexpectedLexemError("Ожидалась лексема " + expectedLexem + ", получена лексема " +
                    LexicalAnalyzer.CurrentLexem);
            else
                LexicalAnalyzer.DecodeLexem();
        }

        static void DecodeVariableDeclaring()
        {
            CheckLexem(Lexems.Type);
            do
            {
                string name = string.Copy(LexicalAnalyzer.CurrentName);
                CheckLexem(Lexems.Identifier);
                NameTable.AddIdentifier(name, Category.Variable);
            }
            while (LexicalAnalyzer.CurrentLexem == Lexems.DeclareSeparator);
        }
    }
}
