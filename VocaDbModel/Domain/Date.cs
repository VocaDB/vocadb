#nullable disable

using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain
{
	/// <summary>
	/// Date object without time or timezone.
	/// Dates are considered equal as long as day, month and year are the same, even if timezones were different.
	/// Null date is allowed.
	/// </summary>
	/// <remarks>
	/// For example, normally midnight 2015/03/09 00:00 in Japan time would be 2015/03/08 in the US because of timezones.
	/// Obviously 2015/03/09 00:00 in Japan time and US time would be different dates, but here we want to handle them the same.
	/// </remarks>
	public struct Date : IComparable<Date>
	{
		public static implicit operator Date(DateTime? dateTime)
		{
			return new Date(dateTime);
		}

		public static implicit operator DateTime?(Date? date)
		{
			return date.HasValue ? date.Value.DateTimeUtc : null;
		}

		private DateTime? dateTime;

		/// <summary>
		/// Initializes Date object based on <see cref="DateTimeUtc"/>.
		/// Only the date portion will be included.
		/// </summary>
		/// <param name="dateTime">DateTime instance. Can be null.</param>
		public Date(DateTime? dateTime) : this()
		{
			DateTimeUtc = dateTime;
		}

		public Date(DateTimeOffset? dateTimeOffset) : this()
		{
			DateTimeUtc = dateTimeOffset.HasValue ? (DateTime?)dateTimeOffset.Value.Date : null;
		}

		public Date(int year, int month, int day) : this(new DateTime(year, month, day)) { }

		/// <summary>
		/// Internal DateTime instance. Can be null.
		/// </summary>
		public DateTime? DateTimeUtc
		{
			get => dateTime;
			set
			{
				// Change Kind to UTC and remove time portion. 
				// It's important to do the SpecifyKind *before* extracting date as we don't want any timezone conversions.
				dateTime = value != null ? (DateTime?)System.DateTime.SpecifyKind(value.Value, DateTimeKind.Utc).Date : null;
			}
		}

		public bool IsEmpty => !DateTimeUtc.HasValue;

		public int CompareTo(Date other)
		{
			return Nullable.Compare(DateTimeUtc, other.DateTimeUtc);
		}

#nullable enable
		public bool Equals(DateTime? anotherDateTime)
		{
			return DateTimeHelper.DateEquals(DateTimeUtc, anotherDateTime);
		}

		public bool Equals(Date? another)
		{
			if (another == null)
				return false;

			return DateTimeHelper.DateEquals(DateTimeUtc, another.Value.DateTimeUtc);
		}

		public bool Equals(Date another)
		{
			return DateTimeHelper.DateEquals(DateTimeUtc, another.DateTimeUtc);
		}

		public override bool Equals(object? obj)
		{
			if (obj is DateTime?)
				return Equals((DateTime?)obj);

			if (obj is Date?)
				return Equals((Date?)obj);

			return false;
		}

		public override int GetHashCode()
		{
			return DateTimeUtc.HasValue ? DateTimeUtc.Value.GetHashCode() : base.GetHashCode();
		}

		public override string? ToString()
		{
			return DateTimeUtc.HasValue ? DateTimeUtc.Value.ToShortDateString() : base.ToString();
		}
#nullable disable
	}
}
