using MegaSena.Api.Models;
using MegaSena.Core;
using MegaSena.Entity;
using MegaSena.Models;

namespace MegaSena.Api.Services
{
    /// <summary>
    /// Service for generating MegaSena predictions
    /// </summary>
    public class PredictionService
    {
        /// <summary>
        /// Get predictions for the next MegaSena draw
        /// </summary>
        /// <returns>Prediction response with recommendations and scenarios</returns>
        public PredictionResponse GetNextDrawPrediction()
        {
            // Read all MegaSena draws
            List<MegaSenaDraw> lstMegaSena = MegaSenaResults.ReadMegaSenaXLSX();

            // Get current cycle state
            var cycleState = GetCurrentCycleState(lstMegaSena);

            if (cycleState.RemainingNumbers.Count == 0)
            {
                return new PredictionResponse
                {
                    RemainingNumbers = new List<int>(),
                    TotalRemaining = 0,
                    FrequencyDistribution = new List<FrequencyGroup>(),
                    RecommendedBets = new List<BettingRecommendation>(),
                    Scenarios = new List<ScenarioPrediction>(),
                    CurrentCycle = cycleState.CycleInfo
                };
            }

            // Generate predictions using the existing logic
            var predictions = GeneratePredictions(lstMegaSena, cycleState);

            return predictions;
        }

        private CycleStateInfo GetCurrentCycleState(List<MegaSenaDraw> draws)
        {
            var state = new CycleStateInfo();
            
            // Track which numbers have been drawn
            bool[] drawnNumbers = new bool[61]; // 1-60
            Cycle currentCycle = new Cycle();
            DateTime? cycleStartDate = null;
            int drawsInCycle = 0;

            for (int i = 0; i < draws.Count; i++)
            {
                var draw = draws[i];
                
                if (cycleStartDate == null)
                    cycleStartDate = draw.DataDoSorteio;

                drawsInCycle++;

                // Mark numbers as drawn and update cycle
                int[] numbers = { draw.Bola1, draw.Bola2, draw.Bola3, draw.Bola4, draw.Bola5, draw.Bola6 };
                foreach (int num in numbers)
                {
                    if (!drawnNumbers[num])
                    {
                        drawnNumbers[num] = true;
                        currentCycle.CycleNumbers[num - 1].Times++;
                    }
                    else
                    {
                        currentCycle.CycleNumbers[num - 1].Times++;
                    }
                    currentCycle.CycleNumbers[num - 1].LastDrawn = draw.DataDoSorteio;
                }

                // Check if cycle is complete
                bool cycleComplete = true;
                for (int n = 1; n <= 60; n++)
                {
                    if (!drawnNumbers[n])
                    {
                        cycleComplete = false;
                        break;
                    }
                }

                // If cycle complete and not last draw, reset for new cycle
                if (cycleComplete && i < draws.Count - 1)
                {
                    drawnNumbers = new bool[61];
                    currentCycle = new Cycle();
                    cycleStartDate = null;
                    drawsInCycle = 0;
                }
            }

            // Get remaining numbers
            for (int n = 1; n <= 60; n++)
            {
                if (!drawnNumbers[n])
                {
                    state.RemainingNumbers.Add(n);
                }
            }

            state.CurrentCycle = currentCycle;
            state.CycleInfo = new CycleInfo
            {
                StartDate = cycleStartDate,
                LastConcurso = draws.Last().Concurso,
                DrawsInCycle = drawsInCycle,
                DaysSinceStart = cycleStartDate.HasValue 
                    ? (DateTime.Now - cycleStartDate.Value).Days 
                    : 0
            };

            return state;
        }

        private PredictionResponse GeneratePredictions(List<MegaSenaDraw> draws, CycleStateInfo cycleState)
        {
            var response = new PredictionResponse
            {
                RemainingNumbers = cycleState.RemainingNumbers,
                TotalRemaining = cycleState.RemainingNumbers.Count,
                CurrentCycle = cycleState.CycleInfo
            };

            // Build frequency distribution
            var frequencyGroups = new Dictionary<int, List<int>>();
            for (int i = 0; i < 60; i++)
            {
                int times = cycleState.CurrentCycle.CycleNumbers[i].Times;
                int number = cycleState.CurrentCycle.CycleNumbers[i].Number;

                if (!frequencyGroups.ContainsKey(times))
                    frequencyGroups[times] = new List<int>();

                frequencyGroups[times].Add(number);
            }

            // Convert to FrequencyGroup list for API response
            response.FrequencyDistribution = frequencyGroups
                .OrderByDescending(x => x.Key)
                .Select(x => new FrequencyGroup
                {
                    Frequency = x.Key,
                    Numbers = x.Value.OrderBy(n => n).ToList()
                })
                .ToList();

            // Generate scenarios and recommendations
            // This is a simplified version - you can enhance it with the full PredictNextDraw logic
            response.Scenarios = GenerateScenarios(cycleState, frequencyGroups);
            response.RecommendedBets = GenerateRecommendedBets(cycleState, frequencyGroups);

            return response;
        }

