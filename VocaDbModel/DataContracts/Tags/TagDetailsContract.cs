using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Tags {

	public class TagDetailsContract : TagContract, IEntryWithStatus {

		string IEntryBase.DefaultName => Name;

		EntryType IEntryBase.EntryType => EntryType.Tag;

		public TagDetailsContract() { }

		public TagDetailsContract(Tag tag, 
			IEnumerable<Artist> artists, int artistCount, IEnumerable<Album> albums, int albumCount,
			IEnumerable<Song> songs, int songCount, ContentLanguagePreference languagePreference)
			: base(tag, languagePreference) {

			AdditionalNames = tag.Names.AdditionalNamesString;
			Translations = tag.Names.GetTranslationsString(languagePreference);

			Albums = albums.Select(a => new AlbumContract(a, languagePreference)).ToArray();
			AlbumCount = albumCount;

			Artists = artists.Select(a => new ArtistContract(a, languagePreference)).ToArray();
			ArtistCount = artistCount;

			Description = tag.Description;
			RelatedTags = tag.RelatedTags
				.Select(a => new TagBaseContract(a.LinkedTag, languagePreference, true))
				.OrderBy(t => t.Name)
				.ToArray();

			Children = tag.Children
				.Select(a => new TagBaseContract(a, languagePreference))
				.OrderBy(t => t.Name)
				.ToArray();

			Siblings = tag.Parent != null ? tag.Parent.Children
				.Where(t => !t.Equals(tag))
				.Select(a => new TagBaseContract(a, languagePreference))
				.OrderBy(t => t.Name)
				.ToArray() 
				: new TagBaseContract[0];

			Songs = songs.Select(a => new SongForApiContract(a, languagePreference, SongOptionalFields.AdditionalNames | SongOptionalFields.ThumbUrl)).ToArray();
			SongCount = songCount;

			Thumb = (tag.Thumb != null ? new EntryThumbContract(tag.Thumb) : null);
			WebLinks = tag.WebLinks.Links.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();

		}

		public int AlbumCount { get; set; }

		public int ArtistCount { get; set; }

		public AlbumContract[] Albums { get; set; }

		public ArtistContract[] Artists { get; set; }

		public TagBaseContract[] Children { get; set; }

		public int CommentCount { get; set; }

		public new EnglishTranslatedString Description { get; set; }

		public bool IsFollowing { get; set; }

		public CommentForApiContract[] LatestComments { get; set; }

		public TagBaseContract[] RelatedTags { get; set; }

		public TagBaseContract[] Siblings { get; set; }

		public SongForApiContract[] Songs { get; set; }

		public int SongCount { get; set; }

		public EntryThumbContract Thumb { get; set; }

		public string Translations { get; set; }

		public WebLinkContract[] WebLinks { get; set; }

	}

}
