using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Domain {

	public class LinkAlreadyExistsException : Exception {

		public LinkAlreadyExistsException() { }

		public LinkAlreadyExistsException(string message)
			: base(message) { }

	}

}