        private List<ScenarioPrediction> GenerateScenarios(CycleStateInfo cycleState, Dictionary<int, List<int>> frequencyGroups)
        {
            var scenarios = new List<ScenarioPrediction>();

            // Get numbers by frequency for strategies
            var mostCommonFreq = frequencyGroups.Keys.Max();
            var mostCommonNumbers = frequencyGroups[mostCommonFreq];

            foreach (var remainingNum in cycleState.RemainingNumbers.Take(3)) // Top 3 scenarios
            {
                var scenario = new ScenarioPrediction
                {
                    RemainingNumber = remainingNum,
                    Strategies = new List<StrategyBet>()
                };

                // Strategy 1: Most common frequency
                var strategy1Numbers = new List<int> { remainingNum };
                strategy1Numbers.AddRange(mostCommonNumbers.Where(n => n != remainingNum).Take(5));
                scenario.Strategies.Add(new StrategyBet
                {
                    StrategyName = "Most Common",
                    Description = $"Numbers drawn {mostCommonFreq} times",
                    Numbers = strategy1Numbers.OrderBy(n => n).ToList(),
                    FormattedBet = string.Join("-", strategy1Numbers.OrderBy(n => n))
                });

                // Strategy 2: Mid-range frequency (2-4 times)
                var midRangeNumbers = frequencyGroups
                    .Where(kvp => kvp.Key >= 2 && kvp.Key <= 4)
                    .SelectMany(kvp => kvp.Value)
                    .Where(n => n != remainingNum)
                    .OrderBy(n => n)
                    .Take(5)
                    .ToList();

                var strategy2Numbers = new List<int> { remainingNum };
                strategy2Numbers.AddRange(midRangeNumbers);
                scenario.Strategies.Add(new StrategyBet
                {
                    StrategyName = "Mid-Range",
                    Description = "Mix of numbers drawn 2-4 times",
                    Numbers = strategy2Numbers.OrderBy(n => n).ToList(),
                    FormattedBet = string.Join("-", strategy2Numbers.OrderBy(n => n))
                });

                // Strategy 3: Balanced approach
                var balancedNumbers = frequencyGroups
                    .OrderByDescending(kvp => kvp.Key)
                    .SelectMany(kvp => kvp.Value)
                    .Where(n => n != remainingNum)
                    .Take(5)
                    .ToList();

                var strategy3Numbers = new List<int> { remainingNum };
                strategy3Numbers.AddRange(balancedNumbers);
                scenario.Strategies.Add(new StrategyBet
                {
                    StrategyName = "Balanced",
                    Description = "Balanced frequency distribution",
                    Numbers = strategy3Numbers.OrderBy(n => n).ToList(),
                    FormattedBet = string.Join("-", strategy3Numbers.OrderBy(n => n))
                });

                scenarios.Add(scenario);
            }

            return scenarios;
        }

        private List<BettingRecommendation> GenerateRecommendedBets(CycleStateInfo cycleState, Dictionary<int, List<int>> frequencyGroups)
        {
            var recommendations = new List<BettingRecommendation>();

            // Generate top 3 recommendations using mid-range strategy
            var midRangeNumbers = frequencyGroups
                .Where(kvp => kvp.Key >= 2 && kvp.Key <= 4)
                .SelectMany(kvp => kvp.Value)
                .OrderBy(n => n)
                .ToList();

            foreach (var remainingNum in cycleState.RemainingNumbers.Take(3))
            {
                var betNumbers = new List<int> { remainingNum };
                betNumbers.AddRange(midRangeNumbers.Where(n => n != remainingNum).Take(5));
                betNumbers = betNumbers.OrderBy(n => n).ToList();

                recommendations.Add(new BettingRecommendation
                {
                    Numbers = betNumbers,
                    Strategy = "Mid-range frequency (2-4 times)",
                    RemainingNumberIncluded = remainingNum,
                    FormattedBet = string.Join("-", betNumbers)
                });
            }

            return recommendations;
        }

        private class CycleStateInfo
        {
            public List<int> RemainingNumbers { get; set; } = new();
            public Cycle CurrentCycle { get; set; } = new();
            public CycleInfo? CycleInfo { get; set; }
        }
    }
}

