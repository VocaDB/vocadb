using System.Collections.Generic;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Web.Models.Shared.Partials.Shared
{

	public class EventThumbsViewModel
	{

		public EventThumbsViewModel(IEnumerable<ReleaseEventForApiContract> events, ImageSize imageSize = ImageSize.SmallThumb)
		{
			Events = events;
			ImageSize = imageSize;
		}

		public IEnumerable<ReleaseEventForApiContract> Events { get; set; }

		public ImageSize ImageSize { get; set; }

	}

}