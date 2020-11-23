
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.Domain.Albums
{

	public class AlbumRelease : IAlbumRelease
	{

		private string catNum;

		public AlbumRelease() { }

		public AlbumRelease(IAlbumRelease contract, ReleaseEvent releaseEvent)
		{

			ParamIs.NotNull(() => contract);

			CatNum = contract.CatNum;
			ReleaseDate = (contract.ReleaseDate != null ? OptionalDateTime.Create(contract.ReleaseDate) : null);
			ReleaseEvent = releaseEvent;

		}

		public virtual string CatNum
		{
			get { return catNum; }
			set { catNum = value; }
		}

		public virtual bool IsEmpty
		{
			get
			{

				return (string.IsNullOrEmpty(CatNum)
					&& ReleaseEvent == null
					&& (ReleaseDate == null || ReleaseDate.IsEmpty));

			}
		}

		public virtual OptionalDateTime ReleaseDate { get; set; }

		IOptionalDateTime IAlbumRelease.ReleaseDate
		{
			get { return ReleaseDate; }
		}

		public virtual ReleaseEvent ReleaseEvent { get; set; }

		public virtual bool Equals(AlbumRelease another)
		{

			if (another == null)
				return IsEmpty;

			return Equals(CatNum, another.CatNum) && Equals(ReleaseDate, another.ReleaseDate) && Equals(ReleaseEvent, another.ReleaseEvent);

		}

	}
}
