using OfficeOpenXml;

namespace SheetWrapper
{
    public class CellRangeAddress
    {
        private readonly ExcelAddressBase ra;

        public CellRangeAddress(CellAddress f, CellAddress l)
        {
            ra = new ExcelAddressBase(f.Row, f.Col, l.Row, l.Col);
        }
        internal CellRangeAddress(ExcelAddressBase d)
        {
            ra = d;
        }

        public CellRangeAddress(string rangeOfCells)
        {
            ra = new ExcelAddressBase(rangeOfCells);
        }

        public override string ToString()
        {
            return $"{ra.Address}";
        }
    }
}
