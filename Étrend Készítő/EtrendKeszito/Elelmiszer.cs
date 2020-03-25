using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace EtrendKeszito
{
	abstract class Elelmiszer:EtrendAdat
	{
		private string megnevezes;

		public string Megnevezes
		{
			get { return megnevezes; }
			private set
			{
				value = value.Trim();
				if (value!= "" &&value.Length<=30) megnevezes = value.Trim();
				else throw new ArgumentException("The food name should be non-emtpy and at most 30 characters length!");
			}
		}
		
		private byte orom;

		public byte Orom
		{
			get { return orom; }
			set
			{
				if(value>=0&&value<=Konstans.maxOrom)orom = value;
				else throw new ArgumentException("The joy should be between 0 and 10!");
			}
		}

		private Penznem penz;

		public Penznem Penz
		{
			get { return penz; }
			set { penz = value; }
		}

		private double ar;

		public double Ar
		{
			get { return ar; }
			set
			{
				if (value >= 0 && value <= Konstans.maxElelmiszerAr) ar = value;
				else
				{

					throw new ArgumentException("The price should be non-negative and at most " + Konstans.maxElelmiszerAr + " !");
				}
			}
		}


		private bool egysegTobbszorose;

		public bool EgysegTobbszorose
		{
			// ha true, akkor csak az egységmennyisgég többszöröse fogyasztható
			get { return egysegTobbszorose; }
			set { egysegTobbszorose = value; }
		}

		private List<bool> fogyaszthato;
		public List<bool> Fogyaszthato
		{
			get { return fogyaszthato; }
			set { fogyaszthato = value; }
		}

		private bool valtozatossag;

		public bool Valtozatossag
		{
			get { return valtozatossag; }
			set { valtozatossag = value; }
		}

		private bool hasznalhato;// true, ha önmagában fogyaszthatjuk

		public bool Hasznalhato
		{
			get { return hasznalhato; }
			set { hasznalhato = value; }
		}

		private Dictionary<Tapanyag,double> tapanyagTartalom;
		//tápanyagtartalmak az élelmiszerben

		public Dictionary<Tapanyag,double> TapanyagTartalom
		{
			get { return tapanyagTartalom; }
			set { tapanyagTartalom = value; }
		}

		protected Elelmiszer(string felhasznaloNevHash, string megnevezes, byte orom, Penznem penz, double ar, bool egysegTobbszorose, List<bool> fogyaszthato, bool valtozatossag, bool hasznalhato, Dictionary<Tapanyag, double> tapanyagTartalom) : base(felhasznaloNevHash)
		{
			Megnevezes = megnevezes;
			Orom = orom;
			Penz = penz;
			Ar = ar;
			EgysegTobbszorose = egysegTobbszorose;
			Fogyaszthato = fogyaszthato;
			Valtozatossag = valtozatossag;
			Hasznalhato = hasznalhato;
			TapanyagTartalom = tapanyagTartalom;
		}

		public Elelmiszer(XElement e,List<Penznem>penznemek) : base(e.Element("EtrendAdat"))
		{
			Megnevezes = e.Attribute("Megnevezes").Value;
			Orom = byte.Parse(e.Attribute("Orom").Value);
            //Penz =(Penznem) ABKezelo.Kiolvasas().Where(x=>x is Penznem && (x as Penznem).Megnevezes==e.Attribute("PenzMegnevezes").Value).ToList().First();
            Penz=penznemek.Where(x=>x.Megnevezes== e.Attribute("PenzMegnevezes").Value).ToList().First();
		    Ar = double.Parse(e.Attribute("Ar").Value, NumberStyles.Any, new CultureInfo("en-US"));
			EgysegTobbszorose = e.Attribute("EgysegTobbszorose").Value == "1";
			Fogyaszthato = new List<bool>()
			{
				e.Attribute("Fogyaszthato_0").Value == "1",
				e.Attribute("Fogyaszthato_1").Value == "1",
				e.Attribute("Fogyaszthato_2").Value == "1"
			};
			Valtozatossag = e.Attribute("Valtozatossag").Value == "1";
			Hasznalhato = e.Attribute("Hasznalhato").Value == "1";
			TapanyagTartalom=new Dictionary<Tapanyag, double>();
		}

		public override XElement ToXML()
		{
			return new XElement("Elelmiszer",
				new XAttribute("Megnevezes", Megnevezes),
				new XAttribute("Orom", Orom),
				new XAttribute("PenzMegnevezes", Penz.Megnevezes),
				new XAttribute("Ar", Ar),
				new XAttribute("EgysegTobbszorose", EgysegTobbszorose ? "1" : "0"),
				new XAttribute("Fogyaszthato_0", Fogyaszthato[0] ? "1" : "0"),
				new XAttribute("Fogyaszthato_1", Fogyaszthato[1] ? "1" : "0"),
				new XAttribute("Fogyaszthato_2", Fogyaszthato[2] ? "1" : "0"),
				new XAttribute("Valtozatossag", Valtozatossag ? "1" : "0"),
				new XAttribute("Hasznalhato", Hasznalhato ? "1" : "0"),
				base.ToXML());
		}

		public override string ToString()
		{
			return Megnevezes + ";" + Ar + " " + Penz.Megnevezes+";joy:"+Orom;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Elelmiszer)) return false;

			Elelmiszer t = obj as Elelmiszer;

			return FelhasznaloNevHash == t.FelhasznaloNevHash && Megnevezes == t.Megnevezes;
		}

		public override int GetHashCode()
		{
			var hc = FelhasznaloNevHash.GetHashCode();
			hc = unchecked((11*hc) ^ Megnevezes.GetHashCode());
			return hc;
		}
	}
}
