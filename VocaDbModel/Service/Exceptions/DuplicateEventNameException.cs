using System;

namespace VocaDb.Model.Service.Exceptions {

	public class DuplicateEventNameException : Exception {

		public DuplicateEventNameException(string name)
			: base("Event name already in use: " + name) { }

	}

}
