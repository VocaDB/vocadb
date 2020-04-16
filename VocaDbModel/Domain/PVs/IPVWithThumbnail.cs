namespace VocaDb.Model.Domain.PVs {

	/// <summary>
	/// PV with thumbnail.
	/// </summary>
	public interface IPVWithThumbnail : IPV {

		/// <summary>
		/// Thumbnail URL. Can be null or empty.
		/// </summary>
		string ThumbUrl { get; }

	}

	public static class IPVWithThumbnailExtensions {
		public static VocaDbUrl VocaDbThumbUrl(this IPVWithThumbnail pv) => VocaDbUrl.External(pv.ThumbUrl);
	}

}
