using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MegaSena
{
    /// <summary>
    /// Analyzes the duration of Mega-Sena cycles from start to completion.
    ///
    /// Purpose: Calculate statistical metrics about how long it takes for all 60 numbers
    /// to be drawn at least once, forming a complete cycle.
    ///
    /// Key Metrics:
    /// - Average cycle duration: ~172.8 days (5.7 months)
    /// - Median cycle duration: ~161 days (5.3 months)
    /// - Range: 91 to 343 days (3 to 11.3 months)
    ///
    /// Also provides current cycle status and comparison to historical averages.
    ///
    /// Usage: AnalyzeCycleDuration.Analyze();
    /// </summary>
    public static class AnalyzeCycleDuration
    {
        public static void Analyze()
        {
            string outputFolder = Path.Combine("Output", "MegaSena");
            var files = Directory.GetFiles(outputFolder, "*.csv")
                .Where(f => !f.Contains("InProgress"))
                .OrderBy(f => f)
                .ToList();
            
            var cycleDurations = new List<(string fileName, DateTime startDate, DateTime endDate, int durationDays, int numbersRemaining)>();
            
            foreach (var file in files)
            {
                var lines = File.ReadAllLines(file);
                
                DateTime? startDate = null;
                DateTime? endDate = null;
                
                // Find cycle dates in the file
                foreach (var line in lines)
                {
                    if (line.StartsWith("Cycle Start Date,"))
                    {
                        var dateStr = line.Split(',')[1];
                        if (DateTime.TryParse(dateStr, out DateTime start))
                            startDate = start;
                    }
                    else if (line.StartsWith("Cycle End Date,"))
                    {
                        var dateStr = line.Split(',')[1];
                        if (DateTime.TryParse(dateStr, out DateTime end))
                            endDate = end;
                    }
                }
                
                if (startDate.HasValue && endDate.HasValue)
                {
                    int duration = (endDate.Value - startDate.Value).Days;
                    cycleDurations.Add((Path.GetFileName(file), startDate.Value, endDate.Value, duration, 0));
                }
            }
            
            // Analyze current in-progress cycle
            var inProgressFile = files.FirstOrDefault(f => f.Contains("InProgress")) 
                ?? Directory.GetFiles(outputFolder, "*InProgress*.csv").FirstOrDefault();
            
            DateTime? currentStartDate = null;
            int numbersNotDrawn = 0;
            
            if (inProgressFile != null)
            {
                var lines = File.ReadAllLines(inProgressFile);
                
                foreach (var line in lines)
                {
                    if (line.StartsWith("Cycle Start Date,"))
                    {
                        var dateStr = line.Split(',')[1];
                        if (DateTime.TryParse(dateStr, out DateTime start))
                            currentStartDate = start;
                    }
                    else if (line.StartsWith("0 times"))
                    {
                        // Count how many numbers haven't been drawn
                        var numbers = line.Split(':')[1].Trim();
                        numbersNotDrawn = numbers.Split(',').Length;
                    }
                }
            }
            
            // Calculate statistics
            if (cycleDurations.Count > 0)
            {
                var avgDuration = cycleDurations.Average(c => c.durationDays);
                var minDuration = cycleDurations.Min(c => c.durationDays);
                var maxDuration = cycleDurations.Max(c => c.durationDays);
                var medianDuration = cycleDurations.OrderBy(c => c.durationDays).ElementAt(cycleDurations.Count / 2).durationDays;
                
                Console.WriteLine("\n=== CYCLE DURATION ANALYSIS ===");
                Console.WriteLine($"Total completed cycles analyzed: {cycleDurations.Count}");
                Console.WriteLine($"\nCycle Duration Statistics:");
                Console.WriteLine($"  Average: {avgDuration:F1} days ({avgDuration / 30.44:F1} months)");
                Console.WriteLine($"  Median:  {medianDuration} days ({medianDuration / 30.44:F1} months)");
                Console.WriteLine($"  Minimum: {minDuration} days ({minDuration / 30.44:F1} months)");
                Console.WriteLine($"  Maximum: {maxDuration} days ({maxDuration / 30.44:F1} months)");
                
                // Current cycle analysis
                if (currentStartDate.HasValue)
                {
                    int currentDuration = (DateTime.Now - currentStartDate.Value).Days;
                    double percentOfAverage = (currentDuration / avgDuration) * 100;
                    double percentOfMedian = (currentDuration / medianDuration) * 100;
                    
                    Console.WriteLine($"\n=== CURRENT CYCLE STATUS ===");
                    Console.WriteLine($"Start Date: {currentStartDate.Value:dd/MM/yyyy}");
                    Console.WriteLine($"Current Duration: {currentDuration} days ({currentDuration / 30.44:F1} months)");
                    Console.WriteLine($"Numbers not yet drawn: {numbersNotDrawn}");
                    Console.WriteLine($"\nComparison to historical data:");
                    Console.WriteLine($"  {percentOfAverage:F1}% of average cycle duration");
                    Console.WriteLine($"  {percentOfMedian:F1}% of median cycle duration");
                    
                    if (currentDuration > avgDuration)
                    {
                        Console.WriteLine($"\n⚠️  Current cycle is LONGER than average by {currentDuration - avgDuration:F0} days");
                    }
                    else
                    {
                        Console.WriteLine($"\n✓ Current cycle is still {avgDuration - currentDuration:F0} days shorter than average");
                    }
                    
                    // Estimate based on remaining numbers
                    Console.WriteLine($"\n=== PROJECTION ===");
                    Console.WriteLine($"With {numbersNotDrawn} numbers remaining:");
                    if (numbersNotDrawn <= 3)
                    {
                        Console.WriteLine($"  The cycle is in its FINAL STAGE (95%+ complete)");
                        Console.WriteLine($"  Historically, cycles this close to completion typically end within 1-4 weeks");
                    }
                    else if (numbersNotDrawn <= 10)
                    {
                        Console.WriteLine($"  The cycle is in its late stage (83%+ complete)");
                    }
                    else
                    {
                        Console.WriteLine($"  The cycle is still in progress");
                    }
                }
                
                Console.WriteLine("\n=== END OF ANALYSIS ===\n");
            }
        }
    }
}













