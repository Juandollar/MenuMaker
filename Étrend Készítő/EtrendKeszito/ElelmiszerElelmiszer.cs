// xml input/output-hoz

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace EtrendKeszito
{
	class ElelmiszerElelmiszer:EtrendAdat
	{
		private string elelmiszerMegnevezes;

		public string ElelmiszerMegnevezes
		{
			get { return elelmiszerMegnevezes; }
			set { elelmiszerMegnevezes = value; }
		}

		private Elelmiszer elelmiszer;

		public Elelmiszer Elelmiszer
		{
			get { return elelmiszer; }
			set { elelmiszer = value; }
		}

		private double ertek;

		public double Ertek
		{
			get { return ertek; }
			set { ertek = value; }
		}

		public ElelmiszerElelmiszer(string felhasznaloNevHash, string elelmiszerMegnevezes, Elelmiszer elelmiszer, double ertek) : base(felhasznaloNevHash)
		{
			ElelmiszerMegnevezes = elelmiszerMegnevezes;
			Elelmiszer = elelmiszer;
			Ertek = ertek;
		}

		public ElelmiszerElelmiszer(XElement e,List<Elelmiszer>elelmiszerek) : base(e.Element("EtrendAdat"))
		{
			ElelmiszerMegnevezes = e.Attribute("ElelmiszerMegnevezes").Value;
			Elelmiszer = elelmiszerek.Where(x => x.Megnevezes == e.Attribute("Elelmiszer").Value).ToList().First();
			Ertek = double.Parse(e.Attribute("Ertek").Value, NumberStyles.Any, new CultureInfo("en-US"));
		}

		public override XElement ToXML()
		{
			return new XElement("ElelmiszerElelmiszer",
				new XAttribute("ElelmiszerMegnevezes", ElelmiszerMegnevezes),
				new XAttribute("Elelmiszer", Elelmiszer.Megnevezes),
				new XAttribute("Ertek", Ertek),
				base.ToXML());
		}
	}
}
