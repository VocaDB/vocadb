#nullable disable

using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Service.BBCode;
using VocaDb.Web.Code;

namespace VocaDb.Tests.Service.BBCode
{
	/// <summary>
	/// Tests for <see cref="EntryLinkTransformer"/>.
	/// </summary>
	[TestClass]
	public class EntryLinkTransformerTests
	{
		private string ApplyTransform(string raw)
		{
			var code = new StringBuilder(raw);
			var linkFactory = new EntryAnchorFactory("http://test.vocadb.net");
			new EntryLinkTransformer(linkFactory).ApplyTransform(code);
			return code.ToString();
		}

		[TestMethod]
		public void ApplyTransform_Long()
		{
			var result = ApplyTransform("/Artist/Details/39");

			result.Should().Be("<a href=\"/Ar/39\">/Artist/Details/39</a>", "result");
		}

		[TestMethod]
		public void ApplyTransform_Short()
		{
			var result = ApplyTransform("/S/39");

			result.Should().Be("<a href=\"/S/39\">/S/39</a>", "result");
		}
	}
}
