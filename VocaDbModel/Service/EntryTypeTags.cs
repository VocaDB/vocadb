using System;
using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Service {

	public interface IEntryTypeTags {
		int Cover { get; }
		int Instrumental { get; }
		int Remix { get; }
		int SongTypeTag(SongType songType);
	}

	public class EntryTypeTags : IEntryTypeTags {

		public EntryTypeTags(IDatabaseContext ctx) {
			this.ctx = ctx;
		}

		private int GetTagId<TSubType>(EntryType entryType, TSubType subType) where TSubType : Enum {
			return ctx.Query<EntryTypeToTagMapping>()
				.Where(etm => etm.EntryType == entryType && etm.SubType == subType.ToString())
				.Select(etm => etm.Tag.Id)
				.FirstOrDefault();
		}

		private readonly IDatabaseContext ctx;

		public int Cover => SongTypeTag(SongType.Cover);
		public int Instrumental => SongTypeTag(SongType.Instrumental);
		public int Remix => SongTypeTag(SongType.Remix);
		public int SongTypeTag(SongType songType) => GetTagId(EntryType.Song, songType);

	}
}
