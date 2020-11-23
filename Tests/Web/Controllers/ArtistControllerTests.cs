using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Web.Controllers;
using VocaDb.Web.Models.Artist;

namespace VocaDb.Tests.Web.Controllers
{
	/// <summary>
	/// Tests for <see cref="ArtistController"/>.
	/// </summary>
	[TestClass]
	public class ArtistControllerTests
	{
		private ArtistController controller;
		private HttpResponseBase response;

		[TestInitialize]
		public void SetUp()
		{
			controller = new ArtistController(null, null, null);

			var responseMock = new Moq.Mock<HttpResponseBase>();
			responseMock.SetupProperty(m => m.StatusCode);
			responseMock.SetupProperty(m => m.StatusDescription);
			response = responseMock.Object;

			var context = new Moq.Mock<HttpContextBase>();
			context.SetupGet(m => m.Response).Returns(response);
			var controllerContext = new Moq.Mock<ControllerContext>();
			controllerContext.SetupGet(m => m.HttpContext).Returns(context.Object);
			controller.ControllerContext = controllerContext.Object;
		}

		[TestMethod]
		public void Edit_ModelIsNull()
		{
			var result = controller.Edit(new ArtistEditViewModel());

			Assert.IsNotNull(result, "result");
			Assert.AreEqual((int)HttpStatusCode.BadRequest, response.StatusCode, "Response status code");
		}

		[TestMethod]
		public void ViewVersion_NoId()
		{
			var result = controller.ViewVersion();

			Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
		}
	}
}
