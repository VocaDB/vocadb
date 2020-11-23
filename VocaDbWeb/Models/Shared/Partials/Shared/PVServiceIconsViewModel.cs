using VocaDb.Model.Domain.PVs;

namespace VocaDb.Web.Models.Shared.Partials.Shared
{

	public class PVServiceIconsViewModel
	{

		public PVServiceIconsViewModel(PVServices services)
		{
			Services = services;
		}

		public PVServices Services { get; set; }

	}

}