using System;

namespace EtrendKeszito
{
	class Felhasznalo:EtrendAdat
	{
		private string jelszoHash;

		public string JelszoHash
		{
			get { return jelszoHash; }
			set { jelszoHash = value; }
		}

		private string jelszoEmlekeztetoOTP;

		public string JelszoEmlekeztetoOTP
		{
			get { return jelszoEmlekeztetoOTP; }
			set { jelszoEmlekeztetoOTP = value; }
		}

		public Felhasznalo(string felhasznaloNevHash, string jelszoHash, string jelszoEmlekeztetoOTP) : base(felhasznaloNevHash)
		{
			JelszoHash = jelszoHash;
			JelszoEmlekeztetoOTP = jelszoEmlekeztetoOTP;
		}


	}
}
