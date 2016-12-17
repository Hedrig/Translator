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
            TranslatorLib.Controller.ErrorsOccurred += Controller_ErrorsOccurred;
        }

        private void Controller_ErrorsOccurred(object sender, TranslatorLib.ErrorOccurredEventArgs e)
        {
            compileRichTextBox.Text += e.Messages;
        }

        string currentFile;

        private void выполнитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TranslatorLib.Controller.Compile(currentFile);
            resultRichTextBox.Text = TranslatorLib.Controller.Code;
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
            if (currentFile.Length > 0)
                File.WriteAllText(currentFile, sourceRichTextBox.Text, Encoding.Default);
            else
                cохранитьКакToolStripMenuItem_Click(sender, e);
        }

        private void cохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
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

    }
}
