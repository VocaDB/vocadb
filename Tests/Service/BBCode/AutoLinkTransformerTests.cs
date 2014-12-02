using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Service.BBCode;

namespace VocaDb.Tests.Service.BBCode {

	/// <summary>
	/// Tests for <see cref="AutoLinkTransformer"/>.
	/// </summary>
	[TestClass]
	public class AutoLinkTransformerTests {

		private string ApplyTransform(string raw) {

			var code = new StringBuilder(raw);
			new AutoLinkTransformer().ApplyTransform(code);
			return code.ToString();

		}

		[TestMethod]
		public void ApplyTransform() {

			var result = ApplyTransform("http://vocadb.net");

			Assert.AreEqual("<a href='http://vocadb.net'>http://vocadb.net</a>", result, "result");

		}

	}

}
