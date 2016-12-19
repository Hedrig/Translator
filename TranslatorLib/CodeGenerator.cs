using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLib
{
    static class CodeGenerator
    {
        private static List<string> code;

        internal static List<string> Code
        {
            get
            {
                return new List<string>(code);
            }
        }

        public static void Initialize()
        {
            code = new List<string>();
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
                                 "STD_OUTPUT_HANDLE equ -11",
                                 "GetStdHandle PROTO, nStdHandle: DWORD",
                                 "WriteConsoleA PROTO, handle: DWORD, lpBuffer: PTR BYTE, nNumberOfBytesToWrite: DWORD, lpNumberOfBytesWritten: PTR DWORD, lpReserved: DWORD",
                                 "ExitProcess PROTO, dwExitCode: DWORD",
                                 ".stack 512",
                                 ".data",
                                 "consoleOutHandle dd ?",
                                 "bytesWritten dd ?",
                                 "buffer db 6 DUP (0)",
                                 "bufferSize dw 6");
        }

        public static void DeclareStackAndCodeSegments()
        {
            AddSetOfInstructions(".code",
                                 "start:");
        }

        public static void DeclareMainProcedureEnding()
        {
            AddSetOfInstructions("INVOKE ExitProcess, 0",
                                 "end start");
        }

        public static void DeclareVariables()
        {
            foreach(Identifier id in NameTable.Identifiers)
                AddInstruction(id.Name + " dw 1");
        }

        public static void AssignFromStack(string name)
        {
            AddSetOfInstructions("pop ax",
                                 "mov " + name + ", ax");
        }

        public static void AddTwoStackValues()
        {
            AddSetOfInstructions("pop bx",
                                 "pop ax",
                                 "add ax, bx",
                                 "push ax");
        }

        public static void SubTwoStackValues()
        {
            AddSetOfInstructions("pop bx",
                                 "pop ax",
                                 "sub ax, bx",
                                 "push ax");
        }

        public static void MultiplyTwoStackValues()
        {
            AddSetOfInstructions("pop bx",
                                 "pop ax",
                                 "mul bx",
                                 "push ax");
        }

        public static void DivideSecondStackValueByFirst()
        {
            AddSetOfInstructions("pop bx",
                                 "pop ax",
                                 "cwd",
                                 "div bl",
                                 "push ax");
        }

        public static void PushValue(string name)
        {
            AddSetOfInstructions("mov ax, " + name,
                                 "push ax");
        }

        public static void DeclarePrintOperation(string idName)
        {
            AddSetOfInstructions("xor eax, eax",
                                 "mov ax, " + idName,
                                 "xor ebx, ebx",
                                 "mov bx, bufferSize",
                                 "mov cx, 10",
                                 "divide:",
                                 "cdq",
                                 "div cx",
                                 "add dl, 48",
                                 "mov byte ptr[ebx+buffer-1], dl",
                                 "dec bx",
                                 "cmp ax, 0",
                                 "jne divide",
                                 "pushad",
                                 "INVOKE GetStdHandle, STD_OUTPUT_HANDLE",
                                 "mov ebx, offset buffer",
                                 "xor ecx, ecx",
                                 "mov cx, bufferSize",
                                 "buffershrink:",
                                 "cmp byte ptr[ebx], 0",
                                 "jne print",
                                 "inc bx",
                                 "dec cx",
                                 "jmp buffershrink",
                                 "print:",
                                 "INVOKE WriteConsoleA, eax, " +
                                 "ebx, ecx, offset bytesWritten, 0",
                                 "popad");
        }
    }
}
