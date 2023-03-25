using System.IO.Packaging;
using System.Net.Mime;
using System.Runtime.Serialization;
using NHibernate;
using NLog;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.DataSharing;

public interface IPackageCreator
{
	void Dump<T>(T[] contract, int id, string folder);
}

public sealed class JsonPackageCreator : IPackageCreator
{
	private static readonly Logger s_log = LogManager.GetCurrentClassLogger();
	private readonly Package _package;
	private readonly Action _cleanup;

	public JsonPackageCreator(Package package, Action cleanup)
	{
		_package = package;
		_cleanup = cleanup;
	}

	public void Dump<T>(T[] contract, int id, string folder)
	{
		var partUri = PackUriHelper.CreatePartUri(new Uri($"{folder}{id}.json", UriKind.Relative));

		if (_package.PartExists(partUri))
		{
			s_log.Warn("Duplicate path: {0}", partUri);
			return;
		}

		var packagePart = _package.CreatePart(partUri, MediaTypeNames.Application.Json, CompressionOption.Normal);

		var data = JsonHelper.Serialize(contract);

		using var stream = packagePart.GetStream();
		using var writer = new StreamWriter(stream);
		writer.Write(data);

		// Cleanup
		_cleanup();
		GC.Collect();
	}
}

public sealed class DatabaseDumper
{
	private sealed class Loader
	{
		private const int MaxEntries = 1000;
		private readonly IPackageCreator _packageCreator;
		private readonly ISession _session;

		public Loader(ISession session, IPackageCreator packageCreator)
		{
			_session = session;
			_packageCreator = packageCreator;
		}

		private void DumpChunked<TEntry, TContract>(int[] ids, string folder, Func<TEntry, TContract> fac)
			where TEntry : class, IEntryWithIntId
			where TContract : class/* TODO: , IEntryContract */
		{
			var idChunks = ids.Chunk(MaxEntries);
			foreach (var (chunk, index) in idChunks.Select((chunk, index) => (chunk, index)))
			{
				var contracts = _session.Query<TEntry>()
					.Where(entry => chunk.Contains(entry.Id))
					.Select(fac)
					.ToArray();
				_packageCreator.Dump(contracts, MaxEntries * index, folder);
			}
		}

		public void Dump<TEntry, TContract>(string folder, Func<TEntry, TContract> fac)
			where TEntry : class, IEntryWithIntId
			where TContract : class/* TODO: , IEntryContract */
		{

			var ids = _session.Query<TEntry>().Select(entry => entry.Id).ToArray();
			DumpChunked(ids, folder, fac);
		}

		public void DumpSkipDeleted<TEntry, TContract>(string folder, Func<TEntry, TContract> fac)
			where TEntry : class, IDeletableEntry
			where TContract : class/* TODO: , IEntryContract */
		{

			var ids = _session.Query<TEntry>().Where(entry => !entry.Deleted).Select(entry => entry.Id).ToArray();
			DumpChunked(ids, folder, fac);
		}
	}

	[DataContract(Namespace = Schemas.VocaDb)]
	private sealed record ArchivedTagUsageContract
	{
		[DataMember]
		public int Count { get; init; }

		[DataMember]
		public ObjectRefContract Tag { get; init; }

		public ArchivedTagUsageContract(TagUsage tagUsage)
		{
			Count = tagUsage.Count;
			Tag = ObjectRefContract.Create(tagUsage.Tag);
		}
	}

	[DataContract(Namespace = Schemas.VocaDb)]
	private sealed class ArchivedArtistContractWithTags : ArchivedArtistContract
	{
		[DataMember]
		public ArchivedTagUsageContract[] Tags { get; init; }

		public ArchivedArtistContractWithTags(Artist artist)
			: base(artist, new ArtistDiff())
		{
			MainPictureMime = null;
			Pictures = null;
			Tags = artist.Tags.Usages.Select(tagUsage => new ArchivedTagUsageContract(tagUsage)).ToArray();
		}
	}

	[DataContract(Namespace = Schemas.VocaDb)]
	private sealed class ArchivedAlbumContractWithTags : ArchivedAlbumContract
	{
		[DataMember]
		public ArchivedTagUsageContract[] Tags { get; init; }

		public ArchivedAlbumContractWithTags(Album album)
			: base(album, new AlbumDiff())
		{
			MainPictureMime = null;
			Pictures = null;
			PVs = PVs?.Where(pv => pv.PVType == Domain.PVs.PVType.Original).ToArray();
			Tags = album.Tags.Usages.Select(tagUsage => new ArchivedTagUsageContract(tagUsage)).ToArray();
		}
	}

	[DataContract(Namespace = Schemas.VocaDb)]
	private sealed class ArchivedSongContractWithTags : ArchivedSongContract
	{
		[DataMember]
		public ArchivedTagUsageContract[] Tags { get; init; }

		public ArchivedSongContractWithTags(Song song)
			: base(song, new SongDiff())
		{
			Lyrics = null;
			PVs = PVs?.Where(pv => pv.PVType == Domain.PVs.PVType.Original).ToArray();
			Tags = song.Tags.Usages.Select(tagUsage => new ArchivedTagUsageContract(tagUsage)).ToArray();
		}
	}

	[DataContract(Namespace = Schemas.VocaDb)]
	private sealed class ArchivedEventSeriesContractWithTags : ArchivedEventSeriesContract
	{
		[DataMember]
		public ArchivedTagUsageContract[] Tags { get; init; }

		public ArchivedEventSeriesContractWithTags(ReleaseEventSeries releaseEventSeries)
			: base(releaseEventSeries, new ReleaseEventSeriesDiff())
		{
			MainPictureMime = null;
			Tags = releaseEventSeries.Tags.Usages.Select(tagUsage => new ArchivedTagUsageContract(tagUsage)).ToArray();
		}
	}

	[DataContract(Namespace = Schemas.VocaDb)]
	private sealed class ArchivedEventContractWithTags : ArchivedEventContract
	{
		[DataMember]
		public ArchivedTagUsageContract[] Tags { get; init; }

		public ArchivedEventContractWithTags(ReleaseEvent releaseEvent)
			: base(releaseEvent, new ReleaseEventDiff())
		{
			MainPictureMime = null;
			PVs = PVs?.Where(pv => pv.PVType == Domain.PVs.PVType.Original).ToArray();
			Tags = releaseEvent.Tags.Usages.Select(tagUsage => new ArchivedTagUsageContract(tagUsage)).ToArray();
		}
	}

	public void Create(string path, ISession session)
	{
		using var package = Package.Open(path, FileMode.Create);
		var packageCreator = new JsonPackageCreator(package, session.Clear);
		var loader = new Loader(session, packageCreator);
		loader.DumpSkipDeleted<Artist, ArchivedArtistContract>("/Artists/", a => new ArchivedArtistContractWithTags(a));
		loader.DumpSkipDeleted<Album, ArchivedAlbumContract>("/Albums/", a => new ArchivedAlbumContractWithTags(a));
		loader.DumpSkipDeleted<Song, ArchivedSongContract>("/Songs/", a => new ArchivedSongContractWithTags(a));
		loader.Dump<ReleaseEventSeries, ArchivedEventSeriesContract>("/EventSeries/", a => new ArchivedEventSeriesContractWithTags(a));
		loader.Dump<ReleaseEvent, ArchivedEventContract>("/Events/", a => new ArchivedEventContractWithTags(a));
		loader.DumpSkipDeleted<Tag, ArchivedTagContract>("/Tags/", a => new ArchivedTagContract(a, new TagDiff()));
	}
}
