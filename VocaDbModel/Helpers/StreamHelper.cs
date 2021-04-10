using System;
using System.IO;

namespace VocaDb.Model.Helpers
{
	public static class StreamHelper
	{
		public static void CopyStream(Stream source, Stream target)
		{
			const int bufSize = 1024;
			byte[] buf = new byte[bufSize];
			int bytesRead = 0;
			while ((bytesRead = source.Read(buf, 0, bufSize)) > 0)
				target.Write(buf, 0, bytesRead);
		}

		/// <summary>
		/// Reads a stream completely at once and returns the contents as raw byte array.
		/// The stream length must be known.
		/// 
		/// Meant only for small-ish streams (a few megabytes).
		/// The stream will be rewinded to beginning first, if possible.
		/// </summary>
		/// <param name="input">Input stream. Cannot be null.</param>
		/// <returns>Stream bytes. Cannot be null.</returns>
		/// <exception cref="NotSupportedException">If the stream length is not known.</exception>
		public static byte[] ReadStream(Stream input)
		{
			if (input.CanSeek && input.Position > 0)
				input.Position = 0;

			// Fallback if the stream is too large
			if (input.Length >= int.MaxValue)
				return ReadStream(input, input.Length);

			var bytes = new byte[input.Length];
			input.Read(bytes, 0, (int)input.Length);
			return bytes;
		}

		public static byte[] ReadStream(Stream input, long length)
		{
			if (input.CanSeek && input.Position > 0)
				input.Position = 0;

			int buffer = 1024;
			var buf = new byte[buffer];
			var wholeBuf = new byte[length];

			int count = 0;
			int offset = 0;
			do
			{
				count = input.Read(buf, 0, buffer);
				Array.Copy(buf, 0, wholeBuf, offset, count);
				offset += count;
			}
			while (count != 0);

			return wholeBuf;
		}
	}
}
