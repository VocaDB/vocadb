namespace VocaDb.Model.DataContracts.Artists
{

	/// <summary>
	/// Advanced artist statistics. These are updated less frequently.
	/// </summary>
	public class AdvancedArtistStatsContract
	{

		/// <summary>
		/// "Mostly uses"
		/// </summary>
		public TopStatContract<ArtistContract>[] TopVocaloids { get; set; }

	}

	public class TopStatContract<T>
	{

		public int Count { get; set; }

		public T Data { get; set; }

	}

}
