using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace EtrendKeszito
{
	class Tapanyag:EtrendAdat
	{
		private string megnevezes;

		public string Megnevezes
		{
			get { return megnevezes; }
			private set
			{
				if (value.Trim() == "" || value.Trim().Length > 30) throw new ArgumentException("The name should be non-empty and at most 30 characters!");
				megnevezes = value.Trim();
			}
		}

		private Mertekegyseg mertek;

		public Mertekegyseg Mertek
		{
			get { return mertek; }
			set { mertek = value; }
		}

		private double napiMinBevitel;

		public double NapiMinBevitel
		{
			get { return napiMinBevitel; }
			set { napiMinBevitel = value; }
		}

		private double napiMaxBevitel;

		public double NapiMaxBevitel
		{
			get { return napiMaxBevitel; }
			set { napiMaxBevitel = value; }
		}

		private bool napiMax;// true, ha a NapiMaxBevitel-t a program nézi, egyébként false

		public bool NapiMax
		{
			get { return napiMax; }
			set { napiMax = value; }
		}

		private bool hasznalhato;

		public bool Hasznalhato
		{
			get { return hasznalhato; }
			set { hasznalhato = value; }
		}

		public Tapanyag(string felhasznaloNevHash,string megnevezes, Mertekegyseg mertek, double napiMinBevitel, double napiMaxBevitel, bool napiMax,bool hasznalhato): base(felhasznaloNevHash)
		{
			if(napiMax&&napiMinBevitel>napiMaxBevitel)throw new ArgumentException("The minimum daily intake should not be less than the maximal intake!");

			if(napiMax&&napiMaxBevitel<0.0)throw new ArgumentException("The daily maximal intake should ne non-negative!");

			Megnevezes = megnevezes;
			Mertek = mertek;
			NapiMinBevitel = napiMinBevitel;
			NapiMaxBevitel = napiMaxBevitel;
			NapiMax = napiMax;
			Hasznalhato = hasznalhato;
		}

		public Tapanyag(XElement t,List<Mertekegyseg>mertekegysegek) : base(t.Element("EtrendAdat"))
		{
			Megnevezes = t.Attribute("Megnevezes").Value;
			Mertek=mertekegysegek.Where(x=>x.Megnevezes== t.Attribute("MertekMegnevezes").Value).ToList().First();
			//Mertek = (Mertekegyseg) ABKezelo.Kiolvasas().Where(x => x is Mertekegyseg && (x as Mertekegyseg).Megnevezes == t.Attribute("MertekMegnevezes").Value).ToList().First();
			NapiMinBevitel = double.Parse(t.Attribute("NapiMinBevitel").Value, NumberStyles.Any, new CultureInfo("en-US"));
			NapiMaxBevitel = double.Parse(t.Attribute("NapiMaxBevitel").Value, NumberStyles.Any, new CultureInfo("en-US"));
			NapiMax = t.Attribute("NapiMax").Value == "1";
			Hasznalhato = t.Attribute("Hasznalhato").Value == "1";
		}

		public override XElement ToXML()
		{
			return new XElement("Tapanyag",
				new XAttribute("Megnevezes", Megnevezes),
				new XAttribute("MertekMegnevezes", Mertek.Megnevezes),
				new XAttribute("NapiMinBevitel", NapiMinBevitel),
				new XAttribute("NapiMaxBevitel", NapiMaxBevitel),
				new XAttribute("NapiMax", NapiMax ? "1" : "0"),
				new XAttribute("Hasznalhato", Hasznalhato ? "1" : "0"),
				base.ToXML());
		}

		public override string ToString()
		{
			return Megnevezes + ";" + NapiMinBevitel +" "+ Mertek + ";" + (NapiMax ? (NapiMaxBevitel+" "+Mertek):"-");
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Tapanyag)) return false;

			Tapanyag t = obj as Tapanyag;

			bool eq=FelhasznaloNevHash == t.FelhasznaloNevHash && Megnevezes == t.Megnevezes;
			return eq;
		}

		public override int GetHashCode()
		{
			var hc = FelhasznaloNevHash.GetHashCode();
			hc = unchecked((hc * 7) ^ Megnevezes.GetHashCode());
			return hc;
		}
	}
}
