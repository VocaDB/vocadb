namespace VocaDb.Model.Service.Search.Artists {

	// Might wanna move this to client side, would require re-implementing lots of code though...
	public class ParsedArtistQuery {

		public string ExternalLinkUrl { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

		public bool HasNameQuery {
			get {
				return !string.IsNullOrEmpty(Name);
			}
		}

	}

}
