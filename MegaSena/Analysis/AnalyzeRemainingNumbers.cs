using MegaSena.Entity;

using MegaSena.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MegaSena
{
    /// <summary>
    /// GENERIC analysis tool that works with ANY number of remaining numbers (2 to 60).
    ///
    /// Purpose: Analyze which remaining number gets drawn first when N numbers remain in a cycle.
    /// This is the fully flexible version that replaces hardcoded analysis tools.
    ///
    /// Features:
    /// - Works with any count from 2 to 60 remaining numbers
    /// - Automatic position labeling (Lowest/Highest for 2, Lowest/Middle/Highest for 3, Position 1-N for 4+)
    /// - Five comprehensive pattern analyses
    ///
    /// Patterns Analyzed:
    /// 1. Position Analysis - Which position (1st, 2nd, 3rd, etc.) gets drawn first
    /// 2. Number Range - Distribution across 1-20, 21-40, 41-60
    /// 3. Even vs Odd - Parity patterns
    /// 4. Spacing - Average gaps between remaining numbers
    /// 5. Specific Numbers - Top 10 most frequently drawn numbers
    ///
    /// Example Results:
    /// - 2 remaining: Lowest 54.9%, Highest 45.1%
    /// - 3 remaining: Highest 44.2%, Lowest 32.7%, Middle 23.1%
    /// - 5 remaining: Position 3 leads at 25.5%
    /// - 10 remaining: Positions 3 & 4 lead at 17.2%
    ///
    /// Usage:
    /// - AnalyzeRemainingNumbers.Analyze(lstMegaSena, 2);  // 2 remaining
    /// - AnalyzeRemainingNumbers.Analyze(lstMegaSena, 3);  // 3 remaining (default)
    /// - AnalyzeRemainingNumbers.Analyze(lstMegaSena, 10); // 10 remaining
    /// </summary>
    public static class AnalyzeRemainingNumbers
    {
        /// <summary>
        /// Generic analysis that works with any number of remaining numbers
        /// </summary>
        /// <param name="lstMegaSena">List of all Mega-Sena draws</param>
        /// <param name="remainingCount">Number of remaining numbers to track (e.g., 2, 3, 4, 5, 10)</param>
        public static void Analyze(List<MegaSenaDraw> lstMegaSena, int remainingCount = 3)
        {
            if (remainingCount < 2 || remainingCount > 60)
            {
                Console.WriteLine($"Error: remainingCount must be between 2 and 60. Got: {remainingCount}");
                return;
            }

            var scenarios = new List<GenericRemainingNumberScenario>();
            int cycleNumber = 0;
            int numRef = 60;

            var cycleNumbers = CreateCycleNumbers(numRef);
            
            bool trackingRemaining = false;
            List<int> trackedRemainingNumbers = new List<int>();
            
            foreach (MegaSenaDraw draw in lstMegaSena)
            {
                // Update cycle numbers with current draw
                UpdateCycleNumbers(cycleNumbers, draw);
                
                // Check how many numbers are not yet drawn
                var notDrawnNumbers = cycleNumbers.Where(cn => !cn.Drawn).Select(cn => cn.Number).OrderBy(n => n).ToList();
                int notDrawnCount = notDrawnNumbers.Count;
                
                // If we just reached exactly N numbers remaining
                if (notDrawnCount == remainingCount && !trackingRemaining)
                {
                    trackingRemaining = true;
                    trackedRemainingNumbers = new List<int>(notDrawnNumbers);
                }
                
                // If we were tracking N remaining and now one was drawn
                if (trackingRemaining && notDrawnCount == remainingCount - 1)
                {
                    // Find which of the N was drawn
                    var drawnFromRemaining = trackedRemainingNumbers.Except(notDrawnNumbers).FirstOrDefault();
                    
                    if (drawnFromRemaining > 0)
                    {
                        scenarios.Add(new GenericRemainingNumberScenario
                        {
                            CycleNumber = cycleNumber + 1,
                            RemainingCount = remainingCount,
                            RemainingNumbers = new List<int>(trackedRemainingNumbers),
                            FirstDrawn = drawnFromRemaining,
                            ConcursoFirstDrawn = draw.Concurso,
                            DateFirstDrawn = draw.DataDoSorteio
                        });
                    }
                    
                    trackingRemaining = false;
                }
                
                // Check if cycle is complete
                if (AllNumbersDrawn(cycleNumbers))
                {
                    cycleNumber++;
                    cycleNumbers = CreateCycleNumbers(numRef);
                    trackingRemaining = false;
                }
            }
            
            // Display results
            Console.WriteLine($"\n=== GENERIC ANALYSIS: Which of {remainingCount} Remaining Numbers Gets Drawn First? ===");
            Console.WriteLine($"Total scenarios analyzed: {scenarios.Count}\n");
            
            if (scenarios.Count == 0)
            {
                Console.WriteLine($"No scenarios found with exactly {remainingCount} remaining numbers.");
                Console.WriteLine("=== END OF ANALYSIS ===\n");
                return;
            }
            
            // Pattern 1: Position analysis (works for any count)
            AnalyzePositionPattern(scenarios, remainingCount);
            
            // Pattern 2: Number range analysis
            AnalyzeRangePattern(scenarios);
            
            // Pattern 3: Even vs Odd
            AnalyzeEvenOddPattern(scenarios);
            
            // Pattern 4: Spacing analysis
            AnalyzeSpacingPattern(scenarios);
            
            // Pattern 5: Specific number frequency (which actual numbers get drawn most)
            AnalyzeSpecificNumberFrequency(scenarios);
            
            // Show current cycle prediction if applicable
            ShowCurrentCyclePrediction(scenarios, remainingCount);
            
            Console.WriteLine("=== END OF ANALYSIS ===\n");
        }
        
        private static void AnalyzePositionPattern(List<GenericRemainingNumberScenario> scenarios, int remainingCount)
        {
            Console.WriteLine("PATTERN 1: Position Analysis");

            var positionCounts = new Dictionary<string, int>();

            // Initialize position labels
            if (remainingCount == 2)
            {
                positionCounts["Lowest"] = 0;
                positionCounts["Highest"] = 0;
            }
            else if (remainingCount == 3)
            {
                positionCounts["Lowest"] = 0;
                positionCounts["Middle"] = 0;
                positionCounts["Highest"] = 0;
            }
            else
            {
                // For 4+ numbers, use position indices
                for (int i = 0; i < remainingCount; i++)
                {
                    positionCounts[$"Position {i + 1}"] = 0;
                }
            }

            // Count which position gets drawn first
            foreach (var scenario in scenarios)
            {
                var sorted = scenario.RemainingNumbers.OrderBy(n => n).ToList();
                int position = sorted.IndexOf(scenario.FirstDrawn);

                if (remainingCount == 2)
                {
                    if (position == 0) positionCounts["Lowest"]++;
                    else positionCounts["Highest"]++;
                }
                else if (remainingCount == 3)
                {
                    if (position == 0) positionCounts["Lowest"]++;
                    else if (position == 1) positionCounts["Middle"]++;
                    else positionCounts["Highest"]++;
                }
                else
                {
                    positionCounts[$"Position {position + 1}"]++;
                }
            }

            // Display results
            foreach (var pos in positionCounts.OrderBy(p => p.Key))
            {
                double percentage = (pos.Value * 100.0) / scenarios.Count;
                Console.WriteLine($"  {pos.Key} drawn first: {pos.Value} times ({percentage:F1}%)");
            }
            Console.WriteLine();
        }

        private static void AnalyzeRangePattern(List<GenericRemainingNumberScenario> scenarios)
        {
            var rangeAnalysis = new Dictionary<string, int>
            {
                {"1-20", 0},
                {"21-40", 0},
                {"41-60", 0}
            };

            foreach (var scenario in scenarios)
            {
                if (scenario.FirstDrawn <= 20) rangeAnalysis["1-20"]++;
                else if (scenario.FirstDrawn <= 40) rangeAnalysis["21-40"]++;
                else rangeAnalysis["41-60"]++;
            }

            Console.WriteLine("PATTERN 2: Number Range Analysis");
            foreach (var range in rangeAnalysis)
            {
                double percentage = (range.Value * 100.0) / scenarios.Count;
                Console.WriteLine($"  Range {range.Key}: {range.Value} times ({percentage:F1}%)");
            }
            Console.WriteLine();
        }

        private static void AnalyzeEvenOddPattern(List<GenericRemainingNumberScenario> scenarios)
        {
            int evenFirst = scenarios.Count(s => s.FirstDrawn % 2 == 0);
            int oddFirst = scenarios.Count(s => s.FirstDrawn % 2 == 1);

            Console.WriteLine("PATTERN 3: Even vs Odd");
            Console.WriteLine($"  Even number drawn first: {evenFirst} times ({(evenFirst * 100.0 / scenarios.Count):F1}%)");
            Console.WriteLine($"  Odd number drawn first: {oddFirst} times ({(oddFirst * 100.0 / scenarios.Count):F1}%)");
            Console.WriteLine();
        }

        private static void AnalyzeSpacingPattern(List<GenericRemainingNumberScenario> scenarios)
        {
            var spacingAnalysis = new List<int>();

            foreach (var scenario in scenarios)
            {
                var sorted = scenario.RemainingNumbers.OrderBy(n => n).ToList();
                for (int i = 0; i < sorted.Count - 1; i++)
                {
                    spacingAnalysis.Add(sorted[i + 1] - sorted[i]);
                }
            }

            if (spacingAnalysis.Count > 0)
            {
                Console.WriteLine("PATTERN 4: Spacing Between Remaining Numbers");
                Console.WriteLine($"  Average spacing: {spacingAnalysis.Average():F1}");
                Console.WriteLine($"  Min spacing: {spacingAnalysis.Min()}");
                Console.WriteLine($"  Max spacing: {spacingAnalysis.Max()}");
                Console.WriteLine();
            }
        }

        private static void AnalyzeSpecificNumberFrequency(List<GenericRemainingNumberScenario> scenarios)
        {
            var numberFrequency = scenarios
                .GroupBy(s => s.FirstDrawn)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .ToList();

            if (numberFrequency.Count > 0)
            {
                Console.WriteLine("PATTERN 5: Most Frequently Drawn Numbers (Top 10)");
                foreach (var group in numberFrequency)
                {
                    double percentage = (group.Count() * 100.0) / scenarios.Count;
                    Console.WriteLine($"  Number {group.Key}: {group.Count()} times ({percentage:F1}%)");
                }
                Console.WriteLine();
            }
        }

        private static void ShowCurrentCyclePrediction(List<GenericRemainingNumberScenario> scenarios, int remainingCount)
        {
            // This would need to read the current cycle state from CSV
            // For now, we'll show a generic prediction template
            Console.WriteLine("=== CURRENT CYCLE PREDICTION ===");
            Console.WriteLine($"To see predictions for current cycle with {remainingCount} remaining numbers,");
            Console.WriteLine("check the latest Cycle_InProgress CSV file.");
            Console.WriteLine();
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
    }
}















