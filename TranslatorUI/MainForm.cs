using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TranslatorUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            TranslatorLib.Controller.CodeCompiled += Controller_CodeCompiled;
        }

        private void Controller_CodeCompiled(object sender, EventArgs e)
        {
            resultRichTextBox.Text = TranslatorLib.Controller.Code;
            compileRichTextBox.Text = TranslatorLib.Controller.Output;
        }

        string currentFile;

        private void выполнитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Несохранённый код нельзя компилировать
            if (currentFile != null)
                TranslatorLib.Controller.Compile(currentFile);
            else
            {
                сохранитьКакToolStripMenuItem_Click(sender, e);
                if (currentFile != null)
                    TranslatorLib.Controller.Compile(currentFile);
            }
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Source Code File| *.src";
            openFileDialog.ShowDialog(this);
            if (openFileDialog.FileName != "")
            {
                // Прочитать файл, скопировать его содержимое в textBox
                currentFile = openFileDialog.FileName;
                sourceRichTextBox.Text = File.ReadAllText(currentFile, Encoding.Default);
            }
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Сохраняем код в текущий файл
            if (currentFile != null)
                File.WriteAllText(currentFile, sourceRichTextBox.Text, Encoding.Default);
            else
                // Если код новый, запускаем форму сохранения
                сохранитьКакToolStripMenuItem_Click(sender, e);
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Source Code File| *.src";
            saveFileDialog.ShowDialog(this);
            if (saveFileDialog.FileName != "")
            {
                // Прочитать содержимое textBox, сохранить его в файл
                currentFile = saveFileDialog.FileName;
                File.WriteAllText(currentFile, sourceRichTextBox.Text, Encoding.Default);
            }
        }

        private void выйтиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void новыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentFile = null;
            compileRichTextBox.Text = "";
            sourceRichTextBox.Text = "";
            resultRichTextBox.Text = "";
        }
    }
}
