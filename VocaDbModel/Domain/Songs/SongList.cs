using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Songs {

	public class SongList : IEntryWithNames {

		INameManager IEntryWithNames.Names {
			get { return new SingleNameManager(Name); }
		}

		int IEntryBase.Version {
			get { return 0; }
		}

		/// <summary>
		/// Generated image sizes for song list images
		/// </summary>
		public static ImageSizes ImageSizes = ImageSizes.Original | ImageSizes.SmallThumb;

		private ArchivedVersionManager<ArchivedSongListVersion, SongListEditableFields> archivedVersions
			= new ArchivedVersionManager<ArchivedSongListVersion, SongListEditableFields>();		
		private User author;
		private string description;
		private string name;
		private IList<SongInList> songs = new List<SongInList>();

		public SongList() {
			Description = string.Empty;
		}

		public SongList(string name, User author)
			: this() {

			Name = name;
			Author = author;

		}

		public virtual IList<SongInList> AllSongs {
			get { return songs; }
			set {
				ParamIs.NotNull(() => value);
				songs = value;
			}
		}

		public virtual ArchivedVersionManager<ArchivedSongListVersion, SongListEditableFields> ArchivedVersionsManager {
			get { return archivedVersions; }
			set {
				ParamIs.NotNull(() => value);
				archivedVersions = value;
			}
		}

		public virtual User Author {
			get { return author; }
			set {
				ParamIs.NotNull(() => value);
				author = value;
			}
		}

		string IEntryBase.DefaultName {
			get { return Name; }
		}

		public virtual bool Deleted {
			get { return false; }
		}

		public virtual string Description {
			get { return description; }
			set {
				ParamIs.NotNull(() => value);
				description = value;
			}
		}

		public virtual EntryType EntryType {
			get {
				return EntryType.SongList;
			}
		}

		public virtual SongListFeaturedCategory FeaturedCategory { get; set; }

		public virtual bool FeaturedList {
			get {
				return FeaturedCategory != SongListFeaturedCategory.Nothing;
			}
		}

		public virtual int Id { get; set; }

		public virtual string Name {
			get { return name; }
			set {
				ParamIs.NotNullOrWhiteSpace(() => value);
				name = value;
			}
		}

		public virtual IEnumerable<SongInList> SongLinks {
			get {
				return AllSongs.Where(s => !s.Song.Deleted);
			}
		}

		/// <summary>
		/// Entry thumbnail picture. Can be null.
		/// </summary>
		public virtual EntryThumb Thumb { get; set; }

		public virtual SongInList AddSong(Song song) {

			var order = (SongLinks.Any() ? SongLinks.Max(s => s.Order) + 1 : 1);
			return AddSong(song, order, string.Empty);

		}

		public virtual SongInList AddSong(Song song, int order, string notes) {

			ParamIs.NotNull(() => song);

			var link = new SongInList(song, this, order, notes);
			AllSongs.Add(link);
			return link;

		}

		public virtual ArchivedSongListVersion CreateArchivedVersion(SongListDiff diff, AgentLoginData author, EntryEditEvent reason) {

			var archived = new ArchivedSongListVersion(this, diff, author, reason);
			ArchivedVersionsManager.Add(archived);

			return archived;

		}

		public virtual bool Equals(SongList another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as SongList);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public virtual CollectionDiffWithValue<SongInList, SongInList> SyncSongs(
			IEnumerable<SongInListEditContract> newTracks, Func<SongInListEditContract, Song> songGetter) {

			var diff = CollectionHelper.Diff(SongLinks, newTracks, (n1, n2) => n1.Id == n2.SongInListId);
			var created = new List<SongInList>();
			var edited = new List<SongInList>();

			foreach (var n in diff.Removed) {
				n.Delete();
			}

			foreach (var newEntry in diff.Added) {

				var song = songGetter(newEntry);

				var link = AddSong(song, newEntry.Order, newEntry.Notes ?? string.Empty);
				created.Add(link);

			}

			foreach (var linkEntry in diff.Unchanged) {

				var entry = linkEntry;
				var newEntry = newTracks.First(e => e.SongInListId == entry.Id);

				if (newEntry.Order != linkEntry.Order || newEntry.Notes != linkEntry.Notes) {
					linkEntry.Order = newEntry.Order;
					linkEntry.Notes = newEntry.Notes;
					edited.Add(linkEntry);
				}

			}

			return new CollectionDiffWithValue<SongInList, SongInList>(created, diff.Removed, diff.Unchanged, edited);

		}

		public override string ToString() {
			return string.Format("song list '{0}' [{1}]", Name, Id);
		}

	}
}
