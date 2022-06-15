namespace VocaDb.Model.Domain
{
	public interface IEntryWithReadOnlyIntId : IDatabaseObject
	{
		int Id { get; }
	}

	/// <summary>
	/// Base interface for database objects with Int32 ID.
	/// Applies to both root entities and child entities.
	/// </summary>
	public interface IEntryWithIntId : IEntryWithReadOnlyIntId
	{
		new int Id { get; set; }

		int IEntryWithReadOnlyIntId.Id => Id;
	}

	/// <summary>
	/// Base interface for database objects with Int64 ID.
	/// Applies to both root entities and child entities.
	/// </summary>
	public interface IEntryWithLongId : IDatabaseObject
	{
		long Id { get; set; }
	}

	public static class IEntryWithReadOnlyIntIdExtensions
	{
		/// <summary>
		/// Compares the Id of this entry with another.
		/// Null values and entries where the Id is undefined (0) will be handled.
		/// </summary>
		/// <param name="left">First entry to be compared. Can be null.</param>
		/// <param name="right">Second entry to be compared. Can be null.</param>
		/// <returns>True if entries on both sides are equal.</returns>
		/// <remarks>
		/// Null values and entries with undefined (0) Id will be considered equal.
		/// That means, 
		/// null + 0 == true
		/// null + null == true
		/// 0 + 0 == true
		/// 39 + 39 == true
		/// null + 39 == false
		/// 0 + 39 == false
		/// </remarks>
		public static bool NullSafeIdEquals(this IEntryWithReadOnlyIntId? left, IEntryWithReadOnlyIntId? right)
		{
			return left.IdOrDefault() == right.IdOrDefault();
		}

		public static bool IdEquals(this IEntryWithReadOnlyIntId left, IEntryWithReadOnlyIntId right)
		{
			return left.Id == right.Id;
		}

		public static int IdOrDefault(this IEntryWithReadOnlyIntId? entry)
		{
			return entry != null ? entry.Id : 0;
		}

		public static bool IsNullOrDefault(this IEntryWithReadOnlyIntId? entry)
		{
			return entry == null || entry.Id == 0;
		}
	}
}
