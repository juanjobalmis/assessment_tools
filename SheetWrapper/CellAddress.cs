using OfficeOpenXml;
using System;
using System.Text.RegularExpressions;

namespace SheetWrapper
{
    public class CellAddress : ICloneable
    {
        private readonly ExcelCellAddress a;

        public int Row { get { return a.Row; } }
        public int Col { get { return a.Column; } }
        public string ColName 
        { 
            get 
            {
                return Regex.Match(this.ToString(), @"$[A-Z]+").Value;
            } 
        }
        internal CellAddress(ExcelCellAddress a)
        {
            this.a = a;
        }

        public CellAddress(int row, int column)
        {
            a = new ExcelCellAddress(row, column);
        }

        public CellAddress(string cell)
        {
            a = new ExcelCellAddress(cell);
        }

        public override string ToString()
        {
            return $"{a.Address}";
        }

        public object Clone()
        {
            return new CellAddress(a.Row, a.Column);
        }
    }
}