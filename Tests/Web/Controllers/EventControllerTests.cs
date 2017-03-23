using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Controllers;

namespace VocaDb.Tests.Web.Controllers {

	/// <summary>
	/// Tests for <see cref="EventController"/>.
	/// </summary>
	[TestClass]
    public class EventControllerTests {

		private EventController controller;

		[TestInitialize]
		public void SetUp() {
			controller = new EventController(null, null, null, new FakeEntryLinkFactory(), null);
		}

		[TestMethod]
		public void Details_NoId() {

			var result = controller.Details();

			Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));

		}

    }
}
