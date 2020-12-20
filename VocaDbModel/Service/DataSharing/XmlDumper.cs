#nullable disable

using System;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using NLog;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Songs;
using System.IO.Packaging;
using System.IO;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Helpers;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.Service.DataSharing
{
	public class XmlDumper
	{
		public class Loader
		{
			private const int MaxEntries = 100;
			private readonly PackageCreator _packageCreator;
			private readonly ISession _session;


			private TEntry[] LoadSkipDeleted<TEntry>(ISession session, int first, int max) where TEntry : IDeletableEntry
			{
				return session.Query<TEntry>().Where(a => !a.Deleted).Skip(first).Take(max).ToArray();
			}

			private TEntry[] Load<TEntry>(ISession session, int first, int max) where TEntry : IEntryWithIntId
			{
				return session.Query<TEntry>().Skip(first).Take(max).ToArray();
			}

			public Loader(ISession session, Package package)
			{
				_session = session;
				_packageCreator = new PackageCreator(package, session.Clear);
			}

			public void DumpSkipDeleted<TEntry, TContract>(string folder, Func<TEntry, TContract> fac) where TEntry : IDeletableEntry
			{
				_packageCreator.Dump(start => LoadSkipDeleted<TEntry>(_session, start, MaxEntries), folder, fac);
			}

			public void Dump<TEntry, TContract>(string folder, Func<TEntry, TContract> fac) where TEntry : IDeletableEntry
			{
				_packageCreator.Dump(start => Load<TEntry>(_session, start, MaxEntries), folder, fac);
			}
		}

		public void Create(string path, ISession session)
		{
			using (var package = Package.Open(path, FileMode.Create))
			{
				var loader = new Loader(session, package);
				loader.DumpSkipDeleted<Artist, ArchivedArtistContract>("/Artists/", a => new ArchivedArtistContract(a, new ArtistDiff()));
				loader.DumpSkipDeleted<Album, ArchivedAlbumContract>("/Albums/", a => new ArchivedAlbumContract(a, new AlbumDiff()));
				loader.DumpSkipDeleted<Song, ArchivedSongContract>("/Songs/", a => new ArchivedSongContract(a, new SongDiff()));
				loader.Dump<ReleaseEventSeries, ArchivedEventSeriesContract>("/EventSeries/", a => new ArchivedEventSeriesContract(a, new ReleaseEventSeriesDiff()));
				loader.Dump<ReleaseEvent, ArchivedEventContract>("/Events/", a => new ArchivedEventContract(a, new ReleaseEventDiff()));
				loader.DumpSkipDeleted<Tag, ArchivedTagContract>("/Tags/", a => new ArchivedTagContract(a, new TagDiff()));
			}
		}
	}

	public class PackageCreator
	{
		private static readonly Logger s_log = LogManager.GetCurrentClassLogger();
		private readonly Package _package;
		private readonly Action _cleanup;

		public PackageCreator(Package package, Action cleanup)
		{
			_package = package;
			_cleanup = cleanup;
		}

		public void Dump<TEntry, TContract>(Func<int, TEntry[]> loadFunc, string folder, Func<TEntry, TContract> fac)
		{
			bool run = true;
			int start = 0;

			while (run)
			{
				var entries = loadFunc(start);
				var contracts = entries.Select(fac).ToArray();
				DumpXml(contracts, start, folder);

				start += contracts.Length;
				run = entries.Any();

				// Cleanup
				_cleanup();
				GC.Collect();
			}
		}

		private void DumpXml<T>(T[] contract, int id, string folder)
		{
			var partUri = PackUriHelper.CreatePartUri(new Uri($"{folder}{id}.xml", UriKind.Relative));

			if (_package.PartExists(partUri))
			{
				s_log.Warn("Duplicate path: {0}", partUri);
				return;
			}

			var packagePart = _package.CreatePart(partUri, System.Net.Mime.MediaTypeNames.Text.Xml, CompressionOption.Normal);

			var data = XmlHelper.SerializeToXml(contract);

			using (var stream = packagePart.GetStream())
			{
				data.Save(stream);
			}
		}
	}
}
