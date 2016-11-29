using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLib
{
    static class SyntaxAnalyzer
    {
        static StringBuilder errorMessage;

        public static string ErrorMessages
        {
            get { return errorCounter.ToString(); }
        }

        static uint errorCounter;
        internal static string CompiledCode
        {
            get
            {
                StringBuilder result = new StringBuilder();
                foreach (string codeLine in CodeGenerator.Code)
                    result.AppendLine(codeLine);
                return result.ToString();
            }
        }

        internal static void Compile()
        {
            CodeGenerator.DeclareDataSegment();
            LexicalAnalyzer.Initialize();
            errorMessage = new StringBuilder();
            DecodeVariableDeclaring();
            CodeGenerator.DeclareVariables();
            CodeGenerator.DeclareStackAndCodeSegments();
            CheckLexem(Lexems.Separator);
            if (LexicalAnalyzer.CurrentLexem == Lexems.Begin)
            {
                LexicalAnalyzer.DecodeNextLexem();
                DecodeInstructionSequence();
            }
            CheckLexem(Lexems.End);
            CodeGenerator.DeclareMainProcedureEnding();
            CodeGenerator.DeclareCodeEnding();
        }

        static void CheckLexem(Lexems expectedLexem)
        {
            if (LexicalAnalyzer.CurrentLexem != expectedLexem)
                Error("Ожидалась лексема " + expectedLexem + ", получена лексема " +
                    LexicalAnalyzer.CurrentLexem);
            else
                LexicalAnalyzer.DecodeNextLexem();
        }

        private static void Error(string message)
        {
            errorMessage.AppendLine(message);
            errorCounter++;
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
                LexicalAnalyzer.DecodeNextLexem();
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
                    Error(
                        ex.Message + ", строка " + Reader.RowIndex + ", символ " + Reader.ColumnIndex);
                }
                DecodeAssigningOperation();
            }
        }

        static void DecodeAssigningOperation()
        {
            LexicalAnalyzer.DecodeNextLexem();
            CheckLexem(Lexems.Assignment);
            DecodeExpression();
        }

        private static Type DecodeExpression()
        {
            return DecodeAdditionOrSubtraction();
        }

        static Type DecodeAdditionOrSubtraction()
        {
            Type leftExpressionType, rightExpressionType;
            Lexems operation;
            if (LexicalAnalyzer.CurrentLexem == Lexems.Addition ||
                LexicalAnalyzer.CurrentLexem == Lexems.Subtraction)
            {
                operation = LexicalAnalyzer.CurrentLexem;
                LexicalAnalyzer.DecodeNextLexem();
                leftExpressionType = DecodeMultiplicationOrDivision();
            }
            else
                leftExpressionType = DecodeMultiplicationOrDivision();
            if (LexicalAnalyzer.CurrentLexem == Lexems.Addition ||
                LexicalAnalyzer.CurrentLexem == Lexems.Subtraction)
            {
                do
                {
                    operation = LexicalAnalyzer.CurrentLexem;
                    LexicalAnalyzer.DecodeNextLexem();
                    rightExpressionType = DecodeMultiplicationOrDivision();
                    if (leftExpressionType != rightExpressionType)
                        Error("Несоответствие типов выражений, строка " + Reader.ColumnIndex + ", символ " + Reader.CurrentSymbol);
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
            return leftExpressionType;
        }

        static Type DecodeMultiplicationOrDivision()
        {
            Lexems operation;
            Type leftExpressionType = DecodeSubExpression(), rightExpressionType;
            if (LexicalAnalyzer.CurrentLexem == Lexems.Multiplication || 
                LexicalAnalyzer.CurrentLexem == Lexems.Division)
            {
                do
                {
                    operation = LexicalAnalyzer.CurrentLexem;
                    LexicalAnalyzer.DecodeNextLexem();
                    rightExpressionType = DecodeSubExpression();
                    if (leftExpressionType != rightExpressionType)
                        Error("Несоответствие типов выражений, строка " + Reader.ColumnIndex + ", символ " + Reader.CurrentSymbol);
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
            return leftExpressionType;
        }

        static Type DecodeSubExpression()
        {
            Identifier id;
            Type expressionType = Type.None;
            Lexems currentLexem = LexicalAnalyzer.CurrentLexem;
            string currentName = LexicalAnalyzer.CurrentName;
            LexicalAnalyzer.DecodeNextLexem();
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
                        expressionType = DecodeExpression();
                        CheckLexem(Lexems.CloseBracket);
                        return expressionType;
                    }
                default:
                    {
                        Error("Неожиданная лексема " + currentLexem.ToString());
                        return Type.None;
                    }
            }
        }
    }
}
