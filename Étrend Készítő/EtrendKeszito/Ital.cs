using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace EtrendKeszito
{
	class Ital:Elelmiszer
	{
		private double egysegUrTartalomMennyiseg;

		public double EgysegUrTartalomMennyiseg
		{
			get { return egysegUrTartalomMennyiseg; }
			set
			{
				if (value > 0) egysegUrTartalomMennyiseg = value;
				else throw new ArgumentException("The unit value should be positive!");
			}
		}

		private double minUrTartalom;

		public double MinUrTartalom
		{
			get { return minUrTartalom; }
			set { minUrTartalom = value; }
		}

		private double maxUrTartalom;

		public double MaxUrTartalom
		{
			get { return maxUrTartalom; }
			set { maxUrTartalom = value; }
		}

		private bool maxUrTartalomE;

		public bool MaxUrTartalomE
		{
			get { return maxUrTartalomE; }
			set { maxUrTartalomE = value; }
		}


		private Mertekegyseg urmertek;

		public Mertekegyseg Urmertek
		{
			get { return urmertek; }
			set
			{
				if (value == null)
				{
					urmertek = null;
					return;
				}
				if (value.Mertek == MertekegysegFajta.liquidmeasure) urmertek = value;
				else throw new ArgumentException("For drink you can only use capacity unit!");
			}
		}

		public Ital(string felhasznaloNevHash, string megnevezes, byte orom, Penznem penz, double ar, bool egysegTobbszorose, List<bool> fogyaszthato, bool valtozatossag, bool hasznalhato, Dictionary<Tapanyag, double> tapanyagTartalom, double egysegUrTartalomMennyiseg, double minUrTartalom, double maxUrTartalom, bool maxUrTartalomE, Mertekegyseg urMertek) : base(felhasznaloNevHash, megnevezes, orom, penz, ar, egysegTobbszorose, fogyaszthato, valtozatossag, hasznalhato, tapanyagTartalom)
		{
			EgysegUrTartalomMennyiseg = egysegUrTartalomMennyiseg;
			MinUrTartalom = minUrTartalom;
			MaxUrTartalom = maxUrTartalom;
			MaxUrTartalomE = maxUrTartalomE;
			Urmertek = urMertek;
		}

		public Ital(XElement e,List<Penznem>penznemek,List<Mertekegyseg>mertekegysegek) : base(e.Element("Elelmiszer"),penznemek)
		{
			EgysegUrTartalomMennyiseg = double.Parse(e.Attribute("EgysegUrTartalomMennyiseg").Value, NumberStyles.Any, new CultureInfo("en-US"));
			MinUrTartalom = double.Parse(e.Attribute("MinUrTartalom").Value, NumberStyles.Any, new CultureInfo("en-US"));
			MaxUrTartalom = double.Parse(e.Attribute("MaxUrTartalom").Value, NumberStyles.Any, new CultureInfo("en-US"));
			MaxUrTartalomE = e.Attribute("MaxUrTartalomE").Value == "1";
            Urmertek=mertekegysegek.Where(x=>x.Megnevezes==e.Attribute("UrmertekMegnevezes").Value).ToList().First();
            //Urmertek = (Mertekegyseg)ABKezelo.Kiolvasas().Where(x => x is Mertekegyseg && (x as Mertekegyseg).Megnevezes == e.Attribute("UrmertekMegnevezes").Value).ToList().First();
		}

		public override XElement ToXML()
		{
			return new XElement("Ital",
				new XAttribute("EgysegUrTartalomMennyiseg", EgysegUrTartalomMennyiseg),
				new XAttribute("MinUrTartalom", MinUrTartalom),
				new XAttribute("MaxUrTartalom", MaxUrTartalom),
				new XAttribute("MaxUrTartalomE", MaxUrTartalomE ? "1" : "0"),
				new XAttribute("UrmertekMegnevezes", Urmertek.Megnevezes),
				base.ToXML());
		}

		public override string ToString()
		{
			string str = String.Concat("{0} (drink, {1:F", Penz.TizedesekSzama, "} {2}); {3:0.##} {4}");
			return String.Format(str, Megnevezes, Ar, Penz.Megnevezes, EgysegUrTartalomMennyiseg, Urmertek.Megnevezes);
		}
	}
}
