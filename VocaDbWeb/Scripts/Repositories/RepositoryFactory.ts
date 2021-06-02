import HttpClient from '@Shared/HttpClient';
import HttpClientFactory from '@Shared/HttpClientFactory';
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
	constructor(private readonly httpClientFactory: HttpClientFactory) {}

	private createClient = (baseUrl?: string): HttpClient =>
		this.httpClientFactory.createClient(baseUrl);

	public adminRepository = (baseUrl?: string): AdminRepository =>
		new AdminRepository(this.createClient(baseUrl));

	public albumRepository = (baseUrl?: string): AlbumRepository =>
		new AlbumRepository(this.createClient(baseUrl));

	public artistRepository = (baseUrl?: string): ArtistRepository =>
		new ArtistRepository(this.createClient(baseUrl));

	public discussionRepository = (baseUrl?: string): DiscussionRepository =>
		new DiscussionRepository(this.createClient(baseUrl));

	public entryRepository = (baseUrl?: string): EntryRepository =>
		new EntryRepository(this.createClient(baseUrl));

	public eventRepository = (baseUrl?: string): ReleaseEventRepository =>
		new ReleaseEventRepository(this.createClient(baseUrl));

	public pvRepository = (baseUrl?: string): PVRepository =>
		new PVRepository(this.createClient(baseUrl));

	public resourceRepository = (baseUrl?: string): ResourceRepository =>
		new ResourceRepository(this.createClient(baseUrl));

	public songListRepository = (baseUrl?: string): SongListRepository =>
		new SongListRepository(this.createClient(baseUrl));

	public songRepository = (baseUrl?: string): SongRepository =>
		new SongRepository(this.createClient(baseUrl));

	public tagRepository = (baseUrl?: string): TagRepository =>
		new TagRepository(this.createClient(baseUrl));

	public userRepository = (baseUrl?: string): UserRepository =>
		new UserRepository(this.createClient(baseUrl));

	public venueRepository = (baseUrl?: string): VenueRepository =>
		new VenueRepository(this.createClient(baseUrl));
}
