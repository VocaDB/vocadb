using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain;
using VocaDb.Model.Service;
using VocaDb.Model.Utils;

namespace VocaDb.Tests.Service {

	[TestClass]
	public class EntryUrlParserTests {

		private const string baseUrl = "http://test.vocadb.net";
		private const string baseUrlSsl = "https://test.vocadb.net";

		private string GetFullUrl(string relative) {
			return VocaUriBuilder.MergeUrls(baseUrl, relative);
		}

		private void TestParse(string url, int expectedId, EntryType expectedType) {
			
			var result = new EntryUrlParser(baseUrl, baseUrlSsl).Parse(GetFullUrl(url));

			Assert.AreEqual(expectedId, result.Id, "Id");
			Assert.AreEqual(expectedType, result.EntryType, "EntryType");

		}

		private void TestParseRelative(string url, int expectedId, EntryType expectedType) {
			
			var result = new EntryUrlParser(baseUrl, baseUrlSsl).Parse(url, true);

			Assert.AreEqual(expectedId, result.Id, "Id");
			Assert.AreEqual(expectedType, result.EntryType, "EntryType");

		}

		[TestMethod]
		public void HostAddressesAreSame() {
			
			var result = new EntryUrlParser(baseUrl, baseUrl).Parse(GetFullUrl("/Artist/Details/39"));
			Assert.AreEqual(39, result.Id, "Id");
			Assert.AreEqual(EntryType.Artist, result.EntryType, "EntryType");

		}

		[TestMethod]
		public void NoMatch() {
			
			TestParse("/Search", 0, EntryType.Undefined);

		}

		[TestMethod]
		public void Long() {
			
			TestParse("/Artist/Details/39", 39, EntryType.Artist);

		}

		[TestMethod]
		public void Short() {

			TestParse("/S/39", 39, EntryType.Song);
			
		}

		[TestMethod]
		public void Short_Lowercase() {

			TestParse("/al/39", 39, EntryType.Album);
			
		}

		[TestMethod]
		public void Relative() {
		
			TestParseRelative("/S/10", 10, EntryType.Song);
	
		}

	}

}
