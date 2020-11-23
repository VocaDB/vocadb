using System.Xml.Linq;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Tags
{

	public class ArchivedTagVersion : ArchivedObjectVersion, IArchivedObjectVersionWithFields<TagEditableFields>
	{

		public static ArchivedTagVersion Create(Tag tag, TagDiff diff, AgentLoginData author, EntryEditEvent commonEditEvent, string notes)
		{

			var contract = new ArchivedTagContract(tag, diff);
			var data = XmlHelper.SerializeToXml(contract);

			return tag.CreateArchivedVersion(data, diff, author, commonEditEvent, notes);

		}

		private TagDiff diff;
		private Tag tag;

		public ArchivedTagVersion()
		{
			Status = EntryStatus.Finished;
		}

		public ArchivedTagVersion(Tag tag, XDocument data, TagDiff diff, AgentLoginData author,
			EntryEditEvent commonEditEvent, string notes)
			: base(data, author, tag.Version, tag.Status, notes)
		{

			ParamIs.NotNull(() => diff);

			Tag = tag;
			Diff = diff;
			CommonEditEvent = commonEditEvent;

		}

		public virtual EntryEditEvent CommonEditEvent { get; set; }

		public override IEntryDiff DiffBase => Diff;

		public virtual TagDiff Diff
		{
			get { return diff; }
			set
			{
				ParamIs.NotNull(() => value);
				diff = value;
			}
		}

		public override EntryEditEvent EditEvent => CommonEditEvent;

		public override IEntryWithNames EntryBase => Tag;

		public virtual Tag Tag
		{
			get { return tag; }
			set
			{
				ParamIs.NotNull(() => value);
				tag = value;
			}
		}

		public virtual ArchivedTagVersion GetLatestVersionWithField(TagEditableFields field)
		{

			if (IsIncluded(field))
				return this;

			return Tag.ArchivedVersionsManager.GetLatestVersionWithField(field, Version);

		}

		public virtual bool IsIncluded(TagEditableFields field)
		{
			return (Diff != null && Data != null && Diff.IsIncluded(field));
		}

	}

}
