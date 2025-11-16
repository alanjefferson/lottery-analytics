namespace MegaSena.Entity
{
	public class MegaSenaDraw
	{
        private int concurso;
        private DateTime? dataDoSorteio;
        private int bola1;
        private int bola2;
        private int bola3;
        private int bola4;
        private int bola5;
        private int bola6;

        public int Concurso
        {
            get { return concurso; }
            set { concurso = value; }
        }
        
        public DateTime? DataDoSorteio
        {
            get { return dataDoSorteio; }
            set { dataDoSorteio = value; }
        }

        public int Bola1
        {
            get { return bola1; }
            set { bola1 = value; }
        }

        public int Bola2
        {
            get { return bola2; }
            set { bola2 = value; }
        }

        public int Bola3
        {
            get { return bola3; }
            set { bola3 = value; }
        }

        public int Bola4
        {
            get { return bola4; }
            set { bola4 = value; }
        }

        public int Bola5
        {
            get { return bola5; }
            set { bola5 = value; }
        }

        public int Bola6
        {
            get { return bola6; }
            set { bola6 = value; }
        }
    }
}







