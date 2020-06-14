using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Tags {

	public class TagDetailsContract : TagContract, IEntryWithStatus {

		string IEntryBase.DefaultName => Name;

		EntryType IEntryBase.EntryType => EntryType.Tag;

		public TagDetailsContract() { }

		public TagDetailsContract(Tag tag, 
			TagStatsContract stats,
			ContentLanguagePreference languagePreference)
			: base(tag, languagePreference) {

			AdditionalNames = tag.Names.AdditionalNamesString;
			Translations = tag.Names.GetTranslationsString(languagePreference);

			Description = tag.Description;
			RelatedTags = tag.RelatedTags
				.Where(t => !t.LinkedTag.Deleted)
				.Select(a => new TagBaseContract(a.LinkedTag, languagePreference, true))
				.OrderBy(t => t.Name)
				.ToArray();

			Children = tag.Children
				.Select(a => new TagBaseContract(a, languagePreference))
				.OrderBy(t => t.Name)
				.ToArray();

			Siblings = tag.Siblings
				.Select(a => new TagBaseContract(a, languagePreference))
				.OrderBy(t => t.Name)
				.ToArray();

			Thumb = (tag.Thumb != null ? new EntryThumbContract(tag.Thumb) : null);
			WebLinks = tag.WebLinks.Links.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();
			MappedNicoTags = tag.Mappings.Select(t => t.SourceTag).ToArray();

			Stats = stats;

		}

		public int AllUsageCount => Stats.ArtistCount + Stats.AlbumCount + Stats.SongCount + Stats.EventCount + Stats.SongListCount;

		public TagBaseContract[] Children { get; set; }

		public int CommentCount { get; set; }

		public new EnglishTranslatedString Description { get; set; }

		public int FollowerCount { get; set; }

		public bool IsFollowing { get; set; }

		public CommentForApiContract[] LatestComments { get; set; }

		public string[] MappedNicoTags { get; set; }

		public EntryTypeAndSubType RelatedEntryType { get; set; }

		public TagBaseContract[] RelatedTags { get; set; }

		public TagBaseContract[] Siblings { get; set; }

		public TagStatsContract Stats { get; set; }

		public EntryThumbContract Thumb { get; set; }

		public string Translations { get; set; }

		public WebLinkContract[] WebLinks { get; set; }

		public object JsonModel => new {
			Name, 
			Parent, 
			Children = Children.Take(20),
			Siblings = Siblings.Take(20),
			HasMoreChildren = Children.Length > 20,
			HasMoreSiblings = Siblings.Length > 20
		};

	}

}
