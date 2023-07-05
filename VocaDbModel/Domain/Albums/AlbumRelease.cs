using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.Domain.Albums;

public class AlbumRelease : IAlbumRelease
{
	private string? _catNum;
	private IList<ReleaseEvent> _releaseEvents = new List<ReleaseEvent>();

	public AlbumRelease() { }

	public AlbumRelease(IAlbumRelease contract, ReleaseEvent[] releaseEvents)
	{
		ParamIs.NotNull(() => contract);

		CatNum = contract.CatNum;

		ReleaseDate = contract.ReleaseDate != null
			? OptionalDateTime.Create(contract.ReleaseDate)
			: null;

		ReleaseEvents = releaseEvents;
	}

	public virtual string? CatNum
	{
		get => _catNum;
		set => _catNum = value;
	}

	public virtual bool IsEmpty
	{
		get
		{
			return string.IsNullOrEmpty(CatNum)
				&& ReleaseEvent == null
				&& (ReleaseDate == null || ReleaseDate.IsEmpty);
		}
	}

	public virtual OptionalDateTime? ReleaseDate { get; set; }

	IOptionalDateTime? IAlbumRelease.ReleaseDate => ReleaseDate;

	public virtual ReleaseEvent? ReleaseEvent { get; set; }
	public virtual IList<ReleaseEvent> ReleaseEvents
	{
		get => _releaseEvents;
		set
		{
			ParamIs.NotNull(() => value);
			_releaseEvents = value;
		}
	}

	public virtual bool Equals(AlbumRelease? another)
	{
		if (another == null)
			return IsEmpty;

		return Equals(CatNum, another.CatNum) && Equals(ReleaseDate, another.ReleaseDate) && ReleaseEvents.SequenceEqual(another.ReleaseEvents);
	}
}
