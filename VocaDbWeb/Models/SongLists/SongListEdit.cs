using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Web.Code;

namespace VocaDb.Web.Models.SongLists {

	[FromJson]
	public class SongListEdit {

		public SongListEdit() {
			SongLinks = new List<SongInListEditContract>();
			CanCreateFeaturedLists = EntryPermissionManager.CanManageFeaturedLists(MvcApplication.LoginManager);
		}

		public SongListEdit(SongListForEditContract contract)
			: this() {

			ParamIs.NotNull(() => contract);

			CurrentName = contract.Name;
			Description = contract.Description;
			FeaturedCategory = contract.FeaturedCategory;
			Id = contract.Id;
			Name = contract.Name;
			SongLinks = contract.SongLinks;
			Thumb = contract.Thumb;

			CanCreateFeaturedLists = EntryPermissionManager.CanManageFeaturedLists(MvcApplication.LoginManager);

		}

		public bool CanCreateFeaturedLists { get; set; }

		public string CurrentName { get; set; }

		[Display(Name = "Description")]
		[StringLength(2000)]
		public string Description { get; set; }

		[Display(Name = "Featured category")]
		[JsonConverter(typeof(StringEnumConverter))]
		public SongListFeaturedCategory FeaturedCategory { get; set; }

		public int Id { get; set; }

		[Display(Name = "Name")]
		[Required]
		[StringLength(200)]
		public string Name { get; set; }

		[Display(Name = "Songs")]
		public IList<SongInListEditContract> SongLinks { get; set; }

		public EntryThumbContract Thumb { get; set; }

		public SongListForEditContract ToContract() {

			return new SongListForEditContract {
				Description = this.Description ?? string.Empty,
				FeaturedCategory = this.FeaturedCategory,
				Id = this.Id,
				Name = this.Name,
				SongLinks = this.SongLinks.ToArray()
			};

		}

	}

}