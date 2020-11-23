using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Service.QueryableExtenders
{

	/// <summary>
	/// Extension methods for <see cref="IQueryable{EntryTypeToTagMapping}"/>.
	/// </summary>
	public static class EntryTypeToTagMappingQueryableExtender
	{

		public static IQueryable<EntryTypeToTagMapping> WhereEntryTypeIs(this IQueryable<EntryTypeToTagMapping> queryable, EntryType entryType)
			=> queryable.Where(etm => etm.EntryType == entryType);

		public static IQueryable<EntryTypeToTagMapping> WhereEntryTypeIs<TSubType>(this IQueryable<EntryTypeToTagMapping> queryable,
			EntryType entryType, IEnumerable<TSubType> subTypes) where TSubType : struct, Enum
		{

			var subTypeStrings = subTypes.Select(s => s.ToString());

			return queryable.Where(etm => etm.EntryType == entryType && subTypeStrings.Contains(etm.SubType));

		}

		public static IQueryable<EntryTypeToTagMapping> WhereHasSubType(this IQueryable<EntryTypeToTagMapping> queryable)
			=> queryable.Where(etm => etm.SubType != "");

	}
}
