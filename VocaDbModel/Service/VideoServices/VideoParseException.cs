using System;

namespace VocaDb.Model.Service.VideoServices
{
	public class VideoParseException : Exception
	{
		public VideoParseException() { }

		public VideoParseException(string? message)
			: base(message) { }

		public VideoParseException(string? message, Exception? innerException)
			: base(message, innerException) { }
	}
}
