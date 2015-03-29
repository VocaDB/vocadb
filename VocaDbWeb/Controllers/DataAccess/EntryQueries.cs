using System.Linq;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Repositories;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Artists;

namespace VocaDb.Web.Controllers.DataAccess {

	public class EntryQueries : QueriesBase<IAlbumRepository, Album> {

		private readonly IEntryThumbPersister entryThumbPersister;

		public EntryQueries(IAlbumRepository repository, IUserPermissionContext permissionContext, IEntryThumbPersister entryThumbPersister) 
			: base(repository, permissionContext) {
			this.entryThumbPersister = entryThumbPersister;
		}

		public PartialFindResult<EntryForApiContract> GetList(
			string query, 
			string tag,
			EntryStatus? status,
			int start, int maxResults, bool getTotalCount,
			EntrySortRule sort,
			NameMatchMode nameMatchMode,
			EntryOptionalFields fields,
			ContentLanguagePreference lang,
			bool ssl
			) {

			var textQuery = SearchTextQuery.Create(query, nameMatchMode);
			var artistTextQuery = ArtistSearchTextQuery.Create(query, nameMatchMode); // Can't use the existing words collection here as they are noncanonized

			return repository.HandleQuery(ctx => {

				// Get all applicable names per entry type
				var artistNames = ctx.OfType<Artist>().Query()
					.Where(a => !a.Deleted)
					.WhereHasName_Canonized(artistTextQuery)
					.WhereHasTag(tag)
					.WhereStatusIs(status)
					.OrderBy(sort, lang)
					.Take(start + maxResults)
					.SelectEntryBase(lang, EntryType.Artist)
					.ToArray();

				var albumNames = ctx.OfType<Album>().Query()
					.Where(a => !a.Deleted)
					.WhereHasName(textQuery)
					.WhereHasTag(tag)
					.WhereStatusIs(status)
					.OrderBy(sort, lang)
					.Take(start + maxResults)
					.SelectEntryBase(lang, EntryType.Album)
					.ToArray();

				var songNames = ctx.OfType<Song>().Query()
					.Where(a => !a.Deleted)
					.WhereHasName(textQuery)
					.WhereHasTag(tag)
					.WhereStatusIs(status)
					.OrderBy(sort, lang)
					.Take(start + maxResults)
					.SelectEntryBase(lang, EntryType.Song)
					.ToArray();

				// Get page of combined names
				var entryNames = artistNames.Concat(albumNames).Concat(songNames)
					.OrderBy(e => e.DefaultName)
					.Skip(start)
					.Take(maxResults)
					.ToArray();
				
				var artistIds = entryNames.Where(e => e.EntryType == EntryType.Artist).Select(a => a.Id).ToArray();
				var albumIds = entryNames.Where(e => e.EntryType == EntryType.Album).Select(a => a.Id).ToArray();
				var songIds = entryNames.Where(e => e.EntryType == EntryType.Song).Select(a => a.Id).ToArray();

				// Get the actual entries in the page
				var artists = artistIds.Any() ? ctx.OfType<Artist>().Query()
					.Where(a => artistIds.Contains(a.Id))
					.ToArray()
					.Select(a => new EntryForApiContract(a, lang, entryThumbPersister, ssl, fields)) : new EntryForApiContract[0];

				var albums = albumIds.Any() ? ctx.OfType<Album>().Query()
					.Where(a => albumIds.Contains(a.Id))
					.ToArray()
					.Select(a => new EntryForApiContract(a, lang, entryThumbPersister, ssl, fields)) : new EntryForApiContract[0];

				var songs = songIds.Any() ? ctx.OfType<Song>().Query()
					.Where(a => songIds.Contains(a.Id))
					.ToArray()
					.Select(a => new EntryForApiContract(a, lang, fields)) : new EntryForApiContract[0];

				// Merge and sort the final list
				var entries = artists.Concat(albums).Concat(songs);

				if (sort == EntrySortRule.Name)
					entries = entries.OrderBy(a => a.Name);

				if (sort == EntrySortRule.AdditionDate)
					entries = entries.OrderByDescending(a => a.CreateDate);

				var count = 0;

				if (getTotalCount) {
					
					count = 
						ctx.OfType<Artist>().Query()
							.Where(a => !a.Deleted)
							.WhereHasName_Canonized(artistTextQuery)
							.WhereHasTag(tag)
							.WhereStatusIs(status)
							.Count() + 
						ctx.OfType<Album>().Query()
							.Where(a => !a.Deleted)
							.WhereHasName(textQuery)
							.WhereHasTag(tag)
							.WhereStatusIs(status)
							.Count() +
						ctx.OfType<Song>().Query()
							.Where(a => !a.Deleted)
							.WhereHasName(textQuery)
							.WhereHasTag(tag)
							.WhereStatusIs(status)
							.Count();


				}

				return new PartialFindResult<EntryForApiContract>(entries.ToArray(), count);

			});

		}

	}

}