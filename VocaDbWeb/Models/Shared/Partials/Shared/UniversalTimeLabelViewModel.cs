using System;

namespace VocaDb.Web.Models.Shared.Partials.Shared
{

	public class UniversalTimeLabelViewModel
	{

		public UniversalTimeLabelViewModel(DateTime dateTime)
		{
			DateTime = dateTime;
		}

		public DateTime DateTime { get; set; }

	}

}