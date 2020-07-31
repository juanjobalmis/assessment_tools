using System;
using OfficeOpenXml;

namespace SheetWrapper
{
    public class Row
    {
        private readonly ExcelRow r;

        public bool Hidden
        {
            get { return r.Hidden || r.Collapsed; }
        }
        public bool Merged
        {
            get { return r.Merged; }
        }
        public double Height
        {
            get { return r.Height; }
            set { r.Height = value; }
        }

        internal Row(ExcelRow r)
        {
            this.r = r ?? throw new ArgumentNullException(nameof(r));
        }
    }
}
