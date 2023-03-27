using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.Venues;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.DataContracts.Versioning;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record ComparedVersionsForApiContract<T> where T : class
{
	[DataMember]
	public int FirstId { get; init; }

	[DataMember]
	public T FirstData { get; init; }

	[DataMember]
	public int SecondId { get; init; }

	[DataMember]
	public T? SecondData { get; init; }

	public ComparedVersionsForApiContract((int Id, T Data) first, (int Id, T Data)? second)
	{
		FirstId = first.Id;
		FirstData = first.Data;
		SecondId = second?.Id ?? 0;
		SecondData = second?.Data;
	}

	public static ComparedVersionsForApiContract<T> Create<TSource>(
		TSource firstData,
		TSource? secondData,
		Func<TSource, T> dataGetter, Func<TSource, int> idGetter
	) where TSource : class
	{
		return new ComparedVersionsForApiContract<T>(
			first: (Id: idGetter(firstData), Data: dataGetter(firstData)),
			second: secondData is not null ? (Id: idGetter(secondData), Data: dataGetter(secondData)) : null
		);
	}
}

public static class ComparedVersionsForApiContract
{
	public static ComparedVersionsForApiContract<ArchivedAlbumForApiContract> FromAlbum(
		ArchivedAlbumVersion firstData,
		ArchivedAlbumVersion? secondData,
		IUserPermissionContext permissionContext
	)
	{
		return ComparedVersionsForApiContract<ArchivedAlbumForApiContract>.Create(
			firstData,
			secondData,
			version => ArchivedAlbumForApiContract.Create(ArchivedAlbumContract.GetAllProperties(version), permissionContext),
			d => d.Id
		);
	}

	public static ComparedVersionsForApiContract<ArchivedArtistForApiContract> FromArtist(
		ArchivedArtistVersion firstData,
		ArchivedArtistVersion? secondData,
		IUserPermissionContext permissionContext
	)
	{
		return ComparedVersionsForApiContract<ArchivedArtistForApiContract>.Create(
			firstData,
			secondData,
			version => ArchivedArtistForApiContract.Create(ArchivedArtistContract.GetAllProperties(version), permissionContext),
			d => d.Id
		);
	}

	public static ComparedVersionsForApiContract<ArchivedEventForApiContract> FromReleaseEvent(
		ArchivedReleaseEventVersion firstData,
		ArchivedReleaseEventVersion? secondData,
		IUserPermissionContext permissionContext
	)
	{
		return ComparedVersionsForApiContract<ArchivedEventForApiContract>.Create(
			firstData,
			secondData,
			version => ArchivedEventForApiContract.Create(ArchivedEventContract.GetAllProperties(version), permissionContext),
			d => d.Id
		);
	}

	public static ComparedVersionsForApiContract<ArchivedEventSeriesForApiContract> FromReleaseEventSeries(
		ArchivedReleaseEventSeriesVersion firstData,
		ArchivedReleaseEventSeriesVersion? secondData,
		IUserPermissionContext permissionContext
	)
	{
		return ComparedVersionsForApiContract<ArchivedEventSeriesForApiContract>.Create(
			firstData,
			secondData,
			version => ArchivedEventSeriesForApiContract.Create(ArchivedEventSeriesContract.GetAllProperties(version), permissionContext),
			d => d.Id
		);
	}

	public static ComparedVersionsForApiContract<ArchivedSongForApiContract> FromSong(
		ArchivedSongVersion firstData,
		ArchivedSongVersion? secondData,
		IUserPermissionContext permissionContext
	)
	{
		return ComparedVersionsForApiContract<ArchivedSongForApiContract>.Create(
			firstData,
			secondData,
			version => ArchivedSongForApiContract.Create(ArchivedSongContract.GetAllProperties(version), permissionContext),
			d => d.Id
		);
	}

	public static ComparedVersionsForApiContract<ArchivedTagForApiContract> FromTag(
		ArchivedTagVersion firstData,
		ArchivedTagVersion? secondData,
		IUserPermissionContext permissionContext
	)
	{
		return ComparedVersionsForApiContract<ArchivedTagForApiContract>.Create(
			firstData,
			secondData,
			version => ArchivedTagForApiContract.Create(ArchivedTagContract.GetAllProperties(version), permissionContext),
			d => d.Id
		);
	}

	public static ComparedVersionsForApiContract<ArchivedVenueForApiContract> FromVenue(
		ArchivedVenueVersion firstData,
		ArchivedVenueVersion? secondData,
		IUserPermissionContext permissionContext
	)
	{
		return ComparedVersionsForApiContract<ArchivedVenueForApiContract>.Create(
			firstData,
			secondData,
			version => ArchivedVenueForApiContract.Create(ArchivedVenueContract.GetAllProperties(version), permissionContext),
			d => d.Id
		);
	}
}
