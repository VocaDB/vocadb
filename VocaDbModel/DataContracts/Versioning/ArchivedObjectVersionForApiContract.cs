#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.DataContracts.Versioning
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedObjectVersionForApiContract
	{
		public ArchivedObjectVersionForApiContract() { }

#nullable enable
		public ArchivedObjectVersionForApiContract(ArchivedObjectVersion archivedObjectVersion)
		{
			ParamIs.NotNull(() => archivedObjectVersion);

			ChangedFields = archivedObjectVersion.DiffBase.ChangedFieldNames;
			Id = archivedObjectVersion.Id;
			Notes = archivedObjectVersion.Notes;
			Version = archivedObjectVersion.Version;
		}
#nullable disable

		[DataMember]
		public string[] ChangedFields { get; init; }

		[DataMember]
		public int Id { get; init; }

		[DataMember]
		public string Notes { get; init; }

		[DataMember]
		public int Version { get; init; }
	}
}
