#nullable disable

using System;
using System.Linq;
using System.ServiceModel;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Artists;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Model.Service.Search.Tags;
using VocaDb.Model.Service.Search.User;

namespace VocaDb.Web.Services
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "QueryService" in code, svc and config file together.
	[ServiceContract(Namespace = Schemas.VocaDb)]
	public class QueryService
	{
		private readonly AlbumQueries _albumQueries;
		private readonly AlbumService _albumService;
		private readonly ArtistQueries _artistQueries;
		private readonly ArtistService _artistService;
		private readonly EntryQueries _entryQueries;
		private readonly TagQueries _tagQueries;
		private readonly IUserPermissionContext _userPermissionContext;
		private readonly SongQueries _songQueries;
		private readonly SongService _songService;
		private readonly SongListQueries _songListQueries;
		private readonly UserQueries _userQueries;
		private readonly UserService _userService;

		public QueryService(ArtistQueries artistQueries, TagQueries tagQueries, UserQueries userQueries,
			AlbumService albumService, AlbumQueries albumQueries, ArtistService artistService, SongQueries songQueries, SongService songService, SongListQueries songListQueries, UserService userService,
			EntryQueries entryQueries,
			IUserPermissionContext userPermissionContext)
		{
			this._artistQueries = artistQueries;
			this._tagQueries = tagQueries;
			this._userQueries = userQueries;
			this._albumService = albumService;
			this._albumQueries = albumQueries;
			this._artistService = artistService;
			this._songQueries = songQueries;
			this._entryQueries = entryQueries;
			this._songService = songService;
			this._songListQueries = songListQueries;
			this._userService = userService;
			this._userPermissionContext = userPermissionContext;
		}

		#region Common queries
		[OperationContract]
		public PartialFindResult<AlbumContract> FindAlbums(string term, int maxResults,
			NameMatchMode nameMatchMode = NameMatchMode.Auto, AlbumSortRule sort = AlbumSortRule.NameThenReleaseDate)
		{
			return _albumService.Find(SearchTextQuery.Create(term, nameMatchMode), DiscType.Unknown, 0, maxResults, true, moveExactToTop: true, sortRule: sort);
		}

		[OperationContract]
		public PartialFindResult<AlbumContract> FindAlbumsAdvanced(string term, int maxResults)
		{
			return _albumService.FindAdvanced(term, new PagingProperties(0, maxResults, true), AlbumSortRule.Name);
		}

		[OperationContract]
		public PartialFindResult<EntryForApiContract> FindAll(string term, int maxResults, ContentLanguagePreference languagePreference)
		{
			return _entryQueries.GetList(term, null, null, false, null, null, 0, maxResults, true, EntrySortRule.Name,
				NameMatchMode.Auto, EntryOptionalFields.AdditionalNames, languagePreference, false);
		}

		[OperationContract]
		public PartialFindResult<ArtistContract> FindArtists(string term, int maxResults, NameMatchMode nameMatchMode = NameMatchMode.Auto)
		{
			return _artistService.FindArtists(new ArtistQueryParams(ArtistSearchTextQuery.Create(term, nameMatchMode), new ArtistType[] { }, 0, maxResults, true, ArtistSortRule.Name, true));
		}

		[OperationContract]
		public NewSongCheckResultContract FindDuplicate(string[] names, string[] pvs, int[] artistIds, bool getPVInfo = false)
		{
			return _songQueries.FindDuplicates(names, pvs, artistIds, getPVInfo).Result;
		}

		[OperationContract]
		public PartialFindResult<SongWithAlbumAndPVsContract> FindSongs(string term, int maxResults, NameMatchMode nameMatchMode = NameMatchMode.Auto)
		{
			var sampleSize = Math.Min(maxResults * 2, 30);

			var results = _songService.FindWithAlbum(new SongQueryParams(
				SearchTextQuery.Create(term, nameMatchMode), new SongType[] { }, 0, sampleSize, true, SongSortRule.Name, false, true, null), false);

			return new PartialFindResult<SongWithAlbumAndPVsContract>(results.Items.Take(maxResults).ToArray(), results.TotalCount, results.Term);
		}

		[OperationContract]
		public string[] FindTags(string term, int maxResults)
		{
			return _tagQueries.FindNames(TagSearchTextQuery.Create(term), true, 10);
		}

		[OperationContract]
		public AlbumContract GetAlbumDetails(string term, AlbumSortRule sort = AlbumSortRule.NameThenReleaseDate)
		{
			var albums = _albumService.Find(SearchTextQuery.Create(term), DiscType.Unknown, 0, 10, false, moveExactToTop: true, sortRule: sort);
			return albums.Items.FirstOrDefault();
		}

		[OperationContract]
		public AlbumDetailsContract GetAlbumById(int id)
		{
			var album = _albumQueries.GetAlbumDetails(id, null);
			return album;
		}

		[OperationContract]
		public ArtistDetailsContract GetArtistDetails(string term)
		{
			var artists = _artistService.FindArtists(new ArtistQueryParams(ArtistSearchTextQuery.Create(term), new ArtistType[] { }, 0, 10,
				false, ArtistSortRule.Name, true));

			if (!artists.Items.Any())
				return null;

			return _artistQueries.GetDetails(artists.Items[0].Id, null);
		}

		[OperationContract]
		public ArtistDetailsContract GetArtistById(int id)
		{
			var artist = _artistQueries.GetDetails(id, null);
			return artist;
		}

		[OperationContract]
		public ArtistForApiContract[] GetArtistsWithYoutubeChannels(ContentLanguagePreference languagePreference = ContentLanguagePreference.Default)
		{
			return _artistService.GetArtistsWithYoutubeChannels(languagePreference);
		}

		[OperationContract]
		public SongDetailsContract GetSongById(int id, ContentLanguagePreference? language)
		{
			var song = _songQueries.GetSongDetails(id, 0, null, language, null);
			return song;
		}

		[OperationContract]
		public SongDetailsContract GetSongDetailsByNameArtistAndAlbum(string name, string artist, string album)
		{
			return _songService.XGetSongByNameArtistAndAlbum(name, artist, album);
		}

		[OperationContract]
		public SongDetailsContract GetSongDetails(string term, ContentLanguagePreference? language = null, NameMatchMode matchMode = NameMatchMode.Auto)
		{
			if (language.HasValue)
				_userPermissionContext.LanguagePreferenceSetting.OverrideRequestValue(language.Value);

			var song = _songService.FindFirstDetails(SearchTextQuery.Create(term, matchMode));
			return song;
		}

		[OperationContract]
		public SongListContract GetSongListById(int id)
		{
			var list = _songListQueries.GetSongList(id);
			return list;
		}

		[OperationContract]
		public TagContract GetTagById(int id, ContentLanguagePreference? language = null)
		{
			var tag = _tagQueries.GetTag(id, t => new TagContract(t, language ?? _userPermissionContext.LanguagePreference, true));
			return tag;
		}

		[OperationContract]
		public TagContract GetTagByName(string name)
		{
			var tag = _tagQueries.Find(t => new TagContract(t, ContentLanguagePreference.Default, true), new TagQueryParams(new CommonSearchParams(TagSearchTextQuery.Create(name), false, true),
				new PagingProperties(0, 1, false))).Items.FirstOrDefault();

			return tag;
		}

		[OperationContract]
		public UserContract GetUserInfo(string name)
		{
			var queryParams = new UserQueryParams
			{
				Common = new CommonSearchParams(SearchTextQuery.Create(name, NameMatchMode.Exact), false, false),
				Paging = new PagingProperties(0, 1, false)
			};

			var users = _userQueries.GetUsers(queryParams, u => new UserContract(u));
			return users.Items.FirstOrDefault();
		}

		#endregion

		#region MikuDB-specific queries (TODO: move elsewhere)
		[OperationContract]
		public AlbumContract GetAlbumByLinkUrl(string url)
		{
			return _albumService.GetAlbumByLink(url);
		}

		[OperationContract]
		public LyricsForSongContract GetRandomSongLyrics(string query)
		{
			if (string.IsNullOrEmpty(query))
				return _songService.GetRandomSongWithLyricsDetails();
			else
				return _songService.GetRandomLyricsForSong(query);
		}

		[OperationContract]
		public SongWithAlbumContract GetSongWithPV(PVService service, string pvId)
		{
			return _songService.GetSongWithPVAndAlbum(service, pvId);
		}

		[OperationContract]
		public UserContract GetUser(string name, string accessKey)
		{
			return _userService.CheckAccessWithKey(name, accessKey, "localhost", true);
		}

		#endregion
	}
}
