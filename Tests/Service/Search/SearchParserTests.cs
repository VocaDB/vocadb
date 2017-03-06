using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Service.Search;

namespace VocaDb.Tests.Service.Search {

	/// <summary>
	/// Tests for <see cref="SearchParser"/>.
	/// </summary>
	[TestClass]
	public class SearchParserTests {

		private void AssertSearchWord(SearchWordCollection result, string propName, params string[] values) {

			var words = result.TakeAll(propName);

			Assert.AreEqual(values.Length, words.Length, "Number of words matches");

			if (values.Length == 1 && words.Length == 1) {
				Assert.AreEqual(values.First(), words.First().Value);
			} else {

				foreach (var value in values) {
					Assert.IsTrue(words.Any(w => w.Value == value));
				}

			}

		}

		/// <summary>
		/// Query with keywords (in this case artist name)
		/// </summary>
		[TestMethod]
		public void ParseQuery_QueryWithKeywords() {

			var result = SearchParser.ParseQuery("artist:doriko Nostalgia");

			AssertSearchWord(result, "artist", "doriko");
			AssertSearchWord(result, string.Empty, "Nostalgia");

		}

		/// <summary>
		/// Keywords + phrase (series of words)
		/// </summary>
		[TestMethod]
		public void ParseQuery_KeywordWithPhrase() {

			var result = SearchParser.ParseQuery("artist:\"Hatsune Miku\" Nostalgia");

			AssertSearchWord(result, "artist", "Hatsune Miku");
			AssertSearchWord(result, string.Empty, "Nostalgia");

		}

		/// <summary>
		/// Multiple words, no keywords
		/// </summary>
		[TestMethod]
		public void ParseQuery_MultipleWords() {

			var result = SearchParser.ParseQuery("Romeo and Cinderella");

			AssertSearchWord(result, string.Empty, "Romeo", "and", "Cinderella");

		}

		/// <summary>
		/// Single phrase
		/// </summary>
		[TestMethod]
		public void ParseQuery_QueryWithPhrase() {

			var result = SearchParser.ParseQuery("\"Romeo and Cinderella\"");

			AssertSearchWord(result, string.Empty, "Romeo and Cinderella");

		}

		/// <summary>
		/// Words + phrase
		/// </summary>
		[TestMethod]
		public void ParseQuery_WordsAndPhrase() {

			var result = SearchParser.ParseQuery("\"Romeo and Cinderella\" Hatsune Miku");

			AssertSearchWord(result, string.Empty, "Romeo and Cinderella", "Hatsune", "Miku");

		}

		/// <summary>
		/// Phrase with non-word characters
		/// </summary>
		[TestMethod]
		public void ParseQuery_SpecialCharacters() {

			var result = SearchParser.ParseQuery("\"Dancing☆Samurai\" artist:Gackpoid-V2");

			AssertSearchWord(result, string.Empty, "Dancing☆Samurai");
			AssertSearchWord(result, "artist", "Gackpoid-V2");

		}

	}

}
