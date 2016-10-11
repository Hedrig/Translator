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

        public static Lexems CurrentLexem { get { return currentLexem; } }

        public static string CurrentName { get { return currentName; } }

        static void AddKeyword(string word, Lexems lexem)
        {
            keywords.Add(new Keyword(word, lexem));
        }

        static Lexems GetKeyword(string word)
        {
            foreach (Keyword keyword in keywords)
                if (keyword.word.Equals(word)) return keyword.lexem;
            return Lexems.Name;
        }

        public static void DecodeLexem()
        {
            while (Reader.CurrentSymbol == ' ') Reader.ReadNextSymbol();

            if (char.IsLetter((char)Reader.CurrentSymbol)) DecodeIdentifier();
            else
                if (char.IsDigit((char)Reader.CurrentSymbol)) DecodeNumber();
            else
            {
                DecodeSymbol(Reader.CurrentSymbol);
            }
        }

        static void DecodeSymbol(int symbol)
        {
            Reader.ReadNextSymbol();
            switch (Reader.CurrentSymbol)
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
                case ('+'):
                    {
                        currentLexem = Lexems.Addition; break;
                    }
                case ('-'):
                    {
                        currentLexem = Lexems.Subtraction; break;
                    }
                case ('*'):
                    {
                        currentLexem = Lexems.Multiplication; break;
                    }
                case ('/'):
                    {
                        currentLexem = Lexems.Division; break;
                    }
                case ('('):
                    {
                        currentLexem = Lexems.OpenBracket; break;
                    }
                case (')'):
                    {
                        currentLexem = Lexems.CloseBracket; break;
                    }
                default:
                    throw new UnidentifiedSymbolException(
                        "Символ не распознан, строка " + Reader.RowIndex + ", символ " + Reader.ColumnIndex);
            }
        }

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
            number.ToString();
        }

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
        public static void Initialize()
        {
            currentLexem = Lexems.None;
            keywords = new List<Keyword>();
        }
    }
}
