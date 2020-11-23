namespace VocaDb.Model.Service
{
	public enum EntrySortRule
	{
		None,
		Name,
		AdditionDate,
		/// <summary>
		/// Activity date depends on entry type.
		/// For songs and albums this is the release date. For events it's the event date.
		/// </summary>
		ActivityDate
	}
}
