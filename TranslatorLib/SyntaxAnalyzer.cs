using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLib
{
    static class SyntaxAnalyzer
    {
        /// <summary>
        /// Сообщение о несоответствии типов. 
        /// string.Format:
        /// {0} - тип левого выражения;
        /// {1} - тип правого выражения;
        /// {2} - строка из класса Reader.
        /// </summary>
        const string TYPE_MISMATCH_MESSAGE = 
            "Несовпадение типов выражений: {0}:{1}, строка {2}";

        /// <summary>
        /// Сообщение о получении неверной лексемы вместо ожидаемой.
        /// string.Format:
        /// {0} - ожидаемая лексема;
        /// {1} - полученная лексема;
        /// {2} - строка из класса Reader.
        /// </summary>
        const string WRONG_LEXEM_MESSAGE = 
            "Ожидалась лексема {0}, получена лексема {1}, (строка {2})";

        /// <summary>
        /// Сообщение о получении неожиданной лексемы.
        /// string.Format:
        /// {0} - полученная лексема;
        /// {1} - строка из класса Reader.
        /// </summary>
        const string UNEXPECTED_LEXEM_MESSAGE =
            "Неожиданная лексема {0}, (строка {1})";

        /// <summary>
        /// Скомпилированный классом CodeGenerator код.
        /// Каждый раз при вызове собирается из отдельных строк.
        /// </summary>
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

        /// <summary>
        /// Проверка кода на соответствие синтаксическим правилам
        /// и параллельная генерация кода на языке Ассемблера.
        /// </summary>
        internal static void Compile()
        {
            CodeGenerator.DeclareDataSegment();
            DecodeVariableDeclaring();
            CodeGenerator.DeclareVariables();
            CodeGenerator.DeclareStackAndCodeSegments();
            CheckLexemAndProceed(Lexems.Separator);
            CheckLexemAndProceed(Lexems.Begin);
            DecodeInstructionSequence();
            CheckLexemAndProceed(Lexems.End);
            DecodePrintOperation();
            CodeGenerator.DeclareMainProcedureEnding();
        }

        /// <summary>
        /// Проверка лексемы на соответствие ожидаемой.
        /// Выдаёт синтаксическую ошибку в случае, если полученная
        /// лексема не соответствует ожидаемой.
        /// </summary>
        /// <param name="expectedLexem">ожидаемая лексема.</param>
        static void CheckLexem(Lexems expectedLexem)
        {
            if (LexicalAnalyzer.CurrentLexem != expectedLexem)
                Controller.Error(string.Format(WRONG_LEXEM_MESSAGE, 
                    expectedLexem, LexicalAnalyzer.CurrentLexem, Reader.RowIndex));
        }

        /// <summary>
        /// Проверка лексемы и разбор следующей.
        /// </summary>
        /// <param name="expectedLexem">ожидаемая лексема.</param>
        static void CheckLexemAndProceed(Lexems expectedLexem)
        {
            CheckLexem(expectedLexem);
            LexicalAnalyzer.DecodeNextLexem();
        }

        /// <summary>
        /// Разбор объявления типа переменных.
        /// </summary>
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

        /// <summary>
        /// Разбор последовательности инструкций. 
        /// </summary>
        static void DecodeInstructionSequence()
        {
            DecodeInstruction();
            while(LexicalAnalyzer.CurrentLexem == Lexems.Separator)
            {
                LexicalAnalyzer.DecodeNextLexem();
                DecodeInstruction();
            }
        }

        /// <summary>
        /// Разбор отдельной инструкции.
        /// </summary>
        static void DecodeInstruction()
        {
            switch (LexicalAnalyzer.CurrentLexem)
            {
                case (Lexems.Identifier):
                    AssignToVariable();
                    break;
                case (Lexems.If):
                    DecodeBranching();
                    break;
            }
        }

        static string currentLabel;

        /// <summary>
        /// Разбор оператора ветвления.
        /// </summary>
        static void DecodeBranching()
        {
            string elseLabel = CodeGenerator.GetNextLabel();
            currentLabel = string.Copy(elseLabel);
            DecodeLogicalBlock();

            string exitLabel = CodeGenerator.GetNextLabel();
            CodeGenerator.Jump(exitLabel);
            while (LexicalAnalyzer.CurrentLexem == Lexems.ElseIf)
            {
                CodeGenerator.PlaceLabel(elseLabel);
                elseLabel = CodeGenerator.GetNextLabel();
                currentLabel = string.Copy(elseLabel);
                DecodeLogicalBlock();
                CodeGenerator.Jump(exitLabel);
            }
            if (LexicalAnalyzer.CurrentLexem == Lexems.Else)
            {
                CodeGenerator.PlaceLabel(elseLabel);
                LexicalAnalyzer.DecodeNextLexem();
                DecodeInstructionSequence();
            }
            CheckLexemAndProceed(Lexems.EndIf);
            CodeGenerator.PlaceLabel(exitLabel);
        }

        /// <summary>
        /// Разобрать логическое выражение и последовательность
        /// инструкций, идущих после операторов If или ElseIf.
        /// </summary>
        static void DecodeLogicalBlock()
        {
            LexicalAnalyzer.DecodeNextLexem();
            Type t = DecodeExpression();
            if (t != Type.Boolean)
                Controller.Error(string.Format(
                    "Выражение имеет тип ({0})," + 
                    "ожидался тип Boolean, (строка {1})",
                    t, Reader.RowIndex));
            CheckLexemAndProceed(Lexems.Then);
            CodeGenerator.JumpIfFalse(currentLabel);
            DecodeInstructionSequence();
        }

        /// <summary>
        /// Присвоить значение переменной. 
        /// </summary>
        static void AssignToVariable()
        {
            Identifier id = NameTable.FindByName(
                LexicalAnalyzer.CurrentName);
            if (id != null)
            {
                Type t = DecodeAssigningOperation();
                if (id.Type == Type.Var)
                    id.ChangeType(t);
                if (id.Type == t)
                    CodeGenerator.AssignFromStack(id.Name);
                else
                    Controller.Error(string.Format(TYPE_MISMATCH_MESSAGE, 
                        id.Type, t, Reader.RowIndex));
            }
        }

        /// <summary>
        /// Разбор операции присваивания и присваиваемого выражения.
        /// </summary>
        static Type DecodeAssigningOperation()
        {
            LexicalAnalyzer.DecodeNextLexem();
            CheckLexemAndProceed(Lexems.Assignment);
            return DecodeExpression();
        }

        /// <summary>
        /// Разобрать выражение.
        /// </summary>
        /// <returns>тип выражения.</returns>
        private static Type DecodeExpression()
        {
            Type tLeft = DecodeAdditionOrSubtraction();
            switch (LexicalAnalyzer.CurrentLexem)
            {
                case (Lexems.Less):  
                case (Lexems.LessOrEqual):
                case (Lexems.More):
                case (Lexems.MoreOrEqual):
                case (Lexems.NotEqual):
                case (Lexems.Equal):
                    Lexems compareType = LexicalAnalyzer.CurrentLexem;
                    Type result = DecodeLogicalExpression(tLeft);
                    CodeGenerator.ConditionalJump(
                        compareType);
                    return result;

                default: return tLeft;
            }
        }

        private static Type DecodeLogicalExpression(Type tLeft)
        {
            LexicalAnalyzer.DecodeNextLexem();
            Type tRight = DecodeAdditionOrSubtraction();
            if (tLeft != tRight)
                Controller.Error(
                    string.Format(TYPE_MISMATCH_MESSAGE,
                    tLeft, tRight, Reader.RowIndex));
            return Type.Boolean;
        }

        /// <summary>
        /// Разобрать операцию сложения или вычитания.
        /// </summary>
        /// <returns>тип результата операции.</returns>
        static Type DecodeAdditionOrSubtraction()
        {
            Type tLeft, tRight;
            Lexems operation;
            if (LexicalAnalyzer.CurrentLexem == Lexems.Addition ||
                LexicalAnalyzer.CurrentLexem == Lexems.Subtraction)
            {
                operation = LexicalAnalyzer.CurrentLexem;
                LexicalAnalyzer.DecodeNextLexem();
                tLeft = DecodeMultiplicationOrDivision();
            }
            else
                tLeft = DecodeMultiplicationOrDivision();
            if (LexicalAnalyzer.CurrentLexem == Lexems.Addition ||
                LexicalAnalyzer.CurrentLexem == Lexems.Subtraction)
            {
                do
                {
                    operation = LexicalAnalyzer.CurrentLexem;
                    LexicalAnalyzer.DecodeNextLexem();
                    tRight = DecodeMultiplicationOrDivision();
                    if (tLeft != tRight)
                        Controller.Error(
                            string.Format(TYPE_MISMATCH_MESSAGE,
                            tLeft, tRight, Reader.RowIndex));
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
            return tLeft;
        }

        /// <summary>
        /// Разобрать операцию умножения или деления.
        /// </summary>
        /// <returns>тип результата операции.</returns>
        static Type DecodeMultiplicationOrDivision()
        {
            Lexems operation;
            Type tLeft = DecodeSubExpression(), 
                tRight;
            if (LexicalAnalyzer.CurrentLexem == Lexems.Multiplication || 
                LexicalAnalyzer.CurrentLexem == Lexems.Division)
            {
                do
                {
                    operation = LexicalAnalyzer.CurrentLexem;
                    LexicalAnalyzer.DecodeNextLexem();
                    tRight = DecodeSubExpression();
                    if (tLeft != tRight)
                        Controller.Error(
                            string.Format(TYPE_MISMATCH_MESSAGE,
                            tLeft, tRight, Reader.RowIndex));
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
            return tLeft;
        }

        /// <summary>
        /// Разобрать подвыражение (идентификатор, константа или 
        /// выражение в скобках).
        /// </summary>
        /// <returns>тип подвыражения.</returns>
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
                    if (id != null)
                        CodeGenerator.PushValue(id.Name);
                    return id.Type;
                case (Lexems.Number):
                    CodeGenerator.PushValue(currentName);
                    return Type.Integer;
                case (Lexems.BasicBoolean):
                    CodeGenerator.PushValue(
                        (currentName == "true") ? "1" : "0");
                    return Type.Boolean;
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

        /// <summary>
        /// Разобрать операцию печати.
        /// </summary>
        static void DecodePrintOperation()
        {
            LexicalAnalyzer.DecodeNextLexem();
            CheckLexemAndProceed(Lexems.Print);
            CheckLexem(Lexems.Identifier);
            Identifier id = NameTable.FindByName(LexicalAnalyzer.CurrentName);
            if (id != null)
                CodeGenerator.DeclarePrintOperation(id.Name);
        }
    }
}
