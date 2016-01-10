using System;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.DataContracts {

	public class ArchivedObjectVersionContract {

		public ArchivedObjectVersionContract() {}

		public ArchivedObjectVersionContract(ArchivedObjectVersion archivedObjectVersion) {
			
			ParamIs.NotNull(() => archivedObjectVersion);

			AgentName = !string.IsNullOrEmpty(archivedObjectVersion.AgentName) || archivedObjectVersion.Author == null ? archivedObjectVersion.AgentName : archivedObjectVersion.Author.Name;
			Author = (archivedObjectVersion.Author != null ? new UserContract(archivedObjectVersion.Author) : null);
			Created = archivedObjectVersion.Created;
			EditEvent = archivedObjectVersion.EditEvent;
			Id = archivedObjectVersion.Id;
			IsSnapshot = archivedObjectVersion.DiffBase.IsSnapshot;
			Notes = archivedObjectVersion.Notes;
			Status = archivedObjectVersion.Status;
			Version = archivedObjectVersion.Version;

		}

		public string AgentName { get; set; }

		public UserContract Author { get; set; }

		public DateTime Created { get; set; }

		public EntryEditEvent EditEvent { get; set; }

		public int Id { get; set; }

		public bool IsSnapshot { get; set; }

		public string Notes { get; set; }

		public EntryStatus Status { get; set; }

		public int Version { get; set; }

	}

}
