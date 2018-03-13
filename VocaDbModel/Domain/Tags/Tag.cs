using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Tags {

	public class Tag : 
		IEquatable<Tag>, IEntryWithNames<TagName>, IEntryWithStatus, IEntryWithComments, ITag, INameFactory<TagName>, IWebLinkFactory<TagWebLink>,
		IEntryWithVersions, IDeletableEntry {

		IEnumerable<Comment> IEntryWithComments.Comments => Comments;

		/// <summary>
		/// Generated image sizes for tag images
		/// </summary>
		public static ImageSizes ImageSizes = ImageSizes.Original | ImageSizes.SmallThumb;

		public const int MaxDisplayedTags = 4;

		public static bool Equals(ITag left, ITag right) {
			return left?.Id == right?.Id;
		}

		private ISet<AlbumTagUsage> albumTagUsages = new HashSet<AlbumTagUsage>();
		private ArchivedVersionManager<ArchivedTagVersion, TagEditableFields> archivedVersions
			= new ArchivedVersionManager<ArchivedTagVersion, TagEditableFields>();		
		private ISet<ArtistTagUsage> artistTagUsages = new HashSet<ArtistTagUsage>();
		private string categoryName;
		private ISet<Tag> children = new HashSet<Tag>();
		private IList<TagComment> comments = new List<TagComment>();
		private EnglishTranslatedString description;
		private ISet<EventTagUsage> eventTagUsages = new HashSet<EventTagUsage>();
		private ISet<EventSeriesTagUsage> eventSeriesTagUsages = new HashSet<EventSeriesTagUsage>();
		private IList<TagMapping> mappings = new List<TagMapping>();
		private NameManager<TagName> names = new NameManager<TagName>();
		private ISet<RelatedTag> relatedTags = new HashSet<RelatedTag>();
		private ISet<SongTagUsage> songTagUsages = new HashSet<SongTagUsage>();
		private IList<TagForUser> tagsForUsers = new List<TagForUser>();
		private WebLinkManager<TagWebLink> webLinks = new WebLinkManager<TagWebLink>();

		public Tag() {
			CategoryName = string.Empty;
			CreateDate = DateTime.Now;
			Description = new EnglishTranslatedString();
			Status = EntryStatus.Draft;
			Targets = TagTargetTypes.All;
		}

		public Tag(string englishName, string categoryName = "")
			: this(new LocalizedString(englishName, ContentLanguageSelection.English), categoryName) {}

		public Tag(LocalizedString name, string categoryName = "")
			: this() {

			ParamIs.NotNull(() => name);

			Names.SortNames.DefaultLanguage = name.Language;
			Names.Add(new TagName(this, name));
			CategoryName = categoryName;

		}

		/// <summary>
		/// List of all album tag usages (including deleted albums) for this tag.
		/// Warning: this list can be huge! Avoid traversing the list if possible.
		/// </summary>
		public virtual ISet<AlbumTagUsage> AllAlbumTagUsages {
			get => albumTagUsages;
			set {
				ParamIs.NotNull(() => value);
				albumTagUsages = value;
			}
		}

		/// <summary>
		/// List of all album tag usages (not including deleted albums) for this tag.
		/// Warning: this list can be huge! Avoid traversing the list if possible.
		/// </summary>
		public virtual IEnumerable<AlbumTagUsage> AlbumTagUsages {
			get {
				return AllAlbumTagUsages.Where(a => !a.Album.Deleted);
			}
		}

		/// <summary>
		/// List of all artist tag usages (including deleted artists) for this tag.
		/// Warning: this list can be huge! Avoid traversing the list if possible.
		/// </summary>
		public virtual ISet<ArtistTagUsage> AllArtistTagUsages {
			get => artistTagUsages;
			set {
				ParamIs.NotNull(() => value);
				artistTagUsages = value;
			}
		}

		/// <summary>
		/// List of child tags. Includes deleted tags.
		/// </summary>
		public virtual ISet<Tag> AllChildren {
			get => children;
			set {
				ParamIs.NotNull(() => value);
				children = value;
			}
		}

		public virtual IEnumerable<TagUsage> AllTagUsages => AllAlbumTagUsages.Cast<TagUsage>().Concat(AllArtistTagUsages).Concat(AllSongTagUsages);

		public virtual ArchivedVersionManager<ArchivedTagVersion, TagEditableFields> ArchivedVersionsManager {
			get => archivedVersions;
			set {
				ParamIs.NotNull(() => value);
				archivedVersions = value;
			}
		}

		IArchivedVersionsManager IEntryWithVersions.ArchivedVersionsManager => ArchivedVersionsManager;

		/// <summary>
		/// List of all artist tag usages (not including deleted artists) for this tag.
		/// Warning: this list can be huge! Avoid traversing the list if possible.
		/// </summary>
		public virtual IEnumerable<ArtistTagUsage> ArtistTagUsages {
			get {
				return AllArtistTagUsages.Where(a => !a.Artist.Deleted);
			}
		}

		public virtual string CategoryName {
			get => categoryName;
			set {
				ParamIs.NotNull(() => value);
				categoryName = value; 
			}
		}

		/// <summary>
		/// List of child tags. Does not include deleted tags.
		/// </summary>
		public virtual IEnumerable<Tag> Children => AllChildren.Where(t => !t.Deleted);

		public virtual IList<TagComment> Comments {
			get => comments;
			set {
				ParamIs.NotNull(() => value);
				comments = value;
			}
		}

		/// <summary>
		/// Date when this entry was created.
		/// </summary>
		public virtual DateTime CreateDate { get; set; }

		public virtual Comment CreateComment(string message, AgentLoginData loginData) {

			ParamIs.NotNullOrEmpty(() => message);
			ParamIs.NotNull(() => loginData);

			var comment = new TagComment(this, message, loginData);
			Comments.Add(comment);

			return comment;

		}

		public virtual string DefaultName => TranslatedName.Default;

		public virtual bool Deleted { get; set; }

		/// <summary>
		/// Tag description, may contain Markdown formatting.
		/// </summary>
		public virtual EnglishTranslatedString Description {
			get => description;
			set {
				ParamIs.NotNull(() => value);
				description = value;
			}
		}

		public virtual EntryType EntryType => EntryType.Tag;

		public virtual GlobalEntryId GlobalId => new GlobalEntryId(EntryType.Tag, Id);

		public virtual bool HideFromSuggestions { get; set; }

		/// <summary>
		/// Unique database ID, assigned by identity.
		/// </summary>
		public virtual int Id { get; set; }

		public virtual IList<TagMapping> Mappings {
			get => mappings;
			set {
				ParamIs.NotNull(() => value);
				mappings = value;
			}
		}

		public virtual NameManager<TagName> Names{
			get => names;
			set {
				ParamIs.NotNull(() => value);
				names = value;
			}
		}

		INameManager<TagName> IEntryWithNames<TagName>.Names => Names;

		INameManager IEntryWithNames.Names => Names;

		/// <summary>
		/// Parent tag, if any. Can be null.
		/// </summary>
		public virtual Tag Parent { get; set; }

		/// <summary>
		/// List of sibling tags (children of the same parent excluding this one). 
		/// Does not include deleted tags.
		/// </summary>
		public virtual IEnumerable<Tag> Siblings => Parent != null ? Parent.Children.Where(t => !t.Equals(this)) : Enumerable.Empty<Tag>();

		public virtual TagTargetTypes Targets { get; set; }

		/// <summary>
		/// Entry thumbnail picture. Can be null.
		/// </summary>
		public virtual EntryThumb Thumb { get; set; }

		public virtual TranslatedString TranslatedName => Names.SortNames;

		public virtual ArchivedTagVersion CreateArchivedVersion(XDocument data, TagDiff diff, AgentLoginData author, EntryEditEvent reason, string notes) {

			var archived = new ArchivedTagVersion(this, data, diff, author, reason, notes);
			ArchivedVersionsManager.Add(archived);
			Version++;

			return archived;

		}

		public virtual TagMapping CreateMapping(string sourceTag) {

			ParamIs.NotNullOrEmpty(() => sourceTag);

			var mapping = new TagMapping(this, sourceTag);
			Mappings.Add(mapping);
			return mapping;

		}

		public virtual TagName CreateName(string val, ContentLanguageSelection language) {

			ParamIs.NotNullOrEmpty(() => val);

			return CreateName(new LocalizedString(val, language));

		}

		public virtual TagName CreateName(LocalizedString localizedString) {

			ParamIs.NotNull(() => localizedString);

			var name = new TagName(this, localizedString);
			Names.Add(name);

			return name;

		}

		public virtual TagWebLink CreateWebLink(string description, string url, WebLinkCategory category) {

			ParamIs.NotNull(() => description);
			ParamIs.NotNullOrEmpty(() => url);

			var link = new TagWebLink(this, description, url);
			WebLinks.Links.Add(link);

			return link;

		}

		public virtual void Delete() {

			while (AllAlbumTagUsages.Any())
				AllAlbumTagUsages.First().Delete();

			while (AllArtistTagUsages.Any())
				AllArtistTagUsages.First().Delete();

			while (AllSongTagUsages.Any())
				AllSongTagUsages.First().Delete();

			foreach (var child in AllChildren) {
				child.Parent = null;
			}

			while (RelatedTags.Any())
				RelatedTags.First().Delete();

			if (Parent != null)
				Parent.AllChildren.Remove(this);

			TagsForUsers.Clear();
			Mappings.Clear();

		}

		public virtual bool Equals(Tag tag) {

			if (tag == null)
				return false;

			if (ReferenceEquals(this, tag))
				return true;

			return Id != 0 && Id == tag.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as Tag);
		}

		public override int GetHashCode() {
			return Id.GetHashCode();
		}

		public virtual bool HasAncestorTag(Tag tag) {
			return Parent != null && (Parent.Equals(tag) || Parent.HasAncestorTag(tag));
		}

		/// <summary>
		/// Tests whether a tag is a valid parent for this tag.
		/// </summary>
		public virtual bool IsValidParent(Tag tag) {
			return tag == null || (!tag.Equals(this) && !tag.HasAncestorTag(this));

		}

		public virtual void SetParent(Tag newParent) {

			// New parent is current parent, no change
			if (Equals(Parent, newParent)) {
				return;
			}

			if (!IsValidParent(newParent)) {
				throw new ArgumentException("Tag can't be a parent of itself!");
			}

			Parent?.AllChildren.Remove(this);

			Parent = newParent;

			newParent?.AllChildren.Add(this);

		}

		public virtual ISet<EventTagUsage> AllEventTagUsages {
			get => eventTagUsages;
			set {
				ParamIs.NotNull(() => value);
				eventTagUsages = value;
			}
		}

		public virtual ISet<EventSeriesTagUsage> AllEventSeriesTagUsages {
			get => eventSeriesTagUsages;
			set {
				ParamIs.NotNull(() => value);
				eventSeriesTagUsages = value;
			}
		}

		/// <summary>
		/// List of all song tag usages (including deleted songs) for this tag.
		/// Warning: this list can be huge! Avoid traversing the list if possible.
		/// </summary>
		public virtual ISet<SongTagUsage> AllSongTagUsages {
			get => songTagUsages;
			set {
				ParamIs.NotNull(() => value);
				songTagUsages = value;
			}
		}

		public virtual IEnumerable<EventTagUsage> EventTagUsages => AllEventTagUsages.Where(a => !a.Entry.Deleted);

		public virtual IEnumerable<EventSeriesTagUsage> EventSeriesTagUsages => AllEventSeriesTagUsages.Where(a => !a.Entry.Deleted);

		public virtual bool IsValidFor(EntryType entryType) {

			if (Targets == TagTargetTypes.All)
				return true;

			if (Targets == TagTargetTypes.Nothing)
				return false;

			if (entryType == EntryType.ReleaseEventSeries)
				entryType = EntryType.ReleaseEvent;

			return Targets.HasFlag((TagTargetTypes)entryType);

		}

		public virtual ISet<RelatedTag> RelatedTags {
			get => relatedTags;
			set {
				ParamIs.NotNull(() => value);
				relatedTags = value;
			}
		}

		/// <summary>
		/// List of all song tag usages (not including deleted songs) for this tag.
		/// Warning: this list can be huge! Avoid traversing the list if possible.
		/// </summary>
		public virtual IEnumerable<SongTagUsage> SongTagUsages {
			get {
				return AllSongTagUsages.Where(a => !a.Song.Deleted);
			}
		}

		public virtual EntryStatus Status { get; set; }

		public virtual IList<TagForUser> TagsForUsers {
			get => tagsForUsers;
			set {
				ParamIs.NotNull(() => value);
				tagsForUsers = value;
			}
		}

		public virtual string UrlSlug => Utils.UrlFriendlyNameFactory.GetUrlFriendlyName(TranslatedName);

		/// <summary>
		/// Number of tag usages.
		/// This is persisted to database.
		/// </summary>
		public virtual int UsageCount { get; set; }

		public virtual int Version { get; set; }

		public virtual WebLinkManager<TagWebLink> WebLinks {
			get => webLinks;
			set {
				ParamIs.NotNull(() => value);
				webLinks = value;
			}
		}

		public virtual RelatedTag AddRelatedTag(Tag tag) {

			ParamIs.NotNull(() => tag);

			if (Equals(tag))
				throw new ArgumentException("Cannot add self as related tag");

			var link = new RelatedTag(this, tag);
			RelatedTags.Add(link);

			var reverseLink = link.CreateReversed();
			tag.RelatedTags.Add(reverseLink);

			return link;

		}

		public virtual CollectionDiff<RelatedTag> SyncRelatedTags(IEnumerable<ITag> newRelatedTags, Func<int, Tag> loadTagFunc) {

			Func<ITag, RelatedTag> create = tagRef => {
				var tag = loadTagFunc(tagRef.Id);
				return AddRelatedTag(tag);
			};

			var diff = CollectionHelper.Sync(RelatedTags, newRelatedTags, (t1, t2) => Equals(t1.LinkedTag, t2), create, link => link.Delete());
			return diff;

		}

		public override string ToString() {
			return string.Format("tag '{0}' [{1}]", DefaultName, Id);
		}

	}

	public interface ITag {
		int Id { get; }
	}

}
