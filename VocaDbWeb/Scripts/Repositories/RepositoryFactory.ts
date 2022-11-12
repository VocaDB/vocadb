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
	constructor(
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
	) {}

	adminRepository = (): AdminRepository => {
		return new AdminRepository(this.httpClient, this.urlMapper);
	};

	albumRepository = (): AlbumRepository => {
		return new AlbumRepository(this.httpClient, this.urlMapper.baseUrl);
	};

	artistRepository = (): ArtistRepository => {
		return new ArtistRepository(this.httpClient, this.urlMapper.baseUrl);
	};

	discussionRepository = (): DiscussionRepository => {
		return new DiscussionRepository(this.httpClient, this.urlMapper);
	};

	entryRepository = (): EntryRepository => {
		return new EntryRepository(this.httpClient, this.urlMapper.baseUrl);
	};

	eventRepository = (): ReleaseEventRepository => {
		return new ReleaseEventRepository(this.httpClient, this.urlMapper);
	};

	pvRepository = (): PVRepository => {
		return new PVRepository(this.httpClient, this.urlMapper);
	};

	resourceRepository = (): ResourceRepository => {
		return new ResourceRepository(this.httpClient, this.urlMapper.baseUrl);
	};

	songListRepository = (): SongListRepository => {
		return new SongListRepository(this.httpClient, this.urlMapper);
	};

	songRepository = (): SongRepository => {
		return new SongRepository(this.httpClient, this.urlMapper.baseUrl);
	};

	tagRepository = (): TagRepository => {
		return new TagRepository(this.httpClient, this.urlMapper.baseUrl);
	};

	userRepository = (): UserRepository => {
		return new UserRepository(this.httpClient, this.urlMapper);
	};

	venueRepository = (): VenueRepository => {
		return new VenueRepository(this.httpClient, this.urlMapper);
	};
}
