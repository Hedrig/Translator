﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLib
{
    public enum Lexems
    {
        None, Name, Number,
        Addition, Subtraction, Multiplication, Division, Assignment,
        OpenBracket, CloseBracket, Separator,
        Equal, More, Less, MoreOrEqual, LessOrEqual, NotEqual
    }
}