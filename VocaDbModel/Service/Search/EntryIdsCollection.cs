using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace VocaDb.Model.Service.Search
{
	/// <summary>
	/// Collection of integer entry IDs, with various helper methods.
	/// </summary>
	public readonly struct EntryIdsCollection
	{
		public static implicit operator EntryIdsCollection(int[]? ids)
		{
			return new EntryIdsCollection(ids);
		}

		public static EntryIdsCollection CreateWithFallback(int[]? ids, int fallbackId)
		{
			return new EntryIdsCollection(ids != null && ids.Any() ? ids : new[] { fallbackId });
		}

		public EntryIdsCollection(int[]? ids) : this()
		{
			Ids = ids;
		}

		[MemberNotNullWhen(true, nameof(Ids))]
		public bool HasAny => Ids != null && Ids.Any();

		[MemberNotNullWhen(true, nameof(Ids))]
		public bool HasMultiple => Ids != null && Ids.Length > 1;

		[MemberNotNullWhen(true, nameof(Ids))]
		public bool HasSingleId => Ids != null && Ids.Length == 1;

		public int[]? Ids { get; }

		public int Primary => HasSingleId ? Ids[0] : 0;
	}
}
