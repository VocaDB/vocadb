using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Web.Controllers;

namespace VocaDb.Tests.Web.Controllers {

	/// <summary>
	/// Tests for <see cref="ArtistController"/>.
	/// </summary>
	[TestClass]
	public class ArtistControllerTests {

		private ArtistController controller;

		[TestInitialize]
		public void SetUp() {
			controller = new ArtistController(null, null, null);
		}

		[TestMethod]
		public void ViewVersion_NoId() {

			var result = controller.ViewVersion();

			Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
			
		}

	}

}
