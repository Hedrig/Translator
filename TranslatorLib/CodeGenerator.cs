using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLib
{
    static class CodeGenerator
    {
        private static List<string> code = new List<string>();

        internal static List<string> Code
        {
            get
            {
                return new List<string>(code);
            }
        }

        static void AddInstruction(string instruction)
        {
            code.Add(instruction);
        }
        
        static void AddSetOfInstructions(params string[] instructions)
        {
            foreach(string line in instructions)
                code.Add(line);
        }

        public static void DeclareDataSegment()
        {
            AddSetOfInstructions(".386",
                                 ".model flat, stdcall",
                                 "option casemap :none",
                                 ".data");
        }

        public static void DeclareStackAndCodeSegments()
        {
            AddSetOfInstructions(".code",
                                 "start:");
        }

        public static void DeclareMainProcedureEnding()
        {
            AddSetOfInstructions("ret",
                                 "end start");
        }

        public static void DeclareVariables()
        {
            foreach(Identifier id in NameTable.Identifiers)
                AddInstruction(id.Name + " dw 1");
        }
    }
}
