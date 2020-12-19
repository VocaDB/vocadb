#nullable disable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Service.ExtSites;
using VocaDb.Model.Utils.Config;

namespace VocaDb.Tests.Service.ExtSites
{
	[TestClass]
	public class AffiliateLinkGeneratorTests
	{
		private AffiliateLinkGenerator _generator;
		private const string PaAffId = "852809";

		[TestInitialize]
		public void SetUp()
		{
			_generator = new AffiliateLinkGenerator(new VdbConfigManager());
		}

		[TestMethod]
		public void PlayAsia()
		{
			var input = "http://www.play-asia.com/0005-a-galaxy-odyssey-paOS-13-49-en-70-7sjp.html";
			var expected = "http://www.play-asia.com/0005-a-galaxy-odyssey-paOS-13-49-en-70-7sjp.html?affiliate_id=852809";

			var result = _generator.GenerateAffiliateLink(input);

			Assert.AreEqual(expected, result, "Play-asia affiliate link matches");
		}

		[TestMethod]
		public void PlayAsia_ReplaceAffId()
		{
			var input = "http://www.play-asia.com/0005-a-galaxy-odyssey-paOS-13-49-en-70-7sjp.html?affiliate_id=12345";
			var expected = "http://www.play-asia.com/0005-a-galaxy-odyssey-paOS-13-49-en-70-7sjp.html?affiliate_id=852809";

			var result = _generator.GenerateAffiliateLink(input);

			Assert.AreEqual(expected, result, "Play-asia affiliate link matches");
		}

		[TestMethod]
		public void Amazon()
		{
			var input = "http://www.amazon.co.jp/dp/B00K1IV8FM";
			var expected = "http://www.amazon.co.jp/dp/B00K1IV8FM?tag=vocadb";

			var result = _generator.GenerateAffiliateLink(input);

			Assert.AreEqual(expected, result, "Amazon affiliate link matches");
		}

		[TestMethod]
		public void Amazon_ReplaceAffId()
		{
			var input = "http://www.amazon.co.jp/dp/B00K1IV8FM?tag=another";
			var expected = "http://www.amazon.co.jp/dp/B00K1IV8FM?tag=vocadb";

			var result = _generator.GenerateAffiliateLink(input);

			Assert.AreEqual(expected, result, "Amazon affiliate link matches");
		}
	}
}
