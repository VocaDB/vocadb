using VocaDb.Model.Domain.PVs;

namespace VocaDb.Web.Models.Shared.Partials.Shared
{
	public class PVServiceIconViewModel
	{
		public PVServiceIconViewModel(PVService service)
		{
			Service = service;
		}

		public PVService Service { get; set; }
	}
}