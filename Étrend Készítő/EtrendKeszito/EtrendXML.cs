// XML input/output
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;


namespace EtrendKeszito
{
    internal static class EtrendXML
    {
        public static List<EtrendAdat> Csere(List<EtrendAdat> e, string s)
        {
            if (s == null) return e;

            foreach (EtrendAdat item in e)
            {
                item.FelhasznaloNevHash = s;
            }

            return e;
        }

        public static EtrendAdat Csere(EtrendAdat e, string s)
        {
            if (s == null) return e;

            e.FelhasznaloNevHash = s;
            return e;
        }

        public static void XMLRead(string filename, bool check = false, string hash = null, bool msg = true)
        {
            try
            {
                if (File.Exists(filename))
                {
                    List<ABHiba> hibak = new List<ABHiba>();
                    XDocument xml = XDocument.Load(filename);

                    List<Penznem> penznemek =
                        (from p in xml.Root.Elements("Penznem")
                         select new Penznem(p)).ToList();

                    List<EtrendAdat> e = new List<EtrendAdat>();
                    foreach (Penznem item in penznemek)
                    {
                        if (check && item.FelhasznaloNevHash != hash)
                        {
                            throw new ArgumentException("Other user saved this, you have no right to use it!");
                        }

                        e.Add(item);
                    }
                    hibak.AddRange(ABKezelo.Csoportosbeszuras(Csere(e, hash)));

                    e = new List<EtrendAdat>();
                    List<Mertekegyseg> mertekegysegek =
                        (from m in xml.Root.Elements("Mertekegyseg")
                         select new Mertekegyseg(m)).ToList();
                    foreach (Mertekegyseg item in mertekegysegek)
                    {
                        e.Add(item);
                    }
                    hibak.AddRange(ABKezelo.Csoportosbeszuras(Csere(e, hash)));


                    e = new List<EtrendAdat>();
                    List<Tapanyag> tapanyagok =
                        (from t in xml.Root.Elements("Tapanyag") select new Tapanyag(t,mertekegysegek)).ToList();
                    foreach (Tapanyag item in tapanyagok)
                    {
                        e.Add(item);
                    }
                    hibak.AddRange(ABKezelo.Csoportosbeszuras(Csere(e, hash)));


                    e = new List<EtrendAdat>();
                    List<Etel> etelek = (from etel in xml.Root.Elements("Etel") select new Etel(etel,penznemek,mertekegysegek)).ToList();
                    foreach (Etel item in etelek)
                    {
                        e.Add(item);
                    }
                    hibak.AddRange(ABKezelo.Csoportosbeszuras(Csere(e, hash)));

                    e = new List<EtrendAdat>();
                    List<Ital> italok = (from ital in xml.Root.Elements("Ital") select new Ital(ital,penznemek,mertekegysegek)).ToList();
                    foreach (Ital item in italok)
                    {
                        e.Add(item);
                    }
                    hibak.AddRange(ABKezelo.Csoportosbeszuras(Csere(e, hash)));

                    e = new List<EtrendAdat>();
                    List<Menu> menuk = (from menu in xml.Root.Elements("Menu") select new Menu(menu,penznemek,mertekegysegek)).ToList();
                    foreach (Menu item in menuk)
                    {
                        e.Add(item);
                    }
                    hibak.AddRange(ABKezelo.Csoportosbeszuras(Csere(e, hash)));


                    e = new List<EtrendAdat>();
                    List<ElelmiszerTapanyag> elelmiszertapanyag =
                        (from et in xml.Root.Elements("ElelmiszerTapanyag") select new ElelmiszerTapanyag(et,tapanyagok)).ToList();


                    List<Elelmiszer> elelmiszerek = new List<Elelmiszer>();
                    foreach (Etel item in etelek)
                    {
                        elelmiszerek.Add(item);
                    }
                    foreach (Ital item in italok)
                    {
                        elelmiszerek.Add(item);
                    }
                    foreach (Tapanyag t in tapanyagok)
                    {
                        ABKezelo.BeszurTapanyagElelmiszerekbe((Tapanyag)Csere(t, hash),elelmiszerek);
                    }


                    foreach (Elelmiszer item in ABKezelo.Kiolvasas().Where(x => x is Elelmiszer).ToList())
                    {
                        foreach (ElelmiszerTapanyag item2 in elelmiszertapanyag.Where(x => x.ElelmiszerMegnevezes == item.Megnevezes).ToList())
                        {
                            item.TapanyagTartalom[item2.Tapanyag] = item2.Ertek;
                        }
                        ABKezelo.Modositas(item);
                    }

                    elelmiszerek = new List<Elelmiszer>();
                    foreach (Etel item in etelek)
                    {
                        elelmiszerek.Add(item);
                    }
                    foreach (Ital item in italok)
                    {
                        elelmiszerek.Add(item);
                    }
                    
                    e = new List<EtrendAdat>();
                    List<ElelmiszerElelmiszer> elelmiszerelelmiszer =
                        (from et in xml.Root.Elements("ElelmiszerElelmiszer") select new ElelmiszerElelmiszer(et,elelmiszerek)).ToList();

                    foreach (Menu item in menuk)
                    {
                        ABKezelo.BeszurElelmiszerElelmiszerekbe((Elelmiszer)Csere(item, hash));
                    }

                    foreach (Menu item in ABKezelo.Kiolvasas().Where(x => x is Menu).ToList())
                    {
                        foreach (ElelmiszerElelmiszer item2 in elelmiszerelelmiszer.Where(x => x.ElelmiszerMegnevezes == item.Megnevezes).ToList())
                        {
                            item.Osszetevo[item2.Elelmiszer] = item2.Ertek;
                        }
                        ABKezelo.Modositas(item);
                    }

                    EtrendFeltetel etrendfeltetel = (from x in xml.Root.Elements("EtrendFeltetel") select new EtrendFeltetel(x)).ToList().First();
                    ABKezelo.Beszuras(Csere(etrendfeltetel, hash));

                    e = new List<EtrendAdat>();
                    List<EtkezesFeltetel> etkezesfeltetel = (from x in xml.Root.Elements("EtkezesFeltetel") select new EtkezesFeltetel(x)).ToList();
                    foreach (EtkezesFeltetel item in etkezesfeltetel)
                    {
                        e.Add(item);
                    }
                    hibak.AddRange(ABKezelo.Csoportosbeszuras(Csere(e, hash)));

                    var etrendidopont = (from x in xml.Root.Elements("EtrendIdopont") select new EtrendIdopont(x)).ToList();
                    if (etrendidopont.Count > 0)
                    {
                        ABKezelo.Beszuras(Csere(etrendidopont.First(), hash));
                    }

                    if (hibak.Count > 0)
                    {
                        string szoveg = "";
                        foreach (ABHiba item in hibak)
                        {
                            szoveg += item + " " + item.Hibas + Environment.NewLine;
                        }

                        throw new Exception(szoveg);
                    }

                    if (msg)
                    {
                        MessageBox.Show("Successful import!", "Info!", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        Logolas.Ment("Successful XML import!");
                    }
                }
                else
                {
                    throw new FileNotFoundException("Not existing file!");
                }
            }
            catch (Exception ex)
            {
                if (msg) Logolas.Ment("Unsuccessful XML import!");
                throw ex;
            }
        }

        public static void XMLSave(string filename, bool msg = true)
        {
            try
            {
                XDocument xml = new XDocument(new XElement("Gyoker"));


                foreach (Penznem item in ABKezelo.Kiolvasas().Where(x => x is Penznem).ToList())
                {
                    xml.Root.Add(item.ToXML());
                }

                foreach (Mertekegyseg item in ABKezelo.Kiolvasas().Where(x => x is Mertekegyseg).ToList())
                {
                    xml.Root.Add(item.ToXML());
                }

                foreach (Tapanyag item in ABKezelo.Kiolvasas().Where(x => x is Tapanyag).ToList())
                {
                    xml.Root.Add(item.ToXML());
                }

                foreach (Etel item in ABKezelo.Kiolvasas().Where(x => x is Etel).ToList())
                {
                    xml.Root.Add(item.ToXML());
                    foreach (KeyValuePair<Tapanyag, double> item2 in item.TapanyagTartalom)
                    {
                        ElelmiszerTapanyag item3 = new ElelmiszerTapanyag(item.FelhasznaloNevHash, item.Megnevezes, item2.Key, item2.Value);
                        xml.Root.Add(item3.ToXML());
                    }
                }

                foreach (Ital item in ABKezelo.Kiolvasas().Where(x => x is Ital).ToList())
                {
                    xml.Root.Add(item.ToXML());
                    foreach (KeyValuePair<Tapanyag, double> item2 in item.TapanyagTartalom)
                    {
                        ElelmiszerTapanyag item3 = new ElelmiszerTapanyag(item.FelhasznaloNevHash, item.Megnevezes, item2.Key, item2.Value);
                        xml.Root.Add(item3.ToXML());
                    }
                }

                foreach (Menu item in ABKezelo.Kiolvasas().Where(x => x is Menu).ToList())
                {
                    xml.Root.Add(item.ToXML());
                    foreach (KeyValuePair<Tapanyag, double> item2 in item.TapanyagTartalom)
                    {
                        ElelmiszerTapanyag item3 =
                            new ElelmiszerTapanyag(item.FelhasznaloNevHash, item.Megnevezes, item2.Key, item2.Value);
                        xml.Root.Add(item3.ToXML());
                    }

                    foreach (KeyValuePair<Elelmiszer, double> item4 in item.Osszetevo)
                    {
                        ElelmiszerElelmiszer item5 = new ElelmiszerElelmiszer(item.FelhasznaloNevHash, item.Megnevezes, item4.Key, item4.Value);
                        xml.Root.Add(item5.ToXML());
                    }
                }

                xml.Root.Add(ABKezelo.Kiolvasas().Where(x => x is EtrendFeltetel).ToList().First().ToXML());

                foreach (EtkezesFeltetel item in ABKezelo.Kiolvasas().Where(x => x is EtkezesFeltetel).ToList())
                {
                    xml.Root.Add(item.ToXML());
                }

                var e = ABKezelo.Kiolvasas().Where(x => x is EtrendIdopont).ToList();
                if (e.Count > 0)
                {
                    xml.Root.Add(e.First().ToXML());
                }

                try
                {
                    xml.Save(filename);
                    if (msg) Logolas.Ment("Successful XML export!");
                }
                catch (Exception)
                {
                    if (msg) Logolas.Ment("Unsuccessful XML export!");
                    throw new FileNotFoundException("Unsuccessful file export!");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
