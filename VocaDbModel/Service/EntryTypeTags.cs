using System;
using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Service
{

	/// <summary>
	/// Loads tags for entry types (and sub-types).
	/// </summary>
	public interface IEntryTypeTagRepository
	{
		int Cover { get; }
		int Instrumental { get; }
		int Remix { get; }
		Tag GetTag<TSubType>(EntryType entryType, TSubType subType) where TSubType : Enum;
		Tag GetTag(EntryTypeAndSubType fullEntryType);
		int SongTypeTagId(SongType songType);
	}

	public class EntryTypeTags : IEntryTypeTagRepository
	{

		public EntryTypeTags(IDatabaseContext ctx)
		{
			this.ctx = ctx;
		}

		private int GetTagId(EntryType entryType, string subType)
		{
			return ctx.Query<EntryTypeToTagMapping>()
				.Where(etm => etm.EntryType == entryType && etm.SubType == subType)
				.Select(etm => etm.Tag.Id)
				.FirstOrDefault();
		}

		private readonly IDatabaseContext ctx;

		public int Cover => SongTypeTagId(SongType.Cover);
		public int Instrumental => SongTypeTagId(SongType.Instrumental);
		public int Remix => SongTypeTagId(SongType.Remix);
		public Tag GetTag<TSubType>(EntryType entryType, TSubType subType) where TSubType : Enum => ctx.NullSafeLoad<Tag>(GetTagId(entryType, subType.ToString()));
		public Tag GetTag(EntryTypeAndSubType fullEntryType) => ctx.NullSafeLoad<Tag>(GetTagId(fullEntryType.EntryType, fullEntryType.SubType));
		public int SongTypeTagId(SongType songType) => GetTagId(EntryType.Song, songType.ToString());

	}
}
