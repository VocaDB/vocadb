namespace VocaDb.Model.Domain.Images
{

	public enum ImagePurpose
	{
		/// <summary>
		/// Unspecified/doesn't matter
		/// </summary>
		Unspesified,
		/// <summary>
		/// Main picture. There should be only one main picture per entry.
		/// </summary>
		Main,
		/// <summary>
		/// Additional picture. There can be multiple additional pictures per entry.
		/// </summary>
		Additional
	}

}
