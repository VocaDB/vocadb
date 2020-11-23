using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Helpers
{

	/// <summary>
	/// Tests for <see cref="LyricsHelper"/>
	/// </summary>
	[TestClass]
	public class LyricsHelperTests
	{

		private static LyricsForSongContract Lyrics(string cultureCode, TranslationType translationType = TranslationType.Translation)
		{
			return new LyricsForSongContract { CultureCode = cultureCode, TranslationType = translationType, Source = string.Empty, Value = "Miku Miku!" };
		}

		private readonly LyricsForSongContract[] lyrics = {
			Lyrics(OptionalCultureCode.LanguageCode_Japanese, TranslationType.Original),
			Lyrics(OptionalCultureCode.LanguageCode_Japanese, TranslationType.Romanized),
			Lyrics(OptionalCultureCode.LanguageCode_English),
			Lyrics("ru")
		};

		[TestMethod]
		public void GetDefaultLyrics_UiLanguage()
		{

			var result = LyricsHelper.GetDefaultLyrics(lyrics, new OptionalCultureCode(OptionalCultureCode.LanguageCode_English), null, null);

			Assert.AreEqual(OptionalCultureCode.LanguageCode_English, result?.CultureCode, "CultureCode");

		}

		[TestMethod]
		public void GetDefaultLyrics_UserLanguage()
		{

			var result = LyricsHelper.GetDefaultLyrics(lyrics, new OptionalCultureCode("es"),
				new[] { new OptionalCultureCode("mi-ku"), new OptionalCultureCode(OptionalCultureCode.LanguageCode_English) }, null);

			Assert.AreEqual(OptionalCultureCode.LanguageCode_English, result?.CultureCode, "CultureCode");

		}

		[TestMethod]
		public void GetDefaultLyrics_Original()
		{

			var result = LyricsHelper.GetDefaultLyrics(lyrics, new OptionalCultureCode("es"),
				new[] { new OptionalCultureCode("mi-ku") }, new Lazy<IEnumerable<UserKnownLanguage>>(() => null));

			Assert.AreEqual(OptionalCultureCode.LanguageCode_Japanese, result?.CultureCode, "CultureCode");

		}

	}

}
