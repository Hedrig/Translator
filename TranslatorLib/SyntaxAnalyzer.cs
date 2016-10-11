using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLib
{
    static class SyntaxAnalyzer
    {
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
            LexicalAnalyzer.DecodeLexem();
            string name = string.Copy(LexicalAnalyzer.CurrentName);
            CheckLexem(Lexems.Identifier);
            
        }
    }
}
