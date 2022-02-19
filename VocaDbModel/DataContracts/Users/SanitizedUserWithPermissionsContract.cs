using System;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users
{
	/// <summary>
	/// Contains no sensitive information.
	/// </summary>
	[DataContract(Namespace = Schemas.VocaDb)]
	public sealed record SanitizedUserWithPermissionsContract
	{
		[DataMember]
		public int Id { get; init; }

		[DataMember]
		public string Name { get; init; } = string.Empty;

		[DataMember]
		public bool Active { get; init; }

		[DataMember]
		public Guid[] EffectivePermissions { get; init; } = Array.Empty<Guid>();

		[DataMember]
		public int UnreadMessagesCount { get; init; }

		[DataMember]
		public bool VerifiedArtist { get; init; }

		[DataMember]
		public ArtistForUserContract[] OwnedArtistEntries { get; init; } = Array.Empty<ArtistForUserContract>();

		[DataMember]
		public string AlbumFormatString { get; init; } = string.Empty;

		[JsonConverter(typeof(StringEnumConverter))]
		public PVService PreferredVideoService { get; init; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public UserGroupId GroupId { get; init; }

		public SanitizedUserWithPermissionsContract() { }

		public SanitizedUserWithPermissionsContract(ServerOnlyUserWithPermissionsContract user)
		{
			Id = user.Id;
			Name = user.Name;
			Active = user.Active;
			EffectivePermissions = user.EffectivePermissions.Select(p => p.Id).ToArray();
			UnreadMessagesCount = user.UnreadMessagesCount;
			VerifiedArtist = user.VerifiedArtist;
			OwnedArtistEntries = user.OwnedArtistEntries;
			PreferredVideoService = user.PreferredVideoService;
			AlbumFormatString = user.AlbumFormatString;
			GroupId = user.GroupId;
		}
	}
}
