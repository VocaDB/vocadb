using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Activityfeed;

[DataContract(Namespace = Schemas.VocaDb)]
public class ActivityEntryForApiContract
{
#nullable disable
	public ActivityEntryForApiContract() { }
#nullable enable

	public ActivityEntryForApiContract(
		ActivityEntry activityEntry,
		EntryForApiContract entryForApiContract,
		IUserIconFactory userIconFactory,
		IUserPermissionContext permissionContext,
		ActivityEntryOptionalFields fields
	)
	{
		CreateDate = activityEntry.CreateDate.ToUniversalTime();
		EditEvent = activityEntry.EditEvent;

		if (activityEntry.Author != null
			&& ((permissionContext.IsLoggedIn && (permissionContext.LoggedUserId == activityEntry.Author.Id || permissionContext.HasPermission(PermissionToken.DisableUsers)))
			|| !activityEntry.Author.AnonymousActivity))
		{
			Author = new UserForApiContract(activityEntry.Author, userIconFactory, UserOptionalFields.MainPicture);
		}

		if (fields.HasFlag(ActivityEntryOptionalFields.ArchivedVersion) && activityEntry.ArchivedVersionBase != null)
		{
			ArchivedVersion = new ArchivedObjectVersionForApiContract(
				archivedObjectVersion: activityEntry.ArchivedVersionBase,
				anythingChanged: false,
				reason: string.Empty,
				userIconFactory: userIconFactory
			);
		}

		Entry = entryForApiContract;
	}

	[DataMember(EmitDefaultValue = false)]
	public ArchivedObjectVersionForApiContract? ArchivedVersion { get; init; }

	[DataMember]
	public UserForApiContract? Author { get; init; }

	[DataMember]
	public DateTime CreateDate { get; init; }

	[DataMember]
	public EntryEditEvent EditEvent { get; init; }

	[DataMember(EmitDefaultValue = false)]
	public EntryForApiContract Entry { get; init; }
}

[Flags]
public enum ActivityEntryOptionalFields
{
	None = 0,
	ArchivedVersion = 1 << 0,
	Entry = 1 << 1,
}
