using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBuilder
{
    /// <summary>
    /// Класс, предназначенный для сборки *.exe файлов из исходного кода.
    /// </summary>
    public static class Builder
    {

        private static StringBuilder output = new StringBuilder();

        /// <summary>
        /// Вывод компилятора и линковщика после завершения работы.
        /// Пуст до вызова метода BuildAll.
        /// </summary>
        public static string Output
        {
            get
            {
                return output.ToString();
            }
        }

        /// <summary>
        /// Строит *.exe файл из исходников в строке.
        /// </summary>
        /// <param name="fileName">путь к файлу исходников.</param>
        /// <param name="code">код для сборки.</param>
        public static void BuildAll(string fileName, string code)
        {
            // Сохранить скомпилированный код в отдельный файл
            string asmFileName = Path.GetFileNameWithoutExtension(fileName) +
                ".asm";
            File.WriteAllText(asmFileName, code);

            Assemble(asmFileName);
            Link(Path.ChangeExtension(asmFileName, "obj"));

            // В папке, содержащей исходный код, создать папку Compiled
            Directory.CreateDirectory(Path.Combine(
                Path.GetDirectoryName(fileName),
                "Compiled"));
            // Переместить скомпилированный exe-файл в папку Compiled
            string exeFileName = Path.ChangeExtension(asmFileName, "exe");
            string exeFilePath = Path.Combine(
                    Path.GetDirectoryName(fileName), "Compiled",
                    exeFileName);
            if (File.Exists(exeFilePath)) File.Delete(exeFilePath);
            if (File.Exists(exeFileName)) File.Move(exeFileName, exeFilePath);
        }

        /// <summary>
        /// Вызывает файл ml.exe, передавая ему в качестве параметра путь к
        /// собираемому файлу.
        /// </summary>
        /// <param name="filename">путь к *.asm файлу для сборки.</param>
        static void Assemble(string filename)
        {
            Process ml = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ml.exe",
                    Arguments = "/c /coff " + filename,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            ml.Start();
            output.Append(ml.StandardOutput.ReadToEnd());
            File.Delete(filename);
        }

        /// <summary>
        /// Вызывает файл link.exe, передавая ему в качестве параметров путь 
        /// к собираемому файлу.
        /// </summary>
        /// <param name="filename">путь к *.obj файлу для линковки.</param>
        static void Link(string filename)
        {
            Process link = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "link.exe",
                    Arguments = "/SUBSYSTEM:CONSOLE /OPT:NOREF /defaultlib:kernel32.lib " + filename,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            link.Start();
            output.Append(link.StandardOutput.ReadToEnd());
            File.Delete(filename);
        }
    }
}
