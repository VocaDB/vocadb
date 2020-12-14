#nullable disable

using System;

namespace VocaDb.Model.Service.Exceptions
{
	public class NoPasswordException : Exception
	{
		public NoPasswordException() { }
		public NoPasswordException(string message) : base(message) { }
	}
}
