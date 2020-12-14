#nullable disable

using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class UserForApiContract : UserBaseContract
	{
		public UserForApiContract() { }

		public UserForApiContract(User user)
			: this(user, null, null, UserOptionalFields.None) { }

		public UserForApiContract(User user, IUserIconFactory iconFactory, UserOptionalFields optionalFields)
			: this(user, null, iconFactory, optionalFields) { }

		public UserForApiContract(User user, string fallbackName, IUserIconFactory iconFactory, UserOptionalFields optionalFields)
			: base(user, fallbackName)
		{
			if (user == null)
				return;

			Active = user.Active;
			GroupId = user.GroupId;
			MemberSince = user.CreateDate;
			VerifiedArtist = user.VerifiedArtist;

			if (optionalFields.HasFlag(UserOptionalFields.KnownLanguages))
			{
				KnownLanguages = user.KnownLanguages.Select(l => new UserKnownLanguageContract(l)).ToArray();
			}

			if (optionalFields.HasFlag(UserOptionalFields.MainPicture) && !string.IsNullOrEmpty(user.Email) && iconFactory != null)
			{
				MainPicture = iconFactory.GetIcons(user, ImageSizes.All);
			}

			if (optionalFields.HasFlag(UserOptionalFields.OldUsernames))
			{
				OldUsernames = user.OldUsernames.Select(n => new OldUsernameContract(n)).ToArray();
			}
		}

		[DataMember]
		public bool Active { get; set; }

		[DataMember]
		public UserGroupId GroupId { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public UserKnownLanguageContract[] KnownLanguages { get; set; }

		/// <summary>
		/// Can be null.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public EntryThumbForApiContract MainPicture { get; set; }

		[DataMember]
		public DateTime MemberSince { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public OldUsernameContract[] OldUsernames { get; set; }

		[DataMember]
		public bool VerifiedArtist { get; set; }
	}

	[Flags]
	public enum UserOptionalFields
	{
		None = 0,
		KnownLanguages = 1,
		MainPicture = 2,
		OldUsernames = 4
	}
}
