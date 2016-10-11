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

    class NameTable
    {
        LinkedList<Identifier> identifiers;
        
        public Identifier AddIdentifier(string name, Category category, Type type)
        {
            try { FindByName(name); }
            catch(IdentifierNotDefinedException)
            {
                var id = new Identifier(name, category, type);
                identifiers.AddLast(id);
                return id;
            }
            throw new IdentifierAlreadyDefinedException();
        }

        public Identifier FindByName(string name)
        {
            foreach (Identifier id in identifiers)
                if (id.Name.Equals(name)) return id;
            throw new IdentifierNotDefinedException();
        }
    }
}
