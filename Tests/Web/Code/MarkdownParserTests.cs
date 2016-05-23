using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Code.Markdown;

namespace VocaDb.Tests.Web.Code {

	[TestClass]
	public class MarkdownParserTests {

		private string GetHtml(string markdownText) {
			return new MarkdownParser(new FakeObjectCache()).GetHtml(markdownText);
		}

		private HtmlDocument GetHtmlDocument(string markdownText) {
			var doc = new HtmlDocument();
			doc.LoadHtml(GetHtml(markdownText));
			return doc;
		}

		// Test plain text (no markup).
		[TestMethod]
		public void GetHtml_PlainText() {
			
			var result = GetHtml("Miku Miku!").Trim();

			Assert.AreEqual("<p>Miku Miku!</p>", result, "result");

		}

		// Test automatic hyperlink generation.
		[TestMethod]
		public void GetHtml_AutoHyperlink() {
			
			var result = GetHtmlDocument("VocaDB homepage: http://vocadb.net").DocumentNode.ChildNodes[0];

			Assert.AreEqual(2, result.ChildNodes.Count, "Number of nodes");
			Assert.AreEqual("VocaDB homepage: ", result.ChildNodes[0].InnerHtml);
			
			var linkNode = result.ChildNodes[1];
			Assert.AreEqual(HtmlNodeType.Element, linkNode.NodeType, "linkNode.NodeType");
			Assert.AreEqual("a", linkNode.Name, "linkNode.Name");
			Assert.IsNotNull(linkNode.Attributes["href"], "linkNode.href");
			Assert.AreEqual("http://vocadb.net", linkNode.Attributes["href"].Value, "linkNode.href");

		}

		// Test automatic newlines
		[TestMethod]
		public void GetHtml_AutoNewline() {
			
			var result = GetHtml("Miku\nLuka").Trim();

			Assert.AreEqual("<p>Miku<br />\nLuka</p>", result, "result");

		}

		[TestMethod]
		public void GetHtml_StrictBoldItalic() {
			
			// _Vocaloids_ gets transformed, but _Luka_ doesn't
			var result = GetHtml("_Vocaloids_ Miku_Luka_Rin").Trim();

			Assert.AreEqual("<p><em>Vocaloids</em> Miku_Luka_Rin</p>", result, "result");

		}

		// HTML in input stream gets encoded
		[TestMethod]
		public void GetHtml_HtmlElement() {
		
			var result = GetHtml("Hack! <script>alert(1)</script>").Trim();

			Assert.AreEqual("<p>Hack! &lt;script&gt;alert(1)&lt;/script&gt;</p>", result, "result");

		}

		// HTML in link target attribute
		// #39: this doesn't work yet, but should be fixed
		[TestMethod]
		[Ignore]
		public void GetHtml_HtmlAttribute() {
			
			var result = GetHtml("[Click me](javascript:alert(1))");

			Assert.AreEqual("<p><a href=\"\">Click me</a></p>", result, "result");

		}

		private string StripWhitespace(string text) {
			return Regex.Replace(text, @"\s", string.Empty);
		}

		[TestMethod]
		public void GetHtml_BlockQuote() {

			var result = GetHtml(@">Miku Miku!\n>by Miku\n\nThis needs to be encoded :>");

			Assert.AreEqual(StripWhitespace("<blockquote><p>Miku Miku!<br />by Miku</p></blockquote><p>This needs to be encoded :&gt;</p>"), StripWhitespace(result), "result");

		}

	}

}
