namespace VocaDb.Model.Domain {

	public static class Constants {

		/// <summary>
		/// For some entry types we want to prevent users from uploading too large images.
		/// 600px is the minimum recommended size for Facebook.
		/// </summary>
		public const int RestrictedImageOriginalSize = 600;

		/// <summary>
		/// 24h
		/// </summary>
		public const int SecondsInADay = 86400;

	}

}
