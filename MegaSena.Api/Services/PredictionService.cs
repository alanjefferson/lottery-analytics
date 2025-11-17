using MegaSena.Api.Models;
using MegaSena.Models;

namespace MegaSena.Api.Services
{
    /// <summary>
    /// Service for generating MegaSena predictions
    /// Wraps the existing PredictNextDraw analysis logic and converts to API DTOs
    /// </summary>
    public class PredictionService
    {
        /// <summary>
        /// Get predictions for the next MegaSena draw
        /// </summary>
        /// <returns>Prediction response with recommendations and scenarios</returns>
        public PredictionResponse GetNextDrawPrediction()
        {
            // Use the existing PredictNextDraw class to get cycle state from CSV
            var cycleState = PredictNextDraw.GetCurrentCycleStateFromCSV();

            if (cycleState == null || cycleState.RemainingNumbers.Count == 0)
            {
                return new PredictionResponse
                {
                    RemainingNumbers = new List<int>(),
                    TotalRemaining = 0,
                    FrequencyDistribution = new List<FrequencyGroup>(),
                    RecommendedBets = new List<BettingRecommendation>(),
                    Scenarios = new List<ScenarioPrediction>(),
                    CurrentCycle = null
                };
            }

            // Convert cycle state to API response format
            var response = ConvertToApiResponse(cycleState);

            return response;
        }

        /// <summary>
        /// Convert CycleState from Analysis layer to API response format
        /// </summary>
        private PredictionResponse ConvertToApiResponse(CycleState cycleState)
        {
            var response = new PredictionResponse
            {
                RemainingNumbers = cycleState.RemainingNumbers,
                TotalRemaining = cycleState.RemainingNumbers.Count,
                CurrentCycle = null // We don't have cycle info from CSV, could be enhanced later
            };

            // Build frequency distribution from cycle state
            var frequencyGroups = cycleState.CycleNumbers
                .Where(cn => cn.Drawn)
                .GroupBy(cn => cn.Times)
                .OrderByDescending(g => g.Key)
                .Select(g => new FrequencyGroup
                {
                    Frequency = g.Key,
                    Numbers = g.Select(cn => cn.Number).OrderBy(n => n).ToList()
                })
                .ToList();

            response.FrequencyDistribution = frequencyGroups;

            // Generate scenarios using the existing PredictNextDraw logic
            response.Scenarios = GenerateScenarios(cycleState);
            response.RecommendedBets = GenerateRecommendedBets(cycleState);

            return response;
        }

        /// <summary>
        /// Generate prediction scenarios using the existing PredictNextDraw logic
        /// </summary>
        private List<ScenarioPrediction> GenerateScenarios(CycleState cycleState)
        {
            var scenarios = new List<ScenarioPrediction>();

            // Generate scenarios for each remaining number using the existing logic
            foreach (var remainingNum in cycleState.RemainingNumbers)
            {
                // Use the existing PredictNextDraw.GeneratePredictionSet method
                var predictions = PredictNextDraw.GeneratePredictionSet(cycleState, remainingNum);

                var scenario = new ScenarioPrediction
                {
                    RemainingNumber = remainingNum,
                    Strategies = predictions.Select(p => new StrategyBet
                    {
                        StrategyName = ExtractStrategyName(p.Rationale),
                        Description = p.Rationale,
                        Numbers = p.Numbers.OrderBy(n => n).ToList(),
                        FormattedBet = string.Join("-", p.Numbers.OrderBy(n => n))
                    }).ToList()
                };

                scenarios.Add(scenario);
            }

            return scenarios;
        }

        /// <summary>
        /// Generate recommended bets using the existing PredictNextDraw logic
        /// </summary>
        private List<BettingRecommendation> GenerateRecommendedBets(CycleState cycleState)
        {
            var recommendations = new List<BettingRecommendation>();

            // Generate top recommendations (one per remaining number, using mid-range strategy)
            foreach (var remainingNum in cycleState.RemainingNumbers.Take(5))
            {
                // Use the existing mid-range strategy from PredictNextDraw
                var prediction = PredictNextDraw.GeneratePredictionByFrequencyRange(
                    cycleState,
                    remainingNum,
                    2,
                    4,
                    $"Mid-range frequency (2-4 times) with remaining number {remainingNum}");

                recommendations.Add(new BettingRecommendation
                {
                    Numbers = prediction.Numbers.OrderBy(n => n).ToList(),
                    Strategy = prediction.Rationale,
                    RemainingNumberIncluded = remainingNum,
                    FormattedBet = string.Join("-", prediction.Numbers.OrderBy(n => n))
                });
            }

            return recommendations;
        }

        /// <summary>
        /// Extract a short strategy name from the rationale
        /// </summary>
        private string ExtractStrategyName(string rationale)
        {
            if (rationale.Contains("Most common"))
                return "Most Common";
            if (rationale.Contains("Mid-range"))
                return "Mid-Range";
            if (rationale.Contains("Balanced"))
                return "Balanced";
            return "Strategy";
        }
    }
}

