using MegaSena.Entity;

using MegaSena.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MegaSena
{
    /// <summary>
    /// GENERIC prediction system that generates complete 6-number betting combinations
    /// for Mega-Sena based on historical frequency patterns and remaining numbers.
    ///
    /// Purpose: Create data-driven betting strategies by combining one remaining number
    /// with five numbers selected from historical frequency patterns.
    ///
    /// Features:
    /// - Fully generic: Works with ANY number of remaining numbers (2, 3, 4, 5+)
    /// - Automatic adaptation: Messaging and output adjust to actual remaining count
    /// - Multiple strategies: Generates 3 different prediction approaches per remaining number
    /// - Complete bets: All predictions are ready-to-use 6-number combinations
    ///
    /// Prediction Strategies:
    /// 1. Frequency 3 Focus - Numbers drawn 3 times (22.7% historical probability)
    /// 2. Mid-Range 2-4 - Mix of numbers drawn 2-4 times (62.5% combined probability)
    /// 3. Balanced Approach - Average frequency ~3.08 based on historical data
    ///
    /// Output Format:
    /// - Generates N scenarios (one per remaining number)
    /// - Each scenario has 3 complete 6-number bets
    /// - Summary shows top recommended bets
    /// - Clear breakdown: [remaining number] + [5 other numbers]
    ///
    /// Example for 3 remaining (2, 20, 43):
    /// - Scenario 1: If 2 is drawn → 3 complete bets
    /// - Scenario 2: If 20 is drawn → 3 complete bets
    /// - Scenario 3: If 43 is drawn → 3 complete bets
    /// - Summary: Top 3 recommended bets
    ///
    /// Usage:
    /// - PredictNextDraw.GeneratePredictions(lstMegaSena); // Uses current cycle
    /// - PredictNextDraw.GeneratePredictions(lstMegaSena, true, null); // Explicit current cycle
    /// - PredictNextDraw.GeneratePredictions(lstMegaSena, false, customCycleState); // Custom state
    /// </summary>
    public static class PredictNextDraw
    {
        /// <summary>
        /// Generate predictions for next draws - fully generic version
        /// </summary>
        /// <param name="lstMegaSena">List of all Mega-Sena draws</param>
        /// <param name="useCurrentCycle">If true, reads from CSV. If false, must provide cycleState</param>
        /// <param name="cycleState">Optional: provide a specific cycle state to analyze</param>
        public static void GeneratePredictions(List<MegaSenaDraw> lstMegaSena, bool useCurrentCycle = true, CycleState? cycleState = null)
        {
            // Get cycle state
            var currentCycle = useCurrentCycle ? GetCurrentCycleStateFromCSV() : cycleState;

            if (currentCycle == null || currentCycle.RemainingNumbers.Count == 0)
            {
                Console.WriteLine("Error: No cycle state available or no remaining numbers.");
                return;
            }

            int remainingCount = currentCycle.RemainingNumbers.Count;

            Console.WriteLine("\n=== PREDICTION FOR NEXT DRAWS ===");
            Console.WriteLine($"Based on historical analysis when {remainingCount} number{(remainingCount > 1 ? "s" : "")} remain\n");

            Console.WriteLine("Current Cycle Status:");
            Console.WriteLine($"Numbers remaining: {string.Join(", ", currentCycle.RemainingNumbers)}");
            Console.WriteLine($"Total remaining: {remainingCount}");
            Console.WriteLine();

            // Get numbers grouped by frequency
            var numbersByFrequency = currentCycle.CycleNumbers
                .Where(cn => cn.Drawn)
                .GroupBy(cn => cn.Times)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Select(cn => cn.Number).ToList());

            Console.WriteLine("Current Frequency Distribution:");
            foreach (var group in numbersByFrequency.OrderByDescending(g => g.Key))
            {
                Console.WriteLine($"  {group.Key} times: {string.Join(", ", group.Value.OrderBy(n => n))}");
            }
            Console.WriteLine();

            // Generate prediction scenarios (one for each remaining number)
            Console.WriteLine($"=== GENERATING {remainingCount} SCENARIO{(remainingCount > 1 ? "S" : "")} ===\n");

            foreach (int remainingNum in currentCycle.RemainingNumbers)
            {
                Console.WriteLine($"=== SCENARIO: If number {remainingNum} is drawn ===");

                var predictions = GeneratePredictionSet(currentCycle, remainingNum);

                for (int i = 0; i < predictions.Count; i++)
                {
                    Console.WriteLine($"\nStrategy #{i + 1}: {predictions[i].Rationale}");
                    Console.WriteLine($"  Complete 6-number bet: {string.Join("-", predictions[i].Numbers.OrderBy(n => n))}");

                    // Show breakdown
                    var otherNumbers = predictions[i].Numbers.Where(n => n != remainingNum).OrderBy(n => n).ToList();
                    Console.WriteLine($"  Breakdown: [{remainingNum}] + [{string.Join(", ", otherNumbers)}]");
                }

                Console.WriteLine();
            }

            // Summary: Best bets across all scenarios
            int summaryCount = Math.Min(remainingCount, 5); // Show top 5 or less
            Console.WriteLine("=== RECOMMENDED BETS SUMMARY ===");
            Console.WriteLine($"Top {summaryCount} recommended complete bet{(summaryCount > 1 ? "s" : "")}:\n");

            int betNumber = 1;
            foreach (int remainingNum in currentCycle.RemainingNumbers.OrderByDescending(n => n).Take(summaryCount))
            {
                var prediction = GeneratePredictionByFrequencyRange(currentCycle, remainingNum, 2, 4,
                    $"Mid-range frequency (2-4 times) with remaining number {remainingNum}");

                Console.WriteLine($"Bet #{betNumber}: {string.Join("-", prediction.Numbers.OrderBy(n => n))}");
                Console.WriteLine($"  Strategy: {prediction.Rationale}");
                Console.WriteLine();
                betNumber++;
            }

            Console.WriteLine("=== END OF PREDICTIONS ===\n");
        }

        /// <summary>
        /// Generate predictions for a specific number of remaining numbers
        /// This allows testing "what if we had N numbers remaining?"
        /// </summary>
        public static void GeneratePredictionsForRemainingCount(List<MegaSenaDraw> lstMegaSena, int remainingCount)
        {
            if (remainingCount < 1 || remainingCount > 60)
            {
                Console.WriteLine($"Error: remainingCount must be between 1 and 60. Got: {remainingCount}");
                return;
            }

            // Get current cycle state
            var currentCycle = GetCurrentCycleStateFromCSV();

            if (currentCycle == null)
            {
                Console.WriteLine("Error: Could not read current cycle state.");
                return;
            }

            // Check if current cycle has the requested remaining count
            if (currentCycle.RemainingNumbers.Count == remainingCount)
            {
                Console.WriteLine($"Current cycle has exactly {remainingCount} remaining numbers. Generating predictions...\n");
                GeneratePredictions(lstMegaSena, true, null);
            }
            else
            {
                Console.WriteLine($"Note: Current cycle has {currentCycle.RemainingNumbers.Count} remaining numbers, not {remainingCount}.");
                Console.WriteLine($"To generate predictions for {remainingCount} remaining numbers, wait until a cycle reaches that state.\n");
            }
        }

        private static List<Prediction> GeneratePredictionSet(CycleState cycle, int includedRemainingNumber)
        {
            var predictions = new List<Prediction>();
            
            // Prediction 1: Focus on frequency 3 (most common in analysis)
            predictions.Add(GeneratePredictionByFrequency(cycle, includedRemainingNumber, 3, 
                "Most common: Numbers drawn 3 times (22.7% probability)"));
            
            // Prediction 2: Focus on frequency 2-4 range (62.5% combined)
            predictions.Add(GeneratePredictionByFrequencyRange(cycle, includedRemainingNumber, 2, 4,
                "Mid-range: Mix of numbers drawn 2-4 times (62.5% combined probability)"));
            
            // Prediction 3: Balanced approach with average frequency ~3
            predictions.Add(GenerateBalancedPrediction(cycle, includedRemainingNumber,
                "Balanced: Average frequency ~3.08 based on historical data"));
            
            return predictions;
        }
        
        private static Prediction GeneratePredictionByFrequency(CycleState cycle, int includedNum, int targetFreq, string rationale)
        {
            var candidates = cycle.CycleNumbers
                .Where(cn => cn.Drawn && cn.Times == targetFreq)
                .OrderBy(cn => Guid.NewGuid()) // Randomize
                .Take(5)
                .Select(cn => cn.Number)
                .ToList();
            
            // If not enough, fill with nearby frequencies
            if (candidates.Count < 5)
            {
                var additional = cycle.CycleNumbers
                    .Where(cn => cn.Drawn && !candidates.Contains(cn.Number) && Math.Abs(cn.Times - targetFreq) <= 1)
                    .OrderBy(cn => Math.Abs(cn.Times - targetFreq))
                    .ThenBy(cn => Guid.NewGuid())
                    .Take(5 - candidates.Count)
                    .Select(cn => cn.Number);
                
                candidates.AddRange(additional);
            }
            
            candidates.Add(includedNum);
            
            return new Prediction { Numbers = candidates, Rationale = rationale };
        }
        
        private static Prediction GeneratePredictionByFrequencyRange(CycleState cycle, int includedNum, int minFreq, int maxFreq, string rationale)
        {
            var candidates = cycle.CycleNumbers
                .Where(cn => cn.Drawn && cn.Times >= minFreq && cn.Times <= maxFreq)
                .OrderBy(cn => Guid.NewGuid())
                .Take(5)
                .Select(cn => cn.Number)
                .ToList();
            
            candidates.Add(includedNum);
            
            return new Prediction { Numbers = candidates, Rationale = rationale };
        }
        
        private static Prediction GenerateBalancedPrediction(CycleState cycle, int includedNum, string rationale)
        {
            // Try to get a mix that averages to ~3
            var freq2 = cycle.CycleNumbers.Where(cn => cn.Drawn && cn.Times == 2).OrderBy(cn => Guid.NewGuid()).Take(2).Select(cn => cn.Number).ToList();
            var freq3 = cycle.CycleNumbers.Where(cn => cn.Drawn && cn.Times == 3).OrderBy(cn => Guid.NewGuid()).Take(2).Select(cn => cn.Number).ToList();
            var freq4 = cycle.CycleNumbers.Where(cn => cn.Drawn && cn.Times == 4).OrderBy(cn => Guid.NewGuid()).Take(1).Select(cn => cn.Number).ToList();
            
            var candidates = new List<int>();
            candidates.AddRange(freq2);
            candidates.AddRange(freq3);
            candidates.AddRange(freq4);
            
            // Fill if needed
            while (candidates.Count < 5)
            {
                var additional = cycle.CycleNumbers
                    .Where(cn => cn.Drawn && !candidates.Contains(cn.Number) && cn.Times >= 2 && cn.Times <= 4)
                    .OrderBy(cn => Guid.NewGuid())
                    .FirstOrDefault();
                
                if (additional != null)
                    candidates.Add(additional.Number);
                else
                    break;
            }
            
            candidates.Add(includedNum);
            
            return new Prediction { Numbers = candidates.Take(6).ToList(), Rationale = rationale };
        }
        
        private static CycleState GetCurrentCycleStateFromCSV()
        {
            var state = new CycleState();
            state.CycleNumbers = new List<CycleNumber>();
            state.RemainingNumbers = new List<int>();

            // Find the in-progress CSV file
            string outputFolder = FindOutputFolder();
            var inProgressFile = Directory.GetFiles(outputFolder, "*InProgress*.csv").FirstOrDefault();

            if (inProgressFile == null)
            {
                Console.WriteLine("No in-progress cycle found!");
                return state;
            }

            var lines = File.ReadAllLines(inProgressFile);

            // Read the first 60 lines (skip header)
            for (int i = 1; i <= 60 && i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');
                if (parts.Length >= 2)
                {
                    int number = int.Parse(parts[0]);
                    int times = int.Parse(parts[1]);

                    state.CycleNumbers.Add(new CycleNumber
                    {
                        Number = number,
                        Times = times,
                        Drawn = times > 0
                    });
                }
            }

            // Find the "0 times" line to get remaining numbers
            foreach (var line in lines)
            {
                if (line.StartsWith("0 times:"))
                {
                    var numbersStr = line.Split(':')[1].Trim();
                    var numbers = numbersStr.Split(',').Select(s => int.Parse(s.Trim())).ToList();
                    state.RemainingNumbers = numbers;
                    break;
                }
            }

            return state;
        }

        private static string FindOutputFolder()
        {
            // Try multiple possible paths to support both VS Code and Visual Studio
            string[] possiblePaths = new[]
            {
                // From repository root (VS Code, dotnet run)
                Path.Combine("Output", "MegaSena"),
                // From bin/Debug/net6.0 (Visual Studio)
                Path.Combine("..", "..", "..", "..", "Output", "MegaSena"),
                // Absolute path from current directory
                Path.Combine(Directory.GetCurrentDirectory(), "Output", "MegaSena"),
                Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "Output", "MegaSena")
            };

            foreach (var path in possiblePaths)
            {
                try
                {
                    var fullPath = Path.GetFullPath(path);
                    if (Directory.Exists(fullPath))
                    {
                        return fullPath;
                    }
                }
                catch
                {
                    continue;
                }
            }

            // Fallback to first path
            return Path.GetFullPath(Path.Combine("Output", "MegaSena"));
        }
    }
}












