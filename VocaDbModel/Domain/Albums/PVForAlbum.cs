using System.Diagnostics.CodeAnalysis;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Domain.Albums;

public class PVForAlbum : PV, IEntryWithIntId
{
	private Album _album;

#nullable disable
	public PVForAlbum() { }
#nullable enable

	public PVForAlbum(Album album, PVContract contract)
		: base(contract)
	{
		Album = album;
		Length = contract.Length;
		Disabled = contract.Disabled;
	}

	public override bool Disabled { get; set; }

	/// <summary>
	/// Length in seconds.
	/// </summary>
	public virtual int Length { get; set; }

	public virtual Album Album
	{
		get => _album;
		[MemberNotNull(nameof(_album))]
		set
		{
			ParamIs.NotNull(() => value);
			_album = value;
		}
	}

	public virtual bool Equals(PVForAlbum? another)
	{
		if (another == null)
			return false;

		if (ReferenceEquals(this, another))
			return true;

		if (Id == 0)
			return false;

		return Id == another.Id && Disabled == another.Disabled;
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as PVForAlbum);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override void CopyMetaFrom(PVContract contract)
	{
		base.CopyMetaFrom(contract);

		Length = contract.Length;
		Disabled = contract.Disabled;
	}

	public override void OnDelete()
	{
		Album.PVs.Remove(this);
	}

	public override string ToString()
	{
		return $"PV '{PVId}' [{Id}] for {Album}";
	}
}
