using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Helpers {

	[TestClass]
	public class StringHelperTests {

		private void TestRemoveControlChars(string expected, string input) {
			var actual = StringHelper.RemoveControlChars(input);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void RemoveControlCharacter() {
			TestRemoveControlChars("Invisible", "\x10Invisible");
		}

		[TestMethod]
		public void RemoveControlCharsKeepSpaces() {
			TestRemoveControlChars("Hatsune Miku", "Hatsune Miku");
		}

		[TestMethod]
		public void RemoveControlCharsKeepNewlines() {
			TestRemoveControlChars("Hatsune Miku\nVocaloid CV01", "Hatsune Miku\nVocaloid CV01");
		}

		[TestMethod]
		public void RemoveControlCharsKeepNonAscii() {
			TestRemoveControlChars("初音ミク", "初音ミク");
		}

	}

}
