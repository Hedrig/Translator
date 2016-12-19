using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLib
{
    static class SyntaxAnalyzer
    {

        const string TYPE_MISMATCH_MESSAGE = 
            "Несовпадение типов выражений, строка {0}";
        const string WRONG_LEXEM_MESSAGE = 
            "Ожидалась лексема {0}, получена лексема {1}, (строка {2})";
        const string UNEXPECTED_LEXEM_MESSAGE =
            "Неожиданная лексема {0}, (строка {1})";

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
            DecodeVariableDeclaring();
            CodeGenerator.DeclareVariables();
            CodeGenerator.DeclareStackAndCodeSegments();
            CheckLexemAndProceed(Lexems.Separator);
            if (LexicalAnalyzer.CurrentLexem == Lexems.Begin)
            {
                LexicalAnalyzer.DecodeNextLexem();
                DecodeInstructionSequence();
            }
            CheckLexemAndProceed(Lexems.End);
            DecodePrintOperation();
            CodeGenerator.DeclareMainProcedureEnding();
        }

        static void CheckLexem(Lexems expectedLexem)
        {
            if (LexicalAnalyzer.CurrentLexem != expectedLexem)
                Controller.Error(string.Format(WRONG_LEXEM_MESSAGE, 
                    expectedLexem, LexicalAnalyzer.CurrentLexem, Reader.RowIndex));
        }

        static void CheckLexemAndProceed(Lexems expectedLexem)
        {
            CheckLexem(expectedLexem);
            LexicalAnalyzer.DecodeNextLexem();
        }

        static void DecodeVariableDeclaring()
        {
            CheckLexem(Lexems.Type);
            Type type;
            try
            {
                type = (Type)Enum.Parse(typeof(Type), LexicalAnalyzer.CurrentName);
            }
            catch (ArgumentException)
            {
                type = Type.None;
            }
            do
            {
                LexicalAnalyzer.DecodeNextLexem();
                string name = string.Copy(LexicalAnalyzer.CurrentName);
                CheckLexemAndProceed(Lexems.Identifier);
                NameTable.AddIdentifier(name, Category.Variable, type);
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
            switch (LexicalAnalyzer.CurrentLexem)
            {
                case (Lexems.Identifier):
                    Identifier id = NameTable.FindByName(
                    LexicalAnalyzer.CurrentName);
                    if (id.Name != null)
                    {
                        DecodeAssigningOperation();
                        CodeGenerator.AssignFromStack(id.Name);
                    }
                    break;
                default: Controller.Error(
                    string.Format(UNEXPECTED_LEXEM_MESSAGE,
                    LexicalAnalyzer.CurrentLexem, Reader.RowIndex));
                    break;    
            }
        }

        static void DecodeAssigningOperation()
        {
            LexicalAnalyzer.DecodeNextLexem();
            CheckLexemAndProceed(Lexems.Assignment);
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
                        Controller.Error(string.Format(
                            TYPE_MISMATCH_MESSAGE, Reader.RowIndex));
                    switch (operation)
                    {
                        case Lexems.Addition:
                            CodeGenerator.AddTwoStackValues();
                            break;
                        case Lexems.Subtraction:
                            CodeGenerator.SubTwoStackValues();
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
            Type leftExpressionType = DecodeSubExpression(), 
                rightExpressionType;
            if (LexicalAnalyzer.CurrentLexem == Lexems.Multiplication || 
                LexicalAnalyzer.CurrentLexem == Lexems.Division)
            {
                do
                {
                    operation = LexicalAnalyzer.CurrentLexem;
                    LexicalAnalyzer.DecodeNextLexem();
                    rightExpressionType = DecodeSubExpression();
                    if (leftExpressionType != rightExpressionType)
                        Controller.Error(string.Format(
                            TYPE_MISMATCH_MESSAGE, Reader.RowIndex));
                    switch (operation)
                    {
                        case Lexems.Multiplication:
                            CodeGenerator.MultiplyTwoStackValues();
                            break;
                        case Lexems.Division:
                            CodeGenerator.DivideSecondStackValueByFirst();
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
                    id = NameTable.FindByName(currentName);
                    CodeGenerator.PushValue(id.Name);
                    return id.Type;
                case (Lexems.Number):
                    CodeGenerator.PushValue(currentName);
                    return Type.Integer;
                case (Lexems.OpenBracket):
                    expressionType = DecodeExpression();
                    CheckLexemAndProceed(Lexems.CloseBracket);
                    return expressionType;
                default:
                    Controller.Error(string.Format(UNEXPECTED_LEXEM_MESSAGE,
                        LexicalAnalyzer.CurrentLexem, Reader.RowIndex));
                    return Type.None;
            }
        }

        static void DecodePrintOperation()
        {
            LexicalAnalyzer.DecodeNextLexem();
            CheckLexemAndProceed(Lexems.Print);
            CheckLexem(Lexems.Identifier);
            Identifier id = NameTable.FindByName(LexicalAnalyzer.CurrentName);
            if (id.Name != null)
                CodeGenerator.DeclarePrintOperation(id.Name);
        }
    }
}
