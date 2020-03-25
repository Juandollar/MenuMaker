using System;
using System.Globalization;
using System.Text;
using System.Numerics;
using System.Security.Cryptography;
using static System.Text.Encoding;

namespace EtrendKeszito
{
	enum KodolasIranya
	{
		Kódol,Dekódol
	}

	public class Titkositasok
	{
		public static string SHAHash(string szoveg)
		{
			return BitConverter.ToString(new SHA256CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(szoveg))).Replace("-", "").ToLower();
		}

		public BigInteger stringToNum(string s)
		{
			byte[] bytes = UTF8.GetBytes(s);
			BigInteger n = 0, m=0, t = 1;

			for (int i = 0; i < bytes.Length; i++)
			{
				n += bytes[i]*t;
				t *= 256;
			}

			return n;
		}

		public string NumToString(BigInteger n)
		{
			return Encoding.UTF8.GetString(n.ToByteArray());
		}

		internal string f0(string fnev,string uzenet,KodolasIranya k)
		{
			if (k == KodolasIranya.Kódol)
			{
				return (BigInteger.ModPow(2, stringToNum(fnev), BigInteger.Parse(Konstans.stringP)) ^ stringToNum(uzenet)).ToString("X");
			}

			return NumToString(BigInteger.ModPow(2, stringToNum(fnev), BigInteger.Parse(Konstans.stringP)) ^ BigInteger.Parse(uzenet, NumberStyles.HexNumber));
		}
	}
}
