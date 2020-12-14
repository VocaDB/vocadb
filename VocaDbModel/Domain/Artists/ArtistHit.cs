#nullable disable

namespace VocaDb.Model.Domain.Artists
{
	public class ArtistHit : GenericEntryHit<Artist>
	{
		public ArtistHit() { }
		public ArtistHit(Artist entry, int agent) : base(entry, agent) { }
	}
}
