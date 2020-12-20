#nullable disable

using System.Xml.Linq;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.ReleaseEvents
{
	public class ArchivedReleaseEventSeriesVersion : ArchivedObjectVersion, IArchivedObjectVersionWithFields<ReleaseEventSeriesEditableFields>
	{
		public static ArchivedReleaseEventSeriesVersion Create(ReleaseEventSeries series, ReleaseEventSeriesDiff diff, AgentLoginData author, EntryEditEvent commonEditEvent, string notes)
		{
			var contract = new ArchivedEventSeriesContract(series, diff);
			var data = XmlHelper.SerializeToXml(contract);

			return series.CreateArchivedVersion(data, diff, author, commonEditEvent, notes);
		}

		private ReleaseEventSeriesDiff _diff;
		private ReleaseEventSeries _series;

		public ArchivedReleaseEventSeriesVersion()
		{
			Status = EntryStatus.Finished;
		}

		public ArchivedReleaseEventSeriesVersion(ReleaseEventSeries series, XDocument data, ReleaseEventSeriesDiff diff, AgentLoginData author,
			EntryEditEvent commonEditEvent, string notes)
			: base(data, author, series.Version, series.Status, notes)
		{
			ParamIs.NotNull(() => diff);

			Entry = series;
			Diff = diff;
			CommonEditEvent = commonEditEvent;
		}

		public virtual EntryEditEvent CommonEditEvent { get; set; }

		public override IEntryDiff DiffBase => Diff;

		public virtual ReleaseEventSeriesDiff Diff
		{
			get => _diff;
			set
			{
				ParamIs.NotNull(() => value);
				_diff = value;
			}
		}

		public override EntryEditEvent EditEvent => CommonEditEvent;

		public override IEntryWithNames EntryBase => Entry;

		public virtual ReleaseEventSeries Entry
		{
			get => _series;
			set
			{
				ParamIs.NotNull(() => value);
				_series = value;
			}
		}

		public virtual ArchivedReleaseEventSeriesVersion GetLatestVersionWithField(ReleaseEventSeriesEditableFields field)
		{
			if (IsIncluded(field))
				return this;

			return Entry.ArchivedVersionsManager.GetLatestVersionWithField(field, Version);
		}

		public virtual bool IsIncluded(ReleaseEventSeriesEditableFields field)
		{
			return Diff != null && Data != null && Diff.IsIncluded(field);
		}
	}
}
