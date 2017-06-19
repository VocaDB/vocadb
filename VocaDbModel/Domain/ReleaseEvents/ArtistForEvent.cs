using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Domain.ReleaseEvents {

	public class ArtistForEvent : IEntryWithIntId {

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

	[Flags]
	public enum ArtistEventRoles {
		Default = 0,
		/// <summary>
		/// Disc jockey (plays songs)
		/// </summary>
		DJ = 1,
		/// <summary>
		/// Plays an instrument
		/// </summary>
		Instrumentalist = 2,
		/// <summary>
		/// Organizes event (might not participate directly)
		/// </summary>
		Organizer = 4,
		/// <summary>
		/// Promotes (advertises) event (might not participate directly)
		/// </summary>
		Promoter = 8,
		/// <summary>
		/// Video jockey (plays videos)
		/// </summary>
		VJ = 16,
		Vocalist = 32,
		/// <summary>
		/// Voice manipulator of Vocaloid/UTAU
		/// </summary>
		VoiceManipulator = 64,
		Other = 128
	}

}
