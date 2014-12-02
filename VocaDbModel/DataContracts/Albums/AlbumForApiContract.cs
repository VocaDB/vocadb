using System;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.DataContracts.Albums {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class AlbumForApiContract {

		public AlbumForApiContract() { }

		public AlbumForApiContract(
			Album album, AlbumMergeRecord mergeRecord, 
			ContentLanguagePreference languagePreference, 
			IEntryThumbPersister thumbPersister,
			bool ssl,
			AlbumOptionalFields fields) : this(album, mergeRecord, languagePreference, 
				fields.HasFlag(AlbumOptionalFields.Artists), 
				fields.HasFlag(AlbumOptionalFields.Names), 
				fields.HasFlag(AlbumOptionalFields.PVs), 
				fields.HasFlag(AlbumOptionalFields.Tags), 
				fields.HasFlag(AlbumOptionalFields.WebLinks)) {

			if (languagePreference != ContentLanguagePreference.Default || fields.HasFlag(AlbumOptionalFields.AdditionalNames)) {
				AdditionalNames = album.Names.GetAdditionalNamesStringForLanguage(languagePreference);
			}

			if (fields.HasFlag(AlbumOptionalFields.Identifiers)) {
				Identifiers = album.Identifiers.Select(i => new AlbumIdentifierContract(i)).ToArray();
			}

			if (thumbPersister != null && fields.HasFlag(AlbumOptionalFields.MainPicture) && !string.IsNullOrEmpty(album.CoverPictureMime)) {
				
				MainPicture = new EntryThumbForApiContract(new EntryThumb(album, album.CoverPictureMime), thumbPersister, ssl);

			}

		}

		public AlbumForApiContract(Album album, AlbumMergeRecord mergeRecord, ContentLanguagePreference languagePreference, 
			bool artists = true, bool names = true, bool pvs = false, bool tags = true, bool webLinks = false) {

			ArtistString = album.ArtistString[languagePreference];
			CatalogNumber = album.OriginalRelease != null ? album.OriginalRelease.CatNum : null;
			CreateDate = album.CreateDate;
			DefaultName = album.DefaultName;
			DefaultNameLanguage = album.Names.SortNames.DefaultLanguage;
			DiscType = album.DiscType;
			Id = album.Id;
			Name = album.Names.SortNames[languagePreference];				
			RatingAverage = album.RatingAverage;
			RatingCount = album.RatingCount;
			ReleaseDate = new OptionalDateTimeContract(album.OriginalReleaseDate);
			ReleaseEvent = album.OriginalReleaseEventName;
			Status = album.Status;
			Version = album.Version;

			if (languagePreference != ContentLanguagePreference.Default) {
				LocalizedName = album.Names.SortNames[languagePreference];				
			}

			if (artists)
				Artists = album.Artists.Select(a => new ArtistForAlbumForApiContract(a, languagePreference)).ToArray();

			if (names)
				Names = album.Names.Select(n => new LocalizedStringContract(n)).ToArray();

			if (pvs)
				PVs = album.PVs.Select(p => new PVContract(p)).ToArray();

			if (tags)
				Tags = album.Tags.Usages.Select(u => new TagUsageForApiContract(u)).ToArray();

			if (webLinks)
				WebLinks = album.WebLinks.Select(w => new WebLinkContract(w)).ToArray();

			if (mergeRecord != null)
				MergedTo = mergeRecord.Target.Id;

		}

		[DataMember(EmitDefaultValue = false)]
		public string AdditionalNames { get; set;}

		[DataMember(EmitDefaultValue = false)]
		public ArtistForAlbumForApiContract[] Artists { get; set; }

		[DataMember]
		public string ArtistString { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Barcode { get; set; }

		[DataMember]
		public string CatalogNumber { get; set; }

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public string DefaultName { get; set; }

		[DataMember]
		public ContentLanguageSelection DefaultNameLanguage { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public DiscType DiscType { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public AlbumIdentifierContract[] Identifiers { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string LocalizedName { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public EntryThumbForApiContract MainPicture { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int MergedTo { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public LocalizedStringContract[] Names { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public PVContract[] PVs { get; set; }

		[DataMember]
		public double RatingAverage { get; set; }

		[DataMember]
		public int RatingCount { get; set; }

		[DataMember]
		public OptionalDateTimeContract ReleaseDate { get; set; }

		[DataMember]
		public string ReleaseEvent { get; set; }

		[DataMember]
		public EntryStatus Status { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public TagUsageForApiContract[] Tags { get; set; }

		[DataMember]
		public int Version { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public WebLinkContract[] WebLinks { get; set; }

	}

	[Flags]
	public enum AlbumOptionalFields {

		None = 0,
		AdditionalNames = 1,
		Artists = 2,
		Identifiers = 4,
		MainPicture = 8,
		Names = 16,
		PVs = 32,
		Tags = 64,
		WebLinks = 128

	}

}
