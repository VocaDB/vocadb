namespace VocaDb.Model.Domain
{
	public interface IEntryWithStatus : IEntryBase
	{
		EntryStatus Status { get; }
	}
}
