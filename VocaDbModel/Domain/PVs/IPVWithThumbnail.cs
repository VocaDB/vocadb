#nullable disable

namespace VocaDb.Model.Domain.PVs
{
	/// <summary>
	/// PV with thumbnail.
	/// </summary>
	public interface IPVWithThumbnail : IPV
	{
		/// <summary>
		/// Thumbnail URL. Can be null or empty.
		/// </summary>
		string ThumbUrl { get; }
	}
}
