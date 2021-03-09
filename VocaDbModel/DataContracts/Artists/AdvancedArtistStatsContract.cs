#nullable disable

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
		public TopStatContract<ArtistContract>[] TopVocaloids { get; init; }
	}

	public class TopStatContract<T>
	{
		public int Count { get; init; }

		public T Data { get; init; }
	}
}
