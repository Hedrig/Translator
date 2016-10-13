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
            if (LexicalAnalyzer.CurrentLexem == Lexems.Begin)
            {
                LexicalAnalyzer.DecodeLexem();
                DecodeInstructionSequence();
            }
            CheckLexem(Lexems.End);
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

        static void DecodeInstructionSequence()
        {
            DecodeInstruction();
            while(LexicalAnalyzer.CurrentLexem == Lexems.Separator)
            {
                LexicalAnalyzer.DecodeLexem();
                DecodeInstruction();
            }
        }

        static void DecodeInstruction()
        {
            if (LexicalAnalyzer.CurrentLexem == Lexems.Identifier)
            {
                try
                {
                    Identifier id = NameTable.FindByName(LexicalAnalyzer.CurrentName);
                }
                catch (IdentifierNotDefinedException ex)
                {
                    throw new IdentifierNotDefinedException(
                        ex.Message + ", строка " + Reader.RowIndex + ", символ " + Reader.ColumnIndex);
                }
                DecodeAssigningOperation();
            }
        }

        static void DecodeAssigningOperation()
        {
            LexicalAnalyzer.DecodeLexem();
            CheckLexem(Lexems.Assignment);
            DecodeExpression();
        }

        private static void DecodeExpression()
        {
            throw new NotImplementedException();
        }
    }
}
