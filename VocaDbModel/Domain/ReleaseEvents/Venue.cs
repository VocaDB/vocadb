using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.ReleaseEvents {

	public class Venue {

		public virtual string Address { get; set; }

		public virtual string Country { get; set; }

		public virtual int Id { get; set; }

		public virtual decimal Latitude { get; set; }

		public virtual decimal Longitude { get; set; }

		public virtual TranslatedString Name { get; set; }

	}

}
