using System;
//megoldásnál EtrendElelmiszer-eket használva mentjük el

namespace EtrendKeszito
{
	class EtrendElelmiszer:EtrendAdat
	{
		private DateTime datum;

		public DateTime Datum
		{
			get { return datum; }
			set { datum = value; }
		}

		private EtkezesTipus etkezes;

		public EtkezesTipus Etkezes
		{
			get { return etkezes; }
			set { etkezes = value; }
		}

		private Elelmiszer elelmiszer;

		public Elelmiszer Elelmiszer
		{
			get { return elelmiszer; }
			set { elelmiszer = value; }
		}

		private double val;

		public double Val
		{
			get { return val; }
			set { val = value; }
		}

		private double tomeg;

		public double Tomeg
		{
			get { return tomeg; }
			set { tomeg = value; }
		}

		private Mertekegyseg tomegmertekegyseg;

		public Mertekegyseg Tomegmertekegyseg
		{
			get { return tomegmertekegyseg; }
			set { tomegmertekegyseg = value; }
		}

		private double urmertek;

		public double Urmertek
		{
			get { return urmertek; }
			set { urmertek = value; }
		}

		private Mertekegyseg urmertekegyseg;

		public Mertekegyseg Urmertekegyseg
		{
			get { return urmertekegyseg; }
			set { urmertekegyseg = value; }
		}

		public EtrendElelmiszer(string felhasznaloNevHash, DateTime datum, EtkezesTipus etkezes, Elelmiszer elelmiszer, double val, double tomeg, Mertekegyseg tomegmertekegyseg, double urmertek, Mertekegyseg urmertekegyseg) : base(felhasznaloNevHash)
		{
			Datum = datum;
			Etkezes = etkezes;
			Elelmiszer = elelmiszer;
			Val = val;
			Tomeg = tomeg;
			Tomegmertekegyseg = tomegmertekegyseg;
			Urmertek = urmertek;
			Urmertekegyseg = urmertekegyseg;
		}
	}
}
