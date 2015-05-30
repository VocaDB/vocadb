using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Tags {

	public class ArchivedTagVersion : ArchivedObjectVersion, IArchivedObjectVersionWithFields<TagEditableFields> {

		private string categoryName;
		private string description;
		private TagDiff diff;
		private Tag tag;

		public ArchivedTagVersion() { 
			Status = EntryStatus.Finished;
		}

		public ArchivedTagVersion(Tag tag, TagDiff diff, AgentLoginData author,
			EntryEditEvent commonEditEvent)
			: base(null, author, tag.Version, EntryStatus.Finished, string.Empty) {

			ParamIs.NotNull(() => diff);

			Tag = tag;
			Diff = diff;
			CommonEditEvent = commonEditEvent;
			CategoryName = tag.CategoryName;
			Description = tag.Description;
			Status = tag.Status;

		}

		public virtual string CategoryName {
			get { return categoryName; }
			set { 
				ParamIs.NotNull(() => value);
				categoryName = value; 
			}
		}

		public virtual EntryEditEvent CommonEditEvent { get; set; }

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

		public virtual TagDiff Diff {
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
			get { return Tag; }
		}

		public virtual Tag Tag {
			get { return tag; }
			set { 
				ParamIs.NotNull(() => value);
				tag = value; 
			}
		}

		public virtual bool IsIncluded(TagEditableFields field) {
			return true;
		}

	}

}
