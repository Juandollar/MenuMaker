using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace EtrendKeszito
{
    class EAdat
    {
        public int nap;
        public int etkezes;
        public int elelmiszer;
        public double val;

        public EAdat(int nap, int etkezes, int elelmiszer, double val)
        {
            this.nap = nap;
            this.etkezes = etkezes;
            this.elelmiszer = elelmiszer;
            this.val = val;
        }
    }


    public class EtrendMegold
    {
        private List<EtrendAdat> e0;
        private List<Mertekegyseg> mertekegysegek;
        private List<Tapanyag> tapanyagok;
        private List<Penznem> penznemek;
        private List<Elelmiszer> elelmiszerek;
        private List<Elelmiszer> elelmiszerhasznalt;
        private List<EtkezesFeltetel> etkezesfeltetelek;

        private int nagySzam = 1000000;
        private int nagySzam2 = 1000000;

        public List<object> Megoldas()
        {
            e0 = ABKezelo.Kiolvasas().ToList();

            mertekegysegek = new List<Mertekegyseg>();
            tapanyagok = new List<Tapanyag>();
            penznemek = new List<Penznem>();
            elelmiszerek = new List<Elelmiszer>();
            elelmiszerhasznalt = new List<Elelmiszer>();
            etkezesfeltetelek = new List<EtkezesFeltetel>();
            EtrendFeltetel etrendfeltetel = null;

            foreach (EtrendAdat item in e0)
            {
                if (item is Mertekegyseg) mertekegysegek.Add((Mertekegyseg)item);
                else if (item is Tapanyag && (item as Tapanyag).Hasznalhato)
                    tapanyagok.Add((Tapanyag)item); 
                else if (item is Penznem) penznemek.Add((Penznem)item);
                else if (item is EtkezesFeltetel) etkezesfeltetelek.Add((EtkezesFeltetel)item);
                else if (item is Elelmiszer)
                {
                    elelmiszerek.Add((Elelmiszer)item);
                    if (((Elelmiszer)item).Hasznalhato)
                    {
                        elelmiszerhasznalt.Add((Elelmiszer)item);
                    }
                }
                else if (item is EtrendFeltetel) etrendfeltetel = (EtrendFeltetel)item;
            }

            if (elelmiszerhasznalt.Count == 0)
            {
                throw new ArgumentException("Currently you have no usable food!");
            }

            DateTime datum1 = etrendfeltetel.Datum1;
            datum1 = new DateTime(datum1.Year, datum1.Month, datum1.Day, 0, 0, 0);
            DateTime datum2 = etrendfeltetel.Datum2;
            datum2 = new DateTime(datum2.Year, datum2.Month, datum2.Day, 0, 0, 0);

            int E1 = 1 + (int)etrendfeltetel.Etkezes1; //E1,E2 eggyel el van tolva
            int E2 = 1 + (int)etrendfeltetel.Etkezes2;

            if (datum1 > datum2 || (datum1 == datum2 && E1 > E2))
            {
                throw new ArgumentException("The mealtime is empty!");
            }

            int sz = 3 * ((datum2 - datum1).Days -1) + E2 + 4 - E1; // étkezések száma
            if (sz > Konstans.maxEtkezesekSzama)
            {
                throw new ArgumentException(String.Format("{0} given mealtime, the maximal is {1} !", sz,
                    (int)Konstans.maxEtkezesekSzama));
            }

            int maxfutasiido = 1000 * etrendfeltetel.Maxfutasiido;//msec-ben

            int E = elelmiszerhasznalt.Count;
            int T = tapanyagok.Count;
            int N = 1 + (datum2 - datum1).Days; //napok száma

            bool valtozatos = etrendfeltetel.Numvaltozatossag >= 2; 
            bool folytonos = etrendfeltetel.Folytonosmodell;
            int valtozatosEtkezesSzam = etrendfeltetel.Numvaltozatossag;
            if (!valtozatos) valtozatosEtkezesSzam = 0;

            string input_filename = "etkezesprogram.txt";
            string output_filename = "solution.txt";
            string str = "";

            Penznem p = elelmiszerhasznalt[0].Penz;
            Penznem p0 = etrendfeltetel.Penz;

            try
            {
                foreach (Elelmiszer item in elelmiszerhasznalt)
                {
                    if (item is Menu) (item as Menu).update();
                }

                // egészértétű lineáris program megírása
                str += "/* Automatikusan generált program, ne módosítsa a fájlt! */" + Environment.NewLine;
                str += Environment.NewLine;

                str += String.Format("param NagySzam := {0};", nagySzam) + Environment.NewLine;
                str += String.Format("param NagySzam2 := {0};", nagySzam2) + Environment.NewLine;
                str += "/* élelmiszerek száma */" + Environment.NewLine;
                str += String.Format("param E := {0};", E) + Environment.NewLine;
                str += "/* napok száma */" + Environment.NewLine;
                str += String.Format("param N := {0};", N) + Environment.NewLine;
                str += "/* tápanyagok száma */" + Environment.NewLine;
                str += String.Format("param T := {0};", T) + Environment.NewLine;
                str += "/* Változatosság */" + Environment.NewLine;
                str += String.Format("param D := {0};", valtozatosEtkezesSzam) + Environment.NewLine;
                str += "/* Első étkezés */" + Environment.NewLine;
                str += String.Format("param E1:= {0};", E1) + Environment.NewLine; /* első étkezés, 1-el eltolva */
                str += "/* Utolsó étkezés */" + Environment.NewLine;
                str += String.Format("param E2:= {0};", E2) + Environment.NewLine; /* utolsó étkezés, 1-el eltolva */
                str += Environment.NewLine;

                //maxpénz beállítása
                if (!etrendfeltetel.Koltsegmin)
                {
                    str += String.Format("param Maxpenz:= {0};", etrendfeltetel.Maxpenz * p0.Arfolyam / p.Arfolyam).Replace(',', '.') + Environment.NewLine;
                }

                str += "param V{0..15,0..2};" + Environment.NewLine; //étkezésfeltételek
                str += "param W{1..E,1..18};" + Environment.NewLine;

                if (T > 0)
                {
                    str += "param U{1..E,1..T};" + Environment.NewLine;
                    str += "param NapiMaxTapanyag{1..T};" + Environment.NewLine;
                    str += "param NapiMinTapanyag{1..T};" + Environment.NewLine;
                }

                str += Environment.NewLine;

                str += "var x{n in 1..N,i in 1..4,e in 1..E}, >=0;" + Environment.NewLine;
                str += "var y{n in 1..N,i in 1..3,e in 1..E} integer, >=0;" + Environment.NewLine;
                str += "var b{n in 1..N,i in 1..4,e in 1..E} binary;" + Environment.NewLine;
                str += "var c{n in 1..N,i in 1..4,e in 1..E} binary;" + Environment.NewLine;
                str += "var Adat{n in 1..N,i in 1..4,j in 0..3}, >=0;" + Environment.NewLine;
                if (T > 0) str += "var Tapanyag{n in 1..N,t in 1..T}, >=0;" + Environment.NewLine;
                str += "var Tomeg{n in 1..N,i in 1..4,e in 1..E}, >=0;" + Environment.NewLine;
                str += "var Urmertek{n in 1..N,i in 1..4,e in 1..E}, >=0;" + Environment.NewLine;
                str += "var Penz >=0;" + Environment.NewLine;
                str += "var Orom >=0;" + Environment.NewLine;
                str += Environment.NewLine;

                int reggelinelFogyaszthatoE;
                int ebednelFogyaszthatoE;
                int vacsoranalFogyaszthatoE;
                int valtozatosE;
                int egysegTobbszorose;
                int maxDarab;

                int etelSzam;
                double etelTomeg;
                double minEtelTomeg;
                double maxEtelTomeg;

                int italSzam;
                double italUrmertek;
                double minItalUrmertek;
                double maxItalUrmertek;
                double ar, orom;

                int maxTomegE;
                int maxUrmertekE;

                string str2 = "param W: 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18:=" + Environment.NewLine;

                int i = 0, j;
                foreach (Elelmiszer item in elelmiszerhasznalt)
                {
                    i++;

                    maxTomegE = 0;
                    maxUrmertekE = 0;

                    maxEtelTomeg = 0;
                    maxItalUrmertek = 0;

                    if (item is Etel)
                    {
                        Etel e = item as Etel;
                        reggelinelFogyaszthatoE = Convert.ToByte(e.Fogyaszthato[0]);
                        ebednelFogyaszthatoE = Convert.ToByte(e.Fogyaszthato[1]);
                        vacsoranalFogyaszthatoE = Convert.ToByte(e.Fogyaszthato[2]);
                        valtozatosE = valtozatosEtkezesSzam;
                        if (!e.Valtozatossag || !valtozatos) valtozatosE = 0;
                        egysegTobbszorose = Convert.ToByte(e.EgysegTobbszorose);
                        if (folytonos) egysegTobbszorose = 0;
                        maxDarab = -1;

                        etelSzam = 1;
                        etelTomeg = e.EgysegTomegMennyiseg * e.TomegMertek.Valtoszam;
                        minEtelTomeg = e.MinTomeg * e.TomegMertek.Valtoszam;

                        if (e.MaxTomegE)
                        {
                            maxEtelTomeg = e.MaxTomeg * e.TomegMertek.Valtoszam;
                            maxTomegE = 1;
                        }

                        italSzam = 0;
                        italUrmertek = 0;
                        minItalUrmertek = 0;
                        maxItalUrmertek = nagySzam;
                        ar = e.Ar * e.Penz.Arfolyam / p.Arfolyam;
                        orom = 0.001 * etelTomeg * e.Orom;
                    }
                    else if (item is Ital)
                    {
                        Ital e = item as Ital;
                        reggelinelFogyaszthatoE = Convert.ToByte(e.Fogyaszthato[0]);
                        ebednelFogyaszthatoE = Convert.ToByte(e.Fogyaszthato[1]);
                        vacsoranalFogyaszthatoE = Convert.ToByte(e.Fogyaszthato[2]);
                        valtozatosE = valtozatosEtkezesSzam;
                        if (!e.Valtozatossag || !valtozatos) valtozatosE = 0;
                        egysegTobbszorose = Convert.ToByte(e.EgysegTobbszorose);
                        if (folytonos) egysegTobbszorose = 0;
                        maxDarab = -1;

                        etelSzam = 0;
                        etelTomeg = 0;
                        minEtelTomeg = 0;
                        maxEtelTomeg = 0;

                        italSzam = 1;
                        italUrmertek = e.EgysegUrTartalomMennyiseg * e.Urmertek.Valtoszam;
                        minItalUrmertek = e.MinUrTartalom * e.Urmertek.Valtoszam;

                        if (e.MaxUrTartalomE)
                        {
                            maxItalUrmertek = e.MaxUrTartalom * e.Urmertek.Valtoszam;
                            maxUrmertekE = 1;
                        }


                        ar = e.Ar * e.Penz.Arfolyam / p.Arfolyam;
                        orom = italUrmertek * e.Orom;
                    }
                    else
                    {
                        Menu e = item as Menu;
                        reggelinelFogyaszthatoE = Convert.ToByte(e.Fogyaszthato[0]);
                        ebednelFogyaszthatoE = Convert.ToByte(e.Fogyaszthato[1]);
                        vacsoranalFogyaszthatoE = Convert.ToByte(e.Fogyaszthato[2]);
                        valtozatosE = valtozatosEtkezesSzam;
                        if (!e.Valtozatossag || !valtozatos) valtozatosE = 0;
                        egysegTobbszorose = Convert.ToByte(e.EgysegTobbszorose);
                        if (folytonos) egysegTobbszorose = 0;
                        maxDarab = e.MaxDarab;

                        // annyi ételnek/italnak felel meg amennyi ételt/italt tartalmaz
                        etelSzam = 0;
                        italSzam = 0;
                        foreach (KeyValuePair<Elelmiszer, double> item2 in e.Osszetevo)
                        {
                            if (item2.Value > 0)
                            {
                                if (item2.Key is Etel) etelSzam++;
                                else italSzam++;
                            }
                        }


                        etelTomeg = e.EgysegTomegMennyiseg * e.TomegMertek.Valtoszam;
                        minEtelTomeg = 0;
                        maxEtelTomeg = nagySzam;

                        italUrmertek = e.EgysegUrTartalomMennyiseg * e.Urmertek.Valtoszam;
                        minItalUrmertek = 0;
                        maxItalUrmertek = nagySzam;
                        ar = e.Ar * e.Penz.Arfolyam / p.Arfolyam;
                        orom = e.Orom * (0.001 * etelTomeg + italUrmertek); // 1 l=1 kg-nak felel meg
                    }

                    str2 += String.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13} {14} {15} {16} {17} {18}", i,
                                reggelinelFogyaszthatoE, ebednelFogyaszthatoE, vacsoranalFogyaszthatoE,
                                valtozatosE, egysegTobbszorose, maxDarab,
                                etelSzam, etelTomeg, minEtelTomeg, maxEtelTomeg,
                                italSzam, italUrmertek, minItalUrmertek, maxItalUrmertek,
                                ar, orom, maxTomegE, maxUrmertekE
                                ).Replace(',', '.') + ((i == E) ? ";" : "") + Environment.NewLine;
                }

                //étkezésfeltételek
                str2 += Environment.NewLine + "param V: 0 1 2 :=" + Environment.NewLine;
                double[] f1 = new double[16];
                double[] f2 = new double[16];
                int[] b0 = new int[16];

                for (i = 0; i < 16; i++)
                {
                    f1[i] = 0;
                    f2[i] = 0;
                    b0[i] = 0;
                }

                foreach (EtkezesFeltetel item in etkezesfeltetelek)
                {
                    int pos = 8 * ((int)item.Eltipus2) + 2 * ((int)item.Ettipus2) + (int)item.Szamlalo;
                    f1[pos] = item.Minval;
                    f2[pos] = item.Maxval;
                    if (item.Eltipus2 == ElelmiszerTipus2.drink && item.Szamlalo == Szamlalo.quantity)
                    {
                        f1[pos] /= 10;
                        f2[pos] /= 10; // deciliter volt
                    }

                    b0[pos] = item.MaxvalE ? 1 : 0;
                }

                for (i = 0; i < 16; i++)
                {
                    str2 += string.Format("{0} {1} {2} {3}", i, f1[i], b0[i], f2[i]).Replace(',', '.') + (i == 15 ? ";" : "") +
                            Environment.NewLine;
                }
                
                i = 0;
                if (tapanyagok.Count > 0)
                {
                    //tápanyagtömb neve U lesz
                    str2 += Environment.NewLine + "param U: ";

                    for (i = 1; i <= tapanyagok.Count; i++)
                    {
                        str2 += string.Format("  {0}", i);
                    }

                    str2 += " :=" + Environment.NewLine;

                    i = 0;
                    foreach (Elelmiszer item in elelmiszerhasznalt)
                    {
                        i++;
                        j = 0;
                        str2 += String.Format("{0}", i);
                        foreach (Tapanyag item2 in tapanyagok)
                        {
                            j++;
                            str2 += String.Format("  {0}", item.TapanyagTartalom[item2]).Replace(',', '.');
                        }

                        str2 += ((i == E) ? ";" : "") + Environment.NewLine;
                    }

                    str2 += Environment.NewLine;
                    str2 += "param NapiMaxTapanyag:=" + Environment.NewLine;
                    for (i = 1; i <= tapanyagok.Count; i++)
                    {
                        double v = tapanyagok[i - 1].NapiMax ? tapanyagok[i - 1].NapiMaxBevitel : -1;
                        str2 += String.Format("{0} {1}", i, v).Replace(',', '.') + (i < T ? "," : ";") + Environment.NewLine;
                    }

                    str2 += Environment.NewLine;

                    str2 += "param NapiMinTapanyag:=" + Environment.NewLine;
                    for (i = 1; i <= tapanyagok.Count; i++)
                    {
                        str2 += string.Format("{0} {1}", i, tapanyagok[i - 1].NapiMinBevitel).Replace(',', '.') + (i < T ? "," : ";") +
                                Environment.NewLine;
                    }

                    str2 += Environment.NewLine;
                }

                str += "/* napi élelmiszeradagok kiszámítása */" + Environment.NewLine;
                str += "s.t. feltetel0{n in 1..N,e in 1..E}:x[n,4,e]=x[n,1,e]+x[n,2,e]+x[n,3,e];" + Environment.NewLine;
                str += Environment.NewLine;
                str += "/* ha nem fogyaszható, akkor x=0 */" + Environment.NewLine;
                str += "s.t. feltetel1{n in 1..N,i in 1..3,e in 1..E:W[e,i]=0 or (n=1 and i<E1) or (n=N and i>E2)}:x[n,i,e]=0;" +
                       Environment.NewLine;
                str += Environment.NewLine;

                /* pénz és öröm kiszámítása */
                str += "/* pénz kiszámítása */" + Environment.NewLine;
                str += "s.t. feltetel2: sum{n in 1..N,i in 1..3,e in 1..E}x[n,i,e]*W[e,15]=Penz;" + Environment.NewLine;
                str += Environment.NewLine;
                str += "/* öröm kiszámítása */" + Environment.NewLine;
                str += "s.t. feltetel3: sum{n in 1..N,i in 1..3,e in 1..E}x[n,i,e]*W[e,16]=Orom;" + Environment.NewLine;
                str += Environment.NewLine;

                /* maxdarab feltételek */
                str += "/* maxdarab feltételek */" + Environment.NewLine;
                str += "s.t. feltetel4{n in 1..N,i in 1..3,e in 1..E:W[e,6]!=-1}:x[n,i,e]<=W[e,6];" + Environment.NewLine;
                str += Environment.NewLine;

                /* egység többszöröse */
                str += "/* egység többszöröseire feltétel */" + Environment.NewLine;
                str += "s.t. feltetel5{n in 1..N,i in 1..3,e in 1..E:W[e,5]=1}:x[n,i,e]=y[n,i,e];" + Environment.NewLine;
                str += Environment.NewLine;
                str += "s.t. feltetel5a{n in 1..N,i in 1..3,e in 1..E:W[e,5]=1}:b[n,i,e]<=y[n,i,e];" + Environment.NewLine;
                str += Environment.NewLine;
                str += "s.t. feltetel5b{n in 1..N,i in 1..3,e in 1..E:W[e,5]=1}:c[n,i,e]<=y[n,i,e];" + Environment.NewLine;
                str += Environment.NewLine;

                str += "/* tömege,űrmértéke az élelmiszereknek */" + Environment.NewLine;
                str += "s.t. feltetel6{n in 1..N,i in 1..4,e in 1..E}:x[n,i,e]*W[e,8]=Tomeg[n,i,e];" + Environment.NewLine;
                str += "s.t. feltetel7{n in 1..N,i in 1..4,e in 1..E}:x[n,i,e]*W[e,12]=Urmertek[n,i,e];" +
                       Environment.NewLine;
                str += Environment.NewLine;

                str += "/* nagyszam>=tömeg,űrmérték>=1/nagyszam minden élelmiszerre */" + Environment.NewLine;
                str += "s.t. feltetel8{n in 1..N,i in 1..4,e in 1..E}:Tomeg[n,i,e]<=NagySzam*b[n,i,e];" + Environment.NewLine;
                str += "s.t. feltetel9{n in 1..N,i in 1..4,e in 1..E}:Tomeg[n,i,e]*NagySzam2>=b[n,i,e];" + Environment.NewLine;
                str += "s.t. feltetel10{n in 1..N,i in 1..4,e in 1..E}:Urmertek[n,i,e]<=NagySzam*c[n,i,e];" + Environment.NewLine;
                str += "s.t. feltetel11{n in 1..N,i in 1..4,e in 1..E}:Urmertek[n,i,e]*NagySzam2>=c[n,i,e];" + Environment.NewLine;
                str += Environment.NewLine;

                str += "/* tömeg,űrmértékekre feltételek minden élelmiszerre */" + Environment.NewLine;
                str += "s.t. feltetel12{n in 1..N,i in 1..3,e in 1..E}:Tomeg[n,i,e]>=W[e,9]*b[n,i,e];" + Environment.NewLine;
                str += "s.t. feltetel13{n in 1..N,i in 1..3,e in 1..E:W[e,17]=1}:Tomeg[n,i,e]<=W[e,10]*b[n,i,e];" + Environment.NewLine;
                str += "s.t. feltetel14{n in 1..N,i in 1..3,e in 1..E}:Urmertek[n,i,e]>=W[e,13]*c[n,i,e];" + Environment.NewLine;
                str += "s.t. feltetel15{n in 1..N,i in 1..3,e in 1..E:W[e,18]=1}:Urmertek[n,i,e]<=W[e,14]*c[n,i,e];" + Environment.NewLine;
                str += Environment.NewLine;


                if (T > 0)
                {
                    //tápanyagokra feltétel
                    str += "/* napi max,min bevitel a tápanyagokra */" + Environment.NewLine;
                    str += "s.t. feltetel16{n in 1..N,t in 1..T}:sum{e in 1..E}x[n,4,e]*U[e,t]=Tapanyag[n,t];" +
                           Environment.NewLine;
                    if (true)
                    {
                        str +=
                            "s.t. feltetel17{n in 1..N,t in 1..T:NapiMaxTapanyag[t]!=-1}:Tapanyag[n,t]<=NapiMaxTapanyag[t];" +
                            Environment.NewLine;
                    }

                    str += "s.t. feltetel18{n in 1..N,t in 1..T}:Tapanyag[n,t]>=NapiMinTapanyag[t];" +
                           Environment.NewLine;
                    str += Environment.NewLine;
                }

                //ételszám,tömeg,italszám,űrmérték feltételek adott étkezésre, illetve teljes napra [i=4 <=> egész napra feltétel]
                //itt is lehetne egy sokkal tömörebb felírás, de inkább szétszedjük a feltételeket több esetre
                str += "/* ételszám,tömeg,italszám,űrmérték számítása */" + Environment.NewLine;
                str += "s.t. feltetel19{n in 1..N,i in 1..4}:sum{e in 1..E}b[n,i,e]*W[e,7]=Adat[n,i,0];" + Environment.NewLine;
                str += "s.t. feltetel20{n in 1..N,i in 1..4}:sum{e in 1..E}x[n,i,e]*W[e,8]=Adat[n,i,1];" + Environment.NewLine;
                str += "s.t. feltetel21{n in 1..N,i in 1..4}:sum{e in 1..E}c[n,i,e]*W[e,11]=Adat[n,i,2];" + Environment.NewLine;
                str += "s.t. feltetel22{n in 1..N,i in 1..4}:sum{e in 1..E}x[n,i,e]*W[e,12]=Adat[n,i,3];" + Environment.NewLine;
                str += Environment.NewLine;

                // ételszámokra feltétel
                str += "s.t. feltetel23{n in 1..N,i in 1..4:i=4 or (3*n+i>=3+E1 and 3*n+i<=3*N+E2)}:Adat[n,i,0]>=V[2*(i-1),0];" + Environment.NewLine;
                str += "s.t. feltetel24{n in 1..N,i in 1..4:V[2*(i-1),1]=1 and (i=4 or (3*n+i>=3+E1 and 3*n+i<=3*N+E2))}:Adat[n,i,0]<=V[2*(i-1),2];" + Environment.NewLine;
                // ételtömegre feltétel
                str += "s.t. feltetel25{n in 1..N,i in 1..4:i=4 or (3*n+i>=3+E1 and 3*n+i<=3*N+E2)}:Adat[n,i,1]>=V[2*(i-1)+1,0];" + Environment.NewLine;
                str += "s.t. feltetel26{n in 1..N,i in 1..4:V[2*(i-1)+1,1]=1 and (i=4 or (3*n+i>=3+E1 and 3*n+i<=3*N+E2))}:Adat[n,i,1]<=V[2*(i-1)+1,2];" + Environment.NewLine;
                // italszámokra feltétel
                str += "s.t. feltetel27{n in 1..N,i in 1..4:i=4 or (3*n+i>=3+E1 and 3*n+i<=3*N+E2)}:Adat[n,i,2]>=V[2*(i-1)+8,0];" + Environment.NewLine;
                str += "s.t. feltetel28{n in 1..N,i in 1..4:V[2*(i-1)+8,1]=1 and (i=4 or (3*n+i>=3+E1 and 3*n+i<=3*N+E2))}:Adat[n,i,2]<=V[2*(i-1)+8,2];" + Environment.NewLine;
                // italűrmértékre feltétel
                str += "s.t. feltetel29{n in 1..N,i in 1..4:i=4 or (3*n+i>=3+E1 and 3*n+i<=3*N+E2)}:Adat[n,i,3]>=V[2*(i-1)+9,0];" + Environment.NewLine;
                str += "s.t. feltetel30{n in 1..N,i in 1..4:V[2*(i-1)+9,1]=1 and (i=4 or (3*n+i>=3+E1 and 3*n+i<=3*N+E2))}:Adat[n,i,3]<=V[2*(i-1)+9,2];" + Environment.NewLine;
                str += Environment.NewLine;

                //változatosságra feltétel
                //num tudjuk, hogy ételt vagy italt tartalmaz-e az adott élelmiszer, így külön szedjük a 2 esetet
                    str += "/* változatosságra feltétel */" + Environment.NewLine;
                    str +=
                        "s.t. feltetel31{n in 1..N,i in 1..3,e in 1..E:W[e,4]>1}:sum{m in 1..N,j in 1..3:3*m+j>=3*n+i and 3*m+j<3*n+i+D}b[m,j,e]<=1;" +
                        Environment.NewLine;
                    str +=
                        "s.t. feltetel32{n in 1..N,i in 1..3,e in 1..E:W[e,4]>1}:sum{m in 1..N,j in 1..3:3*m+j>=3*n+i and 3*m+j<3*n+i+D}c[m,j,e]<=1;" +
                        Environment.NewLine;
                    str += Environment.NewLine;

                // célfüggvény megadása, illetve még egy plusz feltétel is egy ágban
                if (etrendfeltetel.Koltsegmin)
                {
                    str += "minimize cost:Penz;" + Environment.NewLine + Environment.NewLine;
                }
                else if (etrendfeltetel.Orommax)
                {
                    str += "s.t. feltetel33:Penz<=Maxpenz;" + Environment.NewLine;
                    str += "maximize cost:Orom;" + Environment.NewLine + Environment.NewLine;
                }
                else
                {
                    str += "s.t. feltetel33:Penz<=Maxpenz;" + Environment.NewLine;
                    str += "maximize cost:0;" + Environment.NewLine + Environment.NewLine;
                }

                str += "/* a lineáris program megoldása */" + Environment.NewLine;
                str += "solve;" + Environment.NewLine + Environment.NewLine;
                str += "data;" + Environment.NewLine + Environment.NewLine;
                str += str2 + Environment.NewLine;
                str += "end;" + Environment.NewLine;

                File.WriteAllText(input_filename, str);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Unsuccessful save to file!");
            }


            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false; 
            startInfo.UseShellExecute = etrendfeltetel.Solverelrejt;

            startInfo.FileName = "glpsol.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = "--math -o " + output_filename + " " + input_filename + "\"";

            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    if (!exeProcess.WaitForExit(maxfutasiido))
                    {
                        throw new TimeoutException("We have reached the maximal running time, Glpk killed!");
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is TimeoutException) throw ex;
                throw new FileNotFoundException("Not found the glpsol.exe or dll !");
            }

            if (!File.Exists(output_filename)) throw new FileNotFoundException("File not found!");
            else
            {
                List<EAdat> C = new List<EAdat>();
                int i, j, sorokszama = 0;
                double opt = 0;

                foreach (string item in File.ReadAllLines(output_filename))
                {
                    sorokszama++;
                    if (sorokszama == 5 && item == "Status:     INTEGER EMPTY")
                    {
                        return new List<object> { false };
                    }

                    if (sorokszama == 6)
                    {
                        for (i = 0; item[i] != '='; i++) ;
                        i += 2;
                        for (j = i; item[j] != ' '; j++) ;
                        opt = double.Parse(item.Substring(i, j - i), NumberStyles.Any, new CultureInfo("en-US"));
                    }

                    string u = "";
                    for (i = 0; i < item.Length; i++)
                    {
                        if (item[i] != ' ' || i == 0 || item[i - 1] != ' ') u += item[i];
                    }

                    string[] s = u.Trim().Split(' ');

                    if (s.Length >= 3 && s[1][0] == 'x')
                    {
                        double v = double.Parse(s[2], NumberStyles.Any, new CultureInfo("en-US"));

                        if (v > 0)
                        {
                            string[] t = s[1].Substring(2, s[1].Length - 3).Split(',');
                            int i1 = Convert.ToInt32(t[1]);

                            if (i1 <= 3)
                            {
                                C.Add(new EAdat(Convert.ToInt32(t[0]), i1, Convert.ToInt32(t[2]), v));
                            }
                        }
                    }
                }

                Mertekegyseg u1 = (Mertekegyseg)ABKezelo.Kiolvasas()
                    .Where(x => x is Mertekegyseg && (x as Mertekegyseg).Megnevezes == "gram").ToList().First();
                Mertekegyseg u2 = (Mertekegyseg)ABKezelo.Kiolvasas()
                    .Where(x => x is Mertekegyseg && (x as Mertekegyseg).Megnevezes == "liter").ToList().First();

                string fnev = ABKezelo.GetCurrentUser();
                ABKezelo.TorolEtrendElelmiszer();

                for (int nap = 1; nap <= N; nap++)
                    for (int etkezes = 1; etkezes <= 3; etkezes++)
                    {
                        foreach (EAdat item in C)
                        {
                            if (item.nap == nap && item.etkezes == etkezes)
                            {
                                Elelmiszer e = elelmiszerhasznalt[item.elelmiszer - 1];
                                double tomeg = 0, urtartalom = 0;
                                Mertekegyseg m1 = u1;
                                Mertekegyseg m2 = u2;

                                if (e is Etel)
                                {
                                    tomeg = item.val * (e as Etel).EgysegTomegMennyiseg;
                                    m1 = (e as Etel).TomegMertek;
                                }
                                else if (e is Ital)
                                {
                                    urtartalom = item.val * (e as Ital).EgysegUrTartalomMennyiseg;
                                    m2 = (e as Ital).Urmertek;
                                }
                                else
                                {
                                    tomeg = item.val * (e as Menu).EgysegTomegMennyiseg;
                                    m1 = (e as Menu).TomegMertek;
                                    urtartalom = item.val * (e as Menu).EgysegUrTartalomMennyiseg;
                                    m2 = (e as Menu).Urmertek;
                                }

                                DateTime datum = datum1.AddDays(nap - 1);
                                ABKezelo.Beszuras(new EtrendElelmiszer(fnev, datum, (EtkezesTipus)(etkezes - 1), e, item.val, tomeg, m1,
                                    urtartalom, m2));
                            }
                        }
                    }
                return new List<object>() { true, opt, p };
            }
        }
    }
}
