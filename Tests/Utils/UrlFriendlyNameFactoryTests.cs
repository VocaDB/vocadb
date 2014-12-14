using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Utils;

namespace VocaDb.Tests.Utils {

	[TestClass]
	public class UrlFriendlyNameFactoryTests {

		private void CallGetUrlFriendlyName(string expected, string input) {

			var result = UrlFriendlyNameFactory.GetUrlFriendlyName(input);
			Assert.AreEqual(expected, result, input);

		}

		[TestMethod]
		public void English() {

			// Latin characters get included as is.
			CallGetUrlFriendlyName("hatsune-miku", "Hatsune Miku");

		}

		/// <summary>
		/// </summary>
		[TestMethod]
		public void Japanese() {

			// Non-ASCII characters get removed.
			CallGetUrlFriendlyName(string.Empty, "初音ミク");

		}

		[TestMethod]
		public void Numbers() {

			// Digits are supported.
			CallGetUrlFriendlyName("apple41", "apple41");

		}

		[TestMethod]
		public void Trim_Japanese() {
			
			// Trim Japanese
			CallGetUrlFriendlyName("", "- 神想フ、時ノ境界 -");

		}

		[TestMethod]
		public void PreferRomajiForJapanese() {
			
			var nameManager = new NameManager<LocalizedStringWithId>();
			nameManager.SortNames.DefaultLanguage = ContentLanguageSelection.Japanese;
			nameManager.Add(new LocalizedStringWithId("Japanese name", ContentLanguageSelection.Japanese));
			nameManager.Add(new LocalizedStringWithId("Romaji name", ContentLanguageSelection.Romaji));
			nameManager.Add(new LocalizedStringWithId("English name", ContentLanguageSelection.English));
			nameManager.UpdateSortNames();

			var result = UrlFriendlyNameFactory.GetUrlFriendlyName(nameManager);

			Assert.AreEqual("romaji-name", result);

		}

		[TestMethod]
		public void UseEnglishIfDefaultLanguage() {
			
			var nameManager = new NameManager<LocalizedStringWithId>();
			nameManager.SortNames.DefaultLanguage = ContentLanguageSelection.English;
			nameManager.Add(new LocalizedStringWithId("Japanese name", ContentLanguageSelection.Japanese));
			nameManager.Add(new LocalizedStringWithId("Romaji name", ContentLanguageSelection.Romaji));
			nameManager.Add(new LocalizedStringWithId("English name", ContentLanguageSelection.English));
			nameManager.UpdateSortNames();

			var result = UrlFriendlyNameFactory.GetUrlFriendlyName(nameManager);

			Assert.AreEqual("english-name", result);

		}
	}

}
