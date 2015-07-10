using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Web.Controllers;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Tests.Web.Controllers {

	/// <summary>
	/// Tests for <see cref="SongListController"/>
	/// </summary>
	[TestClass]
	public class SongListControllerTests {

		private SongListController controller;

		[TestInitialize]
		public void SetUp() {
			controller = new SongListController(null, null);
		}

		[TestMethod]
		public void Details_NoId() {

			var result = controller.Details();

			Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));

		}

	}
}
