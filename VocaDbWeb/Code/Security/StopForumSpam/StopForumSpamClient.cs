using System.IO;
using System.Net;
using NLog;
using Newtonsoft.Json;
using VocaDb.Model.Service.Security.StopForumSpam;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Code.Security {

	/// <summary>
	/// Contacts http://www.stopforumspam.com API and queries whether there's a high probability that an IP is malicious.
	/// </summary>
	public class StopForumSpamClient : IStopForumSpamClient {

		private const string apiUrl = "https://www.stopforumspam.com/api?ip={0}&confidence&f=json";
		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public SFSResponseContract CallApi(string ip) {

			if (string.IsNullOrEmpty(ip))
				return null;

			if (WebHelper.IsLocalhost(ip))
				return new SFSResponseContract();

			var url = string.Format(apiUrl, ip);

			var request = WebRequest.Create(url);
			request.Timeout = 10000;
			string data;
			try {
				using (var response = request.GetResponse())
				using (var stream = response.GetResponseStream())
				using (var reader = new StreamReader(stream)) {
					data = reader.ReadToEnd();
				}
			} catch (WebException x) {
				log.Warn(x, "Unable to get response");
				return null;
			}

			var result = JsonConvert.DeserializeObject<SFSResultContract>(data);

			if (!result.Success) {
				log.Warn("Request was not successful");
				return null;
			}

			result.IP.IP = ip;
			return result.IP;

		}

		public bool IsMalicious(string ip) {

			var result = CallApi(ip);

			if (result == null || !result.Appears)
				return false;

			double confidenceTreshold = 75d;

			return (result.Confidence > confidenceTreshold);

		}

	}

}