using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace EtrendKeszito
{
	class EtrendFeltetel:EtrendAdat
	{
		private DateTime datum1;

		public DateTime Datum1
		{
			get { return datum1; }
			set { datum1 = value; }
		}

		private EtkezesTipus etkezes1;

		public EtkezesTipus Etkezes1
		{
			get { return etkezes1; }
			set { etkezes1 = value; }
		}

		private DateTime datum2;

		public DateTime Datum2
		{
			get { return datum2; }
			set { datum2 = value; }
		}

		private EtkezesTipus etkezes2;

		public EtkezesTipus Etkezes2
		{
			get { return etkezes2; }
			set { etkezes2 = value; }
		}

		private bool koltsegmin;

		public bool Koltsegmin
		{
			get { return koltsegmin; }
			set { koltsegmin = value; }
		}

		private bool orommax;

		public bool Orommax
		{
			get { return orommax; }
			set { orommax = value; }
		}

		private double maxpenz;

		public double Maxpenz
		{
			get { return maxpenz; }
			set { maxpenz = value; }
		}

		private Penznem penz;

		public Penznem Penz
		{
			get { return penz; }
			set { penz = value; }
		}


		private bool solverelrejt;

		public bool Solverelrejt
		{
			get { return solverelrejt; }
			set { solverelrejt = value; }
		}

		private bool folytonosmodell;

		public bool Folytonosmodell
		{
			get { return folytonosmodell; }
			set { folytonosmodell = value; }
		}

		private int numvaltozatossag;

		public int Numvaltozatossag
		{
			get { return numvaltozatossag; }
			set
			{
				numvaltozatossag = value;
			}
		}

		private int maxfutasiido;

		public int Maxfutasiido
		{
			get { return maxfutasiido; }
			set
			{
				if (value <= 0 || value > Konstans.maxValaszthatoFutasiIdo)
				{
					throw new ArgumentException("The running time should be positive and at most "+Konstans.maxValaszthatoFutasiIdo+" !");
				}
				maxfutasiido = value;
			}
		}

		private bool naptarbaMent;

		public bool NaptarbaMent
		{
			get { return naptarbaMent; }
			set { naptarbaMent = value; }
		}


		public EtrendFeltetel(string felhasznaloNevHash, DateTime datum1, EtkezesTipus etkezes1, DateTime datum2, EtkezesTipus etkezes2, bool koltsegmin, bool orommax, double maxpenz, Penznem penz,bool solverelrejt, bool folytonosmodell, int numvaltozatossag, int maxfutasiido,bool naptarbaMent) : base(felhasznaloNevHash)
		{
			Datum1 = datum1;
			Etkezes1 = etkezes1;
			Datum2 = datum2;
			Etkezes2 = etkezes2;
			Koltsegmin = koltsegmin;
			Orommax = orommax;
			Maxpenz = maxpenz;
			Penz = penz;
			Solverelrejt = solverelrejt;
			Folytonosmodell = folytonosmodell;
			Numvaltozatossag = numvaltozatossag;
			Maxfutasiido = maxfutasiido;
			NaptarbaMent = naptarbaMent;
		}

		public EtrendFeltetel(XElement e) : base(e.Element("EtrendAdat"))
		{
			Datum1 = DateTime.Parse(e.Attribute("Datum1").Value);
			Etkezes1 = (EtkezesTipus) byte.Parse(e.Attribute("Etkezes1").Value);
			Datum2 = DateTime.Parse(e.Attribute("Datum2").Value);
			Etkezes2 = (EtkezesTipus)byte.Parse(e.Attribute("Etkezes2").Value);
			Koltsegmin = e.Attribute("Koltsegmin").Value == "1";
			Orommax = e.Attribute("Orommax").Value=="1";
			Maxpenz = double.Parse(e.Attribute("Maxpenz").Value, NumberStyles.Any, new CultureInfo("en-US"));
			string str = e.Attribute("Penz").Value;
			Penz = (Penznem) ABKezelo.Kiolvasas().Where(x => x is Penznem && (x as Penznem).Megnevezes == str).ToList().First();
			Solverelrejt = e.Attribute("Solverelrejt").Value == "1";
			Folytonosmodell = e.Attribute("Folytonosmodell").Value == "1";
			Numvaltozatossag = int.Parse(e.Attribute("Numvaltozatossag").Value);
			Maxfutasiido = int.Parse(e.Attribute("Maxfutasiido").Value);
			NaptarbaMent = e.Attribute("NaptarbaMent").Value == "1";
		}

		public override XElement ToXML()
		{
			return new XElement("EtrendFeltetel",
				new XAttribute("Datum1", Datum1),
				new XAttribute("Etkezes1", (int)Etkezes1),
				new XAttribute("Datum2", Datum2),
				new XAttribute("Etkezes2", (int)Etkezes2),
				new XAttribute("Koltsegmin", Koltsegmin ? "1" : "0"),
				new XAttribute("Orommax", Orommax ? "1" : "0"),
				new XAttribute("Maxpenz", Maxpenz),
				new XAttribute("Penz", Penz.Megnevezes),
				new XAttribute("Solverelrejt", Solverelrejt ? "1" : "0"),
				new XAttribute("Folytonosmodell", Folytonosmodell ? "1" : "0"),
				new XAttribute("Numvaltozatossag", Numvaltozatossag),
				new XAttribute("Maxfutasiido", Maxfutasiido),
				new XAttribute("NaptarbaMent", NaptarbaMent ? "1" : "0"),
				base.ToXML());
		}
	}
}
