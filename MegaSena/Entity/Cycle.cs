namespace MegaSena.Entity
{
	public class Cycle
	{
		private DateTime? startCycle;
		private DateTime? endCycle;
		private List<CycleNumber>? cycleNumbers;

		public DateTime? StartCycle { get => startCycle; set => startCycle = value; }
		public DateTime? EndCycle { get => endCycle; set => endCycle = value; }
		public List<CycleNumber> CycleNumbers { get => cycleNumbers == null ? new List<CycleNumber>() : cycleNumbers; set => cycleNumbers = value; }
	}
}






