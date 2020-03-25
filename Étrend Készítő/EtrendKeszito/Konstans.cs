using System;

namespace EtrendKeszito
{
	static class Konstans// programban használt konstansok
	{
		public static string program_name = "Étrendkészitő v.1.0";

		public static string ekb_bank_xml_link = "http://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";

	    public static string logfilename = "log.txt";

		public static uint maxElkolthetoPenz = 1000000;

		public static double maxElelmiszerAr = 80000;

		public static double maxMennyiseg = 1000000;

		public static uint maxOrom = 10;

		public static byte maxEtkezesekSzama = 21;

		public static byte maxMenu = 25;//menük és koktélok összesített max. száma egy étkezésnél

		public static double pondToGramm = 453.59237;// 1 angol font ennyi gramm; vigyázat, sokféle font van használatban! Vigyázat, tömegnél gramm nálunk az alapegység és nem kilogramm !

		public static double jouleToCalorie = 0.239005736;// 1 Joule hány kalória

		public static uint maxValaszthatoFutasiIdo = 2000000;//másodpercben, ez nagyjából 23 nap
															 // 1000-szerese még 32 biten ábrázolható

		public static string stringP = "10462445011159556784495800706984017607745126918870658278228564371915959115517629201906613613936363856403824325945229334268449686529496700278536301496824187";//512 bites prim, titkosításhoz

		public static string salt = "ua0Xlwt3siikXHDOdSOBJuYc7iVvDlci";//32 karakteres véletlen string

		public const double dollarToForint = 265;

		public const double euroToForint = 300;

		public const double fontToForint = 340;
	}
}
