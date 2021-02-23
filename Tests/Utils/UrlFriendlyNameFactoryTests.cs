#nullable disable

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Utils;

namespace VocaDb.Tests.Utils
{
	public class UrlFriendlyNameFactoryTests
	{
		[TestClass]
		public class FromPlainString
		{
			private void CallGetUrlFriendlyName(string expected, string input)
			{
				var result = UrlFriendlyNameFactory.GetUrlFriendlyName(input);
				result.Should().Be(expected, input);
			}

			[TestMethod]
			public void English()
			{
				// Latin characters get included as is.
				CallGetUrlFriendlyName("hatsune-miku", "Hatsune Miku");
			}

			/// <summary>
			/// </summary>
			[TestMethod]
			public void Japanese()
			{
				// Non-ASCII characters get removed.
				CallGetUrlFriendlyName(string.Empty, "初音ミク");
			}

			[TestMethod]
			public void Numbers()
			{
				// Digits are supported.
				CallGetUrlFriendlyName("apple41", "apple41");
			}

			[TestMethod]
			public void Trim_Japanese()
			{
				// Trim Japanese
				CallGetUrlFriendlyName("", "- 神想フ、時ノ境界 -");
			}
		}

		[TestClass]
		public class FromNameManager
		{
			[TestMethod]
			public void PreferRomajiForJapanese()
			{
				var nameManager = new NameManager<LocalizedStringWithId>();
				nameManager.SortNames.DefaultLanguage = ContentLanguageSelection.Japanese;
				nameManager.Add(new LocalizedStringWithId("Japanese name", ContentLanguageSelection.Japanese));
				nameManager.Add(new LocalizedStringWithId("Romaji name", ContentLanguageSelection.Romaji));
				nameManager.Add(new LocalizedStringWithId("English name", ContentLanguageSelection.English));
				nameManager.UpdateSortNames();

				var result = UrlFriendlyNameFactory.GetUrlFriendlyName(nameManager);

				result.Should().Be("romaji-name");
			}

			[TestMethod]
			public void UseEnglishIfDefaultLanguage()
			{
				var nameManager = new NameManager<LocalizedStringWithId>();
				nameManager.SortNames.DefaultLanguage = ContentLanguageSelection.English;
				nameManager.Add(new LocalizedStringWithId("Japanese name", ContentLanguageSelection.Japanese));
				nameManager.Add(new LocalizedStringWithId("Romaji name", ContentLanguageSelection.Romaji));
				nameManager.Add(new LocalizedStringWithId("English name", ContentLanguageSelection.English));
				nameManager.UpdateSortNames();

				var result = UrlFriendlyNameFactory.GetUrlFriendlyName(nameManager);

				result.Should().Be("english-name");
			}
		}

		[TestClass]
		public class FromTranslatedString
		{
			private void CallGetUrlFriendlyName(string expected, TranslatedString input)
			{
				var result = UrlFriendlyNameFactory.GetUrlFriendlyName(input);
				result.Should().Be(expected, input.ToString());
			}

			[TestMethod]
			public void PreferRomajiForJapanese()
			{
				var translatedString = new TranslatedString("進撃の巨人", "Shingeki no Kyojin", "Attack on Titan", ContentLanguageSelection.Japanese);
				CallGetUrlFriendlyName("shingeki-no-kyojin", translatedString);
			}

			[TestMethod]
			public void UseEnglishIfDefaultLanguage()
			{
				var translatedString = new TranslatedString("ロック", string.Empty, "rock", ContentLanguageSelection.English);
				CallGetUrlFriendlyName("rock", translatedString);
			}

			[TestMethod]
			public void AllJapanese()
			{
				var translatedString = TranslatedString.Create("進撃の巨人");
				CallGetUrlFriendlyName(string.Empty, translatedString);
			}
		}
	}
}
