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

	}
}
