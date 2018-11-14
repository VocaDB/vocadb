using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Tests.Service.Helpers {

	/// <summary>
	/// Tests for <see cref="UrlHelper"/>.
	/// </summary>
	[TestClass]
	public class UrlHelperTests {

		[TestMethod]
		public void MakeLink_Empty() {

			var result = UrlHelper.MakeLink(string.Empty);

			Assert.AreEqual(string.Empty, result, "result");

		}

		[TestMethod]
		public void MakeLink_WithHttp() {

			var result = UrlHelper.MakeLink("http://vocadb.net");

			Assert.AreEqual("http://vocadb.net", result, "result");

		}

		[TestMethod]
		public void MakeLink_WithoutHttp() {

			var result = UrlHelper.MakeLink("vocadb.net");

			Assert.AreEqual("http://vocadb.net", result, "result");

		}

		[TestMethod]
		public void MakeLink_Mailto() {

			var result = UrlHelper.MakeLink("mailto:miku@vocadb.net");

			Assert.AreEqual("mailto:miku@vocadb.net", result, "result");

		}

		[TestMethod]
		public void UpgradeToHttps() {

			Assert.AreEqual("https://tn.smilevideo.jp/smile?i=6888548", UrlHelper.UpgradeToHttps("http://tn.smilevideo.jp/smile?i=6888548"), "http://tn.smilevideo.jp was upgraded");
			Assert.AreEqual("https://tn.smilevideo.jp/smile?i=6888548", UrlHelper.UpgradeToHttps("http://tn-skr1.smilevideo.jp/smile?i=6888548"), "http://tn-skr1.smilevideo.jp was upgraded");
			Assert.AreEqual("https://tn.smilevideo.jp/smile?i=6888548", UrlHelper.UpgradeToHttps("https://tn.smilevideo.jp/smile?i=6888548"), "https://tn.smilevideo.jp already HTTPS");
			Assert.AreEqual("https://tn.smilevideo.jp/smile?i=34172016.39165", UrlHelper.UpgradeToHttps("http://tn.smilevideo.jp/smile?i=34172016.39165"), "URL with dot was upgraded");

		}

	}
}
