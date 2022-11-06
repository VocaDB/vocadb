using System.Diagnostics.CodeAnalysis;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Songs;

public class ArchivedSongListVersion : ArchivedObjectVersion, IArchivedObjectVersionWithFields<SongListEditableFields>
{
	private SongListDiff _diff;
	private SongList _songList;

#nullable disable
	public ArchivedSongListVersion()
	{
		Status = EntryStatus.Finished;
	}
#nullable enable

	public ArchivedSongListVersion(
		SongList songList,
		SongListDiff diff,
		AgentLoginData author,
		EntryStatus status,
		EntryEditEvent commonEditEvent,
		string notes
	)
		: base(null, author, songList.Version, status, notes)
	{
		ParamIs.NotNull(() => diff);

		SongList = songList;
		Diff = diff;
		CommonEditEvent = commonEditEvent;
	}

	public virtual EntryEditEvent CommonEditEvent { get; set; }

	public override IEntryDiff DiffBase => Diff;

	public virtual SongListDiff Diff
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

	public override IEntryWithNames EntryBase => SongList;

	public virtual SongList SongList
	{
		get => _songList;
		[MemberNotNull(nameof(_songList))]
		set
		{
			ParamIs.NotNull(() => value);
			_songList = value;
		}
	}

	public virtual bool IsIncluded(SongListEditableFields field)
	{
		return true;
	}
}
