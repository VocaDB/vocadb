using System.Diagnostics.CodeAnalysis;

namespace VocaDb.Model.Domain.Albums;

public class AlbumIdentifier : IEntryWithIntId
{
	private Album _album;
	private string _value;

#nullable disable
	public AlbumIdentifier() { }
#nullable enable

	public AlbumIdentifier(Album album, string value)
	{
		Album = album;
		Value = value;
	}

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

	public virtual int Id { get; set; }

	public virtual string Value
	{
		get => _value;
		[MemberNotNull(nameof(_value))]
		set
		{
			ParamIs.NotNull(() => value);
			_value = value;
		}
	}

	public virtual bool ContentEquals(AlbumIdentifier another)
	{
		return string.Equals(Value, another.Value);
	}
}
