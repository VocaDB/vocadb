#nullable disable

using System.Linq;
using System.Web.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.AlbumSearch;
using VocaDb.Model.Service.Search.Artists;
using VocaDb.Model.Service.Search.Events;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Model.Service.Search.Tags;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Search;

namespace VocaDb.Web.Controllers
{
	public class SearchController : ControllerBase
	{
		private readonly AlbumService _albumService;
		private readonly ArtistService _artistService;
		private readonly EntryQueries _entryQueries;
		private readonly EventQueries _eventQueries;
		private readonly OtherService _services;
		private readonly SongService _songService;
		private readonly SongListQueries _songListQueries;
		private readonly TagQueries _tagQueries;
		private readonly IUserPermissionContext _permissionContext;

		private ActionResult RedirectToAlbum(int id)
		{
			return RedirectToAction("Details", "Album", new { id });
		}

		private ActionResult RedirectToArtist(int id)
		{
			return RedirectToAction("Details", "Artist", new { id });
		}

		private ActionResult RedirectToReleaseEvent(int id, string urlSlug)
		{
			return RedirectToAction("Details", "Event", new { id, urlSlug });
		}

		private ActionResult RedirectToSong(int id)
		{
			return RedirectToAction("Details", "Song", new { id });
		}

		private ActionResult RedirectToSongList(int id)
		{
			return RedirectToAction("Details", "SongList", new { id });
		}

		private ActionResult RedirectToTag(int id, string urlSlug)
		{
			return RedirectToAction("DetailsById", "Tag", new { id, urlSlug });
		}

		private ActionResult TryRedirect(string filter, EntryType searchType)
		{
			var textQuery = SearchTextQuery.Create(filter);
			var artistTextQuery = ArtistSearchTextQuery.Create(filter);

			switch (searchType)
			{
				case EntryType.Undefined:
					{
						var result = _entryQueries.GetList(filter, null, null, false, null, null, 0, 1, true, EntrySortRule.Name,
							NameMatchMode.Auto, Model.DataContracts.Api.EntryOptionalFields.None, Model.Domain.Globalization.ContentLanguagePreference.Default,
							searchTags: true, searchEvents: true);

						if (result.TotalCount == 1)
						{
							var item = result.Items.First();
							var entryId = item.Id;

							switch (item.EntryType)
							{
								case EntryType.Album:
									return RedirectToAlbum(entryId);
								case EntryType.Artist:
									return RedirectToArtist(entryId);
								case EntryType.ReleaseEvent:
									return RedirectToReleaseEvent(entryId, item.UrlSlug);
								case EntryType.Song:
									return RedirectToSong(entryId);
								case EntryType.Tag:
									return RedirectToTag(entryId, item.UrlSlug);
							}
						}
					}
					break;

				case EntryType.Artist:
					var artist = _artistService.FindArtists(new ArtistQueryParams(artistTextQuery, null, 0, 2, false, ArtistSortRule.None, false)
					{
						LanguagePreference = PermissionContext.LanguagePreference
					});
					if (artist.Items.Length == 1)
					{
						return RedirectToArtist(artist.Items[0].Id);
					}
					break;

				case EntryType.Album:
					var album = _albumService.Find(new AlbumQueryParams(textQuery, DiscType.Unknown, 0, 2, false, AlbumSortRule.None, false)
					{
						LanguagePreference = PermissionContext.LanguagePreference
					});
					if (album.Items.Length == 1)
					{
						return RedirectToAlbum(album.Items[0].Id);
					}
					break;

				case EntryType.ReleaseEvent:
					var queryParams = new EventQueryParams
					{
						TextQuery = textQuery,
						Paging = new PagingProperties(0, 2, false)
					};
					var ev = _eventQueries.Find(s => new { s.Id, s.UrlSlug }, queryParams);
					if (ev.Items.Length == 1)
					{
						return RedirectToReleaseEvent(ev.Items[0].Id, ev.Items[0].UrlSlug);
					}
					break;

				case EntryType.Song:
					var song = _songService.Find(new SongQueryParams(textQuery, null, 0, 2, false, SongSortRule.None, false, false, null)
					{
						LanguagePreference = PermissionContext.LanguagePreference
					});
					if (song.Items.Length == 1)
					{
						return RedirectToSong(song.Items[0].Id);
					}
					break;

				case EntryType.SongList:
					var list = _songListQueries.Find(s => s.Id, new SongListQueryParams { TextQuery = textQuery, Paging = new PagingProperties(0, 2, false), SortRule = SongListSortRule.Name });
					if (list.Items.Length == 1)
					{
						return RedirectToSongList(list.Items[0]);
					}
					return RedirectToAction("Featured", "SongList");

				case EntryType.Tag:
					var tags = _tagQueries.Find(new TagQueryParams(new CommonSearchParams(textQuery, true, true), PagingProperties.FirstPage(2))
					{
						AllowChildren = true,
						LanguagePreference = PermissionContext.LanguagePreference
					}, TagOptionalFields.None, _permissionContext.LanguagePreference);
					if (tags.Items.Length == 1)
					{
						return RedirectToTag(tags.Items.First().Id, tags.Items.First().Name);
					}
					break;

				default:
					{
						var action = "Index";
						var controller = searchType.ToString();
						return RedirectToAction(action, controller, new { filter });
					}
			}

			return null;
		}

		public SearchController(OtherService services, ArtistService artistService, AlbumService albumService, SongService songService, SongListQueries songListQueries,
			TagQueries tagQueries, EventQueries eventQueries, EntryQueries entryQueries, IUserPermissionContext permissionContext)
		{
			this._services = services;
			this._artistService = artistService;
			this._albumService = albumService;
			this._songService = songService;
			this._songListQueries = songListQueries;
			this._tagQueries = tagQueries;
			this._eventQueries = eventQueries;
			this._entryQueries = entryQueries;
			this._permissionContext = permissionContext;
		}

		public ActionResult Index(SearchIndexViewModel viewModel)
		{
			if (viewModel == null)
				viewModel = new SearchIndexViewModel();

			var filter = viewModel.Filter;
			filter = !string.IsNullOrEmpty(filter) ? filter.Trim() : string.Empty;

			if (viewModel.AllowRedirect && !string.IsNullOrEmpty(filter))
			{
				var redirectResult = TryRedirect(filter, viewModel.SearchType);

				if (redirectResult != null)
					return redirectResult;
			}

			if (!string.IsNullOrEmpty(viewModel.Tag))
			{
				viewModel.TagId = new[] { _tagQueries.GetTagIdByName(viewModel.Tag) };
			}

			viewModel.Filter = filter;

			SetSearchEntryType(viewModel.SearchType);

			return View("Index", viewModel);
		}

		public ActionResult Radio()
		{
			return Index(new SearchIndexViewModel(EntryType.Song) { MinScore = 1, Sort = "AdditionDate", ViewMode = "PlayList", Autoplay = true, Shuffle = true });
		}
	}
}
