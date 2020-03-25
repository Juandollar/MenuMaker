using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace EtrendKeszito
{
	class Etel : Elelmiszer
	{
		private double egysegTomegMennyiseg;

		public double EgysegTomegMennyiseg
		{
			get { return egysegTomegMennyiseg; }
			set
			{
				if (value > 0) egysegTomegMennyiseg = value;
				else throw new ArgumentException("The unit value should be positive!");
			}
		}

		private double minTomeg;

		public double MinTomeg
		{
			get { return minTomeg; }
			set { minTomeg = value; }
		}

		private double maxTomeg;

		public double MaxTomeg
		{
			get { return maxTomeg; }
			set { maxTomeg = value; }
		}

		private bool maxTomegE;

		public bool MaxTomegE
		{
			get { return maxTomegE; }
			set { maxTomegE = value; }
		}


		private Mertekegyseg tomegMertek;

		public Mertekegyseg TomegMertek
		{
			get { return tomegMertek; }
			set
			{
				if (value == null)
				{
					tomegMertek = null;
					return;
				}
				if (value.Mertek == MertekegysegFajta.weight) tomegMertek = value;
				else throw new ArgumentException("For meal you can only use mass unit!");
			}
		}

		public Etel(string felhasznaloNevHash, string megnevezes, byte orom, Penznem penz, double ar, bool egysegTobbszorose, List<bool> fogyaszthato, bool valtozatossag, bool hasznalhato, Dictionary<Tapanyag, double> tapanyagTartalom, double egysegTomegMennyiseg, double minTomeg, double maxTomeg, bool maxTomegE, Mertekegyseg tomegMertek) : base(felhasznaloNevHash, megnevezes, orom, penz, ar, egysegTobbszorose, fogyaszthato, valtozatossag, hasznalhato, tapanyagTartalom)
		{
			EgysegTomegMennyiseg = egysegTomegMennyiseg;
			MinTomeg = minTomeg;
			MaxTomeg = maxTomeg;
			MaxTomegE = maxTomegE;
			TomegMertek = tomegMertek;
		}

		public Etel(XElement e,List<Penznem>penznemek,List<Mertekegyseg>mertekegysegek) : base(e.Element("Elelmiszer"),penznemek)
		{
			EgysegTomegMennyiseg = double.Parse(e.Attribute("EgysegTomegMennyiseg").Value, NumberStyles.Any, new CultureInfo("en-US"));
			MinTomeg = double.Parse(e.Attribute("MinTomeg").Value, NumberStyles.Any, new CultureInfo("en-US"));
			MaxTomeg = double.Parse(e.Attribute("MaxTomeg").Value, NumberStyles.Any, new CultureInfo("en-US"));
			MaxTomegE = e.Attribute("MaxTomegE").Value == "1";
			TomegMertek=mertekegysegek.Where(x=>x.Megnevezes== e.Attribute("TomegMegnevezes").Value).ToList().First();
            //TomegMertek = (Mertekegyseg)ABKezelo.Kiolvasas().Where(x => x is Mertekegyseg && (x as Mertekegyseg).Megnevezes == e.Attribute("TomegMegnevezes").Value).ToList().First();
		}

		public override XElement ToXML()
		{
			return new XElement("Etel",
				new XAttribute("EgysegTomegMennyiseg", EgysegTomegMennyiseg),
				new XAttribute("MinTomeg", MinTomeg),
				new XAttribute("MaxTomeg", MaxTomeg),
				new XAttribute("MaxTomegE", MaxTomegE ? "1" : "0"),
				new XAttribute("TomegMegnevezes", TomegMertek.Megnevezes),
				base.ToXML());
		}

		public override string ToString()
		{
			string str = String.Concat("{0} (meal, {1:F", Penz.TizedesekSzama, "} {2}); {3:0.##} {4}");
			return String.Format(str, Megnevezes, Ar, Penz.Megnevezes, EgysegTomegMennyiseg, TomegMertek.Megnevezes);
		}
	}
}
