
module vdb.repositories {

	import cls = vdb.models;

	export class RepositoryFactory {
		
		constructor(private urlMapper: vdb.UrlMapper, private lang: cls.globalization.ContentLanguagePreference) { }

		public adminRepository = () => {
			return new AdminRepository(this.urlMapper);
		}

		public albumRepository = () => {
			return new AlbumRepository(this.urlMapper.baseUrl, this.lang);			
		}

		public artistRepository = () => {
			return new ArtistRepository(this.urlMapper.baseUrl, this.lang);
		}

		public entryRepository = () => {
			return new EntryRepository(this.urlMapper.baseUrl);
		}

		public pvRepository = () => {
			return new PVRepository(this.urlMapper);
		}

		public resourceRepository = () => {
			return new ResourceRepository(this.urlMapper.baseUrl);
		}

		public songListRepository = () => {
			return new SongListRepository(this.urlMapper);
		}

		public songRepository = () => {
			return new SongRepository(this.urlMapper.baseUrl, this.lang);
		}

		public tagRepository = () => {
			return new TagRepository(this.urlMapper.baseUrl);
		}

		public userRepository = () => {
			return new UserRepository(this.urlMapper);
		}

	}

}