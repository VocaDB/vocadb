using System;

namespace VocaDb.Model.Service.Exceptions {

	public class TagNameAlreadyInUseException : Exception {

		public TagNameAlreadyInUseException(string name)
			: base("Tag name already in use: " + name) {}

	}

}
