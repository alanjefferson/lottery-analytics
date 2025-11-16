using MegaSena.Entity;


namespace MegaSena.Core
{
	public static class MegaSenaFormat
	{
		public static MegaSenaDraw LinhaConcurso(int colNumber, string innerText, MegaSenaDraw objMegaSena)
		{
            try
            {
				if (colNumber == 0)
					objMegaSena.Concurso = Convert.ToInt32(innerText);

				if (colNumber == 1)
					objMegaSena.DataDoSorteio = Convert.ToDateTime(innerText);

				if (colNumber == 2)
					objMegaSena.Bola1 = Convert.ToInt32(innerText);

				if (colNumber == 3)
					objMegaSena.Bola2 = Convert.ToInt32(innerText);

				if (colNumber == 4)
					objMegaSena.Bola3 = Convert.ToInt32(innerText);

				if (colNumber == 5)
					objMegaSena.Bola4 = Convert.ToInt32(innerText);

				if (colNumber == 6)
					objMegaSena.Bola5 = Convert.ToInt32(innerText);

				if (colNumber == 7)
					objMegaSena.Bola6 = Convert.ToInt32(innerText);

				return objMegaSena;
			}
            catch (Exception) {

				return new MegaSenaDraw();

			}


        }
	}
}







