using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace EtrendKeszito
{
	class EtrendIdopont:EtrendAdat
	{
		private List<DateTime> datum;

		public List<DateTime> Datum
		{
			get { return datum; }
			set { datum = value; }
		}

		public EtrendIdopont(string felhasznaloNevHash, List<DateTime> datum) : base(felhasznaloNevHash)
		{
			Datum = datum;
		}

		public EtrendIdopont(XElement e) : base(e.Element("EtrendAdat"))
		{
			Datum=new List<DateTime>()
			{
				DateTime.Parse(e.Attribute("ReggeliIdopont").Value),
				DateTime.Parse(e.Attribute("EbedIdopont").Value),
				DateTime.Parse(e.Attribute("VacsoraIdopont").Value)
			};
		}

		public override XElement ToXML()
		{
			return new XElement("EtrendIdopont",
				new XAttribute("ReggeliIdopont", Datum[0]),
				new XAttribute("EbedIdopont", Datum[1]),
				new XAttribute("VacsoraIdopont", Datum[2]),
				base.ToXML());
		}
	}
}
