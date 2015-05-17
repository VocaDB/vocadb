using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Versioning;
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
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Shared;

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
				var artistQuery = ctx.OfType<Artist>().Query()
					.Where(a => !a.Deleted)
					.WhereHasName_Canonized(artistTextQuery)
					.WhereHasTag(tag)
					.WhereStatusIs(status);

				var artistNames = artistQuery
					.OrderBy(sort, lang)
					.Take(start + maxResults)
					.SelectEntryBase(lang, EntryType.Artist)
					.ToArray();

				var albumQuery = ctx.OfType<Album>().Query()
					.Where(a => !a.Deleted)
					.WhereHasName(textQuery)
					.WhereHasTag(tag)
					.WhereStatusIs(status);

				var albumNames = albumQuery
					.OrderBy(sort, lang)
					.Take(start + maxResults)
					.SelectEntryBase(lang, EntryType.Album)
					.ToArray();

				var songQuery = ctx.OfType<Song>().Query()
					.Where(a => !a.Deleted)
					.WhereHasName(textQuery)
					.WhereHasTag(tag)
					.WhereStatusIs(status);

				var songNames = songQuery
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
					
					var artistCount = 
						(artistNames.Length >= maxResults ? artistQuery.Count() : artistNames.Length);
 
					var albumCount =
						(albumNames.Length >= maxResults ? albumQuery.Count() : albumNames.Length);

					var songCount =
						(songNames.Length >= maxResults ? songQuery.Count() : songNames.Length);

					count = artistCount + albumCount + songCount;

				}

				return new PartialFindResult<EntryForApiContract>(entries.ToArray(), count);

			});

		}

		// TODO: refactor this
		private ArchivedVersionForReviewContract CreateViewModel(Model.Domain.Versioning.ArchivedObjectVersion archivedObjectVersion) {

			string changeMessage = string.Empty, entryTypeName = string.Empty;

			if (archivedObjectVersion is ArchivedAlbumVersion) {
				var entry = (ArchivedAlbumVersion)archivedObjectVersion;
				changeMessage = Models.Album.Versions.GetChangeString(entry.Diff.ChangedFields);
				entryTypeName = Translate.DiscTypeName(entry.Album.DiscType);
			}

			if (archivedObjectVersion is ArchivedArtistVersion) {
				var entry = (ArchivedArtistVersion)archivedObjectVersion;
				changeMessage = Models.Artist.Versions.GetChangeString(entry.Diff.ChangedFields);
				entryTypeName = Translate.ArtistTypeName(entry.Artist.ArtistType);
			}

			if (archivedObjectVersion is ArchivedSongVersion) {
				var entry = (ArchivedSongVersion)archivedObjectVersion;
				changeMessage = Models.Song.Versions.GetChangeString(entry.Diff.ChangedFields);
				entryTypeName = Translate.SongTypeNames[entry.Song.SongType];
			}

			return new ArchivedVersionForReviewContract(archivedObjectVersion, changeMessage, entryTypeName, LanguagePreference);

		}

		private ArchivedObjectForSorting[] GetArchivedObjectsForSort<T>(IRepositoryContext<Album> ctx, EntryType entryType, int maxResults, DateTime? before)
			where T : Model.Domain.Versioning.ArchivedObjectVersion {
			
			var query = ctx.OfType<T>().Query();

			if (before.HasValue) {
				query = query.Where(a => a.Created < before);
			}

			var results = query.OrderByDescending(e => e.Created).Take(maxResults).Select(a => new ArchivedObjectForSorting {
				Created = a.Created,
				Id = a.Id,
				EntryType = entryType
			}).ToArray();

			return results;

		}

		private Model.Domain.Versioning.ArchivedObjectVersion[] GetArchivedObjects<T>(IRepositoryContext<Album> ctx, int[] ids) where T : Model.Domain.Versioning.ArchivedObjectVersion {
			return ctx.OfType<T>().Query().Where(v => ids.Contains(v.Id)).ToArray();
		}

		public ArchivedVersionForReviewContract[] GetRecentVersions(int maxResults, DateTime? before) {
			
			return repository.HandleQuery(ctx => {

				var albums = GetArchivedObjectsForSort<ArchivedAlbumVersion>(ctx, EntryType.Album, maxResults, before);
				var artists = GetArchivedObjectsForSort<ArchivedArtistVersion>(ctx, EntryType.Artist, maxResults, before);
				var songs = GetArchivedObjectsForSort<ArchivedSongVersion>(ctx, EntryType.Song, maxResults, before);

				var entries = albums.Concat(artists).Concat(songs).OrderByDescending(e => e.Created).Take(maxResults).ToArray();
				var albumVersionIds = entries.Where(e => e.EntryType == EntryType.Album).Select(e => e.Id).ToArray();
				var artistVersionIds = entries.Where(e => e.EntryType == EntryType.Artist).Select(e => e.Id).ToArray();
				var songVersionIds = entries.Where(e => e.EntryType == EntryType.Song).Select(e => e.Id).ToArray();

				var results = GetArchivedObjects<ArchivedAlbumVersion>(ctx, albumVersionIds)
					.Concat(GetArchivedObjects<ArchivedArtistVersion>(ctx, artistVersionIds))
					.Concat(GetArchivedObjects<ArchivedSongVersion>(ctx, songVersionIds))
					.OrderByDescending(e => e.Created)
					.Select(CreateViewModel);

				return results.ToArray();

			});

		}

	}

	public class ArchivedObjectForSorting {

		public DateTime Created { get; set; }

		public EntryType EntryType { get; set; }

		public int Id { get; set; }

	}

}