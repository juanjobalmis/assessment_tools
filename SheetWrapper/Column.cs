using OfficeOpenXml;
using System;

namespace SheetWrapper
{
    public class Column
    {
        private readonly ExcelColumn c;

        public bool Hidden
        {
            get { return (c.Hidden || c.Width < 1e-3); }
        }

        internal Column(ExcelColumn c)
        {
            this.c = c ?? throw new ArgumentNullException(nameof(c));
        }
        public double Width
        {
            get { return c.Width; }
        }

        public override string ToString()
        {
            return c.Width.ToString();
        }
    }
}
