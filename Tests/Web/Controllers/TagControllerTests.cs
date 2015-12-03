using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Tests.TestSupport;
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

			controller = new TagController(null, new FakeEntryLinkFactory());

		}

	}

}
