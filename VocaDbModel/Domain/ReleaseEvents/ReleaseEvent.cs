using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.ReleaseEvents {

	public class ReleaseEvent : IEntryWithNames, IEntryWithVersions, IWebLinkFactory<ReleaseEventWebLink>, IReleaseEvent, 
		IEntryImageInformation, IEntryWithComments<ReleaseEventComment>, IEntryWithStatus {

		IArchivedVersionsManager IEntryWithVersions.ArchivedVersionsManager => ArchivedVersionsManager;
		string IEntryBase.DefaultName => Name;
		INameManager IEntryWithNames.Names => new SingleNameManager(Name);
		string IEntryImageInformation.Mime => PictureMime;

		private IList<Album> albums = new List<Album>();
		private ArchivedVersionManager<ArchivedReleaseEventVersion, ReleaseEventEditableFields> archivedVersions
			= new ArchivedVersionManager<ArchivedReleaseEventVersion, ReleaseEventEditableFields>();
		private IList<ReleaseEventComment> comments = new List<ReleaseEventComment>();
		private string description;
		private string name;
		private ReleaseEventSeries series;
		private string seriesSuffix;
		private IList<Song> songs = new List<Song>();
		private IList<EventForUser> users = new List<EventForUser>();
		private IList<ReleaseEventWebLink> webLinks = new List<ReleaseEventWebLink>();

		public ReleaseEvent() {
			Category = EventCategory.Unspecified;
			Deleted = false;
			Description = SeriesSuffix = string.Empty;
			Status = EntryStatus.Draft;
		}

		public ReleaseEvent(string description, DateTime? date, string name)
			: this() {

			ParamIs.NotNullOrEmpty(() => name);

			Description = description;
			Date = date;
			Name = name;

		}

		public ReleaseEvent(string description, DateTime? date, ReleaseEventSeries series, int seriesNumber, string seriesSuffix,
			string name, bool customName)
			: this() {

			ParamIs.NotNull(() => series);

			Description = description;
			Date = date;
			Series = series;
			SeriesNumber = seriesNumber;
			SeriesSuffix = seriesSuffix;
			CustomName = customName;

			if (!string.IsNullOrWhiteSpace(name)) {
				Name = name;
			}

			UpdateNameFromSeries();

		}

		public virtual IEnumerable<Album> Albums => AllAlbums.Where(a => !a.Deleted);

		public virtual IList<Album> AllAlbums {
			get => albums;
			set {
				ParamIs.NotNull(() => value);
				albums = value; 
			}
		}

		public virtual IList<Song> AllSongs {
			get => songs;
			set {
				ParamIs.NotNull(() => value);
				songs = value;
			}
		}

		public virtual ArchivedVersionManager<ArchivedReleaseEventVersion, ReleaseEventEditableFields> ArchivedVersionsManager {
			get => archivedVersions;
			set {
				ParamIs.NotNull(() => value);
				archivedVersions = value;
			}
		}

		public virtual EventCategory Category { get; set; }

		IEnumerable<Comment> IEntryWithComments.Comments => Comments;

		public virtual IList<ReleaseEventComment> Comments {
			get => comments;
			set {
				ParamIs.NotNull(() => value);
				comments = value;
			}
		}

		public virtual bool CustomName { get; set; }

		public virtual Date Date { get; set; }

		public virtual bool Deleted { get; set; }

		public virtual string Description {
			get => description;
			set {
				ParamIs.NotNull(() => value);
				description = value; 
			}
		}

		public virtual EntryType EntryType => EntryType.ReleaseEvent;

		public virtual int Id { get; set; }

		public virtual string Name {
			get => name;
			set {
				ParamIs.NotNullOrWhiteSpace(() => value);
				name = value; 
			}
		}

		public virtual string PictureMime { get; set; }

		public virtual ReleaseEventSeries Series {
			get => series;
			set => series = value;
		}

		public virtual int SeriesNumber { get; set; }

		public virtual string SeriesSuffix {
			get => seriesSuffix;
			set {
				ParamIs.NotNull(() => value);
				seriesSuffix = value;
			}
		}

		public virtual SongList SongList { get; set; }

		public virtual IEnumerable<Song> Songs => AllSongs.Where(a => !a.Deleted);

		public virtual EntryStatus Status { get; set; }

		/// <summary>
		/// URL slug. Cannot be null. Can be empty.
		/// </summary>
		public virtual string UrlSlug => Utils.UrlFriendlyNameFactory.GetUrlFriendlyName(Name);

		public virtual IList<EventForUser> Users {
			get => users;
			set {
				ParamIs.NotNull(() => value);
				users = value;
			}
		}

		public virtual string Venue { get; set; }

		public virtual int Version { get; set; }

		public virtual IList<ReleaseEventWebLink> WebLinks {
			get => webLinks;
			set {
				ParamIs.NotNull(() => value);
				webLinks = value;
			}
		}

		public virtual ArchivedReleaseEventVersion CreateArchivedVersion(XDocument data, ReleaseEventDiff diff, AgentLoginData author, EntryEditEvent reason, string notes) {

			var archived = new ArchivedReleaseEventVersion(this, data, diff, author, reason, notes);
			ArchivedVersionsManager.Add(archived);
			Version++;

			return archived;

		}

		public virtual Comment CreateComment(string message, AgentLoginData loginData) {

			ParamIs.NotNullOrEmpty(() => message);
			ParamIs.NotNull(() => loginData);

			var comment = new ReleaseEventComment(this, message, loginData);
			Comments.Add(comment);

			return comment;

		}

		public virtual ReleaseEventWebLink CreateWebLink(string description, string url, WebLinkCategory category) {

			ParamIs.NotNull(() => description);
			ParamIs.NotNullOrEmpty(() => url);

			var link = new ReleaseEventWebLink(this, description, url, category);
			WebLinks.Add(link);

			return link;

		}

		public virtual bool Equals(ReleaseEvent another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as ReleaseEvent);
		}

		public override int GetHashCode() {
			return Id.GetHashCode();
		}

		public override string ToString() {
			return string.Format("Release event '{0}' [{1}]", Name, Id);
		}

		public virtual void UpdateNameFromSeries() {

			if (Series != null && !CustomName) {
				Name = Series.GetEventName(SeriesNumber, SeriesSuffix);				
			}
			
		}

	}

	public interface IReleaseEvent : IEntryWithIntId {
		string Name { get; }
	}

}
