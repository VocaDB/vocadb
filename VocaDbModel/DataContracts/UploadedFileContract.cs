#nullable disable

using System.IO;

namespace VocaDb.Model.DataContracts
{
	/// <summary>
	/// Data contract for an uploaded file.
	/// </summary>
	public class UploadedFileContract
	{
		/// <summary>
		/// MIME type. Can be null or empty, although shouldn't be for any of the known file types.
		/// </summary>
		public string Mime { get; init; }

		/// <summary>
		/// Data stream. Cannot be null.
		/// </summary>
		public Stream Stream { get; init; }
	}
}
