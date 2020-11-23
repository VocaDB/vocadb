using System.Net;

namespace VocaDb.Model.Service.Security
{

	public static class SslHelper
	{

		/// <summary>
		/// Force to use TLS 1.2 / TLS 1.3.
		/// </summary>
		public static void ForceStrongTLS()
		{
			// https://twittercommunity.com/t/removing-support-for-legacy-tls-versions-1-0-1-1-on-twitter/126648
			// TODO: TLS 1.2 should already be default in .NET 4.8. Figure out why not.
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
		}

	}
}
