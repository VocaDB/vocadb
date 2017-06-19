using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistForEventContract {

		public ArtistForEventContract() { }

		public ArtistForEventContract(ArtistForEvent artistForEvent, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => artistForEvent);

			Artist = new ArtistContract(artistForEvent.Artist, languagePreference);
			Id = artistForEvent.Id;
			Name = artistForEvent.Name;
			Roles = artistForEvent.Roles;
			EffectiveRoles = artistForEvent.Roles;

		}

		[DataMember]
		public ArtistContract Artist { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public ArtistEventRoles EffectiveRoles { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public ArtistEventRoles Roles { get; set; }

	}

}
