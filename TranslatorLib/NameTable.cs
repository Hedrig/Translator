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
        None, Integer, Boolean, Var
    }

    static class NameTable
    {
        static List<Identifier> identifiers = new List<Identifier>();

        static public List<Identifier> Identifiers
        {
            get { return new List<Identifier>(identifiers); }
        }

        public static void Initialize()
        {
            identifiers = new List<Identifier>();
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

            if (identifier == null)
                if (name != null)
                    Controller.Error(
                        "Обращение к несуществующему идентификатору '" +
                        name + "'");
            return identifier;
        }
    }
}
