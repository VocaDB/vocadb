using System;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class UserForApiContract : UserWithIconContract {

		public UserForApiContract(User user, IUserIconFactory iconFactory) : base(user, iconFactory) {
			Active = user.Active;
			GroupId = user.GroupId;
			MemberSince = user.CreateDate;
		}

		[DataMember]
		public bool Active { get; set; }

		[DataMember]
		public UserGroupId GroupId { get; set; }

		[DataMember]
		public DateTime MemberSince { get; set; }

	}

}
