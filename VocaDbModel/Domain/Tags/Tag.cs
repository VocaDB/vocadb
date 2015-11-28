using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Service.Exceptions;

namespace VocaDb.Model.Domain.Tags {

	public class Tag : IEquatable<Tag>, IEntryWithNames, IEntryWithStatus, IEntryWithComments, ITag {

		string IEntryBase.DefaultName {
			get { return EnglishName; }
		}

		bool IDeletableEntry.Deleted {
			get { return false; }
		}

		INameManager IEntryWithNames.Names {
			get {
				return new SingleNameManager(EnglishName);
			}
		}

		IEnumerable<Comment> IEntryWithComments.Comments => Comments;

		/// <summary>
		/// Generated image sizes for tag images
		/// </summary>
		public static ImageSizes ImageSizes = ImageSizes.Original | ImageSizes.SmallThumb;

		public const int MaxDisplayedTags = 4;
		private static readonly Regex TagNameRegex = new Regex(@"^[a-zA-Z0-9_-]+$");

		/// <summary>
		/// Tests whether a string is a valid name for a tag.
		/// Note that spaces are not allowed.
		/// </summary>
		/// <param name="tagName">Tag name to be validated.</param>
		/// <returns>True if <paramref name="tagName"/> is a valid tag name, otherwise false.</returns>
		public static bool IsValidTagName(string tagName) {
			return TagNameRegex.IsMatch(tagName);
		}

		public static void ValidateName(string name) {

			if (!IsValidTagName(name))
				throw new InvalidTagNameException(name);

		}

		public const string CommonCategory_Distribution = "Distribution";
		public const string CommonCategory_Genres = "Genres";
		public const string CommonCategory_Lyrics = "Lyrics";
		public const string CommonTag_ChangedLyrics = "changed_lyrics";
		public const string CommonTag_Free = "free";
		public const string CommonTag_Instrumental = "instrumental";
		public const string CommonTag_Nicovideo_downloadmusic = "nicovideo_downloadmusic";

		public static bool Equals(Tag tag, string tagName) {
			
			var leftTagName = tag != null ? tag.Name : string.Empty;
			var rightTagName = tagName ?? string.Empty;

			return string.Equals(leftTagName, rightTagName, StringComparison.InvariantCultureIgnoreCase);

		}

		private ISet<AlbumTagUsage> albumTagUsages = new HashSet<AlbumTagUsage>();
		private ISet<Tag> aliases = new HashSet<Tag>();
		private ArchivedVersionManager<ArchivedTagVersion, TagEditableFields> archivedVersions
			= new ArchivedVersionManager<ArchivedTagVersion, TagEditableFields>();		
		private ISet<ArtistTagUsage> artistTagUsages = new HashSet<ArtistTagUsage>();
		private string categoryName;
		private ISet<Tag> children = new HashSet<Tag>();
		private IList<TagComment> comments = new List<TagComment>();
		private string description;
		private ISet<SongTagUsage> songTagUsages = new HashSet<SongTagUsage>();
		private string englishName;

		public Tag() {
			CategoryName = string.Empty;
			Description = string.Empty;
			Status = EntryStatus.Draft;
		}

		public Tag(string name, string categoryName = "")
			: this() {

			ValidateName(name);

			Name = name;
			EnglishName = name;
			CategoryName = categoryName;

		}

		/// <summary>
		/// Actual tag to be used for this tag name.
		/// If this tag has been aliased to some other tag, that tag name will be used.
		/// </summary>
		public virtual Tag ActualTag {
			get { return AliasedTo ?? this; }
		}

		public virtual Tag AliasedTo { get; set; }

		public virtual ISet<Tag> Aliases {
			get { return aliases; }
			set {
				ParamIs.NotNull(() => value);
				aliases = value;
			}
		}

		public virtual ISet<AlbumTagUsage> AllAlbumTagUsages {
			get { return albumTagUsages; }
			set {
				ParamIs.NotNull(() => value);
				albumTagUsages = value;
			}
		}

		public virtual IEnumerable<AlbumTagUsage> AlbumTagUsages {
			get {
				return AllAlbumTagUsages.Where(a => !a.Album.Deleted);
			}
		}

		public virtual ISet<ArtistTagUsage> AllArtistTagUsages {
			get { return artistTagUsages; }
			set {
				ParamIs.NotNull(() => value);
				artistTagUsages = value;
			}
		}

		public virtual ArchivedVersionManager<ArchivedTagVersion, TagEditableFields> ArchivedVersionsManager {
			get { return archivedVersions; }
			set {
				ParamIs.NotNull(() => value);
				archivedVersions = value;
			}
		}

		public virtual IEnumerable<ArtistTagUsage> ArtistTagUsages {
			get {
				return AllArtistTagUsages.Where(a => !a.Artist.Deleted);
			}
		}

		public virtual string CategoryName {
			get { return categoryName; }
			set {
				ParamIs.NotNull(() => value);
				categoryName = value; 
			}
		}

		public virtual ISet<Tag> Children {
			get { return children; }
			set {
				ParamIs.NotNull(() => value);
				children = value;
			}
		}

		public virtual IList<TagComment> Comments {
			get { return comments; }
			set {
				ParamIs.NotNull(() => value);
				comments = value;
			}
		}

		public virtual Comment CreateComment(string message, AgentLoginData loginData) {

			ParamIs.NotNullOrEmpty(() => message);
			ParamIs.NotNull(() => loginData);

			var comment = new TagComment(this, message, loginData);
			Comments.Add(comment);

			return comment;

		}

