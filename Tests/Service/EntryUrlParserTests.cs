#nullable disable

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain;
using VocaDb.Model.Service;
using VocaDb.Model.Utils;

namespace VocaDb.Tests.Service
{
	/// <summary>
	/// Unit tests for <see cref="EntryUrlParser"/>.
	/// </summary>
	[TestClass]
	public class EntryUrlParserTests
	{
		private const string BaseUrl = "https://test.vocadb.net";

		private string GetAbsoluteUrl(string relative)
		{
			return VocaUriBuilder.MergeUrls(BaseUrl, relative);
		}

		private void TestParseAbsolute(string url, int expectedId, EntryType expectedType)
		{
			var result = new EntryUrlParser(BaseUrl).Parse(GetAbsoluteUrl(url));

			result.Id.Should().Be(expectedId, "Id");
			result.EntryType.Should().Be(expectedType, "EntryType");
		}

		private void TestParseRelative(string url, int expectedId, EntryType expectedType)
		{
			var result = new EntryUrlParser(BaseUrl).Parse(url, true);

			result.Id.Should().Be(expectedId, "Id");
			result.EntryType.Should().Be(expectedType, "EntryType");
		}

		[TestMethod]
		public void HostAddressesAreSame()
		{
			var result = new EntryUrlParser(BaseUrl).Parse(GetAbsoluteUrl("/Artist/Details/39"));
			result.Id.Should().Be(39, "Id");
			result.EntryType.Should().Be(EntryType.Artist, "EntryType");
		}

		[TestMethod]
		public void NoMatch()
		{
			TestParseAbsolute("/Search", 0, EntryType.Undefined);
		}

		[TestMethod]
		public void IdIsNotInteger()
		{
			TestParseAbsolute("/Al/undefined", 0, EntryType.Undefined);
		}

		[TestMethod]
		public void Long()
		{
			TestParseAbsolute("/Artist/Details/39", 39, EntryType.Artist);
		}

		[TestMethod]
		public void Short()
		{
			TestParseAbsolute("/S/39", 39, EntryType.Song);
		}

		[TestMethod]
		public void Short_Lowercase()
		{
			TestParseAbsolute("/al/39", 39, EntryType.Album);
		}

		[TestMethod]
		public void Absolute_DifferentScheme()
		{
			var result = new EntryUrlParser(BaseUrl).Parse("http://test.vocadb.net/S/39");

			result.Id.Should().Be(39, "Id");
			result.EntryType.Should().Be(EntryType.Song, "EntryType");
		}

		[TestMethod]
		public void Relative()
		{
			TestParseRelative("/S/10", 10, EntryType.Song);
		}

		[TestMethod]
		public void QueryParameters()
		{
			TestParseAbsolute("/Al/1?pv=39", 1, EntryType.Album);
		}

		[TestMethod]
		public void FriendlyUrl()
		{
			TestParseAbsolute("/Ar/39/miku-hatsune", 39, EntryType.Artist);
		}
	}
}
