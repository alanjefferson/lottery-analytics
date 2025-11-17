namespace MegaSena.Api.Models
{
    /// <summary>
    /// Response model for MegaSena draw predictions
    /// </summary>
    public class PredictionResponse
    {
        /// <summary>
        /// Numbers that haven't been drawn yet in the current cycle
        /// </summary>
        public List<int> RemainingNumbers { get; set; } = new();

        /// <summary>
        /// Total count of remaining numbers
        /// </summary>
        public int TotalRemaining { get; set; }

        /// <summary>
        /// Current frequency distribution of all numbers
        /// </summary>
        public List<FrequencyGroup> FrequencyDistribution { get; set; } = new();

        /// <summary>
        /// Recommended betting combinations
        /// </summary>
        public List<BettingRecommendation> RecommendedBets { get; set; } = new();

        /// <summary>
        /// Predictions for each remaining number scenario
        /// </summary>
        public List<ScenarioPrediction> Scenarios { get; set; } = new();

        /// <summary>
        /// Current cycle information
        /// </summary>
        public CycleInfo? CurrentCycle { get; set; }
    }

    /// <summary>
    /// Frequency group showing numbers drawn a specific number of times
    /// </summary>
    public class FrequencyGroup
    {
        /// <summary>
        /// Number of times these numbers have been drawn
        /// </summary>
        public int Frequency { get; set; }

        /// <summary>
        /// Numbers that have been drawn this many times
        /// </summary>
        public List<int> Numbers { get; set; } = new();
    }

    /// <summary>
    /// Betting recommendation with strategy information
    /// </summary>
    public class BettingRecommendation
    {
        /// <summary>
        /// The 6 numbers to bet (sorted)
        /// </summary>
        public List<int> Numbers { get; set; } = new();

        /// <summary>
        /// Strategy used for this recommendation
        /// </summary>
        public string Strategy { get; set; } = string.Empty;

        /// <summary>
        /// Which remaining number is included in this bet
        /// </summary>
        public int RemainingNumberIncluded { get; set; }

        /// <summary>
        /// Formatted bet string (e.g., "2-12-15-29-41-44")
        /// </summary>
        public string FormattedBet { get; set; } = string.Empty;
    }

    /// <summary>
    /// Prediction scenario for a specific remaining number
    /// </summary>
    public class ScenarioPrediction
    {
        /// <summary>
        /// The remaining number this scenario is based on
        /// </summary>
        public int RemainingNumber { get; set; }

        /// <summary>
        /// Strategies for this scenario
        /// </summary>
        public List<StrategyBet> Strategies { get; set; } = new();
    }

    /// <summary>
    /// A betting strategy with its details
    /// </summary>
    public class StrategyBet
    {
        /// <summary>
        /// Strategy name (e.g., "Most common", "Mid-range", "Balanced")
        /// </summary>
        public string StrategyName { get; set; } = string.Empty;

        /// <summary>
        /// Strategy description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// The 6 numbers for this strategy
        /// </summary>
        public List<int> Numbers { get; set; } = new();

        /// <summary>
        /// Formatted bet string
        /// </summary>
        public string FormattedBet { get; set; } = string.Empty;
    }

    /// <summary>
    /// Information about the current cycle
    /// </summary>
    public class CycleInfo
    {
        /// <summary>
        /// Cycle start date
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Last draw number (Concurso)
        /// </summary>
        public int LastConcurso { get; set; }

        /// <summary>
        /// Number of draws in this cycle so far
        /// </summary>
        public int DrawsInCycle { get; set; }

        /// <summary>
        /// Days since cycle started
        /// </summary>
        public int DaysSinceStart { get; set; }
    }
}

