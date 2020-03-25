using System;

namespace EtrendKeszito
{
	class ABHiba
	{
		private string uzenet;

		public string Uzenet
		{
			get { return uzenet; }
			set { uzenet = value; }
		}

		private EtrendAdat hibas;

		public EtrendAdat Hibas
		{
			get { return hibas; }
			set { hibas = value; }
		}

		public ABHiba(string uzenet, EtrendAdat hibas)
		{
			Uzenet = uzenet;
			Hibas = hibas;
		}

		public override string ToString()
		{
			return uzenet;
		}
	}
}
