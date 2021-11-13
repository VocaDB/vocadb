#nullable disable

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Controllers;

namespace VocaDb.Tests.Web.Controllers
{
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
			_controller = new SongListController(null, new FakeEntryLinkFactory());
		}

		[TestMethod]
		public void Details_NoId()
		{
			var result = _controller.Details();

			result.Should().BeOfType<NotFoundObjectResult>();
		}
	}
}
