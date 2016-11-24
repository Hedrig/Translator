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

        static void DeclareDataSegment()
        {
            AddInstruction("data segment para public \"data\"");
        }

        static void DeclareStackAndCodeSegments()
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

        static void MainProcedureEnding()
        {
            AddSetOfInstructions("mov ax,4c00h",
                                 "int 21h",
                                 "main endp");
        }

        static void CodeEnding()
        {
            AddSetOfInstructions("code ends", 
                                 "end main");
        }

        static void DeclareVariables()
        {
            
        }
    }
}
