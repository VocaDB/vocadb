using System;
using System.Xml.Linq;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Albums {

	public class ArchivedReleaseEventVersion : ArchivedObjectVersion, IArchivedObjectVersionWithFields<ReleaseEventEditableFields> {

		public static ArchivedReleaseEventVersion Create(ReleaseEvent releaseEvent, ReleaseEventDiff diff, AgentLoginData author, EntryEditEvent commonEditEvent, string notes) {

			var contract = new ArchivedEventContract(releaseEvent, diff);
			var data = XmlHelper.SerializeToXml(contract);

			return releaseEvent.CreateArchivedVersion(data, diff, author, commonEditEvent, notes);

		}

		private string description;
		private ReleaseEventDiff diff;
		private string name;
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
			Date = releaseEvent.Date;
			Description = releaseEvent.Description;
			Name = releaseEvent.Name;
			SeriesNumber = releaseEvent.SeriesNumber;
			Status = EntryStatus.Finished;

		}

		public virtual EntryEditEvent CommonEditEvent { get; set; }

		public virtual DateTime? Date { get; set; }

		public virtual string Description {
			get { return description; }
			set { 
				ParamIs.NotNull(() => value);
				description = value; 
			}
		}

		public override IEntryDiff DiffBase {
			get { return Diff; }
		}

		public virtual ReleaseEventDiff Diff {
			get { return diff; }
			set { 
				ParamIs.NotNull(() => value);
				diff = value; 
			}
		}

		public override EntryEditEvent EditEvent {
			get { return CommonEditEvent; }
		}

		public override IEntryWithNames EntryBase {
			get { return ReleaseEvent; }
		}

		public virtual string Name {
			get { return name; }
			set { 
				ParamIs.NotNull(() => value);
				name = value; 
			}
		}

		public virtual ReleaseEvent ReleaseEvent {
			get { return releaseEvent; }
			set { 
				ParamIs.NotNull(() => value);
				releaseEvent = value; 
			}
		}

		public virtual int SeriesNumber { get; set; }

		public virtual bool IsIncluded(ReleaseEventEditableFields field) {
			return true;
		}

	}
	
}
