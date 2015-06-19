using System;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Security {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class PermissionTokenContract : IPermissionToken {

		public PermissionTokenContract() { }

		public PermissionTokenContract(PermissionToken token) {

			Id = token.Id;
			Name = token.Name;

		}

		[DataMember]
		public Guid Id { get; set; }

		[DataMember]
		public string Name { get; set; }

	}

}
