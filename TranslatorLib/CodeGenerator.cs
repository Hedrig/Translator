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
        private static int codePointer = 0;

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
            AddInstruction("data segment para public \"data\"");
        }

        public static void DeclareStackAndCodeSegments()
        {
            AddSetOfInstructions("PRINT_BUF DB ' ' DUP(10)",
                                 "BUFEND    DB '$'",
                                 "data ends",
                                 "stk segment stack",
                                 "db 256 dup (\"?\")",
                                 "stk ends",
                                 "code segment para public \"code\"",
                                 "main proc",
                                 "assume cs:code,ds:data,ss:stk",
                                 "mov ax,data",
                                 "mov ds,ax");
        }

        public static void DeclareMainProcedureEnding()
        {
            AddSetOfInstructions("mov ax,4c00h",
                                 "int 21h",
                                 "main endp");
        }

        public static void DeclareCodeEnding()
        {
            AddSetOfInstructions("code ends", 
                                 "end main");
        }

        public static void DeclareVariables()
        {
            LinkedListNode<Identifier> variable = NameTable.Identifiers.First;
            do
            {
                AddInstruction(variable.Value.Name + " dw 1");
            }
            while ((variable = variable.Next) != null);
        }
    }
}
