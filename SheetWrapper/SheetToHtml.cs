using System.Text;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Xml;
using System.Collections.Generic;
using System.Web;

namespace SheetWrapper
{
    public static class SheetToHtml
    {
        private static string toCSSrgba(Color c, bool useAlpha = true)
        {
            int alpha = useAlpha ? c.A / 255 : 1;
            return $"rgba({c.R},{c.G},{c.B},{alpha})";
        }

        private static void AppendStylesToCurrentElement(XmlWriter html, Dictionary<string, string> cssStyles)
        {
            html.WriteAttributeString("style", cssStyles.Aggregate("", (t, data) => t + $"{data.Key}:{data.Value};"));
        }

        private static void AddCellStyle(
                            XmlWriter html,
                            CellRange cr)
        {
            if (cr.HasContent)
            {
                AppendStylesToCurrentElement(html, new Dictionary<string, string>
                {
                    { "border", $"{cr.Style.Border.Top.Width_px}px {cr.Style.Border.Top.Type} {toCSSrgba(cr.Style.Border.Top.Color, false)}" },
                    { "font-family", $"{cr.Style.Font.Name}" },
                    { "font-size", $"{cr.Style.Font.Size}px" },
                    { "font-weight", cr.Style.Font.Bold ? "bold" : "normal" },
                    { "font-style", cr.Style.Font.Italic ? "italic" : "normal" },
                    { "background-color", toCSSrgba(cr.Style.BackgroundColor) },
                    { "color", toCSSrgba(cr.Style.FontColor, false) },
                    { "padding", "8px" },
                    { "white-space", cr.Style.WrapText ? "normal" : "no-wrap" },
                    { "text-align", cr.Style.HorizontalAlignment.ToString() },
                    { "text-decoration", cr.Style.Font.Underline ? "underline" : "initial" }
                });

            }
        }

    private static void AddData(
                            XmlWriter html,
                            Sheet s,
                            int row,
                            int iStartCol, int iEndCol)
    {
        for (int col = iStartCol; col <= iEndCol; col++)
        {
            if (!s.GetCol(col).Hidden && !s.IsColNullOrEmpty(col))
            {
                CellRange cr = s[row, col];
                if (!cr.IsMerged || cr.IsMerged && cr.HasContent)
                {
                    int colSpan = s.GetColSpan(row, col);
                    int rowSpan = s.GetRowSpan(row, col);

                    html.WriteStartElement("td");
                    if (colSpan > 1)
                        html.WriteAttributeString("colspan", $"{colSpan}");
                    if (rowSpan > 1)
                        html.WriteAttributeString("rowspan", $"{rowSpan}");
                    AddCellStyle(html, cr);
                    string data = s[row, col].Text != "" ? HttpUtility.HtmlEncode(s[row, col].Text) : "&nbsp;";
                    html.WriteRaw(data);
                    html.WriteEndElement();
                }
            }
        }
    }

    public static string Convert(Sheet s)
    {
        string render = "";

        var settings = new XmlWriterSettings
        {
            OmitXmlDeclaration = true,
            Indent = true,
            Encoding = Encoding.UTF8
        };
        using (var htmlString = new StringWriter(new StringBuilder()))
        using (var html = XmlWriter.Create(htmlString, settings))
        {

            CellAddress start = s.Start;
            CellAddress end = s.End;
            html.WriteStartDocument();
            html.WriteStartElement("div");
            AppendStylesToCurrentElement(html, new Dictionary<string, string>
                {
                    { "overflow-x", "auto" },
                    { "display", "flex" },
                    { "justify-content", "left" }
                });

            html.WriteStartElement("table");
            AppendStylesToCurrentElement(html, new Dictionary<string, string>
                {
                    { "border-collapse", "collapse" },
                    { "empty-cells", "hide" }
                });
            for (int row = start.Row; row <= end.Row; row++)
            {
                if (!s.GetRow(row).Hidden)
                {
                    html.WriteStartElement("tr");
                    AddData(html, s, row, start.Col, end.Col);
                    html.WriteEndElement();
                }
            }

            html.WriteEndElement(); // table
            html.WriteEndElement(); // div
            html.WriteEndDocument();
            html.Flush();
            render = htmlString.ToString();
        }

        return render;
    }
}
}
