using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using MegaSena.Entity;


namespace MegaSena.Core
{
	public static class MegaSenaResults
	{
		public static List<MegaSenaDraw> ReadMegaSenaXLSX()
		{
            List<MegaSenaDraw> lstMegaSena = new List<MegaSenaDraw>();

            string filePath = Path.Combine("MegaSena", "Input", "Mega-Sena.xlsx");
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filePath, false))
            {
                WorkbookPart? workbookPart = spreadsheetDocument.WorkbookPart;
                if (workbookPart == null) return lstMegaSena;

                WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                int rowNumber = 0;

                foreach (Row r in sheetData.Elements<Row>())
                {
                    int colNumber = 0;
                    MegaSenaDraw objMegaSena = new MegaSenaDraw();

                    if (rowNumber > 0)
                    {
                        foreach (Cell c in r.Elements<Cell>())
                        {
                            if(!string.IsNullOrEmpty(c.InnerText))
                            {
								objMegaSena = MegaSenaFormat.LinhaConcurso(colNumber, c.InnerText, objMegaSena);
								colNumber++;
							}
                        }
                    }

                    if (rowNumber > 0 && objMegaSena.Concurso > 0)
                    {
                        //Console.WriteLine(string.Format("Reading - Concurso: {0}", objMegaSena.Concurso));
                        lstMegaSena.Add(objMegaSena);
                    }

                    rowNumber++;
                }
            }
            
            return lstMegaSena;
		}
	}
}







