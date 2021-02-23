#nullable disable

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.ExtLinks;

namespace VocaDb.Tests.Domain
{
	/// <summary>
	/// Tests for <see cref="WebLink"/>.
	/// </summary>
	[TestClass]
	public class WebLinkTests
	{
		private readonly WebLinkContract _webLinkContract = new("test", "http://www.test.com", WebLinkCategory.Commercial, disabled: false) { Id = 1 };
		private readonly WebLinkContract _webLinkContract2 = new("test2", "http://www.test2.com", WebLinkCategory.Official, disabled: false) { Id = 2 };
		private IWebLinkFactory<WebLink> _webLinkFactory;

		[TestInitialize]
		public void SetUp()
		{
			var webLinkFactoryMock = new Mock<IWebLinkFactory<WebLink>>();
			webLinkFactoryMock
				.Setup(m => m.CreateWebLink(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<WebLinkCategory>(), It.IsAny<bool>()))
				.Returns<string, string, WebLinkCategory, bool>((description, url, category, disabled) => new WebLink(description, url, category, disabled));
			_webLinkFactory = webLinkFactoryMock.Object;
		}

		[TestMethod]
		public void Ctor_Contract()
		{
			var result = new WebLink(_webLinkContract);

			result.ContentEquals(_webLinkContract).Should().BeTrue("constructed object is equal to contract");
		}

		[TestMethod]
		public void ContentEquals_AnotherLink_AreSame()
		{
			var copy = new WebLink(_webLinkContract);

			var result = copy.ContentEquals(_webLinkContract);

			result.Should().BeTrue("are equal");
		}

		[TestMethod]
		public void ContentEquals_AnotherLink_AreDifferent()
		{
			var copy = new WebLink(_webLinkContract);

			var result = copy.ContentEquals(_webLinkContract2);

			result.Should().BeFalse("are not equal");
		}

		[TestMethod]
		public void Sync_Contracts_NoExistingLinks()
		{
			var newLinks = new[] { _webLinkContract };

			var result = WebLink.Sync(new WebLink[] { }, newLinks, _webLinkFactory);

			result.Should().NotBeNull("result is not null");
			result.Changed.Should().BeTrue("is changed");
			result.Added.Length.Should().Be(1, "1 added");
			result.Edited.Length.Should().Be(0, "none edited");
			result.Removed.Length.Should().Be(0, "none removed");
			result.Unchanged.Length.Should().Be(0, "none unchanged");
			result.Added.First().ContentEquals(_webLinkContract).Should().BeTrue("added link matches contract");
		}

		[TestMethod]
		public void Sync_Contracts_NotChanged()
		{
			var oldLinks = new[] { new WebLink(_webLinkContract) { Id = 1 } };
			var newLinks = new[] { _webLinkContract };

			var result = WebLink.Sync(oldLinks, newLinks, _webLinkFactory);

			result.Should().NotBeNull("result is not null");
			result.Changed.Should().BeFalse("is not changed");
			result.Added.Length.Should().Be(0, "none added");
			result.Edited.Length.Should().Be(0, "none edited");
			result.Removed.Length.Should().Be(0, "none removed");
			result.Unchanged.Length.Should().Be(1, "1 unchanged");
			result.Unchanged.First().ContentEquals(_webLinkContract).Should().BeTrue("unchanged link matches contract");
		}

		[TestMethod]
		public void Sync_Contracts_Edited()
		{
			var oldLinks = new[] { new WebLink(_webLinkContract) { Id = 2 } };
			var newLinks = new[] { _webLinkContract2 };

			var result = WebLink.Sync(oldLinks, newLinks, _webLinkFactory);

			result.Should().NotBeNull("result is not null");
			result.Changed.Should().BeTrue("is changed");
			result.Added.Length.Should().Be(0, "none added");
			result.Edited.Length.Should().Be(1, "1 edited");
			result.Removed.Length.Should().Be(0, "none removed");
			result.Unchanged.Length.Should().Be(1, "1 unchanged");
			result.Edited.First().ContentEquals(_webLinkContract2).Should().BeTrue("edited link matches new contract");
		}

		[TestMethod]
		public void Sync_Contracts_Removed()
		{
			var oldLinks = new List<WebLink> { new WebLink(_webLinkContract) { Id = 1 } };
			var newLinks = new WebLinkContract[] { };

			var result = WebLink.Sync(oldLinks, newLinks, _webLinkFactory);

			result.Should().NotBeNull("result is not null");
			result.Changed.Should().BeTrue("is changed");
			result.Added.Length.Should().Be(0, "none added");
			result.Edited.Length.Should().Be(0, "none edited");
			result.Removed.Length.Should().Be(1, "1 removed");
			result.Unchanged.Length.Should().Be(0, "none unchanged");
			result.Removed.First().ContentEquals(_webLinkContract).Should().BeTrue("removed link matches contract");
		}

		[TestMethod]
		public void Sync_Contracts_SkipWhitespace()
		{
			var newLinks = new[] { new WebLinkContract(" ", "VocaDB", WebLinkCategory.Reference, disabled: false) };

			var result = WebLink.Sync(new WebLink[] { }, newLinks, _webLinkFactory);

			result.Should().NotBeNull("result is not null");
			result.Changed.Should().BeFalse("is changed");
			result.Added.Length.Should().Be(0, "1 added");
			result.Edited.Length.Should().Be(0, "none edited");
			result.Removed.Length.Should().Be(0, "none removed");
			result.Unchanged.Length.Should().Be(0, "none unchanged");
		}
	}
}
