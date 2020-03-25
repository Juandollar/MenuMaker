using System;
using System.Globalization;
using System.Xml.Linq;

namespace EtrendKeszito
{
	class Mertekegyseg:EtrendAdat
	{
		private string megnevezes;

		public string Megnevezes
		{
			get { return megnevezes; }
			set
			{
				if (value == null) megnevezes = null;
				if (value.Trim()==""||value.Trim().Length > 30) throw new ArgumentException("The name should be non-empty and at most 30 characters length!");
				megnevezes = value.Trim();
			}
		}

		private MertekegysegFajta mertek;

		public MertekegysegFajta Mertek
		{
			get { return mertek; }
			set { mertek = value; }
		}

		private double valtoszam;

		public double Valtoszam
		{
			get { return valtoszam; }
			private set
			{
				if(value<=0)throw new ArgumentException("The rate should be positive!");
				valtoszam = value;
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

		public Mertekegyseg(string felhasznaloNevHash,string megnevezes, MertekegysegFajta mertek, double valtoszam, bool hasznalhato, bool torolheto):base(felhasznaloNevHash)
		{
			Megnevezes = megnevezes;
			Mertek = mertek;
			Valtoszam = valtoszam;
			Hasznalhato = hasznalhato;
			Torolheto = torolheto;
		}

		public Mertekegyseg(XElement m) : base(m.Element("EtrendAdat"))
		{
			Megnevezes = m.Attribute("Megnevezes").Value;
			Mertek = (MertekegysegFajta)byte.Parse(m.Attribute("Mertek").Value);
			Valtoszam = double.Parse(m.Attribute("Valtoszam").Value, NumberStyles.Any, new CultureInfo("en-US"));
			Hasznalhato = m.Attribute("Hasznalhato").Value == "1";
			Torolheto = m.Attribute("Torolheto").Value == "1";
		}

		public override XElement ToXML()
		{
			return new XElement("Mertekegyseg",
				new XAttribute("Megnevezes", Megnevezes),
				new XAttribute("Mertek", (byte)Mertek),
				new XAttribute("Valtoszam", Valtoszam),
				new XAttribute("Hasznalhato", Hasznalhato ? "1" : "0"),
				new XAttribute("Torolheto", Torolheto ? "1" : "0"),
				base.ToXML());
		}

		public override string ToString()
		{
			return Megnevezes;
		}
	}
}
