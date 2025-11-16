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

            // Find the Input folder - works from both repository root and bin/Debug/net6.0
            string filePath = FindInputFile("Mega-Sena.xlsx");
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

		private static string FindInputFile(string fileName)
		{
			// Try multiple possible paths to support both VS Code and Visual Studio
			string[] possiblePaths = new[]
			{
				// From repository root (VS Code, dotnet run)
				Path.Combine("MegaSena", "Input", fileName),
				// From bin/Debug/net6.0 (Visual Studio)
				Path.Combine("..", "..", "..", "Input", fileName),
				// From MegaSena project directory
				Path.Combine("Input", fileName),
				// Absolute path fallback - navigate up from current directory
				Path.Combine(Directory.GetCurrentDirectory(), "MegaSena", "Input", fileName),
				Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Input", fileName)
			};

			foreach (var path in possiblePaths)
			{
				var fullPath = Path.GetFullPath(path);
				if (File.Exists(fullPath))
				{
					return fullPath;
				}
			}

			// If not found, throw a helpful error
			throw new FileNotFoundException(
				$"Could not find '{fileName}' in any of the expected locations. " +
				$"Current directory: {Directory.GetCurrentDirectory()}. " +
				$"Please ensure the file exists in MegaSena/Input/ folder.");
		}
	}
}







