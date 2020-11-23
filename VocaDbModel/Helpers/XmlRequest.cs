using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VocaDb.Model.Helpers
{
	public static class XmlRequest
	{
		private static T GetXmlResponse<T>(Stream stream)
		{
			var serializer = new XmlSerializer(typeof(T));
			return (T)serializer.Deserialize(stream);
		}

		/// <summary>
		/// Performers a HTTP request that returns XML and deserializes that XML result into object.
		/// </summary>
		/// <typeparam name="T">Object type.</typeparam>
		/// <param name="url">URL to be requested.</param>
		/// <returns>Deserialized object. Cannot be null.</returns>
		/// <exception cref="HttpRequestException">If the request failed.</exception>
		public static Task<T> GetXmlObjectAsync<T>(string url)
		{
			return HtmlRequestHelper.GetStreamAsync(url, stream => GetXmlResponse<T>(stream));
		}
	}
}
