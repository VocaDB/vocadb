using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Domain.ReleaseEvents {

	public class ArtistForEvent : IEntryWithIntId, IArtistLink {

		private ReleaseEvent releaseEvent;
		private string name;

		public ArtistForEvent() { }

		public ArtistForEvent(ReleaseEvent releaseEvent, Artist artist) {
			this.Artist = artist;
			this.releaseEvent = releaseEvent;
		}

		/// <summary>
		/// Linked artist. Can be null.
		/// </summary>
		public virtual Artist Artist { get; set; }

		public virtual int Id { get; set; }

		public virtual string Name {
			get => name;
			set => name = value;
		}

		public virtual ReleaseEvent ReleaseEvent {
			get => releaseEvent;
			set => releaseEvent = value;
		}

		public virtual ArtistEventRoles Roles { get; set; }

		public virtual void Delete() {
			ReleaseEvent.AllArtists.Remove(this);
		}

	}
}
