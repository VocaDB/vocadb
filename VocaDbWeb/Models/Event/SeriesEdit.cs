using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Web.Code;

namespace VocaDb.Web.Models.Event {

	[PropertyModelBinder]
	public class SeriesEdit {

		public SeriesEdit() { 
			Aliases = new List<string>();
		}

		public SeriesEdit(ReleaseEventSeriesForEditContract contract) {

			ParamIs.NotNull(() => contract);

			Contract = contract;
			Aliases = contract.Aliases.ToList();
			Description = contract.Description;
			Id = contract.Id;
			Name = contract.Name;
			WebLinks = contract.WebLinks;

		}

		public IList<string> Aliases { get; set; }

		public ReleaseEventSeriesForEditContract Contract { get; set; }

		public string Description { get; set; }

		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		[FromJson]
		public WebLinkContract[] WebLinks { get; set; }

		public ReleaseEventSeriesForEditContract ToContract() {

			return new ReleaseEventSeriesForEditContract { 
				Aliases = this.Aliases.ToArray(),
				Description = this.Description ?? string.Empty, 
				Id = this.Id,
				Name = this.Name,
				WebLinks = this.WebLinks
			};

		}

	}
}