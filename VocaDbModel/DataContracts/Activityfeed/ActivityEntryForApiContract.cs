using System;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Activityfeed
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class ActivityEntryForApiContract
	{
		public ActivityEntryForApiContract() { }

		public ActivityEntryForApiContract(ActivityEntry activityEntry, EntryForApiContract entryForApiContract,
			IUserIconFactory userIconFactory,
			IUserPermissionContext permissionContext,
			ActivityEntryOptionalFields fields)
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
				ArchivedVersion = new ArchivedObjectVersionForApiContract(activityEntry.ArchivedVersionBase);
			}

			Entry = entryForApiContract;
		}

		[DataMember(EmitDefaultValue = false)]
		public ArchivedObjectVersionForApiContract ArchivedVersion { get; set; }

		[DataMember]
		public UserForApiContract Author { get; set; }

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public EntryEditEvent EditEvent { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public EntryForApiContract Entry { get; set; }
	}

	[Flags]
	public enum ActivityEntryOptionalFields
	{
		None = 0,
		ArchivedVersion = 1,
		Entry = 2
	}
}
