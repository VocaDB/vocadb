using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.DataContracts.Versioning
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public sealed record ArchivedObjectVersionForApiContract
	{
#nullable disable
		public ArchivedObjectVersionForApiContract() { }
#nullable enable

		public ArchivedObjectVersionForApiContract(ArchivedObjectVersion archivedObjectVersion, IUserIconFactory userIconFactory)
		{
			ParamIs.NotNull(() => archivedObjectVersion);

			AgentName = !string.IsNullOrEmpty(archivedObjectVersion.AgentName) || archivedObjectVersion.Author is null
				? archivedObjectVersion.AgentName
				: archivedObjectVersion.Author.Name;
			Author = archivedObjectVersion.Author is not null ?
				new UserForApiContract(archivedObjectVersion.Author, userIconFactory, UserOptionalFields.MainPicture)
				: null;
			ChangedFields = archivedObjectVersion.DiffBase.ChangedFieldNames;
			Created = archivedObjectVersion.Created;
			EditEvent = archivedObjectVersion.EditEvent;
			Hidden = archivedObjectVersion.Hidden;
			Id = archivedObjectVersion.Id;
			Notes = archivedObjectVersion.Notes;
			Status = archivedObjectVersion.Status;
			Version = archivedObjectVersion.Version;
		}

		[DataMember]
		public string AgentName { get; init; }

		[DataMember]
		public UserForApiContract? Author { get; init; }

		[DataMember]
		public string[] ChangedFields { get; init; }

		[DataMember]
		public DateTime Created { get; init; }

		[DataMember]
		public EntryEditEvent EditEvent { get; init; }

		[DataMember]
		public bool Hidden { get; init; }

		[DataMember]
		public int Id { get; init; }

		[DataMember]
		public string Notes { get; init; }

		[DataMember]
		public EntryStatus Status { get; init; }

		[DataMember]
		public int Version { get; init; }
	}
}
