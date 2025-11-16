using System;
using System.Collections.Generic;

namespace MegaSena.Models
{
    /// <summary>
    /// Represents a scenario tracking which of the remaining numbers gets drawn first.
    /// Used for position analysis (lowest, middle, highest) when 3 numbers remain.
    /// </summary>
    public class RemainingNumberScenario
    {
        public int CycleNumber { get; set; }
        public List<int> RemainingNumbers { get; set; } = new List<int>();
        public int FirstDrawn { get; set; }
        public int ConcursoFirstDrawn { get; set; }
        public DateTime? DateFirstDrawn { get; set; }
    }
}









