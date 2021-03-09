#nullable disable

using System;

namespace VocaDb.Model.DataContracts.Aggregate
{
	public class CountPerDayContract
	{
		public CountPerDayContract()
		{
			Day = 1;
		}

		public CountPerDayContract(int year, int month, int day, int count)
		{
			Year = year;
			Month = month;
			Day = day;
			Count = count;
		}

		public CountPerDayContract(DateTime dt, int count)
		{
			Year = dt.Year;
			Month = dt.Month;
			Day = dt.Day;
			Count = count;
		}

		public int Year { get; init; }
		public int Month { get; init; }
		public int Day { get; init; }
		public int Count { get; init; }

		public DateTime ToDateTime()
		{
			return new DateTime(Year, Month, Day);
		}
	}
}