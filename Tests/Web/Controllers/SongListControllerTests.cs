#nullable disable

using Microsoft.AspNetCore.Mvc;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Controllers;

namespace VocaDb.Tests.Web.Controllers;

/// <summary>
/// Tests for <see cref="SongListController"/>
/// </summary>
[TestClass]
public class SongListControllerTests
{
	private SongListController _controller;

	[TestInitialize]
	public void SetUp()
	{
		_controller = new SongListController(queries: null, entryLinkFactory: new FakeEntryLinkFactory(), markdownParser: null);
	}

	[TestMethod]
	public void Details_NoId()
	{
		var result = _controller.Details();

		result.Should().BeOfType<NotFoundObjectResult>();
	}
}