		/// <summary>
		/// Tag description, may contain Markdown formatting.
		/// </summary>
		public virtual string Description {
			get { return description; }
			set { description = value; }
		}

		/// <summary>
		/// User-visible tag name, primarily in English.
		/// </summary>
		public virtual string EnglishName {
			get { return englishName; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				englishName = value;
			}
		}

		public virtual EntryType EntryType {
			get { return EntryType.Tag; }
		}

		public virtual GlobalEntryId GlobalId => new GlobalEntryId(EntryType.Tag, Id);

		/// <summary>
		/// Database identity column with unique constraint. Generated by an identity.
		/// Provides an unique integer Id, to make tags compatible with other entries.
		/// Note that this is not the primary key of the table, <see cref="Name"/> is the primary key.
		/// </summary>
		public virtual int Id { get; set; }

		/// <summary>
		/// Tag name. Primary key (mapped to database). Guaranteed to be unique (case insensitive).
		/// </summary>
		/// <remarks>
		/// Unlike other entry types, tags use the string name field as the primary key.
		/// This field should only be used as a database identifier, <see cref="EnglishName"/> is the user-visible name.
		/// Eventually all tag references should be migrated to use the <see cref="Id"/> field.
		/// 
		/// Accessing this field does not guarantee the tag even exists in the database because NHibernate won't
		/// try to load the object if only the Id is accessed.
		/// </remarks>
		public virtual string Name { get; set; }

		/// <summary>
		/// Parent tag, if any. Can be null.
		/// </summary>
		public virtual Tag Parent { get; set; }

		/// <summary>
		/// Tag name mapped as a regular column. Value of this property is the same as <see cref="Name"/>.
		/// Forces the tag to be loaded from the database, unlike the Name field.
		/// </summary>
		public virtual string TagName { get; set; }

		/// <summary>
		/// Tag name with underscores replaced with spaces.
		/// Cannot be null or empty.
		/// </summary>
		public virtual string NameWithSpaces {
			get {
				return Name.Replace('_', ' ');
			}
		}

		/// <summary>
		/// Entry thumbnail picture. Can be null.
		/// </summary>
		public virtual EntryThumb Thumb { get; set; }

		public virtual ArchivedTagVersion CreateArchivedVersion(TagDiff diff, AgentLoginData author, EntryEditEvent reason, string notes) {

			var archived = new ArchivedTagVersion(this, diff, author, reason, notes);
			ArchivedVersionsManager.Add(archived);
			Version++;

			return archived;

		}

		public virtual void Delete() {

			while (AllAlbumTagUsages.Any())
				AllAlbumTagUsages.First().Delete();

			while (AllArtistTagUsages.Any())
				AllArtistTagUsages.First().Delete();

			while (AllSongTagUsages.Any())
				AllSongTagUsages.First().Delete();

			foreach (var child in Children) {
				child.Parent = null;
			}

			foreach (var alias in Aliases) {
				alias.AliasedTo = null;
			}

		}

		public virtual bool Equals(Tag tag) {

			if (tag == null)
				return false;

			return tag.Name.Equals(Name, StringComparison.InvariantCultureIgnoreCase);

		}

		public override bool Equals(object obj) {
			return Equals(obj as Tag);
		}

		public override int GetHashCode() {
			return Name.ToLowerInvariant().GetHashCode();
		}

		public virtual void SetParent(Tag newParent) {

			if (Equals(Parent, newParent)) {
				return;
			}

			if (Equals(Parent, this)) {
				throw new ArgumentException("Tag can't be a parent of itself!");
			}

			if (Parent != null) {
				Parent.Children.Remove(this);
			}

			Parent = newParent;

			if (newParent != null) {
				newParent.Children.Add(this);
			}

		}

		public virtual ISet<SongTagUsage> AllSongTagUsages {
			get { return songTagUsages; }
			set {
				ParamIs.NotNull(() => value);
				songTagUsages = value;
			}
		}

		public virtual IEnumerable<SongTagUsage> SongTagUsages {
			get {
				return AllSongTagUsages.Where(a => !a.Song.Deleted);
			}
		}

		public virtual EntryStatus Status { get; set; }

		public virtual string UrlSlug => EnglishName;

		public virtual int Version { get; set; }

		public override string ToString() {
			return string.Format("tag '{0}' [{1},{2}]", EnglishName, Id, Name);
		}

	}

	public interface ITag {
		int Id { get; }
		string Name { get; }
	}

	public class TagIdOrNameEqualityComparer<TTag> : IEqualityComparer<TTag> where TTag : class, ITag {

		public bool Equals(TTag x, TTag y) {

			if (ReferenceEquals(x, y))
				return true;

			if (x == null || y == null)
				return false;

			if (x.Id != 0 && x.Id == y.Id)
				return true;

			if (!string.IsNullOrEmpty(x.Name) && string.Equals(x.Name, y.Name, StringComparison.InvariantCultureIgnoreCase))
				return true;

			return false;

		}

		public int GetHashCode(TTag obj) {

			if (obj == null)
				return 0;

			if (obj.Id != 0)
				return obj.Id.GetHashCode();

			if (!string.IsNullOrEmpty(obj.Name))
				return obj.Name.ToLowerInvariant().GetHashCode();

			return 0;

		}

	}

}
