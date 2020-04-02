using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Helpers {

	[TestClass]
	public class BilibiliHelperTests {

		[TestMethod]
		public void Decode() {

			Assert.AreEqual(170001, BilibiliHelper.Decode("BV17x411w7KC"));
			Assert.AreEqual(455017605, BilibiliHelper.Decode("BV1Q541167Qg"));
			Assert.AreEqual(882584971, BilibiliHelper.Decode("BV1mK4y1C7Bz"));

		}

		[TestMethod]
		public void Encode() {

			Assert.AreEqual("BV17x411w7KC", BilibiliHelper.Encode(170001));
			Assert.AreEqual("BV1Q541167Qg", BilibiliHelper.Encode(455017605));
			Assert.AreEqual("BV1mK4y1C7Bz", BilibiliHelper.Encode(882584971));

		}

	}

}
