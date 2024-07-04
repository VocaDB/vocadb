namespace VocaDb.Model.Domain.Tags;

public interface IEntryWithTags : IEntryBase
{
	/// <summary>
	/// If this is set to false, notifications will not be sent to users.
	/// </summary>
	bool AllowNotifications { get; }

	public Object TagSubtype();

	public string TagTarget()
	{
		return $"{EntryType.ToString().ToLower()}:{TagSubtype().ToString().ToLower()}";
	}

	ITagManager Tags { get; }
}
