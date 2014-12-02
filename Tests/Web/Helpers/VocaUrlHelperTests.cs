using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.Web.Helpers {

	[TestClass]
	public class VocaUrlHelperTests {

		private void CallGetUrlFriendlyName(string expected, string input) {

			var result = VocaUrlHelper.GetUrlFriendlyName(TranslatedString.Create(input));
			Assert.AreEqual(expected, result, input);

		}

		[TestMethod]
		public void GetUrlFriendlyName_English() {

			CallGetUrlFriendlyName("hatsune-miku", "Hatsune Miku");

		}

		[TestMethod]
		public void GetUrlFriendlyName_Japanese() {

			CallGetUrlFriendlyName(string.Empty, "初音ミク");

		}

		[TestMethod]
		public void GetUrlFriendlyName_Numbers() {

			CallGetUrlFriendlyName("apple41", "apple41");

		}

	}
}
