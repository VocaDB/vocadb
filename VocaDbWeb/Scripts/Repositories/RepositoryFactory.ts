import { AdminRepository } from '@/Repositories/AdminRepository';
import { AlbumRepository } from '@/Repositories/AlbumRepository';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { DiscussionRepository } from '@/Repositories/DiscussionRepository';
import { EntryRepository } from '@/Repositories/EntryRepository';
import { PVRepository } from '@/Repositories/PVRepository';
import { ReleaseEventRepository } from '@/Repositories/ReleaseEventRepository';
import { ResourceRepository } from '@/Repositories/ResourceRepository';
import { SongListRepository } from '@/Repositories/SongListRepository';
import { SongRepository } from '@/Repositories/SongRepository';
import { TagRepository } from '@/Repositories/TagRepository';
import { UserRepository } from '@/Repositories/UserRepository';
import { VenueRepository } from '@/Repositories/VenueRepository';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';

export class RepositoryFactory {
	public constructor(
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
	) {}

	public adminRepository = (): AdminRepository => {
		return new AdminRepository(this.httpClient, this.urlMapper);
	};

	public albumRepository = (): AlbumRepository => {
		return new AlbumRepository(this.httpClient, this.urlMapper.baseUrl);
	};

	public artistRepository = (): ArtistRepository => {
		return new ArtistRepository(this.httpClient, this.urlMapper.baseUrl);
	};

	public discussionRepository = (): DiscussionRepository => {
		return new DiscussionRepository(this.httpClient, this.urlMapper);
	};

	public entryRepository = (): EntryRepository => {
		return new EntryRepository(this.httpClient, this.urlMapper.baseUrl);
	};

	public eventRepository = (): ReleaseEventRepository => {
		return new ReleaseEventRepository(this.httpClient, this.urlMapper);
	};

	public pvRepository = (): PVRepository => {
		return new PVRepository(this.httpClient, this.urlMapper);
	};

	public resourceRepository = (): ResourceRepository => {
		return new ResourceRepository(this.httpClient, this.urlMapper.baseUrl);
	};

	public songListRepository = (): SongListRepository => {
		return new SongListRepository(this.httpClient, this.urlMapper);
	};

	public songRepository = (): SongRepository => {
		return new SongRepository(this.httpClient, this.urlMapper.baseUrl);
	};

	public tagRepository = (): TagRepository => {
		return new TagRepository(this.httpClient, this.urlMapper.baseUrl);
	};

	public userRepository = (): UserRepository => {
		return new UserRepository(this.httpClient, this.urlMapper);
	};

	public venueRepository = (): VenueRepository => {
		return new VenueRepository(this.httpClient, this.urlMapper);
	};
}
