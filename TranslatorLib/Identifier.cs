using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLib
{
    class Identifier
    {
        string name;
        Category category;
        Type type;

        public Identifier(string name, Category category)
        {
            this.name = name;
            this.category = category;
            type = Type.None;
        }

        public Identifier(string name, Category category, Type type)
        {
            this.name = name;
            this.type = type;
            this.category = category;
        }

        /// <summary>
        /// Заменяет тип Var на переданный.
        /// </summary>
        /// <param name="newType">новый тип.</param>
        public void ChangeType(Type newType)
        {
            if (type == Type.Var) type = newType;
        }

        public string Name { get { return name; } }

        internal Category Category { get { return category; } }

        internal Type Type { get { return type; } }
    }
}
