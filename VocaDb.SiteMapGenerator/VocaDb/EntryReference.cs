
namespace VocaDb.SiteMapGenerator.VocaDb {

	public struct EntryReference {
		
		public EntryReference(int id, string urlSlug = null) 
			: this() {
			Id = id;
			UrlSlug = urlSlug;
		}

		public int Id { get; set; }
		public string UrlSlug { get; set; }

	}

}
