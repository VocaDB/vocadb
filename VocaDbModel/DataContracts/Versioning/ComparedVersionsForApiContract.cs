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
	public static ComparedVersionsForApiContract<ArchivedAlbumContract> FromAlbum(ArchivedAlbumVersion firstData, ArchivedAlbumVersion? secondData)
	{
		return ComparedVersionsForApiContract<ArchivedAlbumContract>.Create(firstData, secondData, ArchivedAlbumContract.GetAllProperties, d => d.Id);
	}

	public static ComparedVersionsForApiContract<ArchivedArtistContract> FromArtist(ArchivedArtistVersion firstData, ArchivedArtistVersion? secondData)
	{
		return ComparedVersionsForApiContract<ArchivedArtistContract>.Create(firstData, secondData, ArchivedArtistContract.GetAllProperties, d => d.Id);
	}

	public static ComparedVersionsForApiContract<ArchivedEventContract> FromReleaseEvent(ArchivedReleaseEventVersion firstData, ArchivedReleaseEventVersion? secondData)
	{
		return ComparedVersionsForApiContract<ArchivedEventContract>.Create(firstData, secondData, ArchivedEventContract.GetAllProperties, d => d.Id);
	}

	public static ComparedVersionsForApiContract<ArchivedEventSeriesContract> FromReleaseEventSeries(ArchivedReleaseEventSeriesVersion firstData, ArchivedReleaseEventSeriesVersion? secondData)
	{
		return ComparedVersionsForApiContract<ArchivedEventSeriesContract>.Create(firstData, secondData, ArchivedEventSeriesContract.GetAllProperties, d => d.Id);
	}

	public static ComparedVersionsForApiContract<ArchivedSongContract> FromSong(ArchivedSongVersion firstData, ArchivedSongVersion? secondData)
	{
		return ComparedVersionsForApiContract<ArchivedSongContract>.Create(firstData, secondData, ArchivedSongContract.GetAllProperties, d => d.Id);
	}

	public static ComparedVersionsForApiContract<ArchivedTagContract> FromTag(ArchivedTagVersion firstData, ArchivedTagVersion? secondData)
	{
		return ComparedVersionsForApiContract<ArchivedTagContract>.Create(firstData, secondData, ArchivedTagContract.GetAllProperties, d => d.Id);
	}

	public static ComparedVersionsForApiContract<ArchivedVenueContract> FromVenue(ArchivedVenueVersion firstData, ArchivedVenueVersion? secondData)
	{
		return ComparedVersionsForApiContract<ArchivedVenueContract>.Create(firstData, secondData, ArchivedVenueContract.GetAllProperties, d => d.Id);
	}
}
