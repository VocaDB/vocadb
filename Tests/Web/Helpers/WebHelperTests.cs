#nullable disable

using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.Web.Helpers
{
	[TestClass]
	public class WebHelperTests
	{
		private void TestGetInterfaceCultureName(string expected, params string[] input)
		{
			var requestMock = new Mock<HttpRequest>();
			requestMock.SetupGet(m => m.Headers).Returns(new HeaderDictionary
			{
				{ HeaderNames.AcceptLanguage, input }
			});

			var result = WebHelper.GetInterfaceCultureName(requestMock.Object);

			Assert.AreEqual(expected, result, "result");
		}

		[TestMethod]
		public void GetInterfaceCultureName_Valid()
		{
			TestGetInterfaceCultureName("en-US", "no-NO", "en-US");
		}

		[TestMethod]
		public void GetInterfaceCultureName_OnlyLanguage()
		{
			TestGetInterfaceCultureName("en", "no", "en");
		}

		[TestMethod]
		public void GetInterfaceCultureName_NoMatch()
		{
			TestGetInterfaceCultureName(string.Empty, "no-NO");
		}

		[TestMethod]
		public void GetInterfaceCultureName_Invalid()
		{
			TestGetInterfaceCultureName(string.Empty, "not-valid");
		}

		[TestMethod]
		public void GetInterfaceCultureName_Empty()
		{
			TestGetInterfaceCultureName(string.Empty);
		}
	}
}
