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
                throw new UnexpectedLexemError();
            else
                LexicalAnalyzer.DecodeLexem();
        }

        static void DecodeVariableDeclaring()
        {
            LexicalAnalyzer.DecodeLexem();
            if(LexicalAnalyzer.CurrentLexem != Lexems)
        }
    }
}
