import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';

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
  constructor(
    private readonly httpClient: HttpClient,
    private readonly urlMapper: UrlMapper,
    private readonly lang?: ContentLanguagePreference,
    private readonly loggedUserId?: number,
  ) {}

  public adminRepository = (): AdminRepository => {
    return new AdminRepository(this.httpClient, this.urlMapper);
  };

  public albumRepository = (): AlbumRepository => {
    return new AlbumRepository(
      this.httpClient,
      this.urlMapper.baseUrl,
      this.lang,
    );
  };

  public artistRepository = (): ArtistRepository => {
    return new ArtistRepository(
      this.httpClient,
      this.urlMapper.baseUrl,
      this.lang,
    );
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
    return new SongRepository(
      this.httpClient,
      this.urlMapper.baseUrl,
      this.lang,
    );
  };

  public tagRepository = (): TagRepository => {
    return new TagRepository(
      this.httpClient,
      this.urlMapper.baseUrl,
      this.lang,
    );
  };

  public userRepository = (): UserRepository => {
    return new UserRepository(
      this.httpClient,
      this.urlMapper,
      this.loggedUserId,
    );
  };

  public venueRepository = (): VenueRepository => {
    return new VenueRepository(this.httpClient, this.urlMapper);
  };
}
