#nullable disable

namespace VocaDb.Model.Domain
{
	/// <summary>
	/// Object (usually child entity) linked to an entity (usually master entity).
	/// </summary>
	public interface IEntryLink<out TEntry> where TEntry : class, IEntryWithIntId
	{
		TEntry Entry { get; }
	}
}
