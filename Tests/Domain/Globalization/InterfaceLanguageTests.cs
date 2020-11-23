using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Tests.Domain.Globalization
{
	[TestClass]
	public class InterfaceLanguageTests
	{
		private void TestAvailableLanguageCode(string expected, string cultureName)
		{
			var result = InterfaceLanguage.GetAvailableLanguageCode(CultureInfo.GetCultureInfo(cultureName));
			Assert.AreEqual(expected, result, cultureName);
		}

		[TestMethod]
		public void GetAvailableLanguageCode_Us()
		{
			TestAvailableLanguageCode("en", "en-US");
		}

		[TestMethod]
		public void GetAvailableLanguageCode_Gb()
		{
			TestAvailableLanguageCode("en", "en-GB");
		}

		[TestMethod]
		public void GetAvailableLanguageCode_Ja()
		{
			TestAvailableLanguageCode("ja", "ja-JP");
		}

		[TestMethod]
		public void GetAvailableLanguageCode_Chinese_Simplified()
		{
			TestAvailableLanguageCode("zh", "zh-Hans");
		}

		[TestMethod]
		public void GetAvailableLanguageCode_Chinese_China()
		{
			TestAvailableLanguageCode("zh", "zh-CN");
		}

		[TestMethod]
		public void GetAvailableLanguageCode_NoMatch()
		{
			TestAvailableLanguageCode(string.Empty, "sv-SE");
		}
	}
}
