using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace TranslatorLib
{
    static class Reader
    {
        public static int CurrentSymbol
        {
            get
            {
                return currentSymbol;
            }
        }

        public static int RowIndex
        {
            get
            {
                return rowIndex;
            }
        }

        public static int ColumnIndex
        {
            get
            {
                return columnIndex;
            }
        }

        static int currentSymbol;
        static int rowIndex;
        static int columnIndex;
        static StreamReader streamReader;

        public static void Initialize(string filename)
        {
            streamReader.Close();
            streamReader = new StreamReader(filename);
            rowIndex = 1;
            columnIndex = 1;
        }
        public static void ReadNextSymbol()
        {
            currentSymbol = streamReader.Read();
            switch (currentSymbol)
            {
                case ('\0'):
                    {
                        break;
                    }
                case ('\n'):
                    {
                        rowIndex++;
                        columnIndex = 1;
                        break;
                    }
                case ('\t'):
                case ('\r'):
                    {
                        ReadNextSymbol();
                        break;
                    }
                default:
                    {
                        columnIndex++; break;
                    }
            }
        }
        public static void Close()
        {
            streamReader.Close();
        }
    }
}
