using System.Diagnostics.CodeAnalysis;
using VocaDb.Model.DataContracts.PVs;

namespace VocaDb.Model.Domain.PVs;

public abstract class GenericPV<TEntry> : PV, IEntryWithIntId
	where TEntry : class
{
	private TEntry _entry;

#nullable disable
	protected GenericPV()
	{
	}
#nullable enable

	protected GenericPV(TEntry entry, PVContract contract)
		: base(contract)
	{
		Entry = entry;
		Length = contract.Length;
		PublishDate = contract.PublishDate;
	}

	public virtual TEntry Entry
	{
		get => _entry;
		[MemberNotNull(nameof(_entry))]
		set
		{
			ParamIs.NotNull(() => value);
			_entry = value;
		}
	}

	/// <summary>
	/// Length in seconds.
	/// </summary>
	public virtual int Length { get; set; }

	public override void CopyMetaFrom(PVContract contract)
	{
		base.CopyMetaFrom(contract);

		Length = contract.Length;
		PublishDate = contract.PublishDate;
	}

	public virtual bool Equals(GenericPV<TEntry>? another)
	{
		if (another == null)
			return false;

		if (ReferenceEquals(this, another))
			return true;

		if (Id == 0)
			return false;

		return Id == another.Id;
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as GenericPV<TEntry>);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override string ToString()
	{
		return $"PV '{PVId}' on {Service} [{Id}] for {Entry}";
	}
}
