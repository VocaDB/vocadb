using System.Linq;
using System.Text;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Model.Service.ExtSites {

	public class ArtistDescriptionGenerator {

		private void AddVoicebankDetails(StringBuilder sb, ArtistDetailsContract artist) {
			
			if (artist.ReleaseDate.HasValue) {
				sb.AppendFormat(" Released {0}.", artist.ReleaseDate.Value.ToShortDateString());
			}

			if (artist.VoiceProviders.Any()) {
				sb.AppendFormat(" Voice provider: {0}.", string.Join(", ", artist.VoiceProviders.Select(a => a.Name)));
			}

		}

		public string GenerateDescription(ArtistDetailsContract artist, string original, TranslateableEnum<ArtistType> artistTypeNames) {

			var sb = new StringBuilder(original);

			// Note: if original description is not empty, artist type is added to title instead of description
			if (string.IsNullOrEmpty(original)) {
				sb.AppendFormat("{0}.", artistTypeNames[artist.ArtistType]);
			}

			AddVoicebankDetails(sb, artist);

			return sb.ToString();

		}

	}

}
