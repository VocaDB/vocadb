#nullable disable

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ViewRes;
using ViewRes.Album;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Helpers;
using VocaDb.Web.Code;

namespace VocaDb.Web.Models.Album
{
	[PropertyModelBinder]
	public class Create
	{
		public Create()
		{
			Artists = new List<ArtistContract>();
			DiscType = DiscType.Unknown;
		}

		[Display(ResourceType = typeof(SharedStrings), Name = "Artists")]
		[FromJson]
		public IList<ArtistContract> Artists { get; set; }

		[Display(ResourceType = typeof(CreateStrings), Name = "DiscType")]
		public DiscType DiscType { get; set; }

		[Display(ResourceType = typeof(EntryCreateStrings), Name = "EnglishName")]
		[StringLength(255)]
		public string NameEnglish { get; set; }

		[Display(ResourceType = typeof(EntryCreateStrings), Name = "NonEnglishName")]
		[StringLength(255)]
		public string NameOriginal { get; set; }

		[Display(ResourceType = typeof(EntryCreateStrings), Name = "RomajiName")]
		[StringLength(255)]
		public string NameRomaji { get; set; }

		public CreateAlbumContract ToContract()
		{
			return new CreateAlbumContract
			{
				Artists = this.Artists.ToArray(),
				DiscType = this.DiscType,
				Names = LocalizedStringHelper.SkipNullAndEmpty(NameOriginal, NameRomaji, NameEnglish).ToArray()
			};
		}
	}
}