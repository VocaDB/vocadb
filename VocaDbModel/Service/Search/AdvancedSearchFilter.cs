using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VocaDb.Model.Service.Search {

	public class AdvancedSearchFilter {

		public AdvancedFilterType FilterType { get; set; }

		public bool Negate { get; set; }

		public string Param { get; set; }

	}

}
