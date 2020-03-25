// élelmiszer xml file-ba mentéséhez

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace EtrendKeszito
{
	class ElelmiszerTapanyag:EtrendAdat
	{
		private string elelmiszerMegnevezes;

		public string ElelmiszerMegnevezes
		{
			get { return elelmiszerMegnevezes; }
			set { elelmiszerMegnevezes = value; }
		}

		private Tapanyag tapanyag;

		public Tapanyag Tapanyag
		{
			get { return tapanyag; }
			set { tapanyag = value; }
		}

		private double ertek;

		public double Ertek
		{
			get { return ertek; }
			set { ertek = value; }
		}

		public ElelmiszerTapanyag(string felhasznaloNevHash, string elelmiszerMegnevezes, Tapanyag tapanyag, double ertek) : base(felhasznaloNevHash)
		{
			ElelmiszerMegnevezes = elelmiszerMegnevezes;
			Tapanyag = tapanyag;
			Ertek = ertek;
		}

		public ElelmiszerTapanyag(XElement e,List<Tapanyag>tapanyagok) : base(e.Element("EtrendAdat"))
		{
			ElelmiszerMegnevezes = e.Attribute("ElelmiszerMegnevezes").Value;
			Tapanyag = tapanyagok.Where(x => x is Tapanyag && (x as Tapanyag).Megnevezes == e.Attribute("TapanyagMegnevezes").Value).ToList().First();
			Ertek = double.Parse(e.Attribute("Ertek").Value, NumberStyles.Any, new CultureInfo("en-US"));
		}

		public override XElement ToXML()
		{
			return new XElement("ElelmiszerTapanyag",
				new XAttribute("ElelmiszerMegnevezes", ElelmiszerMegnevezes),
				new XAttribute("TapanyagMegnevezes", Tapanyag.Megnevezes),
				new XAttribute("Ertek", Ertek),
				base.ToXML());
		}
	}
}
