#nullable disable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistForEventContract
	{
		public ArtistForEventContract() { }

		public ArtistForEventContract(ArtistForEvent artistForEvent, ContentLanguagePreference languagePreference)
		{
			ParamIs.NotNull(() => artistForEvent);

			Artist = artistForEvent.Artist != null ? new ArtistContract(artistForEvent.Artist, languagePreference) : null;
			Id = artistForEvent.Id;
			Name = artistForEvent.Name;
			Roles = artistForEvent.Roles;
			EffectiveRoles = artistForEvent.Roles;
		}

		[DataMember]
		public ArtistContract Artist { get; init; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public ArtistEventRoles EffectiveRoles { get; init; }

		[DataMember]
		public int Id { get; init; }

		[DataMember]
		public string Name { get; init; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public ArtistEventRoles Roles { get; init; }
	}
}
