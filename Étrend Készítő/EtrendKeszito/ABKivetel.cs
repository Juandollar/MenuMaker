using System;

namespace EtrendKeszito
{
	[Serializable]
	internal class ABKivetel : Exception
	{
		public ABKivetel(string message, Exception ex) : base(message, ex)
		{

		}
	}
}
