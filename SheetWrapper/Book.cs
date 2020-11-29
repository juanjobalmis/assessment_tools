using OfficeOpenXml;
using System;
using System.IO;

namespace SheetWrapper
{
    public partial class Book : IDisposable
    {
        private readonly ExcelPackage book;

        public Sheet MainSheet
        {
            get { return new Sheet(book.Workbook.Worksheets[0]); }
        }
        public string FileName
        {
            get { return book.File.Name; }
        }

        public Book(string path, bool mustExist)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            FileInfo fi = new FileInfo(path);
            if (!mustExist && fi.Exists)
                fi.Delete();
            if (mustExist && !fi.Exists)
                throw new SpreadSheetException($"Book {fi.FullName} Not Found");

            try
            {
                book = new ExcelPackage(fi);                
            }
            catch (Exception e)
            {
                throw new SpreadSheetException($"There is some problem loading or creating the book {fi.FullName}", e);
            }
        }

        public Sheet AddSheet(string name)
        {
            return new Sheet(book.Workbook.Worksheets.Add(name));
        }
        public CellStyle AddStyle(string name)
        {
            return new CellStyle(book.Workbook.Styles.CreateNamedStyle(name).Style);
        }

        public void Save()
        {
            book.Save();
        }

        public void Dispose()
        {
            if (book != null)
                book.Dispose();
        }
    }
}
