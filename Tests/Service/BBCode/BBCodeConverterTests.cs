using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Service.BBCode;

namespace VocaDb.Tests.Service.BBCode {

	/// <summary>
	/// Tests for <see cref="BBCodeConverter"/>.
	/// </summary>
	[TestClass]
	public class BBCodeConverterTests {

		private string ConvertToHtml(string source, params IBBCodeElementTransformer[] transformers) {
			return new BBCodeConverter(new IBBCodeElementTransformer[0]).ConvertToHtml(source);
		}

		[TestInitialize]
		public void SetUp() {

		}

		/// <summary>
		/// Verifies that HTML is encoded.
		/// </summary>
		[TestMethod]
		public void HtmlEncode() {

			var result = ConvertToHtml("<b>This is bolded text</b>");

			Assert.AreEqual("&lt;b&gt;This is bolded text&lt;/b&gt;", result, "result");

		}

	}

}
