using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.ReleaseEvents;

public class ArchivedReleaseEventVersion : ArchivedObjectVersion, IArchivedObjectVersionWithFields<ReleaseEventEditableFields>
{
	public static ArchivedReleaseEventVersion Create(ReleaseEvent releaseEvent, ReleaseEventDiff diff, AgentLoginData author, EntryEditEvent commonEditEvent, string notes)
	{
		var contract = new ArchivedEventContract(releaseEvent, diff);
		var data = XmlHelper.SerializeToXml(contract);

		return releaseEvent.CreateArchivedVersion(data, diff, author, commonEditEvent, notes);
	}

	private ReleaseEventDiff _diff;
	private ReleaseEvent _releaseEvent;

#nullable disable
	public ArchivedReleaseEventVersion()
	{
		Status = EntryStatus.Finished;
	}
#nullable enable

	public ArchivedReleaseEventVersion(
		ReleaseEvent releaseEvent,
		XDocument data,
		ReleaseEventDiff diff,
		AgentLoginData author,
		EntryEditEvent commonEditEvent,
		string notes
	)
		: base(data, author, releaseEvent.Version, releaseEvent.Status, notes)
	{
		ParamIs.NotNull(() => diff);

		ReleaseEvent = releaseEvent;
		Diff = diff;
		CommonEditEvent = commonEditEvent;
	}

	public virtual EntryEditEvent CommonEditEvent { get; set; }

	public override IEntryDiff DiffBase => Diff;

	public virtual ReleaseEventDiff Diff
	{
		get => _diff;
		[MemberNotNull(nameof(_diff))]
		set
		{
			ParamIs.NotNull(() => value);
			_diff = value;
		}
	}

	public override EntryEditEvent EditEvent => CommonEditEvent;

	public override IEntryWithNames EntryBase => ReleaseEvent;

	public virtual ReleaseEvent ReleaseEvent
	{
		get => _releaseEvent;
		[MemberNotNull(nameof(_releaseEvent))]
		set
		{
			ParamIs.NotNull(() => value);
			_releaseEvent = value;
		}
	}

	public virtual ArchivedReleaseEventVersion? GetLatestVersionWithField(ReleaseEventEditableFields field)
	{
		if (IsIncluded(field))
			return this;

		return ReleaseEvent.ArchivedVersionsManager.GetLatestVersionWithField(field, Version);
	}

	public virtual bool IsIncluded(ReleaseEventEditableFields field)
	{
		return true;
	}
}
