namespace VocaDb.Model.Domain
{
	public interface IDeletableEntry : IEntryWithIntId
	{
		bool Deleted { get; }
	}
}
