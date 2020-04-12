using System;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.SongImport;
using VocaDb.Model.DataContracts.SongLists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.Queries;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Model.Service.SongImport;

namespace VocaDb.Model.Database.Queries {

	public class SongListQueries : QueriesBase<ISongListRepository, SongList> {

		private readonly IEntryLinkFactory entryLinkFactory;
		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private readonly IEntryImagePersisterOld imagePersister;
		private readonly IUserIconFactory userIconFactory;

		public ArchivedSongListVersion Archive(IDatabaseContext<SongList> ctx, SongList songList, SongListDiff diff, EntryEditEvent reason, string notes = "") {

			var agentLoginData = ctx.CreateAgentLoginData(PermissionContext);
			var archived = songList.CreateArchivedVersion(diff, agentLoginData, reason, notes);
			ctx.OfType<ArchivedSongListVersion>().Save(archived);
			return archived;

		}

		private PartialImportedSongs FindSongs(PartialImportedSongs songs) {
			
			return HandleQuery(session => {

				foreach (var entry in songs.Items) {

					var pv = session.Query<PVForSong>()
						.FirstOrDefault(p => p.Service == entry.PVService && p.PVId == entry.PVId && !p.Song.Deleted);

					var song = pv != null ? new SongForApiContract(pv.Song, null, LanguagePreference, SongOptionalFields.None) : null;

					entry.MatchedSong = song;

				}

				return songs;

			});

		}

		private PartialFindResult<T> GetSongsInList<T>(IDatabaseContext<SongList> session, SongInListQueryParams queryParams,
			Func<SongInList, T> fac) {

			var q = session.OfType<SongInList>().Query()
				.Where(a => !a.Song.Deleted && a.List.Id == queryParams.ListId)
				.WhereSongHasName(queryParams.TextQuery, true)
				.WhereSongHasPVService(queryParams.PVServices)
				.WhereSongHasArtists(queryParams.ArtistIds, queryParams.ChildVoicebanks)
				.WhereSongHasTags(queryParams.TagIds)
				.WhereSongHasType(queryParams.SongTypes)
				.WhereMatchFilters(queryParams.AdvancedFilters);

			IQueryable<SongInList> resultQ = q.OrderBy(queryParams.SortRule, PermissionContext.LanguagePreference);
			resultQ = resultQ.Paged(queryParams.Paging);

			var contracts = resultQ.ToArray().Select(s => fac(s)).ToArray();
			var totalCount = (queryParams.Paging.GetTotalCount ? q.Count() : 0);

			return new PartialFindResult<T>(contracts, totalCount);

		}

		private SongList CreateSongList(IDatabaseContext<SongList> ctx, SongListForEditContract contract, UploadedFileContract uploadedFile) {

			var user = GetLoggedUser(ctx);
			var newList = new SongList(contract.Name, user);
			newList.Description = contract.Description ?? string.Empty;
			newList.EventDate = contract.EventDate;

			if (EntryPermissionManager.CanManageFeaturedLists(permissionContext))
				newList.FeaturedCategory = contract.FeaturedCategory;

			ctx.Save(newList);

			var songDiff = newList.SyncSongs(contract.SongLinks, c => ctx.OfType<Song>().Load(c.Song.Id));
			ctx.OfType<SongInList>().Sync(songDiff);

			SetThumb(newList, uploadedFile);

			ctx.Update(newList);

			ctx.AuditLogger.AuditLog(string.Format("created song list {0}", entryLinkFactory.CreateEntryLink(newList)), user);
			var archived = Archive(ctx, newList, new SongListDiff(), EntryEditEvent.Created, contract.UpdateNotes);

			if (newList.FeaturedList) {
				AddEntryEditedEntry(ctx.OfType<ActivityEntry>(), newList, EntryEditEvent.Created, archived);						
			}

			return newList;

		}

		private void SetThumb(SongList list, UploadedFileContract uploadedFile) {

			if (uploadedFile != null) {

				var thumb = new EntryThumb(list, uploadedFile.Mime, ImagePurpose.Main);
				list.Thumb = thumb;
				var thumbGenerator = new ImageThumbGenerator(imagePersister);
				thumbGenerator.GenerateThumbsAndMoveImage(uploadedFile.Stream, thumb, SongList.ImageSizes, originalSize: Constants.RestrictedImageOriginalSize);

			}

		}

		public SongListQueries(ISongListRepository repository, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory, 
			IEntryImagePersisterOld imagePersister, IUserIconFactory userIconFactory)
			: base(repository, permissionContext) {

			this.entryLinkFactory = entryLinkFactory;
			this.imagePersister = imagePersister;
			this.userIconFactory = userIconFactory;

		}

		public ICommentQueries Comments(IDatabaseContext<SongList> ctx) {
			return new CommentQueries<SongListComment, SongList>(ctx.OfType<SongListComment>(), PermissionContext, userIconFactory, entryLinkFactory);
		}

		public CommentForApiContract CreateComment(int songId, CommentForApiContract contract) {

			return HandleTransaction(ctx => Comments(ctx).Create(songId, contract));

		}

		public void Delete(int listId, string notes) {

			permissionContext.VerifyManageDatabase();

			repository.HandleTransaction(ctx => {

				var entry = ctx.Load(listId);

				PermissionContext.VerifyEntryDelete(entry);

				entry.Deleted = true;
				ctx.Update(entry);

				Archive(ctx, entry, new SongListDiff(false), EntryEditEvent.Deleted, notes);

				ctx.AuditLogger.AuditLog(string.Format("deleted {0}", entry));

			});

		}

