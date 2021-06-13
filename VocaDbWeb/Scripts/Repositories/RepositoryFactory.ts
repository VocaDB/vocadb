import HttpClient from '@Shared/HttpClient';
import VocaDbContext from '@Shared/VocaDbContext';
import { injectable } from 'inversify';
import 'reflect-metadata';

import AdminRepository from './AdminRepository';
import AlbumRepository from './AlbumRepository';
import ArtistRepository from './ArtistRepository';
import DiscussionRepository from './DiscussionRepository';
import EntryRepository from './EntryRepository';
import PVRepository from './PVRepository';
import ReleaseEventRepository from './ReleaseEventRepository';
import ResourceRepository from './ResourceRepository';
import SongListRepository from './SongListRepository';
import SongRepository from './SongRepository';
import TagRepository from './TagRepository';
import UserRepository from './UserRepository';
import VenueRepository from './VenueRepository';

@injectable()
export default class RepositoryFactory {
	private readonly adminRepositories: { [key: string]: AdminRepository } = {};
	private readonly albumRepositories: { [key: string]: AlbumRepository } = {};
	private readonly artistRepositories: { [key: string]: ArtistRepository } = {};
	private readonly discussionRepositories: {
		[key: string]: DiscussionRepository;
	} = {};
	private readonly entryRepositories: { [key: string]: EntryRepository } = {};
	private readonly eventRepositories: {
		[key: string]: ReleaseEventRepository;
	} = {};
	private readonly pvRepositories: { [key: string]: PVRepository } = {};
	private readonly resourceRepositories: {
		[key: string]: ResourceRepository;
	} = {};
	private readonly songListRepositories: {
		[key: string]: SongListRepository;
	} = {};
	private readonly songRepositories: { [key: string]: SongRepository } = {};
	private readonly tagRepositories: { [key: string]: TagRepository } = {};
	private readonly userRepositories: { [key: string]: UserRepository } = {};
	private readonly venueRepositories: { [key: string]: VenueRepository } = {};

	public constructor(
		private readonly vocaDbContext: VocaDbContext,
		private readonly httpClient: HttpClient,
	) {}

	private get baseUrl(): string {
		return this.vocaDbContext.baseAddress;
	}

	public adminRepository = (
		baseUrl: string = this.baseUrl,
	): AdminRepository => {
		if (!this.adminRepositories[baseUrl]) {
			this.adminRepositories[baseUrl] = new AdminRepository(this.httpClient);
		}
		return this.adminRepositories[baseUrl];
	};

	public albumRepository = (
		baseUrl: string = this.baseUrl,
	): AlbumRepository => {
		if (!this.albumRepositories[baseUrl]) {
			this.albumRepositories[baseUrl] = new AlbumRepository(this.httpClient);
		}
		return this.albumRepositories[baseUrl];
	};

	public artistRepository = (
		baseUrl: string = this.baseUrl,
	): ArtistRepository => {
		if (!this.artistRepositories[baseUrl]) {
			this.artistRepositories[baseUrl] = new ArtistRepository(this.httpClient);
		}
		return this.artistRepositories[baseUrl];
	};

	public discussionRepository = (
		baseUrl: string = this.baseUrl,
	): DiscussionRepository => {
		if (!this.discussionRepositories[baseUrl]) {
			this.discussionRepositories[baseUrl] = new DiscussionRepository(
				this.httpClient,
			);
		}
		return this.discussionRepositories[baseUrl];
	};

	public entryRepository = (
		baseUrl: string = this.baseUrl,
	): EntryRepository => {
		if (!this.entryRepositories[baseUrl]) {
			this.entryRepositories[baseUrl] = new EntryRepository(this.httpClient);
		}
		return this.entryRepositories[baseUrl];
	};

	public eventRepository = (
		baseUrl: string = this.baseUrl,
	): ReleaseEventRepository => {
		if (!this.eventRepositories[baseUrl]) {
			this.eventRepositories[baseUrl] = new ReleaseEventRepository(
				this.httpClient,
			);
		}
		return this.eventRepositories[baseUrl];
	};

	public pvRepository = (baseUrl: string = this.baseUrl): PVRepository => {
		if (!this.pvRepositories[baseUrl]) {
			this.pvRepositories[baseUrl] = new PVRepository(this.httpClient);
		}
		return this.pvRepositories[baseUrl];
	};

	public resourceRepository = (
		baseUrl: string = this.baseUrl,
	): ResourceRepository => {
		if (!this.resourceRepositories[baseUrl]) {
			this.resourceRepositories[baseUrl] = new ResourceRepository(
				this.httpClient,
			);
		}
		return this.resourceRepositories[baseUrl];
	};

	public songListRepository = (
		baseUrl: string = this.baseUrl,
	): SongListRepository => {
		if (!this.songListRepositories[baseUrl]) {
			this.songListRepositories[baseUrl] = new SongListRepository(
				this.httpClient,
			);
		}
		return this.songListRepositories[baseUrl];
	};

	public songRepository = (baseUrl: string = this.baseUrl): SongRepository => {
		if (!this.songRepositories[baseUrl]) {
			this.songRepositories[baseUrl] = new SongRepository(this.httpClient);
		}
		return this.songRepositories[baseUrl];
	};

	public tagRepository = (baseUrl: string = this.baseUrl): TagRepository => {
		if (!this.tagRepositories[baseUrl]) {
			this.tagRepositories[baseUrl] = new TagRepository(this.httpClient);
		}
		return this.tagRepositories[baseUrl];
	};

	public userRepository = (baseUrl: string = this.baseUrl): UserRepository => {
		if (!this.userRepositories[baseUrl]) {
			this.userRepositories[baseUrl] = new UserRepository(this.httpClient);
		}
		return this.userRepositories[baseUrl];
	};

	public venueRepository = (
		baseUrl: string = this.baseUrl,
	): VenueRepository => {
		if (!this.venueRepositories[baseUrl]) {
			this.venueRepositories[baseUrl] = new VenueRepository(this.httpClient);
		}
		return this.venueRepositories[baseUrl];
	};
}
