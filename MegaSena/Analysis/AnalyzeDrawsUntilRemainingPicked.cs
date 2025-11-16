using MegaSena.Entity;

using MegaSena.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MegaSena
{
    /// <summary>
    /// Analyzes how many draws it takes from when 3 numbers remain until one is finally picked.
    ///
    /// Purpose: Predict the timeline for cycle completion - when can we expect one of the
    /// remaining numbers to be drawn?
    ///
    /// Key Findings:
    /// - Average: 4.4 draws until one of the 3 remaining is picked
    /// - Most common: 1 or 2 draws (21.2% each)
    /// - 80% of the time: within 7 draws
    /// - Range: 1 to 16 draws
    ///
    /// Distribution:
    /// - 1 draw: 21.2% (very likely!)
    /// - 2 draws: 21.2%
    /// - 3-5 draws: 28.8%
    /// - 6-7 draws: 15.4%
    /// - 8+ draws: 13.4% (rare outliers)
    ///
    /// Usage: AnalyzeDrawsUntilRemainingPicked.Analyze(lstMegaSena);
    /// </summary>
    public static class AnalyzeDrawsUntilRemainingPicked
    {
        public static void Analyze(List<MegaSenaDraw> lstMegaSena)
        {
            var scenarios = new List<DrawsUntilPickedScenario>();
            int cycleNumber = 0;
            int numRef = 60;

            var cycleNumbers = CreateCycleNumbers(numRef);
            
            bool trackingThreeRemaining = false;
            List<int> threeRemainingNumbers = new List<int>();
            int concursoWhenThreeRemaining = 0;
            DateTime? dateWhenThreeRemaining = null;
            int drawsSinceThreeRemaining = 0;
            
            foreach (MegaSenaDraw draw in lstMegaSena)
            {
                UpdateCycleNumbers(cycleNumbers, draw);
                
                var notDrawnNumbers = cycleNumbers.Where(cn => !cn.Drawn).Select(cn => cn.Number).OrderBy(n => n).ToList();
                int notDrawnCount = notDrawnNumbers.Count;
                
                // Start tracking when we reach exactly 3 remaining
                if (notDrawnCount == 3 && !trackingThreeRemaining)
                {
                    trackingThreeRemaining = true;
                    threeRemainingNumbers = new List<int>(notDrawnNumbers);
                    concursoWhenThreeRemaining = draw.Concurso;
                    dateWhenThreeRemaining = draw.DataDoSorteio;
                    drawsSinceThreeRemaining = 0;
                }
                
                // If we're tracking and still have 3 remaining, increment counter
                if (trackingThreeRemaining && notDrawnCount == 3)
                {
                    drawsSinceThreeRemaining++;
                }
                
                // If we were tracking 3 remaining and now one was drawn
                if (trackingThreeRemaining && notDrawnCount == 2)
                {
                    // Find which of the 3 was drawn
                    var drawnFromRemaining = threeRemainingNumbers.Except(notDrawnNumbers).FirstOrDefault();
                    
                    if (drawnFromRemaining > 0)
                    {
                        scenarios.Add(new DrawsUntilPickedScenario
                        {
                            CycleNumber = cycleNumber + 1,
                            RemainingNumbers = new List<int>(threeRemainingNumbers),
                            ConcursoWhenThreeRemaining = concursoWhenThreeRemaining,
                            DateWhenThreeRemaining = dateWhenThreeRemaining,
                            DrawsUntilOnePicked = drawsSinceThreeRemaining,
                            NumberPicked = drawnFromRemaining,
                            ConcursoWhenPicked = draw.Concurso,
                            DateWhenPicked = draw.DataDoSorteio
                        });
                    }
                    
                    trackingThreeRemaining = false;
                }
                
                // Check if cycle is complete
                if (AllNumbersDrawn(cycleNumbers))
                {
                    cycleNumber++;
                    cycleNumbers = CreateCycleNumbers(numRef);
                    trackingThreeRemaining = false;
                }
            }
            
            // Display results
            Console.WriteLine("\n=== ANALYSIS: How Many Draws Until One of 3 Remaining is Picked? ===");
            Console.WriteLine($"Total scenarios analyzed: {scenarios.Count}\n");
            
            foreach (var scenario in scenarios)
            {
                Console.WriteLine($"Cycle #{scenario.CycleNumber}");
                Console.WriteLine($"  3 Remaining at: Concurso {scenario.ConcursoWhenThreeRemaining} ({scenario.DateWhenThreeRemaining:dd/MM/yyyy})");
                Console.WriteLine($"  Remaining numbers: {string.Join(", ", scenario.RemainingNumbers)}");
                Console.WriteLine($"  Draws until one picked: {scenario.DrawsUntilOnePicked}");
                Console.WriteLine($"  Number picked: {scenario.NumberPicked}");
                Console.WriteLine($"  Picked at: Concurso {scenario.ConcursoWhenPicked} ({scenario.DateWhenPicked:dd/MM/yyyy})");
                Console.WriteLine();
            }
            
            // Statistical analysis
            if (scenarios.Count > 0)
            {
                var drawCounts = scenarios.Select(s => s.DrawsUntilOnePicked).ToList();
                
                Console.WriteLine("=== STATISTICAL SUMMARY ===");
                Console.WriteLine($"Average draws until one picked: {drawCounts.Average():F2}");
                Console.WriteLine($"Median draws: {GetMedian(drawCounts)}");
                Console.WriteLine($"Min draws: {drawCounts.Min()}");
                Console.WriteLine($"Max draws: {drawCounts.Max()}");
                Console.WriteLine();
                
                // Distribution
                var distribution = drawCounts.GroupBy(d => d).OrderBy(g => g.Key);
                Console.WriteLine("DISTRIBUTION:");
                foreach (var group in distribution)
                {
                    int count = group.Count();
                    double percentage = (count * 100.0) / scenarios.Count;
                    Console.WriteLine($"  {group.Key} draw(s): {count} times ({percentage:F1}%)");
                }
                
                // Current cycle prediction
                Console.WriteLine("\n=== CURRENT CYCLE PREDICTION ===");
                Console.WriteLine("Based on historical data:");
                Console.WriteLine($"  Most likely: {drawCounts.GroupBy(d => d).OrderByDescending(g => g.Count()).First().Key} draw(s)");
                Console.WriteLine($"  Average: {drawCounts.Average():F1} draws");
                Console.WriteLine($"  80% of the time: within {GetPercentile(drawCounts, 80)} draws");
            }
            
            Console.WriteLine("\n=== END OF ANALYSIS ===\n");
        }
        
        private static List<CycleNumber> CreateCycleNumbers(int numRef)
        {
            var list = new List<CycleNumber>();
            for (int i = 1; i <= numRef; i++)
            {
                list.Add(new CycleNumber { Number = i });
            }
            return list;
        }
        
        private static void UpdateCycleNumbers(List<CycleNumber> cycleNumbers, MegaSenaDraw draw)
        {
            var drawnNumbers = new[] { draw.Bola1, draw.Bola2, draw.Bola3, draw.Bola4, draw.Bola5, draw.Bola6 };
            
            foreach (int num in drawnNumbers)
            {
                var cycleNum = cycleNumbers.FirstOrDefault(cn => cn.Number == num);
                if (cycleNum != null)
                {
                    cycleNum.Drawn = true;
                    cycleNum.Times++;
                    cycleNum.LastDrawn = draw.DataDoSorteio;
                }
            }
        }
        
        private static bool AllNumbersDrawn(List<CycleNumber> cycleNumbers)
        {
            return cycleNumbers.All(cn => cn.Drawn);
        }
        
        private static double GetMedian(List<int> values)
        {
            var sorted = values.OrderBy(v => v).ToList();
            int count = sorted.Count;
            if (count % 2 == 0)
                return (sorted[count / 2 - 1] + sorted[count / 2]) / 2.0;
            else
                return sorted[count / 2];
        }
        
        private static int GetPercentile(List<int> values, int percentile)
        {
            var sorted = values.OrderBy(v => v).ToList();
            int index = (int)Math.Ceiling(percentile / 100.0 * sorted.Count) - 1;
            return sorted[Math.Max(0, Math.Min(index, sorted.Count - 1))];
        }
    }
}












