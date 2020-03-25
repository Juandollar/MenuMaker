using System;
using System.Globalization;
using System.Xml.Linq;

namespace EtrendKeszito
{
	class Penznem:EtrendAdat
	{
		private string megnevezes;

		public string Megnevezes
		{
			get { return megnevezes; }
			private set
			{
				if (value == ""||value.Trim().Length>30) throw new ArgumentException("The name should be non-empty and at most 30 characters length!");
				megnevezes = value;
			}
		}

		private string penzKod;//3 betűs pénzkód

		public string PenzKod
		{
			get { return penzKod; }
			private set
			{
				if(value.Length!=3)throw new ArgumentException("The currency code should be exactly 3 characters!");
				penzKod = value;
			}
		}
		
		private double arfolyam;

		public double Arfolyam
		{
			get { return arfolyam; }
			set
			{
				if(value<=0.0)throw new ArgumentException("The exchange rate should be positive !");
				arfolyam = value;
			}
		}

		private byte tizedesekSzama;

		public byte TizedesekSzama
		{
			get { return tizedesekSzama; }
			set
			{
				if(value>=0&&value<=6)tizedesekSzama = value;
				else throw new ArgumentException("The number of decimals should be between 0 and 6 !");
			}
		}

		private bool hasznalhato;

		public bool Hasznalhato
		{
			get { return hasznalhato; }
			set { hasznalhato = value; }
		}

		private bool torolheto;

		public bool Torolheto
		{
			get { return torolheto; }
			private set { torolheto = value; }
		}

		public Penznem(string felhasznaloNevHash, string megnevezes, string penzKod, double arfolyam, byte tizedesekSzama, bool hasznalhato, bool torolheto) : base(felhasznaloNevHash)
		{
			Megnevezes = megnevezes;
			PenzKod = penzKod;
			Arfolyam = arfolyam;
			TizedesekSzama = tizedesekSzama;
			Hasznalhato = hasznalhato;
			Torolheto = torolheto;
		}

		public override string ToString()
		{
			string str = String.Concat("{0}; {1:F", TizedesekSzama, "} forint");
			return String.Format(str, Megnevezes,Arfolyam);
		}

		public Penznem(XElement p) : base(p.Element("EtrendAdat"))
		{
			Megnevezes = p.Attribute("Megnevezes").Value;
			PenzKod = p.Attribute("PenzKod").Value;
			Arfolyam = double.Parse(p.Attribute("Arfolyam").Value,NumberStyles.Any,new CultureInfo("en-US"));
			TizedesekSzama = Byte.Parse(p.Attribute("TizedesekSzama").Value);
			Hasznalhato = p.Attribute("Hasznalhato").Value == "1";
			Torolheto = p.Attribute("Torolheto").Value == "1";
		}

		public override XElement ToXML()
		{
			return new XElement("Penznem",
				new XAttribute("Megnevezes", Megnevezes),
				new XAttribute("PenzKod", PenzKod),
				new XAttribute("Arfolyam", Arfolyam),
				new XAttribute("TizedesekSzama", TizedesekSzama),
				new XAttribute("Hasznalhato", Hasznalhato ? "1" : "0"),
				new XAttribute("Torolheto", Torolheto ? "1" : "0"),
				base.ToXML());
		}
	}
}
