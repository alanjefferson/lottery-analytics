using MegaSena.Entity;
using System.Collections.Generic;

namespace MegaSena.Models
{
    /// <summary>
    /// Represents the current state of a Mega-Sena cycle, including all numbers
    /// and their draw frequencies, plus which numbers remain undrawn.
    /// Used for prediction generation and cycle analysis.
    /// </summary>
    public class CycleState
    {
        public List<CycleNumber> CycleNumbers { get; set; } = new List<CycleNumber>();
        public List<int> RemainingNumbers { get; set; } = new List<int>();
    }
}









