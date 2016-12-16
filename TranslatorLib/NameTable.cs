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
        static List<Identifier> identifiers;

        static public List<Identifier> Identifiers
        {
            get { return new List<Identifier>(identifiers); }
        }

        static public void AddIdentifier(string name, 
                                         Category category, Type type)
        {
            if (!identifiers.Exists(id => id.Name == name))
                identifiers.Add(new Identifier(name, category, type));
            else
                Controller.Error(
                    "Идентификатор с именем '" + name + "' уже существует");
        }

        static public Identifier FindByName(string name)
        {
            Identifier identifier = identifiers.Find(id => id.Name == name);

            if (identifier.Name == null)
                Controller.Error(
                    "Обращение к несуществующему идентификатору '" +
                    name + "'");
            return identifier;
        }

        private static void Error(string errorMessage)
        {
            Controller.Error(errorMessage);
        }
    }
}
