using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using NLog;
using Newtonsoft.Json;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Code.Security {

	public interface IStopForumSpamClient {

		SFSResponseContract CallApi(string ip);

	}

	/// <summary>
	/// Contacts http://www.stopforumspam.com API and queries whether there's a high probability that an IP is malicious.
	/// </summary>
	public class StopForumSpamClient : IStopForumSpamClient {

		private const string apiUrl = "http://www.stopforumspam.com/api?ip={0}&confidence&f=json";
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
				log.WarnException("Unable to get response", x);
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

	[DataContract]
	public class SFSResultContract {

		[DataMember]
		public bool Success { get; set; }

		[DataMember]
		public SFSResponseContract IP { get; set; }

	}

	[DataContract]
	public class SFSResponseContract {

		public const double confidenceTreshold = 75d;

		[DataMember]
		public bool Appears { get; set; }

		public SFSCheckResultType Conclusion {
			get {
				if (Appears && Confidence > confidenceTreshold)
					return SFSCheckResultType.Malicious;
				if (Appears)
					return SFSCheckResultType.Uncertain;
				return SFSCheckResultType.Harmless;
			}
		}

		[DataMember]
		public double Confidence { get; set; }

		[DataMember]
		public int Frequency { get; set; }

		[DataMember]
		public string IP { get; set; }

		[DataMember]
		public DateTime LastSeen { get; set; }

	}

	public enum SFSCheckResultType {
		
		Harmless,

		Malicious,

		Uncertain

	}

	/*[DataContract(Name = "response", Namespace = "")]
	public class SFSResponseContract {

		public const double confidenceTreshold = 75d;

		[DataMember(Name = "appears")]
		public string Appears { get; set; }

		public bool AppearsBool { 
			get {
				return (Appears == "yes");
			}
		}

		[DataMember(Name = "confidence")]
		public double Confidence { get; set; }

		[DataMember(Name = "frequency")]
		public int Frequency { get; set; }

		[DataMember]
		public string IP { get; set; }

		public bool Malicious {
			get {
				return Confidence > confidenceTreshold;
			}
		}

		[DataMember]
		public DateTime LastSeen { get; set; }

	}*/

}