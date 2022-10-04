using System;
using System.Drawing;
using OfficeOpenXml.Style;

namespace SheetWrapper
{
    public class CellStyle
    {
        private readonly ExcelStyle s;
        public enum TextAlign { left, right, center, justify }

        public class CellBorder
        {
            public Color Color { get; set; }
            internal CellBorder(ExcelBorderItem top, ExcelBorderItem bottom, ExcelBorderItem left, ExcelBorderItem right)
            {
                Color = Color.FromArgb(Convert.ToInt32(top.Color.Rgb, 16));

                Top = new Line(Color.FromArgb(Convert.ToInt32(top.Color.Rgb, 16)), Line.MapFrom(top.Style), Line.WidhtFrom(top.Style));
                Bottom = new Line(Color.FromArgb(Convert.ToInt32(bottom.Color.Rgb, 16)), Line.MapFrom(bottom.Style), Line.WidhtFrom(bottom.Style));
                Left = new Line(Color.FromArgb(Convert.ToInt32(left.Color.Rgb, 16)), Line.MapFrom(left.Style), Line.WidhtFrom(left.Style));
                Right = new Line(Color.FromArgb(Convert.ToInt32(right.Color.Rgb, 16)), Line.MapFrom(right.Style), Line.WidhtFrom(right.Style));
            }

            internal CellBorder(Line top, Line bottom, Line left, Line right)
            {
                Top = top.Clone() as Line;
                Bottom = bottom.Clone() as Line;
                Left = left.Clone() as Line;
                Right = right.Clone() as Line;
            }
            public CellBorder(Line all, Color color)
            {
                Color = color;
                Top = all.Clone() as Line;
                Bottom = all.Clone() as Line;
                Left = all.Clone() as Line;
                Right = all.Clone() as Line;
            }

            internal void CopyTo(OfficeOpenXml.Style.Border border)
            {
                border.Top.Style = Top.MapTo();
                border.Top.Color.SetColor(Color);
                border.Bottom.Style = Bottom.MapTo();
                border.Bottom.Color.SetColor(Color);
                border.Right.Style = Right.MapTo();
                border.Right.Color.SetColor(Color);
                border.Left.Style = Left.MapTo();
                border.Left.Color.SetColor(Color);
            }

            public class Line : ICloneable
            {
                internal static Style MapFrom(ExcelBorderStyle s)
                {
                    switch (s)
                    {
                        case ExcelBorderStyle.None:
                            return Style.none;
                        case ExcelBorderStyle.Dotted:
                            return Style.dotted;
                        case ExcelBorderStyle.Dashed:
                            return Style.dashed;
                        default:
                            return Style.solid;
                    }
                }
                internal ExcelBorderStyle MapTo()
                {
                    switch (this.Type)
                    {
                        case Style.none:
                            return ExcelBorderStyle.None;
                        case Style.dotted:
                            return ExcelBorderStyle.Dotted;
                        case Style.solid:
                            switch (this.Width_px)
                            {
                                case 1:
                                    return ExcelBorderStyle.Thin;
                                case 2:
                                    return ExcelBorderStyle.Medium;
                                case 3:
                                    return ExcelBorderStyle.Thick;
                                default:
                                    return ExcelBorderStyle.Thin;
                            }
                        case Style.dashed:
                            return ExcelBorderStyle.Dashed;
                        default:
                            return ExcelBorderStyle.None;
                    }
                }
                internal static int WidhtFrom(ExcelBorderStyle s)
                {
                    switch (s)
                    {
                        case ExcelBorderStyle.None:
                            return 0;
                        case ExcelBorderStyle.Dotted:
                        case ExcelBorderStyle.DashDot:
                        case ExcelBorderStyle.DashDotDot:
                        case ExcelBorderStyle.Thin:
                        case ExcelBorderStyle.Dashed:
                        case ExcelBorderStyle.Medium:
                            return 1;
                        case ExcelBorderStyle.Thick:
                        case ExcelBorderStyle.Double:
                            return 3;
                        default:
                            return 2;
                    }
                }

