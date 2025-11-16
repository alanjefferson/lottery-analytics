using System;
using System.Collections.Generic;

namespace MegaSena.Models
{
    /// <summary>
    /// Represents a scenario tracking how many draws it takes from when 3 numbers remain
    /// until one of them is finally picked. Used for timeline prediction analysis.
    /// </summary>
    public class DrawsUntilPickedScenario
    {
        public int CycleNumber { get; set; }
        public List<int> RemainingNumbers { get; set; } = new List<int>();
        public int ConcursoWhenThreeRemaining { get; set; }
        public DateTime? DateWhenThreeRemaining { get; set; }
        public int DrawsUntilOnePicked { get; set; }
        public int NumberPicked { get; set; }
        public int ConcursoWhenPicked { get; set; }
        public DateTime? DateWhenPicked { get; set; }
    }
}









