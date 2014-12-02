using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ViewRes;
using ViewRes.Song;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;
using VocaDb.Web.Code;

namespace VocaDb.Web.Models.Song {

	[PropertyModelBinder]
	public class Create {

		public Create() {
			Artists = new List<ArtistContract>();
			Draft = false;
			SongType = SongType.Original;
			NameEnglish = NameOriginal = NameRomaji = PVUrl = ReprintPVUrl = string.Empty;
		}

		[Display(ResourceType = typeof(SharedStrings), Name = "Artists")]
		[FromJson]
		public IList<ArtistContract> Artists { get; set; }

		[Display(ResourceType = typeof(CreateStrings), Name = "Draft")]
		public bool Draft { get; set; }

		[Display(ResourceType = typeof(EntryCreateStrings), Name = "EnglishName")]
		[StringLength(255)]
		public string NameEnglish { get; set; }

		[Display(ResourceType = typeof(EntryCreateStrings), Name = "NonEnglishName")]
		[StringLength(255)]
		public string NameOriginal { get; set; }

		[Display(ResourceType = typeof(CreateStrings), Name = "OriginalPV")]
		[StringLength(255)]
		public string PVUrl { get; set; }

		[Display(ResourceType = typeof(EntryCreateStrings), Name = "RomajiName")]
		[StringLength(255)]
		public string NameRomaji { get; set; }

		[Display(ResourceType = typeof(CreateStrings), Name = "ReprintPV")]
		[StringLength(255)]
		public string ReprintPVUrl { get; set; }

		[Display(ResourceType = typeof(CreateStrings), Name = "SongType")]
		public SongType SongType { get; set; }

		public CreateSongContract ToContract() {

			return new CreateSongContract {
				Artists = this.Artists.ToArray(),
				Draft = this.Draft,
				Names = LocalizedStringHelper.SkipNullAndEmpty(NameOriginal, NameRomaji, NameEnglish).ToArray(),
				PVUrl = this.PVUrl,
				ReprintPVUrl = this.ReprintPVUrl,
				SongType = this.SongType
			};

		}

	}

}