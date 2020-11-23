using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Helpers
{
	/// <summary>
	/// Tests for <see cref="StringExtensions"/>.
	/// </summary>
	[TestClass]
	public class StringExtensionsTests
	{
		private void TestSummarize(string expected, string input, int minLength, int maxLength)
		{
			Assert.AreEqual(expected, input.Summarize(minLength, maxLength));
		}

		[TestMethod]
		public void Summarize_SingleLine_BetweenMinMax()
		{
			TestSummarize("Hatsune", "Hatsune", 5, 10);
		}

		[TestMethod]
		public void Summarize_SingleLine_BelowMinLength()
		{
			TestSummarize("Miku", "Miku", 5, 10);
		}

		[TestMethod]
		public void Summarize_SingleLine_AboveMaxLength()
		{
			TestSummarize("Hatsune Mi...", "Hatsune Miku", 5, 10);
		}

		[TestMethod]
		public void Summarize_MultipleLines_BelowMin()
		{
			TestSummarize("Miku\nLuka", "Miku\nLuka", 5, 10);
		}

		[TestMethod]
		public void Summarize_MultipleLines_FirstLineBetweenMinMax()
		{
			TestSummarize("Hatsune", "Hatsune\nMegurine", 5, 10);
		}

		[TestMethod]
		public void Summarize_MultipleLines_FirstLineAboveMax()
		{
			TestSummarize("Hatsune Mi...", "Hatsune Miku\nMegurine Luka", 5, 10);
		}
	}
}
