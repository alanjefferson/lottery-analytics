using MegaSena.Core;
using MegaSena.Entity;


namespace MegaSena
{
	public class CycleResults
	{
		int numRef = 60;

		public CycleResults(int num = 0) 
		{
			if(num > 0)
				numRef = num;
		}

		public List<Cycle> GetCycleList(List<MegaSenaDraw> lstMegaSena)
		{
			Cycle objCycle = new Cycle();
			objCycle.CycleNumbers = createCycleNumbers();

			List<Cycle> lstCycle = new List<Cycle>();
			DateTime? endCycleDate = new DateTime?();
			int lastNumber = 0;
			bool isFirstDraw = true;

			foreach (MegaSenaDraw obj in lstMegaSena)
			{
				if (allNumberHasBeenDrawn(objCycle))
				{
					objCycle.EndCycle = endCycleDate;
					lstCycle.Add(objCycle);

					CycleResultsWriter.WriteCycleToCSV(objCycle, endCycleDate, lastNumber);

					Cycle newObjCycle = CreateNewCycleWithLastNumbersDate(objCycle);
					objCycle = newObjCycle;
					isFirstDraw = true;
				}

				FlagCountUpdateDateNumbers(objCycle, obj);

				// Set the start date when the first number is drawn in the cycle
				if (isFirstDraw && objCycle.StartCycle == null)
				{
					objCycle.StartCycle = obj.DataDoSorteio;
					isFirstDraw = false;
				}

				endCycleDate = obj.DataDoSorteio;
				lastNumber = obj.Concurso;

				if (lstMegaSena.Last().Concurso == obj.Concurso)
				{
					CycleResultsWriter.WriteCycleToCSV(objCycle, null, lastNumber, true);
				}

			}

			return lstCycle;
		}

		private List<CycleNumber> createCycleNumbers()
		{
			List<CycleNumber> lstCycleNumber = new List<CycleNumber>();

			for (int i = 1; i <= numRef; i++)
			{
				CycleNumber objCycleNumber = new CycleNumber();
				objCycleNumber.Number = i;
				lstCycleNumber.Add(objCycleNumber);
			}

			return lstCycleNumber;
		}

		private bool allNumberHasBeenDrawn(Cycle objCycle)
		{
			foreach (CycleNumber item in objCycle.CycleNumbers)
				if (!item.Drawn)
					return false;

			return true;
		}

		private void FlagCountUpdateDateNumbers(Cycle objCycle, MegaSenaDraw obj)
		{
			for (int i = 1; i <= numRef; i++)
			{
				if (obj.Bola1 == i || obj.Bola2 == i || obj.Bola3 == i || obj.Bola4 == i || obj.Bola5 == i || obj.Bola6 == i)
				{
					objCycle.CycleNumbers[i - 1].Drawn = true;
					objCycle.CycleNumbers[i - 1].Times++;
					objCycle.CycleNumbers[i - 1].LastDrawn = obj.DataDoSorteio;
				}
			}
		}



		private Cycle CreateNewCycleWithLastNumbersDate(Cycle objCycle)
		{
			Cycle newObjCycle = new Cycle();
			newObjCycle.CycleNumbers = createCycleNumbers();

			for (int i = 0; i < numRef; i++)
				newObjCycle.CycleNumbers[i].LastDrawn = objCycle.CycleNumbers[i].LastDrawn;

			return newObjCycle;
		}
	}
}






