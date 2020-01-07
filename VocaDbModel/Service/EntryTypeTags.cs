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
		Tag GetTag<TSubType>(EntryType entryType, TSubType subType) where TSubType : Enum;
		int SongTypeTagId(SongType songType);
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

		public int Cover => SongTypeTagId(SongType.Cover);
		public int Instrumental => SongTypeTagId(SongType.Instrumental);
		public int Remix => SongTypeTagId(SongType.Remix);
		public Tag GetTag<TSubType>(EntryType entryType, TSubType subType) where TSubType : Enum => ctx.NullSafeLoad<Tag>(GetTagId(entryType, subType));
		public int SongTypeTagId(SongType songType) => GetTagId(EntryType.Song, songType);
	}
}
