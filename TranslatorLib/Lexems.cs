using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLib
{
    public enum Lexems
    {
        None, Number, Identifier, Type,
        Addition, Subtraction, Multiplication, Division, Assignment,
        OpenBracket, CloseBracket, Separator,
        Equal, More, Less, MoreOrEqual, LessOrEqual, NotEqual,
        Begin, End, EOF, DeclareSeparator, Print,
        If, Then, ElseIf, Else, EndIf,
        Conjunction, Negation, Disjunction, BasicBoolean,
        Case, Of, EndCase, StatementSeparator
    }
}
