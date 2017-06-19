using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Domain.ReleaseEvents {

	public class ArtistForEvent {

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

		public virtual string Name {
			get => name;
			set => name = value;
		}

		public virtual ReleaseEvent ReleaseEvent {
			get => releaseEvent;
			set => releaseEvent = value;
		}

		public virtual ArtistEventRoles Roles { get; set; }

	}

	public enum ArtistEventRoles {
		Default = 0,
		Organizer = 1,
		Performer = 2,
		Promoter = 4,
		VoiceManipulator = 8,
		Other = 16
	}

}
