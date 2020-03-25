using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace EtrendKeszito
{
	class Menu : Elelmiszer
	{
		private byte maxDarab;

		public byte MaxDarab
		{
			get { return maxDarab; }
			set
			{
				if (value >= 0 && value <= Konstans.maxMenu) maxDarab = value;
				else throw new ArgumentException("The number of menus should be non-negative and at most " + Konstans.maxMenu);
			}
		}

		private double egysegTomegMennyiseg;

		public double EgysegTomegMennyiseg
		{
			get { return egysegTomegMennyiseg; }
			set
			{
				if (value >= 0) egysegTomegMennyiseg = value;
				else throw new ArgumentException("The mass of unit menu should be non-negative!");
			}
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

		private double egysegUrTartalomMennyiseg;

		public double EgysegUrTartalomMennyiseg
		{
			get { return egysegUrTartalomMennyiseg; }
			set
			{
				if (value >= 0) egysegUrTartalomMennyiseg = value;
				else throw new ArgumentException("The capacity of unit menu should be non-negative!");
			}
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

		private Dictionary<Elelmiszer, double> osszetevo;

		public Dictionary<Elelmiszer, double> Osszetevo
		{
			get { return osszetevo; }
			set { osszetevo = value; }
		}

		private bool arszamitas;

		public bool Arszamitas
		{
			get { return arszamitas; }
			set { arszamitas = value; }
		}



		public Menu(string felhasznaloNevHash, string megnevezes, byte orom, Penznem penz, double ar, bool egysegTobbszorose,
			List<bool> fogyaszthato, bool valtozatossag, bool hasznalhato, Dictionary<Tapanyag, double> tapanyagTartalom,
			byte maxDarab, double egysegTomegMennyiseg, Mertekegyseg tomegMertek, double egysegUrTartalomMennyiseg,
			Mertekegyseg urmertek, Dictionary<Elelmiszer, double> osszetevo,bool arszamitas) : base(felhasznaloNevHash, megnevezes, orom, penz,
			ar, egysegTobbszorose, fogyaszthato, valtozatossag, hasznalhato, tapanyagTartalom)
		{
			MaxDarab = maxDarab;
			EgysegTomegMennyiseg = egysegTomegMennyiseg;
			TomegMertek = tomegMertek;
			EgysegUrTartalomMennyiseg = egysegUrTartalomMennyiseg;
			Urmertek = urmertek;
			Osszetevo = osszetevo;
			Arszamitas = arszamitas;
		}

		public Menu(XElement e,List<Penznem>penznemek,List<Mertekegyseg>mertekegysegek) : base(e.Element("Elelmiszer"),penznemek)
		{
			MaxDarab = byte.Parse(e.Attribute("MaxDarab").Value);
			EgysegTomegMennyiseg = double.Parse(e.Attribute("EgysegTomegMennyiseg").Value, NumberStyles.Any, new CultureInfo("en-US"));
            TomegMertek=mertekegysegek.Where(x=>x.Megnevezes== e.Attribute("TomegMertekMegnevezes").Value).ToList().First();
            //TomegMertek = (Mertekegyseg)ABKezelo.Kiolvasas().Where(x => x is Mertekegyseg && (x as Mertekegyseg).Megnevezes == e.Attribute("TomegMertekMegnevezes").Value).ToList().First();
			EgysegUrTartalomMennyiseg = double.Parse(e.Attribute("EgysegUrTartalomMennyiseg").Value, NumberStyles.Any, new CultureInfo("en-US"));
            Urmertek=mertekegysegek.Where(x=>x.Megnevezes== e.Attribute("UrmertekMegnevezes").Value).ToList().First();
            //Urmertek = (Mertekegyseg)ABKezelo.Kiolvasas().Where(x => x is Mertekegyseg && (x as Mertekegyseg).Megnevezes == e.Attribute("UrmertekMegnevezes").Value).ToList().First();
			Osszetevo=new Dictionary<Elelmiszer, double>();
			Arszamitas = e.Attribute("Arszamitas").Value == "1";
		}

		public override XElement ToXML()
		{
			return new XElement("Menu",
				new XAttribute("MaxDarab", MaxDarab),
				new XAttribute("EgysegTomegMennyiseg", EgysegTomegMennyiseg),
				new XAttribute("TomegMertekMegnevezes", TomegMertek.Megnevezes),
				new XAttribute("EgysegUrTartalomMennyiseg", EgysegUrTartalomMennyiseg),
				new XAttribute("UrmertekMegnevezes", Urmertek.Megnevezes),
				new XAttribute("Arszamitas", Arszamitas ? "1" : "0"),
				base.ToXML());
		}

		public override string ToString()
		{
			string str = String.Concat("{0} (menu, {1:F", Penz.TizedesekSzama, "} {2}); {3:0.##} {4}; {5:0.##} {6}");
			return String.Format(str, Megnevezes, Ar, Penz.Megnevezes, EgysegTomegMennyiseg, TomegMertek.Megnevezes,EgysegUrTartalomMennyiseg,Urmertek.Megnevezes);
		}

		public void update()
		{
			try
			{
				int i = 0;
				double ar = 0;
				Penznem p = null;

				List<Mertekegyseg> mertekegysegek = new List<Mertekegyseg>();
				List<Tapanyag> tapanyagok = new List<Tapanyag>();

				var e = ABKezelo.Kiolvasas().ToList();
				foreach (EtrendAdat item in e)
				{
					if (item is Mertekegyseg) mertekegysegek.Add((Mertekegyseg) item);
					else if (item is Tapanyag) tapanyagok.Add((Tapanyag) item);
				}

				double tomeg = 0;
				Mertekegyseg m1 = mertekegysegek.Where(x => x.Megnevezes == "gram").First();
				double urmertek = 0;
				Mertekegyseg m2 = mertekegysegek.Where(x => x.Megnevezes == "liter").First();

				Dictionary<Tapanyag, double> dict = new Dictionary<Tapanyag, double>();
				foreach (Tapanyag item in tapanyagok)
				{
					dict.Add(item, 0);
				}

				foreach (KeyValuePair<Elelmiszer, double> item in Osszetevo)
				{
					if (i == 0) p = item.Key.Penz;
					i++;

					if (item.Key is Etel)
					{
						tomeg += item.Value * (item.Key as Etel).TomegMertek.Valtoszam;
						ar += item.Value / (item.Key as Etel).EgysegTomegMennyiseg * item.Key.Ar / p.Arfolyam * item.Key.Penz.Arfolyam;
					}
					else
					{
						urmertek += item.Value * (item.Key as Ital).Urmertek.Valtoszam;
						ar += item.Value / (item.Key as Ital).EgysegUrTartalomMennyiseg * item.Key.Ar / p.Arfolyam * item.Key.Penz.Arfolyam;
					}

					foreach (Tapanyag item2 in tapanyagok)
					{
						if (!item2.Hasznalhato) continue;

						double val;
						if (item.Key is Etel) val = item.Value / (item.Key as Etel).EgysegTomegMennyiseg;
						else val = item.Value / (item.Key as Ital).EgysegUrTartalomMennyiseg;

						val *= item.Key.TapanyagTartalom[item2];
						dict[item2] += val;
					}
				}

				EgysegTomegMennyiseg = tomeg;
				TomegMertek = m1;
				EgysegUrTartalomMennyiseg = urmertek;
				Urmertek = m2;

				if (Arszamitas)
				{
					Ar = ar;
					Penz = p;
				}

				TapanyagTartalom = dict;
				ABKezelo.Modositas(this);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}
