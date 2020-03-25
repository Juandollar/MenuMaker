using System.Xml.Linq;

namespace EtrendKeszito
{
	abstract class EtrendAdat// nem lehet általános étrend adatot felvinni, így
	// ez absztrakt
	{
		private string felhasznaloNevHash;//csak a hash-elt nevet tároljuk le
		// biztonsági okokból

		public string FelhasznaloNevHash
		{
			get { return felhasznaloNevHash; }
			set { felhasznaloNevHash = value; }
		}

		public EtrendAdat(string felhasznaloNevHash)
		{
			FelhasznaloNevHash = felhasznaloNevHash;
		}

		protected EtrendAdat(XElement e)
		{
			FelhasznaloNevHash = e.Attribute("FelhasznaloNevHash").Value;
		}

		public virtual XElement ToXML()
		{
			return new XElement("EtrendAdat",
				new XAttribute("FelhasznaloNevHash", FelhasznaloNevHash));
		}
	}
}
