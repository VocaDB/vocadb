namespace VocaDb.Model.Service.VideoServices.PiaproClient;

using System;
using System.Runtime.Serialization;

/// <summary>
/// Exception thrown if the query failed.
/// </summary>
public class PiaproException : Exception
{
	public PiaproException()
	{
	}

	public PiaproException(string message) : base(message)
	{
	}

	public PiaproException(string message, Exception innerException) : base(message, innerException)
	{
	}

	protected PiaproException(SerializationInfo info, StreamingContext context) : base(info, context)
	{
	}
}