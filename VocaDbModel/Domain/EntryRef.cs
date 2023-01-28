#nullable disable


namespace VocaDb.Model.Domain;

public class EntryRef : IEquatable<EntryRef>
{
	public EntryRef() { }

#nullable enable
	public EntryRef(IEntryBase entryBase)
	{
		ParamIs.NotNull(() => entryBase);

		EntryType = entryBase.EntryType;
		Id = entryBase.Id;
	}
#nullable disable

	public EntryRef(EntryType entryType, int id)
	{
		EntryType = entryType;
		Id = id;
	}

	public EntryType EntryType { get; set; }

	public int Id { get; set; }

#nullable enable
	public bool Equals(EntryRef? another)
	{
		if (another == null)
			return false;

		return (EntryType == another.EntryType && Id == another.Id);
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as EntryRef);
	}

	public override int GetHashCode()
	{
		return (EntryType.ToString() + "_" + Id).GetHashCode();
	}

	public override string ToString()
	{
		return $"entry of type {EntryType}, ID {Id}";
	}
#nullable disable
}
