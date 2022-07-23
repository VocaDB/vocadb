using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.SongLists;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record SongListForEditForApiContract
{
	[DataMember]
	public bool Deleted { get; init; }

	[DataMember]
	public string Description { get; init; }

	[DataMember]
	public DateTime? EventDate { get; init; }

	[DataMember]
	[JsonConverter(typeof(StringEnumConverter))]
	public SongListFeaturedCategory FeaturedCategory { get; init; }

	[DataMember]
	public int Id { get; set; }

	[DataMember(EmitDefaultValue = false)]
	public EntryThumbForApiContract? MainPicture { get; init; }

	[DataMember]
	public string Name { get; init; }

	[DataMember]
	public SongInListEditContract[] SongLinks { get; set; }

	[DataMember]
	public EntryStatus Status { get; init; }

	[DataMember]
	public string UpdateNotes { get; init; }

	public SongListForEditForApiContract()
	{
		Description = string.Empty;
		Name = string.Empty;
		SongLinks = Array.Empty<SongInListEditContract>();
		UpdateNotes = string.Empty;
	}

	public SongListForEditForApiContract(
		SongList songList,
		IUserPermissionContext permissionContext,
		IAggregatedEntryImageUrlFactory imagePersister
	)
	{
		Deleted = songList.Deleted;
		Description = songList.Description;
		EventDate = songList.EventDate;
		FeaturedCategory = songList.FeaturedCategory;
		Id = songList.Id;
		MainPicture = songList.Thumb is not null
			? new EntryThumbForApiContract(songList.Thumb, imagePersister)
			: null;
		Name = songList.Name;
		SongLinks = songList.SongLinks
			.OrderBy(s => s.Order)
			.Select(s => new SongInListEditContract(s, permissionContext.LanguagePreference))
			.ToArray();
		Status = songList.Status;
		UpdateNotes = string.Empty;
	}
}
