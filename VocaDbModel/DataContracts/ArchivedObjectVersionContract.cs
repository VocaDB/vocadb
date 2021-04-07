#nullable disable

using System;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Model.DataContracts
{
	public class ArchivedObjectVersionContract
	{
		public ArchivedObjectVersionContract() { }

#nullable enable
		public ArchivedObjectVersionContract(ArchivedObjectVersion archivedObjectVersion, IUserIconFactory userIconFactory)
		{
			ParamIs.NotNull(() => archivedObjectVersion);

			AgentName = !string.IsNullOrEmpty(archivedObjectVersion.AgentName) || archivedObjectVersion.Author == null ? archivedObjectVersion.AgentName : archivedObjectVersion.Author.Name;
			Author = (archivedObjectVersion.Author != null ? new UserForApiContract(archivedObjectVersion.Author, userIconFactory, UserOptionalFields.MainPicture) : null);
			Created = archivedObjectVersion.Created;
			EditEvent = archivedObjectVersion.EditEvent;
			Hidden = archivedObjectVersion.Hidden;
			Id = archivedObjectVersion.Id;
			IsSnapshot = archivedObjectVersion.DiffBase.IsSnapshot;
			Notes = archivedObjectVersion.Notes;
			Status = archivedObjectVersion.Status;
			Version = archivedObjectVersion.Version;
		}
#nullable disable

		public string AgentName { get; init; }

		public UserForApiContract Author { get; init; }

		public DateTime Created { get; init; }

		public EntryEditEvent EditEvent { get; init; }

		public bool Hidden { get; init; }

		public int Id { get; init; }

		public bool IsSnapshot { get; init; }

		public string Notes { get; init; }

		public EntryStatus Status { get; init; }

		public int Version { get; init; }

		public virtual bool IsAnythingChanged()
		{
			return false;
		}

		public virtual string TranslateChangedFields(IEnumTranslations translator)
		{
			return string.Empty;
		}

		public virtual string TranslateReason(IEnumTranslations translator)
		{
			return string.Empty;
		}
	}
}
