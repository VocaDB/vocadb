import HttpClient from '@Shared/HttpClient';

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

export default class RepositoryFactory {
  constructor(private readonly httpClient: HttpClient) {}

  public adminRepository = (): AdminRepository => {
    return new AdminRepository(this.httpClient);
  };

  public albumRepository = (): AlbumRepository => {
    return new AlbumRepository(this.httpClient);
  };

  public artistRepository = (): ArtistRepository => {
    return new ArtistRepository(this.httpClient);
  };

  public discussionRepository = (): DiscussionRepository => {
    return new DiscussionRepository(this.httpClient);
  };

  public entryRepository = (): EntryRepository => {
    return new EntryRepository(this.httpClient);
  };

  public eventRepository = (): ReleaseEventRepository => {
    return new ReleaseEventRepository(this.httpClient);
  };

  public pvRepository = (): PVRepository => {
    return new PVRepository(this.httpClient);
  };

  public resourceRepository = (): ResourceRepository => {
    return new ResourceRepository(this.httpClient);
  };

  public songListRepository = (): SongListRepository => {
    return new SongListRepository(this.httpClient);
  };

  public songRepository = (): SongRepository => {
    return new SongRepository(this.httpClient);
  };

  public tagRepository = (): TagRepository => {
    return new TagRepository(this.httpClient);
  };

  public userRepository = (): UserRepository => {
    return new UserRepository(this.httpClient);
  };

  public venueRepository = (): VenueRepository => {
    return new VenueRepository(this.httpClient);
  };
}
