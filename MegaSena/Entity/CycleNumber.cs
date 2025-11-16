namespace MegaSena.Entity
{
	public class CycleNumber
	{
		private int number;
		private bool drawn;
		private int times;
		private DateTime? lastDrawn;

		public int Number { get => number; set => number = value; }
		public bool Drawn { get => drawn; set => drawn = value; }
		public int Times { get => times; set => times = value; }
		public DateTime? LastDrawn { get => lastDrawn; set => lastDrawn = value; }
	}
}






