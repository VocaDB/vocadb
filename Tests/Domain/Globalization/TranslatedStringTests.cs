#nullable disable

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
		private const string Eng = "Miku Miku! (English)";
		private const string Orig = "Miku Miku! (original)";
		private const string Rom = "Miku Miku! (Romaji)";
		private TranslatedString _str;

		[TestInitialize]
		public void SetUp()
		{
			_str = new TranslatedString(Orig, Rom, Eng);
		}

		/// <summary>
		/// Default property. Default language is Unspecified.
		/// </summary>
		[TestMethod]
		public void Default_DefaultLanguageIsUnspecified()
		{
			_str.DefaultLanguage = ContentLanguageSelection.Unspecified;

			Assert.AreEqual(Orig, _str.Default, "result is original");
		}

		/// <summary>
		/// Default property. Default language is Original.
		/// </summary>
		[TestMethod]
		public void Default_DefaultLanguageIsOriginal()
		{
			_str.DefaultLanguage = ContentLanguageSelection.Japanese;

			Assert.AreEqual(Orig, _str.Default, "result is original");
		}

		/// <summary>
		/// Default property. Default language is Romaji.
		/// </summary>
		[TestMethod]
		public void Default_DefaultLanguageIsRomaji()
		{
			_str.DefaultLanguage = ContentLanguageSelection.Romaji;

			Assert.AreEqual(Rom, _str.Default, "result is Romaji");
		}

		/// <summary>
		/// Default property. Default language is English.
		/// </summary>
		[TestMethod]
		public void Default_DefaultLanguageIsEnglish()
		{
			_str.DefaultLanguage = ContentLanguageSelection.English;

			Assert.AreEqual(Eng, _str.Default, "result is English");
		}

		/// <summary>
		/// Default property. Default language is Original. No Original translation is specified.
		/// </summary>
		[TestMethod]
		public void Default_DefaultLanguageIsOriginal_NoOriginalName()
		{
			_str.Japanese = string.Empty;
			_str.DefaultLanguage = ContentLanguageSelection.Japanese;

			Assert.AreEqual(Rom, _str.Default, "result is Romaji");
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
			_str.DefaultLanguage = ContentLanguageSelection.Japanese;

			var result = _str.GetDefaultOrFirst(ContentLanguageSelection.Unspecified);
			Assert.AreEqual(Orig, result, "result is first");
		}

		/// <summary>
		/// GetDefaultOrFirst method. Get Unspecified translation when default language is English.
		/// </summary>
		[TestMethod]
		public void GetDefaultOrFirst_Unspecified_DefaultLanguageIsEnglish()
		{
			_str.DefaultLanguage = ContentLanguageSelection.English;

			var result = _str.GetDefaultOrFirst(ContentLanguageSelection.Unspecified);
			Assert.AreEqual(Eng, result, "result is English");
		}

		/// <summary>
		/// GetDefaultOrFirst method. Get Unspecified translation when default language is Unspecified.
		/// </summary>
		[TestMethod]
		public void GetDefaultOrFirst_Unspecified_DefaultLanguageIsUnspecified()
		{
			_str.DefaultLanguage = ContentLanguageSelection.Unspecified;

			var result = _str.GetDefaultOrFirst(ContentLanguageSelection.Unspecified);
			Assert.AreEqual(Orig, result, "result is first");
		}

		/// <summary>
		/// GetDefaultOrFirst method. Get English translation when default language is Japanese.
		/// </summary>
		[TestMethod]
		public void GetDefaultOrFirst_English_DefaultLanguageIsJapanese()
		{
			_str.DefaultLanguage = ContentLanguageSelection.Japanese;

			var result = _str.GetDefaultOrFirst(ContentLanguageSelection.English);
			Assert.AreEqual(Eng, result, "result is English");
		}

		/// <summary>
		/// GetDefaultOrFirst method. Get English translation when default language is English.
		/// </summary>
		[TestMethod]
		public void GetDefaultOrFirst_English_DefaultLanguageIsEnglish()
		{
			_str.DefaultLanguage = ContentLanguageSelection.English;

			var result = _str.GetDefaultOrFirst(ContentLanguageSelection.English);
			Assert.AreEqual(Eng, result, "result is English");
		}

		/// <summary>
		/// GetDefaultOrFirst method. Get English translation when default language is Unspecified.
		/// </summary>
		[TestMethod]
		public void GetDefaultOrFirst_English_DefaultLanguageIsUnspecified()
		{
			_str.DefaultLanguage = ContentLanguageSelection.Unspecified;

			var result = _str.GetDefaultOrFirst(ContentLanguageSelection.English);
			Assert.AreEqual(Eng, result, "result is English");
		}

		/// <summary>
		/// GetDefaultOrFirst method. Get Unspecified translation when default language is Unspecified and no Original translation is specified.
		/// </summary>
		[TestMethod]
		public void GetDefaultOrFirst_Unspecified_DefaultLanguageIsUnspecified_NoOriginalName()
		{
			_str.DefaultLanguage = ContentLanguageSelection.Unspecified;
			_str.Japanese = string.Empty;

			var result = _str.GetDefaultOrFirst(ContentLanguageSelection.Unspecified);
			Assert.AreEqual(Rom, result, "result is Romaji");
		}

		/// <summary>
		/// GetDefaultOrFirst method. Get Original translation when default language is Unspecified and no Original translation is specified.
		/// </summary>
		[TestMethod]
		public void GetDefaultOrFirst_Original_DefaultLanguageIsUnspecified_NoOriginalName()
		{
			_str.DefaultLanguage = ContentLanguageSelection.Unspecified;
			_str.Japanese = string.Empty;

			var result = _str.GetDefaultOrFirst(ContentLanguageSelection.Japanese);
			Assert.AreEqual(Rom, result, "result is Romaji");
		}

		/// <summary>
		/// GetDefaultOrFirst method. Get English translation when default language is Unspecified and no Original translation is specified.
		/// </summary>
		[TestMethod]
		public void GetDefaultOrFirst_English_DefaultLanguageIsUnspecified_NoOriginalName()
		{
			_str.DefaultLanguage = ContentLanguageSelection.Unspecified;
			_str.Japanese = string.Empty;

			var result = _str.GetDefaultOrFirst(ContentLanguageSelection.English);
			Assert.AreEqual(Eng, result, "result is English");
		}
	}
}
