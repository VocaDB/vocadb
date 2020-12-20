#nullable disable

using VocaDb.Model.DataContracts;

namespace VocaDb.Web.Models.Shared.Partials.Shared
{
	public class EmbedGoogleMapsViewModel
	{
		public EmbedGoogleMapsViewModel(OptionalGeoPointContract coordinates)
		{
			Coordinates = coordinates;
		}

		public OptionalGeoPointContract Coordinates { get; set; }
	}
}