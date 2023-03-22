#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs;

[DataContract(Namespace = Schemas.VocaDb)]
public class SongInListEditContract : SongInListForApiContract
{
	public SongInListEditContract() { }

#nullable enable
	public SongInListEditContract(SongInList songInList, ContentLanguagePreference languagePreference, IUserPermissionContext permissionContext)
		: base(songInList, languagePreference, permissionContext, SongOptionalFields.AdditionalNames)
	{
		ParamIs.NotNull(() => songInList);

		SongInListId = songInList.Id;
	}
#nullable disable

	[DataMember]
	public int SongInListId { get; init; }
}
