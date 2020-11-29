using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using OfficeOpenXml;
using OfficeOpenXml.Style.XmlAccess;

namespace SheetWrapper
{
    public class CellRange : IDisposable, IEnumerable<CellRange>
    {
        private readonly ExcelRangeBase cells;

        private CellRange(ExcelRangeBase cells)
        {
            this.cells = cells;
        }
        internal CellRange(ExcelRange cells)
        {
            this.cells = cells;
        }
        internal CellRange(ExcelNamedRange cells)
        {
            this.cells = cells;
        }
        private void LoadData<T>(IEnumerable<T> data, string namedStyle, bool verticalLoad)
        {
            CellAddress a = Address;
            int i = verticalLoad ? a.Row : a.Col;

            if (namedStyle != null) // ????
            {
                IEnumerable<ExcelNamedStyleXml> namedStyles = cells.Worksheet.Workbook.Styles.NamedStyles.Where(s => s.Name == namedStyle);
                if (!namedStyles.Any())
                    throw new ArgumentException($"Named style {namedStyle} is not defined.");
                foreach (var d in data)
                {
                    var c = verticalLoad ? cells.Worksheet.Cells[i++, a.Col] : cells.Worksheet.Cells[a.Row, i++];
                    c.Value = d;
                    c.StyleName = namedStyle;
                    c.AutoFitColumns();
                }
            }
        }
        public void LoadDataVertically<T>(IEnumerable<T> data, string namedStyle)
        {
            LoadData(data, namedStyle, true);
        }
        public void LoadDataHorizontally<T>(IEnumerable<T> data, string namedStyle)
        {
            LoadData(data, namedStyle, false);
        }
        public void LoadSingleData<T>(T data, string namedStyle)
        {
            LoadData(new T[] { data }, namedStyle, false);
        }

        public bool IsMerged
        {
            get { return cells.Merge; }
        }
        public bool HasContent
        {
            get { return !string.IsNullOrEmpty(Text); }
        }
        public CellAddress Address
        {
            get { return new CellAddress(cells.Address); }
        }
        public CellStyle Style
        {
            get
            {
                return new CellStyle(cells.Style);
            }
        }
        public string Text
        {
            get { return cells.Text; }
            set { cells.Value = value; }
        }

        public void Dispose()
        {
            cells.Dispose();
        }

        public override string ToString()
        {
            return $"{cells.Address.ToString()} = {Text}";
        }

        public IEnumerator<CellRange> GetEnumerator()
        {
            return new Enumerador(cells.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerador(cells.GetEnumerator());
        }

        private class Enumerador : IEnumerator<CellRange>
        {
            private readonly IEnumerator<ExcelRangeBase> enumerator;
            public Enumerador(IEnumerator<ExcelRangeBase> enumerator)
            {
                this.enumerator = enumerator;
                Reset();
            }

            public CellRange Current
            {
                get
                {
                    return new CellRange(enumerator.Current);
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public void Dispose()
            {
                enumerator.Dispose();
            }

            public bool MoveNext()
            {
                return enumerator.MoveNext();
            }

            public void Reset()
            {
                enumerator.Reset();
            }
        }

    }
}
