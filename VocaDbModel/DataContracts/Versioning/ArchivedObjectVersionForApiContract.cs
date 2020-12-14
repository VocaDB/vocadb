#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.DataContracts.Versioning
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedObjectVersionForApiContract
	{
		public ArchivedObjectVersionForApiContract() { }

		public ArchivedObjectVersionForApiContract(ArchivedObjectVersion archivedObjectVersion)
		{
			ParamIs.NotNull(() => archivedObjectVersion);

			ChangedFields = archivedObjectVersion.DiffBase.ChangedFieldNames;
			Id = archivedObjectVersion.Id;
			Notes = archivedObjectVersion.Notes;
			Version = archivedObjectVersion.Version;
		}

		[DataMember]
		public string[] ChangedFields { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Notes { get; set; }

		[DataMember]
		public int Version { get; set; }
	}
}
