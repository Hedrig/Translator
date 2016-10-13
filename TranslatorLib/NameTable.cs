using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLib
{
    enum Category
    {
        Constant, Variable, Type
    }

    enum Type
    {
        None, Integer, Bool
    }

    struct Identifier
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

        public string Name { get { return name; } }

        internal Category Category { get { return category; } }

        internal Type Type { get { return type; } }
    }

    static class NameTable
    {
        static LinkedList<Identifier> identifiers;

        static public Identifier AddIdentifier(string name, Category category)
        {
            try { FindByName(name); }
            catch (IdentifierNotDefinedException)
            {
                var id = new Identifier(name, category);
                identifiers.AddLast(id);
                return id;
            }
            throw new IdentifierAlreadyDefinedException("Идентификатор с именем '" + name + "' уже существует");
        }

        static public Identifier FindByName(string name)
        {
            foreach (Identifier id in identifiers)
                if (id.Name.Equals(name)) return id;
            throw new IdentifierNotDefinedException("Обращение к несуществующему идентификатору '" + name + "'");
        }
    }
}
