namespace VocaDb.Model.Domain
{

	/// <summary>
	/// Base interface for database objects with Int32 ID.
	/// Applies to both root entities and child entities.
	/// </summary>
	public interface IEntryWithIntId : IDatabaseObject
	{
		int Id { get; set; }
	}

	/// <summary>
	/// Base interface for database objects with Int64 ID.
	/// Applies to both root entities and child entities.
	/// </summary>
	public interface IEntryWithLongId : IDatabaseObject
	{
		long Id { get; set; }
	}

	public static class IEntryWithIntIdExtender
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
		public static bool NullSafeIdEquals(this IEntryWithIntId left, IEntryWithIntId right)
		{

			return left.IdOrDefault() == right.IdOrDefault();

		}

		public static bool IdEquals(this IEntryWithIntId left, IEntryWithIntId right)
		{
			return left.Id == right.Id;
		}

		public static int IdOrDefault(this IEntryWithIntId entry)
		{
			return entry != null ? entry.Id : 0;
		}

		public static bool IsNullOrDefault(this IEntryWithIntId entry)
		{
			return entry == null || entry.Id == 0;
		}

	}

}
