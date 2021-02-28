#nullable disable

using FluentAssertions;
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

			changed.Should().BeTrue("changed");
			source.Original.Should().Be("ミクさんマジ天使", "Original");
		}

		[TestMethod]
		public void CopyFrom_Trim()
		{
			var source = new EnglishTranslatedString();
			var target = new EnglishTranslatedStringContract { Original = " " };

			var changed = source.CopyFrom(target);

			changed.Should().BeFalse("changed");
			source.Original.Should().Be(string.Empty, "Original");
		}
	}
}
