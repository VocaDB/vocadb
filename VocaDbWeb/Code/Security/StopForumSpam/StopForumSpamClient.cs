using System.IO;
using System.Net;
using NLog;
using Newtonsoft.Json;
using VocaDb.Model.Service.Security.StopForumSpam;
using VocaDb.Web.Helpers;
using System.Threading.Tasks;
using VocaDb.Model.Helpers;

namespace VocaDb.Web.Code.Security
{
	/// <summary>
	/// Contacts http://www.stopforumspam.com API and queries whether there's a high probability that an IP is malicious.
	/// </summary>
	public class StopForumSpamClient : IStopForumSpamClient
	{
		private const string apiUrl = "https://www.stopforumspam.com/api?ip={0}&confidence&f=json";
		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public async Task<SFSResponseContract> CallApiAsync(string ip)
		{
			if (string.IsNullOrEmpty(ip))
				return null;

			if (WebHelper.IsLocalhost(ip))
				return new SFSResponseContract();

			var url = string.Format(apiUrl, ip);
			SFSResultContract result;
			try
			{
				result = await JsonRequest.ReadObjectAsync<SFSResultContract>(url);
			}
			catch (WebException x)
			{
				log.Warn(x, "Unable to get response");
				return null;
			}
			catch (JsonSerializationException x)
			{
				log.Warn(x, "Unable to get response");
				return null;
			}

			if (!result.Success)
			{
				log.Warn("Request was not successful");
				return null;
			}

			result.IP.IP = ip;
			return result.IP;
		}
	}
}