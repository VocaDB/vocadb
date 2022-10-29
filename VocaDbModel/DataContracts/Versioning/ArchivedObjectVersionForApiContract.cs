using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Venues;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.DataContracts.Versioning;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record ArchivedObjectVersionForApiContract
{
	[DataMember]
	public string AgentName { get; init; }

	[DataMember]
	public bool AnythingChanged { get; init; }

	[DataMember]
	public UserForApiContract? Author { get; init; }

	[DataMember]
	public string[] ChangedFields { get; init; }

	[DataMember]
	public DateTime Created { get; init; }

	[DataMember]
	public bool Hidden { get; init; }

	[DataMember]
	public int Id { get; init; }

	[DataMember]
	public bool IsSnapshot { get; init; }

	[DataMember]
	public string Notes { get; init; }

	[DataMember]
	public string Reason { get; init; }

	[DataMember]
	public EntryStatus Status { get; init; }

	[DataMember]
	public int Version { get; init; }

#nullable disable
	public ArchivedObjectVersionForApiContract() { }
#nullable enable

	public ArchivedObjectVersionForApiContract(ArchivedObjectVersion archivedObjectVersion, bool anythingChanged, string reason, IUserIconFactory userIconFactory)
	{
		ParamIs.NotNull(() => archivedObjectVersion);

		AgentName = !string.IsNullOrEmpty(archivedObjectVersion.AgentName) || archivedObjectVersion.Author is null
			? archivedObjectVersion.AgentName
			: archivedObjectVersion.Author.Name;
		AnythingChanged = anythingChanged;
		Author = archivedObjectVersion.Author is not null ?
			new UserForApiContract(archivedObjectVersion.Author, userIconFactory, UserOptionalFields.MainPicture)
			: null;
		ChangedFields = archivedObjectVersion.DiffBase.ChangedFieldNames;
		Created = archivedObjectVersion.CreatedUtc;
		Hidden = archivedObjectVersion.Hidden;
		Id = archivedObjectVersion.Id;
		IsSnapshot = archivedObjectVersion.DiffBase.IsSnapshot;
		Notes = archivedObjectVersion.Notes;
		Reason = reason;
		Status = archivedObjectVersion.Status;
		Version = archivedObjectVersion.Version;
	}

	public static ArchivedObjectVersionForApiContract FromAlbum(ArchivedAlbumVersion archived, IUserIconFactory userIconFactory)
	{
		return new(
			archivedObjectVersion: archived,
			anythingChanged: archived.Reason != AlbumArchiveReason.PropertiesUpdated || archived.Diff.ChangedFields.Value != AlbumEditableFields.Nothing,
			reason: archived.Reason.ToString(),
			userIconFactory: userIconFactory
		);
	}

	public static ArchivedObjectVersionForApiContract FromArtist(ArchivedArtistVersion archived, IUserIconFactory userIconFactory)
	{
		return new(
			archivedObjectVersion: archived,
			anythingChanged: archived.Reason != ArtistArchiveReason.PropertiesUpdated || archived.Diff.ChangedFields.Value != ArtistEditableFields.Nothing,
			reason: archived.Reason.ToString(),
			userIconFactory: userIconFactory
		);
	}

	public static ArchivedObjectVersionForApiContract FromReleaseEvent(ArchivedReleaseEventVersion archived, IUserIconFactory userIconFactory)
	{
		return new(
			archivedObjectVersion: archived,
			anythingChanged: !Equals(archived.Diff.ChangedFields, default(ReleaseEventEditableFields)) || !Equals(archived.CommonEditEvent, default(EntryEditEvent)),
			reason: archived.CommonEditEvent.ToString(),
			userIconFactory: userIconFactory
		);
	}

	public static ArchivedObjectVersionForApiContract FromReleaseEventSeries(ArchivedReleaseEventSeriesVersion archived, IUserIconFactory userIconFactory)
	{
		return new(
			archivedObjectVersion: archived,
			anythingChanged: !Equals(archived.Diff.ChangedFields, default(ReleaseEventSeriesEditableFields)) || !Equals(archived.CommonEditEvent, default(EntryEditEvent)),
			reason: archived.CommonEditEvent.ToString(),
			userIconFactory: userIconFactory
		);
	}

	public static ArchivedObjectVersionForApiContract FromSong(ArchivedSongVersion archived, IUserIconFactory userIconFactory)
	{
		return new(
			archivedObjectVersion: archived,
			anythingChanged: archived.Reason != SongArchiveReason.PropertiesUpdated || archived.Diff.ChangedFields.Value != SongEditableFields.Nothing,
			reason: archived.Reason.ToString(),
			userIconFactory: userIconFactory
		);
	}

	public static ArchivedObjectVersionForApiContract FromSongList(ArchivedSongListVersion archived, IUserIconFactory userIconFactory)
	{
		return new(
			archivedObjectVersion: archived,
			anythingChanged: true,
			reason: archived.CommonEditEvent.ToString(),
			userIconFactory: userIconFactory
		);
	}

	public static ArchivedObjectVersionForApiContract FromTag(ArchivedTagVersion archived, IUserIconFactory userIconFactory)
	{
		return new(
			archivedObjectVersion: archived,
			anythingChanged: !Equals(archived.Diff.ChangedFields, default(TagEditableFields)) || !Equals(archived.CommonEditEvent, default(EntryEditEvent)),
			reason: archived.CommonEditEvent.ToString(),
			userIconFactory: userIconFactory
		);
	}

	public static ArchivedObjectVersionForApiContract FromVenue(ArchivedVenueVersion archived, IUserIconFactory userIconFactory)
	{
		return new(
			archivedObjectVersion: archived,
			anythingChanged: !Equals(archived.Diff.ChangedFields, default(VenueEditableFields)) || !Equals(archived.CommonEditEvent, default(EntryEditEvent)),
			reason: archived.CommonEditEvent.ToString(),
			userIconFactory: userIconFactory
		);
	}
}
