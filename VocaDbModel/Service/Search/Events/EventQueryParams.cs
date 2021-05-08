using System;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtensions;

namespace VocaDb.Model.Service.Search.Events
{
	public sealed record EventQueryParams
	{
		public DateTime? AfterDate { get; init; }
		public DateTime? BeforeDate { get; init; }
		public EventCategory Category { get; init; }
		public bool ChildTags { get; init; }
		public PagingProperties Paging { get; init; } = PagingProperties.Default;
		public int SeriesId { get; init; }
		public SortDirection? SortDirection { get; init; }
		public EventSortRule SortRule { get; init; }
		public EntryStatus? EntryStatus { get; init; }
		public int[]? TagIds { get; init; }
		public SearchTextQuery TextQuery { get; init; } = SearchTextQuery.Empty;
		public int UserId { get; init; }

		public EntryIdsCollection ArtistIds { get; init; }
		public bool ChildVoicebanks { get; init; }
		public bool IncludeMembers { get; init; }
	}
}
