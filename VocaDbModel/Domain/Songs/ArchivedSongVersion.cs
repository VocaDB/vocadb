using VocaDb.Model.Domain.Security;
using VocaDb.Model.DataContracts.Songs;
using System.Xml.Linq;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Helpers;
using VocaDb.Model.Domain.Activityfeed;

namespace VocaDb.Model.Domain.Songs
{

	public class ArchivedSongVersion : ArchivedObjectVersion, IArchivedObjectVersionWithFields<SongEditableFields>
	{

		public static ArchivedSongVersion Create(Song song, SongDiff diff, AgentLoginData author, SongArchiveReason reason, string notes)
		{

			var contract = new ArchivedSongContract(song, diff);
			var data = XmlHelper.SerializeToXml(contract);

			return song.CreateArchivedVersion(data, diff, author, reason, notes);

		}

		private SongDiff diff;
		private Song song;

		public ArchivedSongVersion() { }

		public ArchivedSongVersion(Song song, XDocument data, SongDiff diff, AgentLoginData author, int version, EntryStatus status,
			SongArchiveReason reason, string notes)
			: base(data, author, version, status, notes)
		{

			ParamIs.NotNull(() => data);

			Song = song;
			Diff = diff;
			Reason = reason;

		}

		public virtual SongDiff Diff
		{
			get { return diff; }
			protected set { diff = value; }
		}

		public override IEntryDiff DiffBase => Diff;

		public override EntryEditEvent EditEvent
		{
			get
			{
				return (Reason == SongArchiveReason.Created || Reason == SongArchiveReason.AutoImportedFromMikuDb ?
					EntryEditEvent.Created : EntryEditEvent.Updated);
			}
		}

		public override IEntryWithNames EntryBase => Song;

		public virtual SongArchiveReason Reason { get; set; }

		public virtual Song Song
		{
			get { return song; }
			protected set
			{
				ParamIs.NotNull(() => value);
				song = value;
			}
		}

		public virtual ArchivedSongVersion GetLatestVersionWithField(SongEditableFields field)
		{

			if (IsIncluded(field))
				return this;

			return Song.ArchivedVersionsManager.GetLatestVersionWithField(field, Version);

		}

		public virtual bool IsIncluded(SongEditableFields field)
		{
			return (Diff != null && Data != null && Diff.IsIncluded(field));
		}

	}

}
