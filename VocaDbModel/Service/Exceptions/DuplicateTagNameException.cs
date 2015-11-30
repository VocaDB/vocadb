using System;

namespace VocaDb.Model.Service.Exceptions {

	public class DuplicateTagNameException : Exception {

		public DuplicateTagNameException(string name)
			: base("Tag name already in use: " + name) {}

	}

}
