using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Helpers
{

	[TestClass]
	public class RelatedSitesHelperTests
	{

		private void TestRelatedSite(bool expected, string url)
		{
			var result = RelatedSitesHelper.IsRelatedSite(url);
			Assert.AreEqual(expected, result, url);
		}

		[TestMethod]
		public void IsRelatedSite_NoMatch()
		{
			TestRelatedSite(false, "http://google.com");
		}

		[TestMethod]
		public void IsRelatedSite_NoMatch2()
		{
			TestRelatedSite(false, "https://www5.atwiki.jp/hmiku/pages/37501.html");
		}

		[TestMethod]
		public void IsRelatedSite_MatchHttp()
		{
			TestRelatedSite(true, "http://vocadb.net/S/3939");
		}

		[TestMethod]
		public void IsRelatedSite_MatchHttps()
		{
			TestRelatedSite(true, "https://vocadb.net/S/3939");
		}

	}

}
