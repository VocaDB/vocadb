#nullable disable

using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Code.Markdown;

namespace VocaDb.Tests.Web.Code
{
	[TestClass]
	public class MarkdownParserTests
	{
		private readonly MarkdownParser parser = new MarkdownParser(new FakeObjectCache());

		private string GetHtml(string markdownText)
		{
			return parser.GetHtml(markdownText);
		}

		private void TestGetHtml(string expected, string markdownText)
		{
			var result = GetHtml(markdownText)?.Trim();
			Assert.AreEqual(expected, result, markdownText);
		}

		private void TestGetPlainText(string expected, string input)
		{
			var result = parser.GetPlainText(input)?.Trim();
			Assert.AreEqual(expected, result, input);
		}

		private HtmlDocument GetHtmlDocument(string markdownText)
		{
			var doc = new HtmlDocument();
			doc.LoadHtml(GetHtml(markdownText));
			return doc;
		}

		// Test plain text (no markup).
		[TestMethod]
		public void GetHtml_PlainText()
		{
			TestGetHtml("<p>Miku Miku!</p>", "Miku Miku!");
		}

		// Test automatic hyperlink generation.
		[TestMethod]
		public void GetHtml_AutoHyperlink()
		{
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
		public void GetHtml_AutoNewline()
		{
			TestGetHtml("<p>Miku<br />\nLuka</p>", "Miku\nLuka");
		}

		[TestMethod]
		public void GetHtml_StrictBoldItalic()
		{
			// _Vocaloids_ gets transformed, but _Luka_ doesn't
			TestGetHtml("<p><em>Vocaloids</em> Miku_Luka_Rin</p>", "_Vocaloids_ Miku_Luka_Rin");
		}

		// HTML in input stream gets encoded
		[TestMethod]
		public void GetHtml_HtmlElement()
		{
			TestGetHtml("<p>Hack! &lt;script&gt;alert(1)&lt;/script&gt;</p>", "Hack! <script>alert(1)</script>");
		}

		// HTML in link target attribute
		// #39: this doesn't work yet, but should be fixed
		[TestMethod]
		[Ignore]
		public void GetHtml_HtmlAttribute()
		{
			TestGetHtml("<p><a href=\"\">Click me</a></p>", "[Click me](javascript:alert(1))");
		}

		private string StripWhitespace(string text)
		{
			return Regex.Replace(text, @"\s", string.Empty);
		}

		[TestMethod]
		public void GetHtml_BlockQuote()
		{
			var result = GetHtml(">Miku Miku!\n>by Miku\n\nThis needs to be encoded :>");

			Assert.AreEqual(StripWhitespace("<blockquote><p>Miku Miku!<br />by Miku</p></blockquote><p>This needs to be encoded :&gt;</p>"), StripWhitespace(result), "result");
		}

		[TestMethod]
		public void GetPlainText_PlainText()
		{
			TestGetPlainText("Miku Miku!", "Miku Miku!");
		}

		[TestMethod]
		public void GetPlainText_Markdown()
		{
			TestGetPlainText("Miku Miku!", "*Miku Miku!*");
		}

		[TestMethod]
		public void GetPlainText_Html()
		{
			TestGetPlainText("Hack! &lt;script&gt;alert(1)&lt;/script&gt;", "Hack! <script>alert(1)</script>");
		}

		[TestMethod]
		public void GetPlainText_Apostrophe()
		{
			TestGetPlainText("'Miku Miku!'", "'Miku Miku!'");
		}
	}
}
