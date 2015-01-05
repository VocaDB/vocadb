using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Web.Controllers;

namespace VocaDb.Tests.Web.Controllers {

	/// <summary>
	/// Tests for <see cref="TagController"/>.
	/// </summary>
	[TestClass]
	public class TagControllerTests {

		private TagController controller;

		[TestInitialize]
		public void SetUp() {

			controller = new TagController(null);

		}

		[TestMethod]
		public void Details_Null() {

			var result = controller.Details(null);

			Assert.IsTrue(result is HttpNotFoundResult, "result is HttpNotFoundResult");

		}

		[TestMethod]
		public void Versions_Null() {

			var result = controller.Versions(null);

			Assert.IsTrue(result is HttpNotFoundResult, "result is HttpNotFoundResult");

		}

		[TestMethod]
		public void Versions_Empty() {

			var result = controller.Versions(string.Empty);

			Assert.IsTrue(result is HttpNotFoundResult, "result is HttpNotFoundResult");

		}

	}

}
