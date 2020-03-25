using System.Globalization;
using System.Xml.Linq;

namespace EtrendKeszito
{
	class EtkezesFeltetel:EtrendAdat
	{
		private ElelmiszerTipus2 eltipus2;

		public ElelmiszerTipus2 Eltipus2
		{
			get { return eltipus2; }
			set { eltipus2 = value; }
		}

		private EtkezesTipus2 ettipus2;

		public EtkezesTipus2 Ettipus2
		{
			get { return ettipus2; }
			set { ettipus2 = value; }
		}

		private Szamlalo szamlalo;

		public Szamlalo Szamlalo
		{
			get { return szamlalo; }
			set { szamlalo = value; }
		}

		private double minval;

		public double Minval
		{
			get { return minval; }
			set { minval = value; }
		}

		private bool maxvalE;

		public bool MaxvalE
		{
			get { return maxvalE; }
			set { maxvalE = value; }
		}

		private double maxval;

		public double Maxval
		{
			get { return maxval; }
			set { maxval = value; }
		}

		public EtkezesFeltetel(string felhasznaloNevHash, ElelmiszerTipus2 eltipus2, EtkezesTipus2 ettipus2, Szamlalo szamlalo, double minval, bool maxvalE, double maxval) : base(felhasznaloNevHash)
		{
			Eltipus2 = eltipus2;
			Ettipus2 = ettipus2;
			Szamlalo = szamlalo;
			Minval = minval;
			MaxvalE = maxvalE;
			Maxval = maxval;
		}

		public EtkezesFeltetel(XElement e) : base(e.Element("EtrendAdat"))
		{
			Eltipus2 = (ElelmiszerTipus2)byte.Parse(e.Attribute("Eltipus2").Value);
			Ettipus2 = (EtkezesTipus2) byte.Parse(e.Attribute("Ettipus2").Value);
			Szamlalo = (Szamlalo) byte.Parse(e.Attribute("Szamlalo").Value);
			Minval = double.Parse(e.Attribute("Minval").Value, NumberStyles.Any, new CultureInfo("en-US"));
			MaxvalE = e.Attribute("MaxvalE").Value == "1";
			Maxval = double.Parse(e.Attribute("Maxval").Value, NumberStyles.Any, new CultureInfo("en-US"));
		}

		public override XElement ToXML()
		{
			return new XElement("EtkezesFeltetel",
				new XAttribute("Eltipus2", (byte)Eltipus2),
				new XAttribute("Ettipus2", (byte)Ettipus2),
				new XAttribute("Szamlalo", (byte)Szamlalo),
				new XAttribute("Minval", Minval),
				new XAttribute("MaxvalE", MaxvalE ? "1" : "0"),
				new XAttribute("Maxval", Maxval),
				base.ToXML());
		}
	}
}
