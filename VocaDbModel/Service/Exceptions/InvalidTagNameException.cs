using System;

namespace VocaDb.Model.Service.Exceptions {

	public class InvalidTagNameException : Exception {

		public InvalidTagNameException(string name) 
			: base(string.Format("{0} is not a valid name for a tag - tag name must contain only word characters", name)) {}

	}
}
