using System;
using System.Collections.Generic;

namespace MegaSena.Models
{
    /// <summary>
    /// Generic scenario model that works with ANY number of remaining numbers (2 to 60).
    /// Tracks which remaining number gets drawn first for flexible analysis.
    /// </summary>
    public class GenericRemainingNumberScenario
    {
        public int CycleNumber { get; set; }
        public int RemainingCount { get; set; }
        public List<int> RemainingNumbers { get; set; } = new List<int>();
        public int FirstDrawn { get; set; }
        public int ConcursoFirstDrawn { get; set; }
        public DateTime? DateFirstDrawn { get; set; }
    }
}









