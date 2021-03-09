#nullable disable

using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.DataContracts.UseCases
{
	/// <summary>
	/// Result of checking a new PV to be posted.
	/// </summary>
	[DataContract(Namespace = Schemas.VocaDb)]
	public class NewSongCheckResultContract
	{
		public NewSongCheckResultContract()
		{
			Matches = new DuplicateEntryResultContract<SongMatchProperty>[] { };
		}

		public NewSongCheckResultContract(DuplicateEntryResultContract<SongMatchProperty>[] matches, NicoTitleParseResult titleParseResult, ContentLanguagePreference languagePreference)
		{
			Matches = matches;

			if (titleParseResult != null)
			{
				Artists = titleParseResult.Artists.Where(a => a != null).Select(a => new ArtistContract(a, languagePreference)).ToArray();
				SongType = titleParseResult.SongType;
				Title = titleParseResult.Title;
				TitleLanguage = titleParseResult.TitleLanguage;
			}
		}

		/// <summary>
		/// List of parsed artists for the song, identified based on the PVs.
		/// </summary>
		[DataMember]
		public ArtistContract[] Artists { get; init; }

		/// <summary>
		/// List of matched duplicate songs already in the database.
		/// </summary>
		[DataMember]
		public DuplicateEntryResultContract<SongMatchProperty>[] Matches { get; init; }

		/// <summary>
		/// Type of song, identified based on the PVs.
		/// </summary>
		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public SongType SongType { get; init; }

		/// <summary>
		/// Parsed song title, based on the PV.
		/// This is not the user-entered song title.
		/// </summary>
		[DataMember]
		public string Title { get; init; }

		/// <summary>
		/// Possible language of the title.
		/// If unknown, this will be Unspecified.
		/// </summary>
		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public ContentLanguageSelection TitleLanguage { get; init; }
	}
}
