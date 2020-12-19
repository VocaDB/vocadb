#nullable disable

using System.Web.Mvc;
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

			Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
		}
	}
}
