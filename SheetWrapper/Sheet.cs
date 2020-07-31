using OfficeOpenXml;
using System.Collections.Generic;
using System.Linq;

namespace SheetWrapper
{
    public class Sheet
    {
        private readonly ExcelWorksheet sheet;
        public CellAddress Start { get => new CellAddress(sheet.Dimension.Start); }
        public CellAddress End { get => new CellAddress(sheet.Dimension.End); }
        public string Name { get => sheet.Name; }

        public bool ShowGridLines {
            get => sheet.View.ShowGridLines;
            set => sheet.View.ShowGridLines = value;
        }
        public bool IsColNullOrEmpty(int col)
        {
            bool nullOrEmty = true;

            foreach (var c in sheet.Cells[Start.Row, col, End.Row, col])
            {
                nullOrEmty = string.IsNullOrEmpty(c.Text);
                if (!nullOrEmty)
                    break;
            }
            return nullOrEmty;
        }
        public bool IsRowNullOrEmpty(int row)
        {
            bool nullOrEmty = true;

            foreach (var c in sheet.Cells[row, Start.Col, row, End.Col])
            {
                nullOrEmty = string.IsNullOrEmpty(c.Text);
                if (!nullOrEmty)
                    break;
            }
            return nullOrEmty;
        }

        public int GetColSpan(int row, int col)
        {
            var mc = sheet.MergedCells[row, col];
            return mc != null ? sheet.SelectedRange[mc].Columns : 1;
        }
        public int GetRowSpan(int row, int col)
        {
            var mc = sheet.MergedCells[row, col];
            return mc != null ? sheet.SelectedRange[mc].Rows : 1;
        }

        public Row GetRow(int row)
        {
            return new Row(sheet.Row(row));
        }
        public Column GetCol(int col)
        {
            return new Column(sheet.Column(col));
        }
        public CellRange this[string rangeName]
        {
            get
            {
                var namesWorkbook = sheet.Workbook.Names;
                var namesSheet = sheet.Names;
                bool nameInW = namesWorkbook.ContainsKey(rangeName);
                bool nameInS = namesSheet.ContainsKey(rangeName);
                if (!nameInW && !nameInS)
                    throw new SpreadSheetException($"The name {rangeName} is not defined in the workbook.");
                var range = nameInW ? namesWorkbook[rangeName] : namesSheet[rangeName];
                return new CellRange(range);
            }
        }
        public CellRange this[CellAddress cell]
        {
            get
            {
                return new CellRange(sheet.Cells[cell.ToString()]);
            }
        }
        public CellRange this[int row, int col]
        {
            get
            {
                return new CellRange(sheet.Cells[row, col]);
            }
        }

        public void AddHorizontal(IEnumerable<object> data, CellAddress adress, string namedStyle = null)
        {
            var horizontalRange = sheet.Cells[adress.Row, adress.Col, adress.Row, adress.Row + data.Count()];
            if (namedStyle != null && sheet.Workbook.Styles.NamedStyles.FirstOrDefault(s => s.Name == namedStyle) != default)
                horizontalRange.StyleName = namedStyle;
            horizontalRange.LoadFromCollection(data);
        }

        internal Sheet(ExcelWorksheet sheet)
        {
            this.sheet = sheet;
        }
    }
}
