using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace VocaDb.Model.Helpers {

	public class JsonRequest {

		/// <exception cref="WebException">If an error occurred.</exception>
		public static T ReadObject<T>(string url) {
			
			var request = (HttpWebRequest)WebRequest.Create(url);
			request.UserAgent = "VocaDB";

			using (var response = request.GetResponse())
			using (var stream = response.GetResponseStream())
			using (var streamReader = new StreamReader(stream))
			using (var jsonReader = new JsonTextReader(streamReader)) {
				var serializer = new JsonSerializer();
				return serializer.Deserialize<T>(jsonReader);
			}

		}

	}

}
