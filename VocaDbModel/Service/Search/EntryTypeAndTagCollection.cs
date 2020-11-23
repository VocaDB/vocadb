using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service.QueryableExtenders;

namespace VocaDb.Model.Service.Search
{

	/// <summary>
	/// Collection of entry sub-types and associated tags.
	/// </summary>
	/// <typeparam name="TSubType">Entry sub-type type. For example SongType.</typeparam>
	public class EntryTypeAndTagCollection<TSubType> where TSubType : struct, Enum
	{

		public static EntryTypeAndTagCollection<TSubType> Create(
			EntryType entryType, int tagId, IDatabaseContext ctx, bool allowAllTags = false)
		{
			return Create(entryType, new TSubType[0], new int[] { tagId }, ctx, allowAllTags);
		}

		public static EntryTypeAndTagCollection<TSubType> Create(
			EntryType entryType, IReadOnlyCollection<TSubType> subTypes,
			IReadOnlyCollection<int> tagIds, IDatabaseContext ctx, bool allowAllTags = false)
		{

			TSubType[] allTypes;
			int[] songTypeTagIds;
			int[] allTagIds;

			if (tagIds.Any())
			{

				var songTypesAndTagsFromTags = ctx.Query<EntryTypeToTagMapping>()
					.WhereEntryTypeIs(entryType)
					.WhereHasSubType()
					.Where(etm => tagIds.Contains(etm.Tag.Id))
					.Select(etm => new { TagId = etm.Tag.Id, etm.SubType })
					.ToArray();

				songTypeTagIds = songTypesAndTagsFromTags.Select(etm => etm.TagId).ToArray();
				var songTypesFromTags = songTypesAndTagsFromTags.Select(etm => EnumVal<TSubType>.Parse(etm.SubType));

				allTypes = subTypes.Union(songTypesFromTags).ToArray();

			}
			else
			{
				allTypes = subTypes.ToArray();
				songTypeTagIds = new int[0];
			}

			if (subTypes.Any())
			{
				var tagsFromSongTypes = ctx.Query<EntryTypeToTagMapping>()
					.WhereEntryTypeIs(entryType, subTypes)
					.Select(etm => etm.Tag.Id)
					.ToArray();
				allTagIds = songTypeTagIds.Union(tagsFromSongTypes).ToArray();
			}
			else
			{
				allTagIds = songTypeTagIds;
			}

			if (allowAllTags)
				allTagIds = allTagIds.Union(tagIds).ToArray();

			return new EntryTypeAndTagCollection<TSubType>(allTypes, allTagIds);

		}

		public EntryTypeAndTagCollection(TSubType[] subTypes, int[] tagIds)
		{
			SubTypes = subTypes;
			TagIds = tagIds;
		}

		public bool IsEmpty => !SubTypes.Any() && !TagIds.Any();

		public TSubType[] SubTypes { get; }

		public int[] TagIds { get; }

	}

}
