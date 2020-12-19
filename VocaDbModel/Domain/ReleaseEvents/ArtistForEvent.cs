#nullable disable

using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Domain.ReleaseEvents
{
	public class ArtistForEvent : IEntryWithIntId, IArtistLink
	{
		private ReleaseEvent _releaseEvent;
		private string _name;

		public ArtistForEvent() { }

		public ArtistForEvent(ReleaseEvent releaseEvent, Artist artist)
		{
			this.Artist = artist;
			this._releaseEvent = releaseEvent;
		}

		/// <summary>
		/// Linked artist. Can be null.
		/// </summary>
		public virtual Artist Artist { get; set; }

		public virtual int Id { get; set; }

		public virtual string Name
		{
			get => _name;
			set => _name = value;
		}

		public virtual ReleaseEvent ReleaseEvent
		{
			get => _releaseEvent;
			set => _releaseEvent = value;
		}

		public virtual ArtistEventRoles Roles { get; set; }

		public virtual void Delete()
		{
			ReleaseEvent.AllArtists.Remove(this);
		}
	}
}