		public void MoveToTrash(int listId) {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			HandleTransaction(ctx => {

				var user = GetLoggedUser(ctx);
				var list = ctx.Load<SongList>(listId);

				ctx.AuditLogger.SysLog(string.Format("deleting {0}", list.ToString()));

				EntryPermissionManager.VerifyEdit(PermissionContext, list);

				var archivedVersions = list.ArchivedVersionsManager.Versions;
				ctx.DeleteAll(archivedVersions);
				list.ArchivedVersionsManager.Versions.Clear();

				var activityEntries = ctx.Query<SongListActivityEntry>().Where(a => a.Entry.Id == listId).ToArray();
				ctx.DeleteAll(activityEntries);

				ctx.Delete(list);

				ctx.AuditLogger.AuditLog(string.Format("deleted {0}", list.ToString()), user);

			});

		}

		public PartialFindResult<TResult> Find<TResult>(Func<SongList, TResult> fac, SongListQueryParams queryParams) {

			return HandleQuery(ctx => {

				var listQuery = ctx.Query()
					.WhereNotDeleted()
					.WhereHasFeaturedCategory(queryParams.FeaturedCategory, false)
					.WhereHasName(queryParams.TextQuery)
					.WhereHasTags(queryParams.TagIds, queryParams.ChildTags);

				var count = queryParams.Paging.GetTotalCount ? listQuery.Count() : 0;

				return new PartialFindResult<TResult>(listQuery
					.OrderBy(queryParams.SortRule)
					.Paged(queryParams.Paging)
					.ToArray()
					.Select(s => fac(s))
					.ToArray(), count);

			});

		}

		public CommentForApiContract[] GetComments(int listId) {

			return HandleQuery(ctx => Comments(ctx).GetAll(listId));

		}

		public SongListDetailsContract GetDetails(int listId) {

			return repository.HandleQuery(ctx => {
				return new SongListDetailsContract(ctx.Load(listId), PermissionContext) {
					LatestComments = Comments(ctx).GetList(listId, 3)
				};
            });

		}

		public PartialFindResult<SongInListContract> GetSongsInList(SongInListQueryParams queryParams) {

			return repository.HandleQuery(session => GetSongsInList(session, queryParams, s => new SongInListContract(s, PermissionContext.LanguagePreference)));

		}

		public PartialFindResult<T> GetSongsInList<T>(SongInListQueryParams queryParams, Func<SongInList, T> fac) {

			return repository.HandleQuery(ctx => GetSongsInList(ctx, queryParams, fac));

		}

		public SongListContract GetSongList(int listId) {

			return repository.HandleQuery(session => new SongListContract(session.Load(listId), PermissionContext));
		
		}

		public SongListForEditContract GetSongListForEdit(int listId) {

			return repository.HandleQuery(session => new SongListForEditContract(session.Load(listId), PermissionContext));

		}

		public SongListWithArchivedVersionsContract GetSongListWithArchivedVersions(int id) {

			return repository.HandleQuery(session => new SongListWithArchivedVersionsContract(session.Load(id), PermissionContext));

		}

		public async Task<ImportedSongListContract> Import(string url, bool parseAll) {

			var parsed = await new SongListImporters().Parse(url, parseAll);

			FindSongs(parsed.Songs);

			return parsed;

		}

		public async Task<PartialImportedSongs> ImportSongs(string url, string pageToken, int maxResults, bool parseAll) {

			var songs = await new SongListImporters().GetSongs(url, pageToken, maxResults, parseAll);
			return FindSongs(songs);

		}

		public int UpdateSongList(SongListForEditContract contract, UploadedFileContract uploadedFile) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			return repository.HandleTransaction(ctx => {

				var user = GetLoggedUser(ctx);
				SongList list;

				if (contract.Id == 0) {

					list = CreateSongList(ctx, contract, uploadedFile);

				} else {

					list = ctx.Load(contract.Id);
					var diff = new SongListDiff();

					EntryPermissionManager.VerifyEdit(PermissionContext, list);

					if (list.Description != contract.Description) {
						diff.Description.Set();
						list.Description = contract.Description ?? string.Empty;						
					}

					if (list.Name != contract.Name) {
						diff.Name.Set();
						list.Name = contract.Name;						
					}

					if (EntryPermissionManager.CanManageFeaturedLists(PermissionContext) && list.FeaturedCategory != contract.FeaturedCategory) {
						diff.FeaturedCategory.Set();
						list.FeaturedCategory = contract.FeaturedCategory;						
					}

					if (list.EventDate != contract.EventDate) {
						diff.SetChanged(SongListEditableFields.EventDate);
						list.EventDate = contract.EventDate;
					}

					if (list.Status != contract.Status) {
						diff.Status.Set();
						list.Status = contract.Status;
					}

					var songDiff = list.SyncSongs(contract.SongLinks, c => ctx.OfType<Song>().Load(c.Song.Id));

					if (songDiff.Changed) {
						diff.Songs.Set();
					}

					ctx.OfType<SongInList>().Sync(songDiff);

					if (uploadedFile != null) {
						diff.Thumbnail.Set();
						SetThumb(list, uploadedFile);						
					}

					ctx.Update(list);

					ctx.AuditLogger.AuditLog(
						string.Format("updated song list {0} ({1})", entryLinkFactory.CreateEntryLink(list), diff.ChangedFieldsString), user);

					var archived = Archive(ctx, list, diff, EntryEditEvent.Updated, contract.UpdateNotes);

					if (list.FeaturedList) {
						AddEntryEditedEntry(ctx.OfType<ActivityEntry>(), list, EntryEditEvent.Updated, archived);						
					}

				}

				return list.Id;

			});

		}

	}

}