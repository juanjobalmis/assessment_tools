using System;
using System.Drawing;

namespace SheetWrapper
{
    public static class StylesPackageBN
    {
        public enum Style 
        {
            LongPrimaryColumnName,
            ShortSecondaryColumnName,
            RemarkableCellLeft,
            RemarkableCellCenter,
            RegularCell
        }

        private static void AddNamedStyle(
            Book b,
            string name,
            Color fgColor,
            Color bgColor,
            CellStyle.TextAlign horizontalAlignment,
            int rotation_Degrees,
            bool bold,
            bool wrapText = true)
        {
            CellStyle s = b.AddStyle(name);
            FontStyle fs = FontStyle.Regular;
            fs = bold ? (fs | FontStyle.Bold) : fs;
            s.Font = new Font("Arial", 11f, fs, GraphicsUnit.Point);
            s.Border = new CellStyle.CellBorder(new CellStyle.CellBorder.Line(Color.White, CellStyle.CellBorder.Line.Style.solid, 1), fgColor);
            s.FontColor = fgColor;
            s.BackgroundColor = bgColor;
            s.HorizontalAlignment = horizontalAlignment;
            s.TextRotation = rotation_Degrees;
            s.WrapText = wrapText;
        }
        public static void ApplyTo(Book b)
        {
            AddNamedStyle(b,
                          Style.RegularCell.ToString(),
                          Color.Black,
                          Color.White,
                          CellStyle.TextAlign.center,
                          0,
                          false,
                          false);
            AddNamedStyle(b,
                          Style.RemarkableCellLeft.ToString(),
                          Color.Black,
                          Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0),
                          CellStyle.TextAlign.left,
                          0,
                          true,
                          false);
            AddNamedStyle(b,
                          Style.RemarkableCellCenter.ToString(),
                          Color.Black,
                          Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0),
                          CellStyle.TextAlign.center,
                          0,
                          true,
                          false);
            AddNamedStyle(b,
                          Style.LongPrimaryColumnName.ToString(),
                          Color.White,
                          Color.FromArgb(0xFF, 0x30, 0x30, 0x30),
                          CellStyle.TextAlign.center,
                          90,
                          true);
            AddNamedStyle(b,
                          Style.ShortSecondaryColumnName.ToString(),
                          Color.White,
                          Color.FromArgb(0xFF,0x60,0x60,0x60),
                          CellStyle.TextAlign.center,
                          0,
                          true);
        }
    }
}
