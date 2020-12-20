#nullable disable

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
		private ArtistController _controller;
		private HttpResponseBase _response;

		[TestInitialize]
		public void SetUp()
		{
			_controller = new ArtistController(null, null, null);

			var responseMock = new Moq.Mock<HttpResponseBase>();
			responseMock.SetupProperty(m => m.StatusCode);
			responseMock.SetupProperty(m => m.StatusDescription);
			_response = responseMock.Object;

			var context = new Moq.Mock<HttpContextBase>();
			context.SetupGet(m => m.Response).Returns(_response);
			var controllerContext = new Moq.Mock<ControllerContext>();
			controllerContext.SetupGet(m => m.HttpContext).Returns(context.Object);
			_controller.ControllerContext = controllerContext.Object;
		}

		[TestMethod]
		public void Edit_ModelIsNull()
		{
			var result = _controller.Edit(new ArtistEditViewModel());

			Assert.IsNotNull(result, "result");
			Assert.AreEqual((int)HttpStatusCode.BadRequest, _response.StatusCode, "Response status code");
		}

		[TestMethod]
		public void ViewVersion_NoId()
		{
			var result = _controller.ViewVersion();

			Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
		}
	}
}
