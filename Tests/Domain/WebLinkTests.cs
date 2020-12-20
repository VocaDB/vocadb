#nullable disable

using System.Collections.Generic;
using System.Linq;
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

			Assert.IsTrue(result.ContentEquals(_webLinkContract), "constructed object is equal to contract");
		}

		[TestMethod]
		public void ContentEquals_AnotherLink_AreSame()
		{
			var copy = new WebLink(_webLinkContract);

			var result = copy.ContentEquals(_webLinkContract);

			Assert.IsTrue(result, "are equal");
		}

		[TestMethod]
		public void ContentEquals_AnotherLink_AreDifferent()
		{
			var copy = new WebLink(_webLinkContract);

			var result = copy.ContentEquals(_webLinkContract2);

			Assert.IsFalse(result, "are not equal");
		}

		[TestMethod]
		public void Sync_Contracts_NoExistingLinks()
		{
			var newLinks = new[] { _webLinkContract };

			var result = WebLink.Sync(new WebLink[] { }, newLinks, _webLinkFactory);

			Assert.IsNotNull(result, "result is not null");
			Assert.IsTrue(result.Changed, "is changed");
			Assert.AreEqual(1, result.Added.Length, "1 added");
			Assert.AreEqual(0, result.Edited.Length, "none edited");
			Assert.AreEqual(0, result.Removed.Length, "none removed");
			Assert.AreEqual(0, result.Unchanged.Length, "none unchanged");
			Assert.IsTrue(result.Added.First().ContentEquals(_webLinkContract), "added link matches contract");
		}

		[TestMethod]
		public void Sync_Contracts_NotChanged()
		{
			var oldLinks = new[] { new WebLink(_webLinkContract) { Id = 1 } };
			var newLinks = new[] { _webLinkContract };

			var result = WebLink.Sync(oldLinks, newLinks, _webLinkFactory);

			Assert.IsNotNull(result, "result is not null");
			Assert.IsFalse(result.Changed, "is not changed");
			Assert.AreEqual(0, result.Added.Length, "none added");
			Assert.AreEqual(0, result.Edited.Length, "none edited");
			Assert.AreEqual(0, result.Removed.Length, "none removed");
			Assert.AreEqual(1, result.Unchanged.Length, "1 unchanged");
			Assert.IsTrue(result.Unchanged.First().ContentEquals(_webLinkContract), "unchanged link matches contract");
		}

		[TestMethod]
		public void Sync_Contracts_Edited()
		{
			var oldLinks = new[] { new WebLink(_webLinkContract) { Id = 2 } };
			var newLinks = new[] { _webLinkContract2 };

			var result = WebLink.Sync(oldLinks, newLinks, _webLinkFactory);

			Assert.IsNotNull(result, "result is not null");
			Assert.IsTrue(result.Changed, "is changed");
			Assert.AreEqual(0, result.Added.Length, "none added");
			Assert.AreEqual(1, result.Edited.Length, "1 edited");
			Assert.AreEqual(0, result.Removed.Length, "none removed");
			Assert.AreEqual(1, result.Unchanged.Length, "1 unchanged");
			Assert.IsTrue(result.Edited.First().ContentEquals(_webLinkContract2), "edited link matches new contract");
		}

		[TestMethod]
		public void Sync_Contracts_Removed()
		{
			var oldLinks = new List<WebLink> { new WebLink(_webLinkContract) { Id = 1 } };
			var newLinks = new WebLinkContract[] { };

			var result = WebLink.Sync(oldLinks, newLinks, _webLinkFactory);

			Assert.IsNotNull(result, "result is not null");
			Assert.IsTrue(result.Changed, "is changed");
			Assert.AreEqual(0, result.Added.Length, "none added");
			Assert.AreEqual(0, result.Edited.Length, "none edited");
			Assert.AreEqual(1, result.Removed.Length, "1 removed");
			Assert.AreEqual(0, result.Unchanged.Length, "none unchanged");
			Assert.IsTrue(result.Removed.First().ContentEquals(_webLinkContract), "removed link matches contract");
		}

		[TestMethod]
		public void Sync_Contracts_SkipWhitespace()
		{
			var newLinks = new[] { new WebLinkContract(" ", "VocaDB", WebLinkCategory.Reference, disabled: false) };

			var result = WebLink.Sync(new WebLink[] { }, newLinks, _webLinkFactory);

			Assert.IsNotNull(result, "result is not null");
			Assert.IsFalse(result.Changed, "is changed");
			Assert.AreEqual(0, result.Added.Length, "1 added");
			Assert.AreEqual(0, result.Edited.Length, "none edited");
			Assert.AreEqual(0, result.Removed.Length, "none removed");
			Assert.AreEqual(0, result.Unchanged.Length, "none unchanged");
		}
	}
}
