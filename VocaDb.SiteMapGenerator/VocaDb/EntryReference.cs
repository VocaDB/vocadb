
namespace VocaDb.SiteMapGenerator.VocaDb {

	public readonly struct EntryReference {
		
		public EntryReference(int id, string urlSlug = null) 
			: this() {
			Id = id;
			UrlSlug = urlSlug;
		}

		public int Id { get; }
		public string UrlSlug { get; }

	}

}
