#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs;

[Obsolete]
[DataContract(Namespace = Schemas.VocaDb)]
public class SongListForEditContract : SongListContract
{
	public SongListForEditContract()
	{
		SongLinks = Array.Empty<SongInListEditContract>();
		UpdateNotes = string.Empty;
	}

	public SongListForEditContract(SongList songList, IUserPermissionContext permissionContext)
		: base(songList, permissionContext)
	{
		SongLinks = songList.SongLinks
			.OrderBy(s => s.Order)
			.Select(s => new SongInListEditContract(s, permissionContext.LanguagePreference, permissionContext))
			.ToArray();
	}

	[DataMember]
	public SongInListEditContract[] SongLinks { get; set; }

	[DataMember]
	public string UpdateNotes { get; init; }
}