                public object Clone()
                {
                    return new Line(this.Color, this.Type, this.Width_px);
                }

                public Line(Color color, Style type, int width_px)
                {
                    Color = color;
                    Type = type;
                    Width_px = width_px;
                }

                public enum Style { none, dotted, solid, dashed }
                public Color Color { get; private set; }
                public Style Type { get; private set; }
                public int Width_px { get; private set; }

            }
            public Line Top { get; private set; }
            public Line Bottom { get; private set; }
            public Line Left { get; private set; }
            public Line Right { get; private set; }
        }

        public TextAlign HorizontalAlignment
        {
            get
            {
                switch (s.HorizontalAlignment)
                {
                    case ExcelHorizontalAlignment.Left:
                        return TextAlign.left;
                    case ExcelHorizontalAlignment.Center:
                    case ExcelHorizontalAlignment.CenterContinuous:
                        return TextAlign.center;
                    case ExcelHorizontalAlignment.Right:
                        return TextAlign.right;
                    case ExcelHorizontalAlignment.Fill:
                    case ExcelHorizontalAlignment.Distributed:
                    case ExcelHorizontalAlignment.Justify:
                        return TextAlign.justify;
                    default:
                        return TextAlign.left;
                }
            }
            set
            {
                switch (value)
                {
                    case TextAlign.left:
                        s.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        break;
                    case TextAlign.right:
                        s.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        break;
                    case TextAlign.center:
                        s.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        break;
                    case TextAlign.justify:
                        s.HorizontalAlignment = ExcelHorizontalAlignment.Justify;
                        break;
                    default:
                        s.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        break;
                }
            }
        }
        public Font Font
        {
            get
            {
                FontStyle style = FontStyle.Regular;
                if (s.Font.Bold) style |= FontStyle.Bold;
                if (s.Font.UnderLine) style |= FontStyle.Underline;
                if (s.Font.Italic) style |= FontStyle.Italic;

                return new Font(new FontFamily(s.Font.Name), s.Font.Size, style);
            }
            set
            {
                s.Font.SetFromFont(value.Name, value.Size);
            }
        }

        public int TextRotation
        {
            get { return s.TextRotation;  }
            set { s.TextRotation = value; }
        }

        public CellBorder Border
        {
            get
            {
                return new CellBorder(s.Border.Top, s.Border.Bottom, s.Border.Left, s.Border.Right);
            }
            set
            {
                 value.CopyTo(s.Border);
            }
        }

        public Color BackgroundColor
        {
            get
            {
                string argb = s.Fill.BackgroundColor.Rgb;
                return Color.FromArgb(Convert.ToInt32(argb, 16));
            }
            set
            {
                s.Fill.PatternType = ExcelFillStyle.Solid;
                s.Fill.PatternColor.SetColor(value);
                s.Fill.BackgroundColor.SetColor(value);
            }
        }
        public Color FontColor
        {
            get
            {
                return Color.FromArgb(Convert.ToInt32(s.Font.Color.Rgb, 16));
            }
            set
            {
                s.Font.Color.SetColor(value);
            }
        }
        public bool WrapText
        {
            get
            {
                return s.WrapText;
            }
            set
            {
                s.WrapText = value;
            }
        }
        public bool IsThereBorder
        {
            get
            {
                return
                    Border.Top.Type != CellBorder.Line.Style.none && Border.Top.Width_px > 0 ||
                    Border.Bottom.Type != CellBorder.Line.Style.none && Border.Bottom.Width_px > 0 ||
                    Border.Left.Type != CellBorder.Line.Style.none && Border.Left.Width_px > 0 ||
                    Border.Right.Type != CellBorder.Line.Style.none && Border.Right.Width_px > 0;
            }
        }

        internal CellStyle(ExcelStyle s)
        {
            this.s = s ?? throw new ArgumentNullException(nameof(s));
        }
    }
}
