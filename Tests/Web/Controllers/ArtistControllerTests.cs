#nullable disable

using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
		private HttpResponse _response;

		[TestInitialize]
		public void SetUp()
		{
			_controller = new ArtistController(null, null, null);

			var responseMock = new Moq.Mock<HttpResponse>();
			responseMock.SetupProperty(m => m.StatusCode);
			_response = responseMock.Object;

			var context = new Moq.Mock<HttpContext>();
			context.SetupGet(m => m.Response).Returns(_response);
			var controllerContextMock = new Moq.Mock<ControllerContext>();
			var controllerContext = controllerContextMock.Object;
			controllerContext.HttpContext = context.Object;
			_controller.ControllerContext = controllerContext;
		}

		[TestMethod]
		public async Task Edit_ModelIsNull()
		{
			var result = await _controller.Edit(new ArtistEditViewModel()) as ContentResult;

			Assert.IsNotNull(result, "result");
			Assert.AreEqual((int)HttpStatusCode.BadRequest, _response.StatusCode, "Response status code");
		}

		[TestMethod]
		public void ViewVersion_NoId()
		{
			var result = _controller.ViewVersion();

			Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
		}
	}
}
