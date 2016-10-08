using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLib
{
    public struct Keyword
    {
        public Keyword(string word, Lexems lexem)
        {
            this.word = string.Copy(word); this.lexem = lexem;
        }
        public string word;
        public Lexems lexem;
    }
}
