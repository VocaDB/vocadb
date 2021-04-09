#nullable disable

using System;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users
{
	/// <summary>
	/// Data contract for <see cref="User"/> with most properties.
	/// SECURITY NOTE: take care when sending to client due to the contained sensitive information.
	/// </summary>
	[DataContract(Namespace = Schemas.VocaDb, Name = "UserContract")]
	public class ServerOnlyUserContract : ServerOnlyUserWithEmailContract
	{
		public ServerOnlyUserContract()
		{
			Language = string.Empty;
		}

#nullable enable
		public ServerOnlyUserContract(User user, bool getPublicCollection = false)
			: base(user)
		{
			ParamIs.NotNull(() => user);

			Active = user.Active;
			AnonymousActivity = user.AnonymousActivity;
			CreateDate = user.CreateDate;
			Culture = user.Culture;
			DefaultLanguageSelection = user.DefaultLanguageSelection;
			EmailOptions = user.EmailOptions;
			GroupId = user.GroupId;
			Language = user.Language.CultureCode;
			PreferredVideoService = user.PreferredVideoService;
			VerifiedArtist = user.VerifiedArtist;

			if (getPublicCollection)
				PublicAlbumCollection = user.Options.PublicAlbumCollection;
		}
#nullable disable

		[DataMember]
		public bool Active { get; init; }

		[DataMember]
		public bool AnonymousActivity { get; init; }

		[DataMember]
		public DateTime CreateDate { get; init; }

		[DataMember]
		public string Culture { get; set; }

		[DataMember]
		public ContentLanguagePreference DefaultLanguageSelection { get; set; }

		[DataMember]
		public UserEmailOptions EmailOptions { get; init; }

		[DataMember]
		public UserGroupId GroupId { get; init; }

		[DataMember]
		public string Language { get; set; }

		[DataMember]
		public PVService PreferredVideoService { get; init; }

		[DataMember]
		public bool PublicAlbumCollection { get; init; }

		[DataMember]
		public bool VerifiedArtist { get; init; }
	}
}
