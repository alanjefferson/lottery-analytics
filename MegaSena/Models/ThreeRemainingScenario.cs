using System;
using System.Collections.Generic;

namespace MegaSena.Models
{
    /// <summary>
    /// Represents a scenario when exactly 3 numbers remain in a Mega-Sena cycle.
    /// Used to analyze winning number patterns at this critical stage (95% cycle completion).
    /// </summary>
    public class ThreeRemainingScenario
    {
        public int Concurso { get; set; }
        public DateTime? Date { get; set; }
        public int CycleNumber { get; set; }
        public List<int> WinningNumbers { get; set; } = new List<int>();
        public List<int> FrequenciesOfWinningNumbers { get; set; } = new List<int>();
        public List<int> RemainingNumbers { get; set; } = new List<int>();
    }
}









