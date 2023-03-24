using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Venues;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.GitHubIssues.Issue1053;

[TestClass]
public class WebLinksTests
{
	private static readonly (string Description, string Url)[] s_webLinks = new[]
	{
		(Description: string.Empty, Url: "5"),
		(Description: "4", Url: "0"),
		(Description: string.Empty, Url: "3"),
		(Description: "2", Url: "0"),
		(Description: string.Empty, Url: "1"),
	};

	private static readonly (string Description, string Url)[] s_expected = new[]
	{
		(Description: string.Empty, Url: "1"),
		(Description: "2", Url: "0"),
		(Description: string.Empty, Url: "3"),
		(Description: "4", Url: "0"),
		(Description: string.Empty, Url: "5"),
	};

	private IUserPermissionContext _userContext = default!;
	private IAggregatedEntryImageUrlFactory _thumbPersister = default!;
	private IUserIconFactory _iconFactory = default!;

	[TestInitialize]
	public void SetUp()
	{
		_userContext = new FakePermissionContext();
		_thumbPersister = new InMemoryImagePersister();
		_iconFactory = new FakeUserIconFactory();
	}

	[TestMethod]
	public void AlbumDetailsForApiContract_WebLinks()
	{
		var album = CreateEntry.Album();

		foreach (var (description, url) in s_webLinks)
			album.CreateWebLink(description: description, url: url, category: WebLinkCategory.Official, disabled: false);

		var contract = new AlbumDetailsForApiContract(album, _userContext.LanguagePreference, _userContext, _thumbPersister);

		contract.WebLinks.Select(webLink => (webLink.Description, webLink.Url)).Should().Equal(s_expected);
	}

	[TestMethod]
	public void ArtistDetailsForApiContract_WebLinks()
	{
		var artist = CreateEntry.Artist(ArtistType.Vocaloid);

		foreach (var (description, url) in s_webLinks)
			artist.CreateWebLink(description: description, url: url, category: WebLinkCategory.Official, disabled: false);

		var contract = new ArtistDetailsForApiContract(artist, _userContext.LanguagePreference, _userContext, _thumbPersister, _iconFactory);

		contract.WebLinks.Select(webLink => (webLink.Description, webLink.Url)).Should().Equal(s_expected);
	}

	[TestMethod]
	public void ReleaseEventDetailsForApiContract_WebLinks()
	{
		var series = CreateEntry.EventSeries("Magical Mirai");

		foreach (var (description, url) in s_webLinks)
			series.CreateWebLink(description: description, url: url, category: WebLinkCategory.Official, disabled: false);

		var seriesContract = new ReleaseEventSeriesDetailsForApiContract(series, _userContext.LanguagePreference, _userContext, _thumbPersister);

		seriesContract.WebLinks.Select(webLink => (webLink.Description, webLink.Url)).Should().Equal(s_expected);

		var releaseEvent = CreateEntry.SeriesEvent(series, seriesNumber: 1);

		foreach (var (description, url) in s_webLinks)
			releaseEvent.CreateWebLink(description: description, url: url, category: WebLinkCategory.Official, disabled: false);

		var contract = new ReleaseEventDetailsForApiContract(
			releaseEvent,
			_userContext.LanguagePreference,
			_userContext, _iconFactory,
			entryTypeTags: null,
			eventAssociationType: null,
			latestComments: Array.Empty<CommentForApiContract>(),
			_thumbPersister
		);

		contract.WebLinks.Select(webLink => (webLink.Description, webLink.Url)).Should().Equal(s_expected);

		contract.Series.Should().NotBeNull();
		if (contract.Series is null) throw new Exception();

		contract.Series.WebLinks.Select(webLink => (webLink.Description, webLink.Url)).Should().Equal(s_expected);
	}

	[TestMethod]
	public void ReleaseEventSeriesDetailsForApiContract_WebLinks()
	{
		var series = CreateEntry.EventSeries("Magical Mirai");

		foreach (var (description, url) in s_webLinks)
			series.CreateWebLink(description: description, url: url, category: WebLinkCategory.Official, disabled: false);

		var contract = new ReleaseEventSeriesDetailsForApiContract(series, _userContext.LanguagePreference, _userContext, _thumbPersister);

		contract.WebLinks.Select(webLink => (webLink.Description, webLink.Url)).Should().Equal(s_expected);
	}

	[TestMethod]
	public void SongDetailsForApiContract_WebLinks()
	{
		var song = CreateEntry.Song();

		foreach (var (description, url) in s_webLinks)
			song.CreateWebLink(description: description, url: url, category: WebLinkCategory.Official, disabled: false);

		var contract = new SongDetailsForApiContract(
			song,
			_userContext.LanguagePreference,
			pools: Array.Empty<SongListBaseContract>(),
			specialTags: null,
			entryTypeTags: null,
			_userContext,
			_thumbPersister
		);

		contract.WebLinks.Select(webLink => (webLink.Description, webLink.Url)).Should().Equal(s_expected);
	}

	[TestMethod]
	public void TagDetailsForApiContract_WebLinks()
	{
		var tag = CreateEntry.Tag("rock");

		foreach (var (description, url) in s_webLinks)
			tag.CreateWebLink(description: description, url: url, category: WebLinkCategory.Official, disabled: false);

		var stats = new TagStatsForApiContract(
			_userContext.LanguagePreference,
			_userContext,
			_thumbPersister,
			artists: Array.Empty<Artist>(),
			artistCount: 0,
			albums: Array.Empty<Album>(),
			albumCount: 0,
			songLists: Array.Empty<SongList>(),
			songListCount: 0,
			songs: Array.Empty<Song>(),
			songCount: 0,
			eventSeries: Array.Empty<ReleaseEventSeries>(),
			eventSeriesCount: 0,
			events: Array.Empty<ReleaseEvent>(),
			eventCount: 0,
			followerCount: 0
		);

		var contract = new TagDetailsForApiContract(
			tag,
			stats,
			_userContext.LanguagePreference,
			commentCount: 0,
			latestComments: Array.Empty<CommentForApiContract>(),
			isFollowing: false,
			relatedEntryType: new EntryTypeAndSubType(),
			_thumbPersister
		);

		contract.WebLinks.Select(webLink => (webLink.Description, webLink.Url)).Should().Equal(s_expected);
	}

	[TestMethod]
	public void UserDetailsForApiContract_WebLinks()
	{
		var user = CreateEntry.User();

		foreach (var (description, url) in s_webLinks)
			user.CreateWebLink(description: description, url: url, category: WebLinkCategory.Official, disabled: false);

		var contract = new UserDetailsForApiContract(
			user,
			_iconFactory,
			_userContext.LanguagePreference,
			_thumbPersister,
			_userContext
		);

		contract.WebLinks.Select(webLink => (webLink.Description, webLink.Url)).Should().Equal(s_expected);
	}

	[TestMethod]
	public void VenueForApiContract_WebLinks()
	{
		var venue = CreateEntry.Venue();

		foreach (var (description, url) in s_webLinks)
			venue.CreateWebLink(description: description, url: url, category: WebLinkCategory.Official, disabled: false);

		var contract = new VenueForApiContract(venue, _userContext.LanguagePreference, fields: VenueOptionalFields.WebLinks);

		contract.WebLinks.Select(webLink => (webLink.Description, webLink.Url)).Should().Equal(s_expected);
	}
}
