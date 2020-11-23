using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Tests.Domain.Globalization
{

	/// <summary>
	/// Tests for <see cref="TranslatedString"/>.
	/// </summary>
	[TestClass]
	public class TranslatedStringTests
	{

		private const string eng = "Miku Miku! (English)";
		private const string orig = "Miku Miku! (original)";
		private const string rom = "Miku Miku! (Romaji)";
		private TranslatedString str;

		[TestInitialize]
		public void SetUp()
		{

			str = new TranslatedString(orig, rom, eng);

		}

		/// <summary>
		/// Default property. Default language is Unspecified.
		/// </summary>
		[TestMethod]
		public void Default_DefaultLanguageIsUnspecified()
		{

			str.DefaultLanguage = ContentLanguageSelection.Unspecified;

			Assert.AreEqual(orig, str.Default, "result is original");

		}

		/// <summary>
		/// Default property. Default language is Original.
		/// </summary>
		[TestMethod]
		public void Default_DefaultLanguageIsOriginal()
		{

			str.DefaultLanguage = ContentLanguageSelection.Japanese;

			Assert.AreEqual(orig, str.Default, "result is original");

		}

		/// <summary>
		/// Default property. Default language is Romaji.
		/// </summary>
		[TestMethod]
		public void Default_DefaultLanguageIsRomaji()
		{

			str.DefaultLanguage = ContentLanguageSelection.Romaji;

			Assert.AreEqual(rom, str.Default, "result is Romaji");

		}

		/// <summary>
		/// Default property. Default language is English.
		/// </summary>
		[TestMethod]
		public void Default_DefaultLanguageIsEnglish()
		{

			str.DefaultLanguage = ContentLanguageSelection.English;

			Assert.AreEqual(eng, str.Default, "result is English");

		}

		/// <summary>
		/// Default property. Default language is Original. No Original translation is specified.
		/// </summary>
		[TestMethod]
		public void Default_DefaultLanguageIsOriginal_NoOriginalName()
		{

			str.Japanese = string.Empty;
			str.DefaultLanguage = ContentLanguageSelection.Japanese;

			Assert.AreEqual(rom, str.Default, "result is Romaji");

		}

		/// <summary>
		/// Default property. All names are empty.
		/// </summary>
		[TestMethod]
		public void Default_AllNamesEmpty()
		{

			var str = new TranslatedString();

			Assert.AreEqual(string.Empty, str.Default, "result is empty");

		}

		/// <summary>
		/// GetDefaultOrFirst method. Get Unspecified translation when default language is Japanese.
		/// </summary>
		[TestMethod]
		public void GetDefaultOrFirst_Unspecified_DefaultLanguageIsJapanese()
		{

			str.DefaultLanguage = ContentLanguageSelection.Japanese;

			var result = str.GetDefaultOrFirst(ContentLanguageSelection.Unspecified);
			Assert.AreEqual(orig, result, "result is first");

		}

		/// <summary>
		/// GetDefaultOrFirst method. Get Unspecified translation when default language is English.
		/// </summary>
		[TestMethod]
		public void GetDefaultOrFirst_Unspecified_DefaultLanguageIsEnglish()
		{

			str.DefaultLanguage = ContentLanguageSelection.English;

			var result = str.GetDefaultOrFirst(ContentLanguageSelection.Unspecified);
			Assert.AreEqual(eng, result, "result is English");

		}

		/// <summary>
		/// GetDefaultOrFirst method. Get Unspecified translation when default language is Unspecified.
		/// </summary>
		[TestMethod]
		public void GetDefaultOrFirst_Unspecified_DefaultLanguageIsUnspecified()
		{

			str.DefaultLanguage = ContentLanguageSelection.Unspecified;

			var result = str.GetDefaultOrFirst(ContentLanguageSelection.Unspecified);
			Assert.AreEqual(orig, result, "result is first");

		}

		/// <summary>
		/// GetDefaultOrFirst method. Get English translation when default language is Japanese.
		/// </summary>
		[TestMethod]
		public void GetDefaultOrFirst_English_DefaultLanguageIsJapanese()
		{

			str.DefaultLanguage = ContentLanguageSelection.Japanese;

			var result = str.GetDefaultOrFirst(ContentLanguageSelection.English);
			Assert.AreEqual(eng, result, "result is English");

		}

		/// <summary>
		/// GetDefaultOrFirst method. Get English translation when default language is English.
		/// </summary>
		[TestMethod]
		public void GetDefaultOrFirst_English_DefaultLanguageIsEnglish()
		{

			str.DefaultLanguage = ContentLanguageSelection.English;

			var result = str.GetDefaultOrFirst(ContentLanguageSelection.English);
			Assert.AreEqual(eng, result, "result is English");

		}

		/// <summary>
		/// GetDefaultOrFirst method. Get English translation when default language is Unspecified.
		/// </summary>
		[TestMethod]
		public void GetDefaultOrFirst_English_DefaultLanguageIsUnspecified()
		{

			str.DefaultLanguage = ContentLanguageSelection.Unspecified;

			var result = str.GetDefaultOrFirst(ContentLanguageSelection.English);
			Assert.AreEqual(eng, result, "result is English");

		}

		/// <summary>
		/// GetDefaultOrFirst method. Get Unspecified translation when default language is Unspecified and no Original translation is specified.
		/// </summary>
		[TestMethod]
		public void GetDefaultOrFirst_Unspecified_DefaultLanguageIsUnspecified_NoOriginalName()
		{

			str.DefaultLanguage = ContentLanguageSelection.Unspecified;
			str.Japanese = string.Empty;

			var result = str.GetDefaultOrFirst(ContentLanguageSelection.Unspecified);
			Assert.AreEqual(rom, result, "result is Romaji");

		}

		/// <summary>
		/// GetDefaultOrFirst method. Get Original translation when default language is Unspecified and no Original translation is specified.
		/// </summary>
		[TestMethod]
		public void GetDefaultOrFirst_Original_DefaultLanguageIsUnspecified_NoOriginalName()
		{

			str.DefaultLanguage = ContentLanguageSelection.Unspecified;
			str.Japanese = string.Empty;

			var result = str.GetDefaultOrFirst(ContentLanguageSelection.Japanese);
			Assert.AreEqual(rom, result, "result is Romaji");

		}

		/// <summary>
		/// GetDefaultOrFirst method. Get English translation when default language is Unspecified and no Original translation is specified.
		/// </summary>
		[TestMethod]
		public void GetDefaultOrFirst_English_DefaultLanguageIsUnspecified_NoOriginalName()
		{

			str.DefaultLanguage = ContentLanguageSelection.Unspecified;
			str.Japanese = string.Empty;

			var result = str.GetDefaultOrFirst(ContentLanguageSelection.English);
			Assert.AreEqual(eng, result, "result is English");

		}

	}

}
