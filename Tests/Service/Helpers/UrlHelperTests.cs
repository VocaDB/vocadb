#nullable disable

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Tests.Service.Helpers
{
	/// <summary>
	/// Tests for <see cref="UrlHelper"/>.
	/// </summary>
	[TestClass]
	public class UrlHelperTests
	{
		[TestMethod]
		public void MakeLink_Empty()
		{
			var result = UrlHelper.MakeLink(string.Empty);

			result.Should().Be(string.Empty, "result");
		}

		[TestMethod]
		public void MakeLink_WithHttp()
		{
			var result = UrlHelper.MakeLink("http://vocadb.net");

			result.Should().Be("http://vocadb.net", "result");
		}

		[TestMethod]
		public void MakeLink_WithoutHttp()
		{
			var result = UrlHelper.MakeLink("vocadb.net");

			result.Should().Be("http://vocadb.net", "result");
		}

		[TestMethod]
		public void MakeLink_Mailto()
		{
			var result = UrlHelper.MakeLink("mailto:miku@vocadb.net");

			result.Should().Be("mailto:miku@vocadb.net", "result");
		}

		[TestMethod]
		public void UpgradeToHttps()
		{
			UrlHelper.UpgradeToHttps("http://tn.smilevideo.jp/smile?i=6888548").Should().Be("https://tn.smilevideo.jp/smile?i=6888548", "http://tn.smilevideo.jp was upgraded");
			UrlHelper.UpgradeToHttps("http://tn-skr1.smilevideo.jp/smile?i=6888548").Should().Be("https://tn.smilevideo.jp/smile?i=6888548", "http://tn-skr1.smilevideo.jp was upgraded");
			UrlHelper.UpgradeToHttps("https://tn.smilevideo.jp/smile?i=6888548").Should().Be("https://tn.smilevideo.jp/smile?i=6888548", "https://tn.smilevideo.jp already HTTPS");
			UrlHelper.UpgradeToHttps("http://tn.smilevideo.jp/smile?i=34172016.39165").Should().Be("https://tn.smilevideo.jp/smile?i=34172016.39165", "URL with dot was upgraded");
		}
	}
}
