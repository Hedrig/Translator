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
        private static int labelCount = 1;

        internal static List<string> Code
        {
            get
            {
                return new List<string>(code);
            }
        }

        /// <summary>
        /// Стирает сгенерированный ранее код.
        /// </summary>
        public static void Initialize()
        {
            code = new List<string>();
        }

        /// <summary>
        /// Добавляет инструкцию-строку в код.
        /// </summary>
        /// <param name="instruction">строка для добавления.</param>
        static void AddInstruction(string instruction)
        {
            code.Add(instruction);
        }
        
        /// <summary>
        /// Добавляет несколько инструкций подряд в код, 
        /// в порядке сверху вниз.
        /// </summary>
        /// <param name="instructions">набор инструкций.</param>
        static void AddSetOfInstructions(params string[] instructions)
        {
            foreach(string line in instructions)
                code.Add(line);
        }

        /// <summary>
        /// Объявляет начало кода, задаёт стандартные функции, 
        /// константы и соглашения, а также объявляет сегмент данных.
        /// </summary>
        public static void DeclareDataSegment()
        {
            AddSetOfInstructions(".386",
                                 ".model flat, stdcall",
                                 "option casemap :none",
                                 "STD_OUTPUT_HANDLE equ -11",
                                 "GetStdHandle PROTO, nStdHandle: DWORD",
                                 "WriteConsoleA PROTO, handle: DWORD, lpBuffer: PTR BYTE, nNumberOfBytesToWrite: DWORD, lpNumberOfBytesWritten: PTR DWORD, lpReserved: DWORD",
                                 "ExitProcess PROTO, dwExitCode: DWORD",
                                 ".data",
                                 "consoleOutHandle dd ?",
                                 "bytesWritten dd ?",
                                 "buffer db 6 DUP (0)",
                                 "bufferSize dw 6");
        }

        /// <summary>
        /// Объявляет сегменты стека и кода.
        /// </summary>
        public static void DeclareStackAndCodeSegments()
        {
            AddSetOfInstructions(".stack 512",
                                 ".code",
                                 "start:");
        }

        /// <summary>
        /// Описывает выход из процедуры.
        /// </summary>
        public static void DeclareMainProcedureEnding()
        {
            AddSetOfInstructions("INVOKE ExitProcess, 0",
                                 "end start");
        }

        /// <summary>
        /// Объявляет переменные в сегменте данных.
        /// </summary>
        public static void DeclareVariables()
        {
            foreach(Identifier id in NameTable.Identifiers)
                AddInstruction(id.Name + " dw 1");
        }

        /// <summary>
        /// Записывает в переменную верхнее значение стека.
        /// </summary>
        /// <param name="name">название переменной.</param>
        public static void AssignFromStack(string name)
        {
            AddSetOfInstructions("pop ax",
                                 "mov " + name + ", ax");
        }

        /// <summary>
        /// Складывает два верхних значения стека.
        /// Результат помещается в стек.
        /// </summary>
        public static void AddTwoStackValues()
        {
            AddSetOfInstructions("pop bx",
                                 "pop ax",
                                 "add ax, bx",
                                 "push ax");
        }

        /// <summary>
        /// Вычитает второе значение стека сверху из первого.
        /// Результат помещается в стек.
        /// </summary>
        public static void SubTwoStackValues()
        {
            AddSetOfInstructions("pop bx",
                                 "pop ax",
                                 "sub ax, bx",
                                 "push ax");
        }

        /// <summary>
        /// Перемножает два верхних значения стека.
        /// Результат помещается в стек.
        /// </summary>
        public static void MultiplyTwoStackValues()
        {
            AddSetOfInstructions("pop bx",
                                 "pop ax",
                                 "mul bx",
                                 "push ax");
        }

        /// <summary>
        /// Делит первое верхнее значение стека на второе нацело.
        /// Результат помещается в стек.
        /// </summary>
        public static void DivideSecondStackValueByFirst()
        {
            AddSetOfInstructions("pop bx",
                                 "pop ax",
                                 "cwd",
                                 "div bl",
                                 "push ax");
        }

        /// <summary>
        /// Помещает значение переменной в стек.
        /// </summary>
        /// <param name="name">имя переменной.</param>
        public static void PushValue(string name)
        {
            AddSetOfInstructions("mov ax, " + name,
                                 "push ax");
        }

        /// <summary>
        /// Описывает операцию вывода переменной на экран консоли.
        /// </summary>
        /// <param name="idName">имя переменной.</param>
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

        /// <summary>
        /// Возвращает следующую метку и увеличивает 
        /// счётчик меток на единицу.
        /// </summary>
        /// <returns>название метки в формате label'число'</returns>
        public static string GetNextLabel()
        {
            return "label" + labelCount++;
        }

        /// <summary>
        /// Добавляет операцию перемещения к метке, если
        /// верхнее значение стека равно 0(False).
        /// </summary>
        /// <param name="label">метка для перемещения.</param>
        internal static void JumpIfFalse(string label)
        {
            AddSetOfInstructions("pop ax",
                                 "cmp ax, 0",
                                 "je " + label);
        }

        /// <summary>
        /// Выполняет над двумя верхними стековыми значениями 
        /// выбранную операцию сравнения и в зависимости от результата 
        /// помещает в стек 1(True) или 0(False).
        /// </summary>
        /// <param name="compareType">операция сравнения.</param>
        internal static void Compare(Lexems compareType)
        {
            string jumpType;
            switch (compareType)
            {
                case (Lexems.Less):        jumpType = "jl";  break;
                case (Lexems.LessOrEqual): jumpType = "jle"; break;
                case (Lexems.More):        jumpType = "jg";  break;
                case (Lexems.MoreOrEqual): jumpType = "jge"; break;
                case (Lexems.NotEqual):    jumpType = "jne"; break;
                case (Lexems.Equal):       jumpType = "je";  break;
                default:                   jumpType = "";    break;
            }
            string elseLabel = GetNextLabel(), exitLabel = GetNextLabel();
            AddSetOfInstructions("pop ax",
                                 "pop bx",
                                 "cmp bx, ax",
                                 jumpType + " " + elseLabel,
                                 "push 0",
                                 "jmp " + exitLabel,
                                 elseLabel + ":",
                                 "push 1",
                                 exitLabel + ":");
        }

        /// <summary>
        /// Устанавливает выбранную метку.
        /// </summary>
        /// <param name="label">метка.</param>
        internal static void PlaceLabel(string label)
        {
            AddInstruction(label + ":");
        }

        /// <summary>
        /// Добавляет операцию безусловного перемещения к выбранной метке. 
        /// </summary>
        /// <param name="label">метка.</param>
        internal static void Jump(string label)
        {
            AddInstruction("jmp " + label);
        }
    }
}
