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
	public class SongListEditViewModel {

		public SongListEditViewModel() {
			SongLinks = new List<SongInListEditContract>();
		}

		public SongListEditViewModel(SongListContract contract, IUserPermissionContext permissionContext)
			: this() {

			ParamIs.NotNull(() => contract);

			CurrentName = contract.Name;
			Description = contract.Description;
			FeaturedCategory = contract.FeaturedCategory;
			Id = contract.Id;
			Name = contract.Name;
			Thumb = contract.Thumb;

			CanCreateFeaturedLists = EntryPermissionManager.CanManageFeaturedLists(permissionContext);

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