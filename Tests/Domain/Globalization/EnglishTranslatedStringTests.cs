using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.Globalization;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Tests.Domain.Globalization
{

	/// <summary>
	/// Tests for <see cref="EnglishTranslatedString"/>.
	/// </summary>
	[TestClass]
	public class EnglishTranslatedStringTests
	{

		[TestMethod]
		public void CopyFrom_Changed()
		{

			var source = new EnglishTranslatedString("ミク", "Miku");
			var target = new EnglishTranslatedStringContract { Original = "ミクさんマジ天使", English = "Hatsune Miku is truly my angel" };

			var changed = source.CopyFrom(target);

			Assert.IsTrue(changed, "changed");
			Assert.AreEqual("ミクさんマジ天使", source.Original, "Original");

		}

		[TestMethod]
		public void CopyFrom_Trim()
		{

			var source = new EnglishTranslatedString();
			var target = new EnglishTranslatedStringContract { Original = " " };

			var changed = source.CopyFrom(target);

			Assert.IsFalse(changed, "changed");
			Assert.AreEqual(string.Empty, source.Original, "Original");

		}

	}

}
