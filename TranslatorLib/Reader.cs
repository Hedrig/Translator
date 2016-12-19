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
            if (streamReader != null) streamReader.Close();
            streamReader = new StreamReader(filename);
            rowIndex = 1;
            columnIndex = 1;
            currentSymbol = streamReader.Read();
        }
        public static void ReadNextSymbol()
        {
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
            currentSymbol = streamReader.Read();
        }
        public static void Close()
        {
            streamReader.Close();
        }
    }
}
