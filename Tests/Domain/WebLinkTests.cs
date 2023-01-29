#nullable disable

using Moq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Users;
using VocaDb.Tests.DatabaseTests;
using VocaDb.Tests.TestData;

namespace VocaDb.Tests.Domain;

/// <summary>
/// Tests for <see cref="WebLink"/>.
/// </summary>
[TestClass]
public class WebLinkTests
{
	private readonly WebLinkContract _webLinkContract = new(url: "http://www.test.com/", description: "test", category: WebLinkCategory.Commercial, disabled: false) { Id = 1 };
	private readonly WebLinkContract _webLinkContract2 = new(url: "http://www.test2.com/", description: "test2", category: WebLinkCategory.Official, disabled: false) { Id = 2 };
	private IWebLinkFactory<WebLink> _webLinkFactory;
	private User _actor;

	private readonly DatabaseTestContext<IDatabaseContext> _context = new();

	[TestInitialize]
	public void SetUp()
	{
		var webLinkFactoryMock = new Mock<IWebLinkFactory<WebLink>>();
		webLinkFactoryMock
			.Setup(m => m.CreateWebLink(It.IsAny<string>(), It.IsAny<WebAddress>(), It.IsAny<WebLinkCategory>(), It.IsAny<bool>()))
			.Returns<string, WebAddress, WebLinkCategory, bool>((description, address, category, disabled) => new WebLink(description, address, category, disabled));
		_webLinkFactory = webLinkFactoryMock.Object;
		_actor = CreateEntry.User(id: 1);
	}

	private WebLink CreateWebLink(WebLinkContract contract, int id = 0)
	{
		var uri = new Uri(contract.Url);
		return new WebLink(
			description: contract.Description,
			address: new WebAddress(uri, host: new WebAddressHost(hostname: uri.Host, _actor), _actor),
			category: contract.Category,
			disabled: contract.Disabled
		)
		{ Id = id };
	}

	[TestMethod]
	public void Ctor_Contract()
	{
		var result = CreateWebLink(_webLinkContract);

		result.ContentEquals(_webLinkContract).Should().BeTrue("constructed object is equal to contract");
	}

	[TestMethod]
	public void ContentEquals_AnotherLink_AreSame()
	{
		var copy = CreateWebLink(_webLinkContract);

		var result = copy.ContentEquals(_webLinkContract);

		result.Should().BeTrue("are equal");
	}

	[TestMethod]
	public void ContentEquals_AnotherLink_AreDifferent()
	{
		var copy = CreateWebLink(_webLinkContract);

		var result = copy.ContentEquals(_webLinkContract2);

		result.Should().BeFalse("are not equal");
	}

	[TestMethod]
	public void Sync_Contracts_NoExistingLinks()
	{
		_context.RunTest(ctx =>
		{
			var newLinks = new[] { _webLinkContract };

			var result = WebLink.Sync(ctx, Array.Empty<WebLink>(), newLinks, _webLinkFactory, _actor);

			result.Should().NotBeNull("result is not null");
			result.Changed.Should().BeTrue("is changed");
			result.Added.Length.Should().Be(1, "1 added");
			result.Edited.Length.Should().Be(0, "none edited");
			result.Removed.Length.Should().Be(0, "none removed");
			result.Unchanged.Length.Should().Be(0, "none unchanged");
			result.Added.First().ContentEquals(_webLinkContract).Should().BeTrue("added link matches contract");
		});
	}

	[TestMethod]
	public void Sync_Contracts_NotChanged()
	{
		_context.RunTest(ctx =>
		{
			var oldLinks = new[] { CreateWebLink(_webLinkContract, id: 1) };
			var newLinks = new[] { _webLinkContract };

			var result = WebLink.Sync(ctx, oldLinks, newLinks, _webLinkFactory, _actor);

			result.Should().NotBeNull("result is not null");
			result.Changed.Should().BeFalse("is not changed");
			result.Added.Length.Should().Be(0, "none added");
			result.Edited.Length.Should().Be(0, "none edited");
			result.Removed.Length.Should().Be(0, "none removed");
			result.Unchanged.Length.Should().Be(1, "1 unchanged");
			result.Unchanged.First().ContentEquals(_webLinkContract).Should().BeTrue("unchanged link matches contract");
		});
	}

	[TestMethod]
	public void Sync_Contracts_Edited()
	{
		_context.RunTest(ctx =>
		{
			var oldLinks = new[] { CreateWebLink(_webLinkContract, id: 2) };
			var newLinks = new[] { _webLinkContract2 };

			var result = WebLink.Sync(ctx, oldLinks, newLinks, _webLinkFactory, _actor);

			result.Should().NotBeNull("result is not null");
			result.Changed.Should().BeTrue("is changed");
			result.Added.Length.Should().Be(0, "none added");
			result.Edited.Length.Should().Be(1, "1 edited");
			result.Removed.Length.Should().Be(0, "none removed");
			result.Unchanged.Length.Should().Be(1, "1 unchanged");
			result.Edited.First().ContentEquals(_webLinkContract2).Should().BeTrue("edited link matches new contract");
		});
	}

	[TestMethod]
	public void Sync_Contracts_Removed()
	{
		_context.RunTest(ctx =>
		{
			var oldLinks = new List<WebLink> { CreateWebLink(_webLinkContract, id: 1) };
			var newLinks = Array.Empty<WebLinkContract>();

			var result = WebLink.Sync(ctx, oldLinks, newLinks, _webLinkFactory, _actor);

			result.Should().NotBeNull("result is not null");
			result.Changed.Should().BeTrue("is changed");
			result.Added.Length.Should().Be(0, "none added");
			result.Edited.Length.Should().Be(0, "none edited");
			result.Removed.Length.Should().Be(1, "1 removed");
			result.Unchanged.Length.Should().Be(0, "none unchanged");
			result.Removed.First().ContentEquals(_webLinkContract).Should().BeTrue("removed link matches contract");
		});
	}

	[TestMethod]
	public void Sync_Contracts_SkipWhitespace()
	{
		_context.RunTest(ctx =>
		{
			var newLinks = new[] { new WebLinkContract(" ", "VocaDB", WebLinkCategory.Reference, disabled: false) };

			var result = WebLink.Sync(ctx, Array.Empty<WebLink>(), newLinks, _webLinkFactory, _actor);

			result.Should().NotBeNull("result is not null");
			result.Changed.Should().BeFalse("is changed");
			result.Added.Length.Should().Be(0, "1 added");
			result.Edited.Length.Should().Be(0, "none edited");
			result.Removed.Length.Should().Be(0, "none removed");
			result.Unchanged.Length.Should().Be(0, "none unchanged");
		});
	}
}
