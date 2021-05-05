import AdminRepository from './AdminRepository';
import AlbumRepository from './AlbumRepository';
import ArtistRepository from './ArtistRepository';
import ContentLanguagePreference from '../Models/Globalization/ContentLanguagePreference';
import DiscussionRepository from './DiscussionRepository';
import EntryRepository from './EntryRepository';
import PVRepository from './PVRepository';
import ReleaseEventRepository from './ReleaseEventRepository';
import ResourceRepository from './ResourceRepository';
import SongListRepository from './SongListRepository';
import SongRepository from './SongRepository';
import TagRepository from './TagRepository';
import UrlMapper from '../Shared/UrlMapper';
import UserRepository from './UserRepository';
import VenueRepository from './VenueRepository';
import HttpClient from '../Shared/HttpClient';

export default class RepositoryFactory {
  constructor(
    private readonly httpClient: HttpClient,
    private readonly urlMapper: UrlMapper,
    private readonly lang?: ContentLanguagePreference,
    private readonly loggedUserId?: number,
  ) {}

  public adminRepository = () => {
    return new AdminRepository(this.httpClient, this.urlMapper);
  };

  public albumRepository = () => {
    return new AlbumRepository(
      this.httpClient,
      this.urlMapper.baseUrl,
      this.lang,
    );
  };

  public artistRepository = () => {
    return new ArtistRepository(
      this.httpClient,
      this.urlMapper.baseUrl,
      this.lang,
    );
  };

  public discussionRepository = () => {
    return new DiscussionRepository(this.httpClient, this.urlMapper);
  };

  public entryRepository = () => {
    return new EntryRepository(this.httpClient, this.urlMapper.baseUrl);
  };

  public eventRepository = () => {
    return new ReleaseEventRepository(this.httpClient, this.urlMapper);
  };

  public pvRepository = () => {
    return new PVRepository(this.httpClient, this.urlMapper);
  };

  public resourceRepository = () => {
    return new ResourceRepository(this.httpClient, this.urlMapper.baseUrl);
  };

  public songListRepository = () => {
    return new SongListRepository(this.httpClient, this.urlMapper);
  };

  public songRepository = () => {
    return new SongRepository(
      this.httpClient,
      this.urlMapper.baseUrl,
      this.lang,
    );
  };

  public tagRepository = () => {
    return new TagRepository(
      this.httpClient,
      this.urlMapper.baseUrl,
      this.lang,
    );
  };

  public userRepository = () => {
    return new UserRepository(
      this.httpClient,
      this.urlMapper,
      this.loggedUserId,
    );
  };

  public venueRepository = () => {
    return new VenueRepository(this.httpClient, this.urlMapper);
  };
}
