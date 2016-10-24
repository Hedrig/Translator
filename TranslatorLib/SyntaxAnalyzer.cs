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
                throw new UnexpectedLexemException("Ожидалась лексема " + expectedLexem + ", получена лексема " +
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

        private static Type DecodeExpression()
        {
            return DecodeAdditionOrSubtraction();
        }

        static Type DecodeAdditionOrSubtraction()
        {
            Type t;
            Lexems operation;
            if (LexicalAnalyzer.CurrentLexem == Lexems.Addition ||
                LexicalAnalyzer.CurrentLexem == Lexems.Subtraction)
            {
                operation = LexicalAnalyzer.CurrentLexem;
                LexicalAnalyzer.DecodeLexem();
                t = DecodeMultiplicationOrDivision();
            }
            else
                t = DecodeMultiplicationOrDivision();
            if (LexicalAnalyzer.CurrentLexem == Lexems.Addition ||
                LexicalAnalyzer.CurrentLexem == Lexems.Subtraction)
            {
                do
                {
                    operation = LexicalAnalyzer.CurrentLexem;
                    LexicalAnalyzer.DecodeLexem();
                    t = DecodeMultiplicationOrDivision();
                    switch (operation)
                    {
                        case Lexems.Addition:
                            break;
                        case Lexems.Subtraction:
                            break;
                    }
                }
                while (LexicalAnalyzer.CurrentLexem == Lexems.Addition ||
                       LexicalAnalyzer.CurrentLexem == Lexems.Subtraction);
            }
            return t;
        }

        static Type DecodeMultiplicationOrDivision()
        {
            Lexems operation;
            Type t = DecodeSubExpression();
            if (LexicalAnalyzer.CurrentLexem == Lexems.Multiplication || 
                LexicalAnalyzer.CurrentLexem == Lexems.Division)
            {
                do
                {
                    operation = LexicalAnalyzer.CurrentLexem;
                    LexicalAnalyzer.DecodeLexem();
                    t = DecodeSubExpression();
                    switch (operation)
                    {
                        case Lexems.Multiplication:
                            break;
                        case Lexems.Division:
                            break;
                    }
                }
                while (LexicalAnalyzer.CurrentLexem == Lexems.Multiplication ||
                LexicalAnalyzer.CurrentLexem == Lexems.Division);
            }
            return t;
        }

        static Type DecodeSubExpression()
        {
            Identifier id;
            Type t = Type.None;
            Lexems currentLexem = LexicalAnalyzer.CurrentLexem;
            string currentName = LexicalAnalyzer.CurrentName;
            LexicalAnalyzer.DecodeLexem();
            switch (currentLexem)
            {
                case (Lexems.Identifier):
                    {
                        id = NameTable.FindByName(currentName);
                        return id.Type;
                    }
                case (Lexems.Number):
                    {
                        return Type.Integer;
                    }
                case (Lexems.OpenBracket):
                    {
                        t = DecodeExpression();
                        CheckLexem(Lexems.CloseBracket);
                        return t;
                    }
                default:
                    {
                        throw new UnexpectedLexemException("Неожиданная лексема " + currentLexem.ToString());
                    }
            }
        }
    }
}
