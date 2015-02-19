using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Web.Controllers;

namespace VocaDb.Tests.Web.Controllers {

	/// <summary>
	/// Tests for <see cref="SongController"/>.
	/// </summary>
	[TestClass]
	public class SongControllerTests {

		private SongController controller;

		[TestInitialize]
		public void SetUp() {
			controller = new SongController(null, null, null, null);
		}

		[TestMethod]
		public void PVEmbedNND_NoId() {

			var result = controller.PVEmbedNicoIFrame();

			Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));

		}

	}

}
