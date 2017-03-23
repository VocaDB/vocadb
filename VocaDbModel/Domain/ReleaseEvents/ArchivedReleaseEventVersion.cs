using System.Xml.Linq;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.ReleaseEvents {

	public class ArchivedReleaseEventVersion : ArchivedObjectVersion, IArchivedObjectVersionWithFields<ReleaseEventEditableFields> {

		public static ArchivedReleaseEventVersion Create(ReleaseEvent releaseEvent, ReleaseEventDiff diff, AgentLoginData author, EntryEditEvent commonEditEvent, string notes) {

			var contract = new ArchivedEventContract(releaseEvent, diff);
			var data = XmlHelper.SerializeToXml(contract);

			return releaseEvent.CreateArchivedVersion(data, diff, author, commonEditEvent, notes);

		}

		private ReleaseEventDiff diff;
		private ReleaseEvent releaseEvent;

		public ArchivedReleaseEventVersion() {
			Status = EntryStatus.Finished;
		}

		public ArchivedReleaseEventVersion(ReleaseEvent releaseEvent, XDocument data, ReleaseEventDiff diff, AgentLoginData author,
			EntryEditEvent commonEditEvent, string notes)
			: base(data, author, releaseEvent.Version, EntryStatus.Finished, notes) {

			ParamIs.NotNull(() => diff);

			ReleaseEvent = releaseEvent;
			Diff = diff;
			CommonEditEvent = commonEditEvent;
			Status = EntryStatus.Finished;

		}

		public virtual EntryEditEvent CommonEditEvent { get; set; }

		public override IEntryDiff DiffBase => Diff;

		public virtual ReleaseEventDiff Diff {
			get => diff;
			set { 
				ParamIs.NotNull(() => value);
				diff = value; 
			}
		}

		public override EntryEditEvent EditEvent => CommonEditEvent;

		public override IEntryWithNames EntryBase => ReleaseEvent;

		public virtual ReleaseEvent ReleaseEvent {
			get => releaseEvent;
			set { 
				ParamIs.NotNull(() => value);
				releaseEvent = value; 
			}
		}

		public virtual bool IsIncluded(ReleaseEventEditableFields field) {
			return true;
		}

	}
	
}
