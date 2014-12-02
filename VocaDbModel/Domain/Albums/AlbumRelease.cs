
namespace VocaDb.Model.Domain.Albums {

	public class AlbumRelease : IAlbumRelease {

		private string catNum;
		private string eventName;

		public AlbumRelease() {}

		public AlbumRelease(IAlbumRelease contract) {
			
			ParamIs.NotNull(() => contract);

			CatNum = contract.CatNum;
			ReleaseDate = (contract.ReleaseDate != null ? OptionalDateTime.Create(contract.ReleaseDate) : null);
			EventName = contract.EventName;

		}

		public virtual string CatNum {
			get { return catNum; }
			set { catNum = value; }
		}

		public virtual string EventName {
			get { return eventName; }
			set { eventName = value; }
		}

		public virtual bool IsEmpty {
			get {

				return (string.IsNullOrEmpty(CatNum) 
					&& string.IsNullOrEmpty(EventName) 
					&& (ReleaseDate == null || ReleaseDate.IsEmpty));

			}
		}

		public virtual OptionalDateTime ReleaseDate { get; set; }

		IOptionalDateTime IAlbumRelease.ReleaseDate {
			get { return ReleaseDate; }
		}

		public virtual bool Equals(AlbumRelease another) {

			if (another == null)
				return IsEmpty;

			return (Equals(CatNum, another.CatNum) && Equals(ReleaseDate, another.ReleaseDate) && Equals(EventName, another.EventName));

		}

	}
}
