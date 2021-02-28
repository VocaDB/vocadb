#nullable disable

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Tests.Service.Helpers
{
	/// <summary>
	/// Tests for <see cref="FindHelpers"/>.
	/// </summary>
	[TestClass]
	public class FindHelpersTests
	{
		private void TestGetMatchModeAndQueryForSearch(string query, string expectedQuery, NameMatchMode? expectedMode = null, NameMatchMode originalMode = NameMatchMode.Auto)
		{
			var result = FindHelpers.GetMatchModeAndQueryForSearch(query, ref originalMode);

			result.Should().Be(expectedQuery, "query");
			if (expectedMode != null)
				originalMode.Should().Be(expectedMode, "matchMode");
		}

		[TestMethod]
		public void GetMatchModeAndQueryForSearch_Default()
		{
			TestGetMatchModeAndQueryForSearch("Hatsune Miku", "Hatsune Miku", NameMatchMode.Words);
		}

		[TestMethod]
		public void GetMatchModeAndQueryForSearch_EmptyQuery()
		{
			TestGetMatchModeAndQueryForSearch("", "");
		}

		[TestMethod]
		public void GetMatchModeAndQueryForSearch_Trim()
		{
			TestGetMatchModeAndQueryForSearch("Hatsune Miku         ", "Hatsune Miku", NameMatchMode.Words);
		}

		[TestMethod]
		public void GetMatchModeAndQueryForSearch_WildCard()
		{
			TestGetMatchModeAndQueryForSearch("Hatsune Miku*", "Hatsune Miku", NameMatchMode.StartsWith);
		}

		[TestMethod]
		public void GetMatchModeAndQueryForSearch_StartsWithWildCard()
		{
			TestGetMatchModeAndQueryForSearch("*Hatsune Miku", "Hatsune Miku", NameMatchMode.Words);
		}

		[TestMethod]
		public void GetMatchModeAndQueryForSearch_ShortQuery()
		{
			TestGetMatchModeAndQueryForSearch("H", "H", NameMatchMode.StartsWith);
		}

		[TestMethod]
		public void GetMatchModeAndQueryForSearch_ShortQueryWithWildcard()
		{
			TestGetMatchModeAndQueryForSearch("1*", "1", NameMatchMode.StartsWith);
		}

		[TestMethod]
		public void GetMatchModeAndQueryForSearch_ExactQuery()
		{
			TestGetMatchModeAndQueryForSearch("\"アキ\"", "アキ", NameMatchMode.Exact);
		}

		[TestMethod]
		public void GetMatchModeAndQueryForSearch_CleanTerm()
		{
			// For words search special SQL characters should be encoded.
			TestGetMatchModeAndQueryForSearch("alone [SNDI RMX]", "alone [[]SNDI RMX]");
		}

		[TestMethod]
		public void GetMatchModeAndQueryForSearch_QuotedQueryWithWildcards()
		{
			// Query is quoted so exact match is used. Special SQL characters are *not* encoded.
			TestGetMatchModeAndQueryForSearch("\"alone [SNDI RMX]\"", "alone [SNDI RMX]", NameMatchMode.Exact);
		}

		[TestMethod]
		public void GetMatchModeAndQueryForSearch_ExplicitExactQueryWithWildcards()
		{
			// Exact match mode is explicitly specified. Special SQL characters are *not* encoded.
			TestGetMatchModeAndQueryForSearch("alone [SNDI RMX]", "alone [SNDI RMX]", originalMode: NameMatchMode.Exact);
		}
	}
}
