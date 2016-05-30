using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Service.Search;

namespace VocaDb.Tests.Service.Search {

	[TestClass]
	public class SearchParserTests {

		[TestMethod]
		public void ParseQuery_QueryWithKeywords() {

			var result = SearchParser.ParseQuery("artist-name:doriko Nostalgia");

			var artistNames = result.TakeAll("artist-name");
			Assert.AreEqual("doriko", artistNames.FirstOrDefault()?.Value, "Found artist name");

			var queryWords = result.TakeAll(string.Empty);
			Assert.AreEqual("Nostalgia", queryWords.FirstOrDefault()?.Value, "Found query word");

		}

		[TestMethod]
		public void ParseQuery_MultipleWords() {

			var result = SearchParser.ParseQuery("Romeo and Cinderella");

			var queryWords = result.TakeAll(string.Empty);
			Assert.AreEqual(3, queryWords.Length, "Found 3 words");
			Assert.IsTrue(queryWords.Any(w => w.Value == "Romeo"), "Found the first word");

		}

		[TestMethod]
		[Ignore]
		public void ParseQuery_QueryWithPhrase() {

			// TODO
			var result = SearchParser.ParseQuery("\"Romeo and Cinderella\"");

			var queryWords = result.TakeAll(string.Empty);
			Assert.AreEqual(1, queryWords.Length, "Found one phrase");
			Assert.IsTrue(queryWords.Any(w => w.Value == "Romeo and Cinderella"), "Found the phrase");

		}

	}

}
