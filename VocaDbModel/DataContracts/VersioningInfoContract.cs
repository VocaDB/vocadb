using System;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.DataContracts {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class VersioningInfoContract {

		public VersioningInfoContract() { }

		public VersioningInfoContract(ArchivedObjectVersion archivedObjectVersion) {

			ParamIs.NotNull(() => archivedObjectVersion);

			Author = archivedObjectVersion.Author.Name;
			Created = archivedObjectVersion.Created;
			Version = archivedObjectVersion.Version;

		}

		[DataMember]
		public string Author { get; set; }

		[DataMember]
		public DateTime Created { get; set; }

		[DataMember]
		public int Version { get; set; }

	}

}
