#nullable disable

using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Web.Controllers.DataAccess;

/// <summary>
/// Unit tests for <see cref="EntryQueries"/>.
/// </summary>
[TestClass]
public class EntryQueriesTests
{
	private FakeAlbumRepository _repository;
	private EntryQueries _queries;
	private Tag _tag;

	private EntryForApiContract AssertHasEntry(PartialFindResult<EntryForApiContract> result, string name, EntryType entryType)
	{
		var entry = result.Items.FirstOrDefault(a => string.Equals(a.DefaultName, name, StringComparison.InvariantCultureIgnoreCase)
			&& a.EntryType == entryType);

		entry.Should().NotBeNull("Entry found");
		return entry;
	}

	private PartialFindResult<EntryForApiContract> CallGetList(
		string query = null,
		int[] tag = null,
		EntryStatus? status = null,
		int start = 0, int maxResults = 10, bool getTotalCount = true,
		NameMatchMode nameMatchMode = NameMatchMode.Words,
		EntryOptionalFields fields = EntryOptionalFields.None,
		ContentLanguagePreference lang = ContentLanguagePreference.Default)
	{
		return _queries.GetList(query, tag, null, false, status, null, start, maxResults, getTotalCount, EntrySortRule.Name, nameMatchMode, fields, lang);
	}

	[TestInitialize]
	public void SetUp()
	{
		_repository = new FakeAlbumRepository();
		var permissionContext = new FakePermissionContext();
		var thumbPersister = new InMemoryImagePersister();

		_queries = new EntryQueries(_repository, permissionContext, thumbPersister);

		var group = CreateEntry.Artist(ArtistType.OtherGroup, name: "1640mP");
		var artist = CreateEntry.Producer(name: "40mP");
		_tag = new Tag("pop_rock");
		artist.Tags.Usages.Add(new ArtistTagUsage(artist, _tag));
		var artist2 = CreateEntry.Producer(name: "Tripshots");
		var album = CreateEntry.Album(name: "40mP Piano Arrange Album");
		var song = CreateEntry.Song(name: "Mosaik Role [40mP ver.]");

		_repository.Save(group, artist, artist2);
		_repository.Save(album);
		_repository.Save(song);
		_repository.Save(_tag);
	}

	/// <summary>
	/// List while filtering by title (words).
	/// </summary>
	[TestMethod]
	public void List_FilterByTitle()
	{
		var result = CallGetList(query: "40mP");

		result.TotalCount.Should().Be(4, "TotalCount");
		result.Items.Length.Should().Be(4, "Items.Length");

		AssertHasEntry(result, "40mP", EntryType.Artist);
		AssertHasEntry(result, "1640mP", EntryType.Artist);
		AssertHasEntry(result, "40mP Piano Arrange Album", EntryType.Album);
		AssertHasEntry(result, "Mosaik Role [40mP ver.]", EntryType.Song);
	}

	/// <summary>
	/// List while filtering by canonized artist name.
	/// </summary>
	[TestMethod]
	public void List_FilterByCanonizedArtistName()
	{
		var artist = CreateEntry.Producer(name: "nightmare-P");
		_repository.Save(artist);

		var resultExact = CallGetList(query: "nightmare-P");
		var resultVariant = CallGetList(query: "nightmareP");
		var resultPartial = CallGetList(query: "nightmare");

		AssertHasEntry(resultExact, "nightmare-P", EntryType.Artist);
		AssertHasEntry(resultVariant, "nightmare-P", EntryType.Artist);
		AssertHasEntry(resultPartial, "nightmare-P", EntryType.Artist);
	}

	/// <summary>
	/// List while filtering by tag.
	/// </summary>
	[TestMethod]
	public void List_FilterByTag()
	{
		var result = CallGetList(tag: new[] { _tag.Id });

		result.TotalCount.Should().Be(1, "TotalCount");
		AssertHasEntry(result, "40mP", EntryType.Artist);
	}

	/// <summary>
	/// List, test paging.
	/// </summary>
	[TestMethod]
	public void List_Paging()
	{
		var result = CallGetList("40mP", start: 1, maxResults: 1);

		result.Items.Length.Should().Be(1, "Items.Length");
		result.TotalCount.Should().Be(4, "TotalCount");
		AssertHasEntry(result, "40mP", EntryType.Artist);
	}
}
