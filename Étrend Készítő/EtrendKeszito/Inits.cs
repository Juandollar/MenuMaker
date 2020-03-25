using System;
using System.Collections.Generic;
using System.Linq;

namespace EtrendKeszito
{
	static class Inits
	{
		public static void Init()
		{
		    try
		    {
		        string fnevhash = ABKezelo.GetCurrentUser();

		        List<EtrendAdat> e = ABKezelo.Kiolvasas();
		        List<EtrendAdat> e0 = new List<EtrendAdat>()
		        {
		            new Mertekegyseg(fnevhash, "gram", MertekegysegFajta.weight, 1, true, false),
		            new Mertekegyseg(fnevhash, "milligram", MertekegysegFajta.weight, 0.001, true, false),
		            new Mertekegyseg(fnevhash, "pound", MertekegysegFajta.weight, Konstans.pondToGramm, true, false),
		            new Mertekegyseg(fnevhash, "calorie", MertekegysegFajta.energy, 1, true, false),
		            new Mertekegyseg(fnevhash, "joule", MertekegysegFajta.energy, Konstans.jouleToCalorie, true,
		                false),
		            new Mertekegyseg(fnevhash, "deciliter", MertekegysegFajta.liquidmeasure, 0.1, true, false),
		            new Mertekegyseg(fnevhash, "liter", MertekegysegFajta.liquidmeasure, 1, true, false),
		            new Penznem(fnevhash, "Forint", "HUF", 1, 0, true, false),
		            new Penznem(fnevhash, "Dollar", "USD", Konstans.dollarToForint, 2, true, false),
		            new Penznem(fnevhash, "Euro", "EUR", Konstans.euroToForint, 2, true, false),
		            new Penznem(fnevhash, "English pound", "GBP", Konstans.fontToForint, 2, true, false)
		        };

		        int num = 0;
		        foreach (EtrendAdat item in e0)
		        {
		            int cnt = 0;
		            if (item is Mertekegyseg)
		            {
		                cnt = e.Where(x =>
		                        x is Mertekegyseg &&
		                        (x as Mertekegyseg).Megnevezes == (item as Mertekegyseg).Megnevezes)
		                    .Count();
		            }
		            else if (item is Penznem)
		            {
		                cnt = e.Where(x => x is Penznem && (x as Penznem).Megnevezes == (item as Penznem).Megnevezes)
		                    .Count();
		            }

		            if (cnt == 0)
		            {
		                ABKezelo.Beszuras(item);
		                num++;
		            }
		            else if (!(item is Penznem))
		            {
		                ABKezelo.Modositas(item);
		            }
		        }
		    }
		    catch (Exception ex)
		    {
		        throw ex;
		    }
		}
	}
}
