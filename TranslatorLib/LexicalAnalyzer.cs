using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLib
{
    static class LexicalAnalyzer
    {
        static List<Keyword> keywords = new List<Keyword>();
        static Lexems currentLexem;
        static string currentName;

        static LexicalAnalyzer()
        {
            AddKeyword("Begin", Lexems.Begin);
            AddKeyword("End", Lexems.End);
            AddKeyword("Print", Lexems.Print);
            AddKeyword("If", Lexems.If);
            AddKeyword("Then", Lexems.Then);
            AddKeyword("ElseIf", Lexems.ElseIf);
            AddKeyword("Else", Lexems.Else);
            AddKeyword("EndIf", Lexems.EndIf);
            AddKeyword("true", Lexems.BasicBoolean);
            AddKeyword("false", Lexems.BasicBoolean);
            AddKeyword("Case", Lexems.Case);
            AddKeyword("Of", Lexems.Of);
            AddKeyword("EndCase", Lexems.EndCase);
        }

        /// <summary>
        /// Текущая лексема. Значение по умолчанию - Lexem.None.
        /// Перезаписывается после вызова метода DecodeNextLexem.
        /// </summary>
        public static Lexems CurrentLexem { get { return currentLexem; } }

        /// <summary>
        /// Текущее имя, соответствующее лексеме. 
        /// Значение по умолчанию - null.
        /// Перезаписывается после вызова метода DecodeNextLexem.
        /// </summary>
        public static string CurrentName { get { return currentName; } }

        /// <summary>
        /// Добавляет ключевое слово и соответствующую лексему 
        /// в таблицу ключевых слов.
        /// </summary>
        /// <param name="word">ключевое слово.</param>
        /// <param name="lexem">лексема.</param>
        static void AddKeyword(string word, Lexems lexem)
        {
            keywords.Add(new Keyword(word, lexem));
        }

        /// <summary>
        /// Получает лексему, соответствующую слову.
        /// </summary>
        /// <param name="word">слово.</param>
        /// <returns>лексема, соответствующая ключевому слову,
        /// или Lexem.Identifier.</returns>
        static Lexems GetKeyword(string word)
        {
            foreach (Keyword keyword in keywords)
                if (keyword.word.Equals(word)) return keyword.lexem;
            // Попытка найти тип с данным именем в списке типов, 
            // в случае неудачи лексема считается переменной
            try
            {
                Enum.Parse(typeof(Type), word);
                return Lexems.Type;
            }
            catch (ArgumentException)
            {
                return Lexems.Identifier;
            }
        }

        /// <summary>
        /// Разобрать следующую лексему. Сохраняет строку и соответствующую
        /// ей лексему.
        /// </summary>
        public static void DecodeNextLexem()
        {
            currentLexem = Lexems.None;
            currentName = null;
            while (Reader.CurrentSymbol == ' ') Reader.ReadNextSymbol();

            if (char.IsLetter((char)Reader.CurrentSymbol)) DecodeIdentifier();
            else
                if (char.IsDigit((char)Reader.CurrentSymbol)) DecodeNumber();
            else
            {
                DecodeSymbol(Reader.CurrentSymbol);
            }
        }

        /// <summary>
        /// Разобрать символ и получить соответствующую лексему.
        /// </summary>
        /// <param name="symbol">код символа.</param>
        static void DecodeSymbol(int symbol)
        {
            Reader.ReadNextSymbol();
            switch (symbol)
            {
                case (','): { currentLexem = Lexems.DeclareSeparator; break; }
                case ('\n'): { currentLexem = Lexems.Separator; break; }
                case ('='):
                    {
                        if (Reader.CurrentSymbol == '=')
                        {
                            currentLexem = Lexems.Equal;
                            Reader.ReadNextSymbol();
                        }
                        else currentLexem = Lexems.Assignment; break;
                    }
                case ('>'):
                    {
                        if (Reader.CurrentSymbol == '=')
                        {
                            currentLexem = Lexems.MoreOrEqual;
                            Reader.ReadNextSymbol();
                        }
                        else currentLexem = Lexems.More;
                        break;
                    }
                case ('<'):
                    {
                        if (Reader.CurrentSymbol == '=')
                        {
                            currentLexem = Lexems.LessOrEqual;
                            Reader.ReadNextSymbol();
                        }
                        else currentLexem = Lexems.Less;
                        break;
                    }
                case ('+'): currentLexem = Lexems.Addition;           break;
                case ('-'): currentLexem = Lexems.Subtraction;        break;
                case ('*'): currentLexem = Lexems.Multiplication;     break;
                case ('/'): currentLexem = Lexems.Division;           break;
                case ('('): currentLexem = Lexems.OpenBracket;        break;
                case (')'): currentLexem = Lexems.CloseBracket;       break;
                case (':'): currentLexem = Lexems.StatementSeparator; break;
                case (-1):  currentLexem = Lexems.EOF;                break;
                default:
                    {
                        Controller.Error(
                        "Символ не распознан, строка " + Reader.RowIndex + 
                        ", символ " + Reader.ColumnIndex);
                        break;
                    }
            }
        }

        /// <summary>
        /// Разобрать число, состоящее из отдельных цифр.
        /// </summary>
        static void DecodeNumber()
        {
            StringBuilder number = new StringBuilder();
            do
            {
                number.Append((char)Reader.CurrentSymbol);
                Reader.ReadNextSymbol();
            }
            while (char.IsDigit((char)Reader.CurrentSymbol));
            currentLexem = Lexems.Number;
            currentName = number.ToString();
        }

        /// <summary>
        /// Разобрать идентификатор. Имя идентификатора должно начинаться
        /// с буквы.
        /// </summary>
        private static void DecodeIdentifier()
        {
            StringBuilder word = new StringBuilder();
            do
            {
                word.Append((char)Reader.CurrentSymbol);
                Reader.ReadNextSymbol();
            }
            while (char.IsLetterOrDigit((char)Reader.CurrentSymbol));
            currentName = word.ToString();
            currentLexem = GetKeyword(currentName);
        }

        /// <summary>
        /// Сбрасывает значения всех полей и обновляет список ключевых слов.
        /// </summary>
        public static void Initialize()
        {
            currentLexem = Lexems.None;
            currentName = null;
            DecodeNextLexem();
        }
    }
}
