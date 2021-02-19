#nullable disable

namespace VocaDb.Model.Domain
{
	public static class Constants
	{
		/// <summary>
		/// For some entry types we want to prevent users from uploading too large images.
		/// 600px is the minimum recommended size for Facebook.
		/// </summary>
		public const int RestrictedImageOriginalSize = 600;

		/// <summary>
		/// 24h
		/// </summary>
		public const int SecondsInADay = 86400;

		public const int AbsoluteMinMilliBpm = 20 * 1000;

		public const int AbsoluteMaxMilliBpm = 400 * 1000;

		public const decimal MilliBpmStep = 0.01M * 1000;
	}
}
