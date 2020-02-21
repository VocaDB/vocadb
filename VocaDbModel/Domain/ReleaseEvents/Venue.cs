using System.Collections.Generic;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.ReleaseEvents {

	public class Venue : IEntryBase {

		private IList<ReleaseEvent> events = new List<ReleaseEvent>();

		public virtual string Address { get; set; }

		public virtual string DefaultName => Name.Default;

		public virtual bool Deleted { get; set; }

		public virtual EntryType EntryType => EntryType.Venue;

		public virtual IList<ReleaseEvent> Events {
			get => events;
			set => events = value;
		}

		public virtual int Id { get; set; }

		public virtual TranslatedString Name { get; set; }

		public virtual int Version { get; set; }

	}

}
