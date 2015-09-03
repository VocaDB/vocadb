using System;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class UserForApiContract : UserBaseContract {

		public UserForApiContract(User user, IUserIconFactory iconFactory, UserOptionalFields optionalFields) 
			: base(user) {

			Active = user.Active;
			GroupId = user.GroupId;
			MemberSince = user.CreateDate;
			VerifiedArtist = user.VerifiedArtist;

			if (optionalFields.HasFlag(UserOptionalFields.MainPicture) && !string.IsNullOrEmpty(user.Email)) {

				var thumbUrl = iconFactory.GetIconUrl(user);

				MainPicture = new EntryThumbForApiContract {
					UrlThumb = thumbUrl,
				};
			}

		}

		[DataMember]
		public bool Active { get; set; }

		[DataMember]
		public UserGroupId GroupId { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public EntryThumbForApiContract MainPicture { get; set; }

		[DataMember]
		public DateTime MemberSince { get; set; }

		[DataMember]
		public bool VerifiedArtist { get; set; }

	}

	[Flags]
	public enum UserOptionalFields {

		None = 0,
		MainPicture = 1,

	}

}
