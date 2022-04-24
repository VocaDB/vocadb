using System.Net;
using Newtonsoft.Json;
using NLog;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Security.StopForumSpam;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Code.Security
{
	/// <summary>
	/// Contacts http://www.stopforumspam.com API and queries whether there's a high probability that an IP is malicious.
	/// </summary>
	public class StopForumSpamClient : IStopForumSpamClient
	{
		private const string ApiUrl = "https://www.stopforumspam.com/api?ip={0}&confidence&f=json";
		private static readonly Logger s_log = LogManager.GetCurrentClassLogger();

		public async Task<SFSResponseContract?> CallApiAsync(string? ip)
		{
			if (string.IsNullOrEmpty(ip))
				return null;

			if (WebHelper.IsLocalhost(ip))
				return new SFSResponseContract();

			var url = string.Format(ApiUrl, ip);
			SFSResultContract result;
			try
			{
				result = await JsonRequest.ReadObjectAsync<SFSResultContract>(url);
			}
			catch (WebException x)
			{
				s_log.Warn(x, "Unable to get response");
				return null;
			}
			catch (JsonSerializationException x)
			{
				s_log.Warn(x, "Unable to get response");
				return null;
			}

			if (!result.Success)
			{
				s_log.Warn("Request was not successful");
				return null;
			}

			result.IP.IP = ip;
			return result.IP;
		}
	}
}