using MegaSena.Entity;
using System.Text;

namespace MegaSena.Core
{
    /// <summary>
    /// Writes Mega-Sena cycle results to CSV files with comprehensive statistics.
    /// Generates formatted output files containing number frequencies, cycle dates,
    /// and grouped number distributions for analysis.
    /// </summary>
    public static class CycleResultsWriter
    {
        private static string GetOutputFolder()
        {
            // Try multiple possible paths to support both VS Code and Visual Studio
            string[] possibleBasePaths = new[]
            {
                // From repository root (VS Code, dotnet run)
                Path.Combine("Output", "MegaSena"),
                // From bin/Debug/net6.0 (Visual Studio)
                Path.Combine("..", "..", "..", "..", "Output", "MegaSena"),
                // Absolute path from current directory
                Path.Combine(Directory.GetCurrentDirectory(), "Output", "MegaSena"),
                Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "Output", "MegaSena")
            };

            // Try to find an existing Output folder or use the first valid path
            foreach (var basePath in possibleBasePaths)
            {
                try
                {
                    var fullPath = Path.GetFullPath(basePath);
                    var parentDir = Path.GetDirectoryName(fullPath);

                    // If parent directory exists or can be created, use this path
                    if (parentDir != null && (Directory.Exists(parentDir) || Directory.Exists(Path.GetDirectoryName(parentDir))))
                    {
                        return fullPath;
                    }
                }
                catch
                {
                    // Skip invalid paths
                    continue;
                }
            }

            // Fallback: use repository root approach
            return Path.GetFullPath(Path.Combine("Output", "MegaSena"));
        }
        
        public static void WriteCycleToCSV(Cycle objCycle, DateTime? endCycleDate = null, int lastNumber = 0, bool isLastCycle = false)
        {
            string outputFolder = GetOutputFolder();
            EnsureOutputFolderExists(outputFolder);

            string fileName = GenerateFileName(endCycleDate, lastNumber, isLastCycle);
            string filePath = Path.Combine(outputFolder, fileName);
            
            var csvContent = new StringBuilder();
            
            // CSV Header
            csvContent.AppendLine("Number,Times,LastDrawn");
            
            int cycleTimeNumbersSum = 0;
            
            // Data rows
            for (int i = 0; i < 60; i++)
            {
                string lastDrawnFormatted = objCycle.CycleNumbers[i].LastDrawn?.ToString("dd/MM/yyyy") ?? "";
                csvContent.AppendLine($"{objCycle.CycleNumbers[i].Number},{objCycle.CycleNumbers[i].Times},{lastDrawnFormatted}");
                cycleTimeNumbersSum += objCycle.CycleNumbers[i].Times;
            }
            
            // Add summary rows
            int averageTimes = cycleTimeNumbersSum / 60;
            csvContent.AppendLine();
            csvContent.AppendLine($"Average Times per Number,{averageTimes}");
            csvContent.AppendLine();

            // Add cycle date information
            string startDateFormatted = objCycle.StartCycle?.ToString("dd/MM/yyyy") ?? "N/A";
            string endDateFormatted = isLastCycle ? "In Progress" : (endCycleDate?.ToString("dd/MM/yyyy") ?? "N/A");

            csvContent.AppendLine($"Cycle Start Date,{startDateFormatted}");
            csvContent.AppendLine($"Cycle End Date,{endDateFormatted}");
            csvContent.AppendLine();

            // Add grouped numbers by frequency
            csvContent.AppendLine("Numbers Grouped by Frequency");
            var groupedNumbers = GroupNumbersByFrequency(objCycle);

            foreach (var group in groupedNumbers.OrderByDescending(g => g.Key))
            {
                string numbers = string.Join(", ", group.Value.OrderBy(n => n));
                csvContent.AppendLine($"{group.Key} times: {numbers}");
            }

            // Write to file
            File.WriteAllText(filePath, csvContent.ToString(), Encoding.UTF8);
            
            // Console feedback
            string cycleInfo = isLastCycle 
                ? $"Cycle End: --, Concurso: {lastNumber}" 
                : $"Cycle End: {endCycleDate}, Concurso: {lastNumber}";
            
            Console.WriteLine($"{cycleInfo} - Output saved to: {fileName}");
        }
        
        private static void EnsureOutputFolderExists(string outputFolder)
        {
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
        }
        
        private static string GenerateFileName(DateTime? endCycleDate, int lastNumber, bool isLastCycle)
        {
            if (isLastCycle)
            {
                return $"Cycle_InProgress_Concurso_{lastNumber}.csv";
            }

            string dateStr = endCycleDate?.ToString("yyyyMMdd") ?? "Unknown";
            return $"Cycle_{dateStr}_Concurso_{lastNumber}.csv";
        }

        private static Dictionary<int, List<int>> GroupNumbersByFrequency(Cycle objCycle)
        {
            var groupedNumbers = new Dictionary<int, List<int>>();

            foreach (var cycleNumber in objCycle.CycleNumbers)
            {
                int times = cycleNumber.Times;

                if (!groupedNumbers.ContainsKey(times))
                {
                    groupedNumbers[times] = new List<int>();
                }

                groupedNumbers[times].Add(cycleNumber.Number);
            }

            return groupedNumbers;
        }
    }
}








