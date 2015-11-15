using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Discussions;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.DataContracts.Api {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class EntryForApiContract : IEntryWithIntId {

		public static EntryForApiContract Create(IEntryWithNames entry, ContentLanguagePreference languagePreference, 
			IEntryThumbPersister thumbPersister, IEntryImagePersisterOld imagePersisterOld, bool ssl,
			EntryOptionalFields includedFields) {
			
			ParamIs.NotNull(() => entry);

			switch (entry.EntryType) {
				case EntryType.Album:
					return new EntryForApiContract((Album)entry, languagePreference, thumbPersister, ssl, includedFields);
				case EntryType.Artist:
					return new EntryForApiContract((Artist)entry, languagePreference, thumbPersister, ssl, includedFields);
				case EntryType.DiscussionTopic:
					return new EntryForApiContract((DiscussionTopic)entry, languagePreference);
				case EntryType.ReleaseEvent:
					return new EntryForApiContract((ReleaseEvent)entry, thumbPersister, ssl, includedFields);
				case EntryType.Song:
					return new EntryForApiContract((Song)entry, languagePreference, includedFields);
				case EntryType.SongList:
					return new EntryForApiContract((SongList)entry, imagePersisterOld, ssl, includedFields);
				case EntryType.Tag:
					return new EntryForApiContract((Tag)entry, imagePersisterOld, ssl, includedFields);
			}

			return new EntryForApiContract(entry, languagePreference, includedFields);

		}

		private EntryForApiContract(IEntryWithNames entry, ContentLanguagePreference languagePreference, EntryOptionalFields fields) {

			EntryType = entry.EntryType;
			Id = entry.Id;

			DefaultName = entry.DefaultName;
			DefaultNameLanguage = entry.Names.SortNames.DefaultLanguage;
			Name = entry.Names.SortNames[languagePreference];				
			Version = entry.Version;

			if (fields.HasFlag(EntryOptionalFields.AdditionalNames)) {
				AdditionalNames = entry.Names.GetAdditionalNamesStringForLanguage(languagePreference);
			}

		}

		public EntryForApiContract(Artist artist, ContentLanguagePreference languagePreference, IEntryThumbPersister thumbPersister, bool ssl, 
			EntryOptionalFields includedFields)
			: this(artist, languagePreference, includedFields) {

			ArtistType = artist.ArtistType;			
			CreateDate = artist.CreateDate;
			Status = artist.Status;

			if (includedFields.HasFlag(EntryOptionalFields.MainPicture) && artist.Picture != null) {
				MainPicture = new EntryThumbForApiContract(new EntryThumb(artist, artist.PictureMime), thumbPersister, ssl);					
			}

			if (includedFields.HasFlag(EntryOptionalFields.Names)) {
				Names = artist.Names.Select(n => new LocalizedStringContract(n)).ToArray();				
			}

			if (includedFields.HasFlag(EntryOptionalFields.Tags)) {
				Tags = artist.Tags.Usages.Select(u => new TagUsageForApiContract(u)).ToArray();				
			}

			if (includedFields.HasFlag(EntryOptionalFields.WebLinks)) {
				WebLinks = artist.WebLinks.Select(w => new ArchivedWebLinkContract(w)).ToArray();				
			}

		}

		public EntryForApiContract(Album album, ContentLanguagePreference languagePreference, IEntryThumbPersister thumbPersister, bool ssl, 
			EntryOptionalFields includedFields)
			: this(album, languagePreference, includedFields) {

			ArtistString = album.ArtistString[languagePreference];
			CreateDate = album.CreateDate;
			DiscType = album.DiscType;
			Status = album.Status;

			if (includedFields.HasFlag(EntryOptionalFields.MainPicture) && album.CoverPictureData != null) {
				MainPicture = new EntryThumbForApiContract(new EntryThumb(album, album.CoverPictureMime), thumbPersister, ssl);					
			}

			if (includedFields.HasFlag(EntryOptionalFields.Names)) {
				Names = album.Names.Select(n => new LocalizedStringContract(n)).ToArray();				
			}

			if (includedFields.HasFlag(EntryOptionalFields.Tags)) {
				Tags = album.Tags.Usages.Select(u => new TagUsageForApiContract(u)).ToArray();				
			}

			if (includedFields.HasFlag(EntryOptionalFields.WebLinks)) {
				WebLinks = album.WebLinks.Select(w => new ArchivedWebLinkContract(w)).ToArray();				
			}

		}

		public EntryForApiContract(ReleaseEvent releaseEvent, IEntryThumbPersister thumbPersister, bool ssl, EntryOptionalFields includedFields)
			: this(releaseEvent, ContentLanguagePreference.Default, includedFields) {

			CreateDate = releaseEvent.Date.DateTime.Value;
			ReleaseEventSeriesName = releaseEvent.Series != null ? releaseEvent.Series.Name : null;

			if (includedFields.HasFlag(EntryOptionalFields.MainPicture) && releaseEvent.Series != null && !string.IsNullOrEmpty(releaseEvent.Series.PictureMime)) {
				MainPicture = new EntryThumbForApiContract(new EntryThumb(releaseEvent.Series, releaseEvent.Series.PictureMime), thumbPersister, ssl);
			}

		}

		// Only used for recent comments atm.
		public EntryForApiContract(DiscussionTopic topic, ContentLanguagePreference languagePreference)
			: this((IEntryWithNames)topic, languagePreference, EntryOptionalFields.None) {

			CreateDate = topic.Created;

		}

		public EntryForApiContract(Song song, ContentLanguagePreference languagePreference, EntryOptionalFields includedFields)
			: this((IEntryWithNames)song, languagePreference, includedFields) {
			
			ArtistString = song.ArtistString[languagePreference];
			CreateDate = song.CreateDate;
			SongType = song.SongType;
			Status = song.Status;

			if (includedFields.HasFlag(EntryOptionalFields.MainPicture)) {

				var thumb = VideoServiceHelper.GetThumbUrl(song.PVs.PVs);

				if (!string.IsNullOrEmpty(thumb)) {
					MainPicture = new EntryThumbForApiContract { UrlSmallThumb = thumb, UrlThumb = thumb, UrlTinyThumb = thumb };
				}

			}

			if (includedFields.HasFlag(EntryOptionalFields.Names)) {
				Names = song.Names.Select(n => new LocalizedStringContract(n)).ToArray();				
			}

			if (includedFields.HasFlag(EntryOptionalFields.Tags)) {
				Tags = song.Tags.Usages.Select(u => new TagUsageForApiContract(u)).ToArray();				
			}

			if (includedFields.HasFlag(EntryOptionalFields.WebLinks)) {
				WebLinks = song.WebLinks.Select(w => new ArchivedWebLinkContract(w)).ToArray();				
			}

		}

		public EntryForApiContract(SongList songList, IEntryImagePersisterOld thumbPersister, bool ssl, 
			EntryOptionalFields includedFields)
			: this(songList, ContentLanguagePreference.Default, includedFields) {

			SongListFeaturedCategory = songList.FeaturedCategory;

			if (includedFields.HasFlag(EntryOptionalFields.MainPicture) && songList.Thumb != null) {
				MainPicture = new EntryThumbForApiContract(songList.Thumb, thumbPersister, ssl, SongList.ImageSizes);					
			}

		}

		public EntryForApiContract(Tag tag, IEntryImagePersisterOld thumbPersister, bool ssl, 
			EntryOptionalFields includedFields)
			: this(tag, ContentLanguagePreference.Default, includedFields) {

			TagCategoryName = tag.CategoryName;

			if (includedFields.HasFlag(EntryOptionalFields.MainPicture) && tag.Thumb != null) {
				MainPicture = new EntryThumbForApiContract(tag.Thumb, thumbPersister, ssl, Tag.ImageSizes);					
			}

			UrlSlug = tag.Name;

		}

		[DataMember(EmitDefaultValue = false)]
		public string AdditionalNames { get; set;}

		[DataMember(EmitDefaultValue = false)]
		public string ArtistString { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public ArtistType? ArtistType { get; set; }

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public string DefaultName { get; set; }

		[DataMember]
		public ContentLanguageSelection DefaultNameLanguage { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Description { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public DiscType? DiscType { get; set; }

		[DataMember]
		public EntryType EntryType { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public EntryThumbForApiContract MainPicture { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public LocalizedStringContract[] Names { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public SongListFeaturedCategory? SongListFeaturedCategory { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public SongType? SongType { get; set; }

		[DataMember]
		public EntryStatus Status { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string ReleaseEventSeriesName { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string TagCategoryName { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public TagUsageForApiContract[] Tags { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string UrlSlug { get; set; }

		[DataMember]
		public int Version { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public ArchivedWebLinkContract[] WebLinks { get; set; }

	}

	[Flags]
	public enum EntryOptionalFields {

		None = 0,
		AdditionalNames = 1,
		Description = 2,
		MainPicture = 4,
		Names = 8,
		Tags = 16,
		WebLinks = 32

	}

}
