using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Tests.DatabaseTests;

namespace VocaDb.Tests.Service
{
	[TestClass]
	public class AlbumQueryableExtensionsTests
	{
		private readonly DatabaseTestContext<IDatabaseContext> _context = new();

		[TestMethod]
		public void WhereHasReleaseYear()
		{
			var albums = _context.RunTest(ctx =>
			{
				return ctx.Query<Album>().WhereHasReleaseYear().ToArray();
			});

			albums.Length.Should().Be(2, "Number of albums returned");
			albums.Any(a => a.DefaultName == "Re:package" && a.OriginalReleaseDate.Year == 2008).Should().BeTrue("Found 'Re:package' album");
			albums.Any(a => a.DefaultName == "Re:MIKUS" && a.OriginalReleaseDate.Year == 2009).Should().BeTrue("Found 'Re:MIKUS' album");
		}
	}
}
