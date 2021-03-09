#nullable disable

using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Artists
{
	/// <summary>
	/// Artist contract with names for all supported language options.
	/// </summary>
	public class TranslatedArtistContract : ArtistContract
	{
		public TranslatedArtistContract() { }

		public TranslatedArtistContract(Artist artist)
			: base(artist, ContentLanguagePreference.Default)
		{
			Names = new BasicNameManager(artist.Names);
		}

		public BasicNameManager Names { get; init; }

		public override string ToString()
		{
			return $"translated artist '{Name}' [{Id}]";
		}
	}
}
