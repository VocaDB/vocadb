using System;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.DataContracts.Albums
{

	[DataContract(Namespace = Schemas.VocaDb)]
	public class AlbumForApiContract : IEntryBase
	{

		EntryType IEntryBase.EntryType => EntryType.Album;

		public AlbumForApiContract() { }

		public AlbumForApiContract(
			Album album,
			ContentLanguagePreference languagePreference,
			IAggregatedEntryImageUrlFactory thumbPersister,
			AlbumOptionalFields fields,
			SongOptionalFields songFields = SongOptionalFields.None) :
			this(album, null, languagePreference, thumbPersister, fields, songFields)
		{ }

		public AlbumForApiContract(
			Album album, AlbumMergeRecord mergeRecord,
			ContentLanguagePreference languagePreference,
			IAggregatedEntryImageUrlFactory thumbPersister,
			AlbumOptionalFields fields,
			SongOptionalFields songFields)
		{

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
			Status = album.Status;
			Version = album.Version;

			if (fields.HasFlag(AlbumOptionalFields.AdditionalNames))
			{
				AdditionalNames = album.Names.GetAdditionalNamesStringForLanguage(languagePreference);
			}

			if (fields.HasFlag(AlbumOptionalFields.Artists))
			{
				Artists = album.Artists.Select(a => new ArtistForAlbumForApiContract(a, languagePreference)).ToArray();
			}

			if (fields.HasFlag(AlbumOptionalFields.Description))
			{
				Description = album.Description[languagePreference];
			}

			if (fields.HasFlag(AlbumOptionalFields.Discs))
			{
				Discs = album.Discs.Select(d => new AlbumDiscPropertiesContract(d)).ToArray();
			}

			if (fields.HasFlag(AlbumOptionalFields.Identifiers))
			{
				Identifiers = album.Identifiers.Select(i => new AlbumIdentifierContract(i)).ToArray();
			}

			if (thumbPersister != null && fields.HasFlag(AlbumOptionalFields.MainPicture) && album.Thumb != null)
			{
				MainPicture = new EntryThumbForApiContract(album.Thumb, thumbPersister);
			}

			if (fields.HasFlag(AlbumOptionalFields.Names))
			{
				Names = album.Names.Select(n => new LocalizedStringContract(n)).ToArray();
			}

			if (fields.HasFlag(AlbumOptionalFields.PVs))
			{
				PVs = album.PVs.Select(p => new PVContract(p)).ToArray();
			}

			if (fields.HasFlag(AlbumOptionalFields.ReleaseEvent))
			{
				ReleaseEvent = album.OriginalReleaseEvent != null ? new ReleaseEventForApiContract(album.OriginalReleaseEvent, languagePreference, ReleaseEventOptionalFields.None, thumbPersister) : null;
			}

			if (fields.HasFlag(AlbumOptionalFields.Tags))
			{
				Tags = album.Tags.ActiveUsages.Select(u => new TagUsageForApiContract(u, languagePreference)).ToArray();
			}

			if (fields.HasFlag(AlbumOptionalFields.Tracks))
			{
				Tracks = album.Songs.Select(s => new SongInAlbumForApiContract(s, languagePreference, songFields)).ToArray();
			}

			if (fields.HasFlag(AlbumOptionalFields.WebLinks))
			{
				WebLinks = album.WebLinks.Select(w => new WebLinkForApiContract(w)).ToArray();
			}

			if (mergeRecord != null)
				MergedTo = mergeRecord.Target.Id;

		}

		/// <summary>
		/// Comma-separated list of all other names that aren't the display name.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string AdditionalNames { get; set; }

		/// <summary>
		/// List of artists for this song. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public ArtistForAlbumForApiContract[] Artists { get; set; }

		/// <summary>
		/// Artist string, for example "Tripshots feat. Hatsune Miku".
		/// </summary>
		[DataMember]
		public string ArtistString { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Barcode { get; set; }

		[DataMember]
		public string CatalogNumber { get; set; }

		/// <summary>
		/// Date this entry was created.
		/// </summary>
		[DataMember]
		public DateTime CreateDate { get; set; }

		/// <summary>
		/// Name in default language.
		/// </summary>
		[DataMember]
		public string DefaultName { get; set; }

		/// <summary>
		/// Language selection of the original name.
		/// </summary>
		[DataMember]
		public ContentLanguageSelection DefaultNameLanguage { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool Deleted { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Description { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public AlbumDiscPropertiesContract[] Discs { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public DiscType DiscType { get; set; }

		[DataMember]
		public int Id { get; set; }

		/// <summary>
		/// List of album identifiers such as barcodes. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public AlbumIdentifierContract[] Identifiers { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public EntryThumbForApiContract MainPicture { get; set; }

		/// <summary>
		/// Id of the entry this entry was merged to, if any.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public int MergedTo { get; set; }

		/// <summary>
		/// Display name (primary name in selected language, or default language).
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// List of all names for this entry. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public LocalizedStringContract[] Names { get; set; }

		/// <summary>
		/// List of PVs. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		[JsonProperty("pvs")]
		public PVContract[] PVs { get; set; }

		/// <summary>
		/// Average of all user ratings (1-5).
		/// </summary>
		[DataMember]
		public double RatingAverage { get; set; }

		/// <summary>
		/// Number of users who have rated this album.
		/// </summary>
		[DataMember]
		public int RatingCount { get; set; }

		/// <summary>
		/// Date when this album was released. May be only the year, or year, month and day.
		/// </summary>
		[DataMember]
		public OptionalDateTimeContract ReleaseDate { get; set; }

		/// <summary>
		/// Event where this album was first distributed.
		/// </summary>
		[DataMember]
		public ReleaseEventForApiContract ReleaseEvent { get; set; }

		[DataMember]
		public EntryStatus Status { get; set; }

		/// <summary>
		/// List of tags. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public TagUsageForApiContract[] Tags { get; set; }

		/// <summary>
		/// List of tracks (songs). Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public SongInAlbumForApiContract[] Tracks { get; set; }

		[DataMember]
		public int Version { get; set; }

		/// <summary>
		/// List of external links. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public WebLinkForApiContract[] WebLinks { get; set; }

	}

	[Flags]
	public enum AlbumOptionalFields
	{

		None = 0,
		AdditionalNames = 1,
		Artists = 2,
		Description = 4,
		Discs = 8,
		Identifiers = 16,
		MainPicture = 32,
		Names = 64,
		PVs = 128,
		ReleaseEvent = 256,
		Tags = 512,
		Tracks = 1024,
		WebLinks = 2048

	}

}
