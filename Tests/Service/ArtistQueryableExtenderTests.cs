using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Search.Artists;

namespace VocaDb.Tests.Service
{

	/// <summary>
	/// Tests for <see cref="ArtistQueryableExtender"/>.
	/// </summary>
	[TestClass]
	public class ArtistQueryableExtenderTests
	{

		private List<ArtistName> CreateArtistNames(params string[] names)
		{
			return names.Select(name => new ArtistName { Value = name }).ToList();
		}

		private List<ArtistName> artists;

		private IQueryable<ArtistName> FilterByArtistName(string artistName)
		{
			return artists.AsQueryable().WhereArtistNameIs(ArtistSearchTextQuery.Create(artistName));
		}

		private void SequenceEqual(IEnumerable<ArtistName> actual, string message, params string[] expected)
		{
			Assert.IsTrue(actual.Select(n => n.Value).SequenceEqual(expected), message);
		}

		[TestInitialize]
		public void SetUp()
		{

			artists = CreateArtistNames("HSP", "Hiroyuki ODA", "8#Prince");

		}

		/// <summary>
		/// Search doesn't match.
		/// HSS -> nothing
		/// </summary>
		[TestMethod]
		public void FilterByArtistName_NotMatch_NotFound()
		{

			var result = FilterByArtistName("HSS");

			SequenceEqual(result, "result");

		}

		/// <summary>
		/// Query is a P name, just below the minimum length for a partial match.
		/// HSP -> HSP
		/// </summary>
		[TestMethod]
		public void FilterByArtistName_PName_QueryJustBelowMinLengthForContains_Found()
		{

			var result = FilterByArtistName("HSP");

			SequenceEqual(result, "result", "HSP");

		}

		/// <summary>
		/// Query is not a P name, just below the minimum length for a partial match.
		/// HS -> HSP
		/// </summary>
		[TestMethod]
		public void FilterByArtistName_NotPName_QueryJustBelowMinLengthForContains_Found()
		{

			var result = FilterByArtistName("HS");

			SequenceEqual(result, "result", "HSP");

		}

		/// <summary>
		/// Query is not a P name, just above the minimum length for a partial match.
		/// The query ends in P however.
		/// 8#P -> 8#Prince.
		/// </summary>
		[TestMethod]
		public void FilterByArtistName_NotPNameButEndsInP_QueryJustAboveMinLengthForContains_Found()
		{

			var result = FilterByArtistName("8#P");

			SequenceEqual(result, "result", "8#Prince");

		}

		/// <summary>
		/// Query is not a P name, just above the minimum length for a partial match.
		/// Hir -> Hiroyuki ODA
		/// </summary>
		[TestMethod]
		public void FilterByArtistName_NotPNameDoesNotEndInP_QueryJustAboveMinLengthForContains_Found()
		{

			var result = FilterByArtistName("Hir");

			SequenceEqual(result, "result", "Hiroyuki ODA");

		}

		/// <summary>
		/// Query is long enough for Partial/Words query.
		/// Hiroyuki -> Hiroyuki ODA
		/// </summary>
		[TestMethod]
		public void FilterByArtistName_NotPName_QueryLongEnoughForContains_Found()
		{

			var result = FilterByArtistName("Hiroyuki");

			SequenceEqual(result, "result", "Hiroyuki ODA");

		}

	}

}
