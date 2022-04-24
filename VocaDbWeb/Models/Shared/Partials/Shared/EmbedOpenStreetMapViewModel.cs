#nullable disable

using VocaDb.Model.DataContracts;

namespace VocaDb.Web.Models.Shared.Partials.Shared
{
	public class EmbedOpenStreetMapViewModel
	{
		public EmbedOpenStreetMapViewModel(OptionalGeoPointContract coordinates)
		{
			Coordinates = coordinates;
		}

		public OptionalGeoPointContract Coordinates { get; set; }
	}
}