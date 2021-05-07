import AlbumRepository from '../../Repositories/AlbumRepository';
import AlbumSearchViewModel from './AlbumSearchViewModel';
import AnythingSearchViewModel from './AnythingSearchViewModel';
import ArtistRepository from '../../Repositories/ArtistRepository';
import ArtistSearchViewModel from './ArtistSearchViewModel';
import ContentLanguagePreference from '../../Models/Globalization/ContentLanguagePreference';
import EntryRepository from '../../Repositories/EntryRepository';
import EventSearchViewModel from './EventSearchViewModel';
import { ISearchCategoryBaseViewModel } from './SearchCategoryBaseViewModel';
import PVPlayersFactory from '../PVs/PVPlayersFactory';
import ReleaseEventRepository from '../../Repositories/ReleaseEventRepository';
import ResourceRepository from '../../Repositories/ResourceRepository';
import ResourcesContract from '../../DataContracts/ResourcesContract';
import ResourcesManager from '../../Models/ResourcesManager';
import SongRepository from '../../Repositories/SongRepository';
import SongSearchViewModel from './SongSearchViewModel';
import Tag from '../../Models/Tags/Tag';
import TagBaseContract from '../../DataContracts/Tag/TagBaseContract';
import TagFilters from './TagFilters';
import TagRepository from '../../Repositories/TagRepository';
import TagSearchViewModel from './TagSearchViewModel';
import UrlMapper from '../../Shared/UrlMapper';
import UserRepository from '../../Repositories/UserRepository';

export default class SearchViewModel {
  constructor(
    urlMapper: UrlMapper,
    entryRepo: EntryRepository,
    artistRepo: ArtistRepository,
    albumRepo: AlbumRepository,
    songRepo: SongRepository,
    eventRepo: ReleaseEventRepository,
    tagRepo: TagRepository,
    resourceRepo: ResourceRepository,
    userRepo: UserRepository,
    unknownPictureUrl: string,
    private languageSelection: string,
    loggedUserId: number,
    cultureCode: string,
    searchType: string,
    searchTerm: string,
    tagIds: number[],
    sort: string,
    artistId: number[],
    childTags: boolean,
    childVoicebanks: boolean,
    eventId: number,
    artistType: string,
    albumType: string,
    songType: string,
    eventCategory: string,
    onlyWithPVs: boolean,
    onlyRatedSongs: boolean,
    since: number,
    minScore: number,
    viewMode: string,
    autoplay: boolean,
    shuffle: boolean,
    pageSize: number,
    pvPlayersFactory: PVPlayersFactory,
  ) {
    this.resourcesManager = new ResourcesManager(resourceRepo, cultureCode);
    this.resources = this.resourcesManager.resources;
    this.tagFilters = new TagFilters(tagRepo, languageSelection);

    if (searchTerm) this.searchTerm(searchTerm);

    var isAlbum = searchType === SearchType.Album;
    var isSong = searchType === SearchType.Song;

    this.anythingSearchViewModel = new AnythingSearchViewModel(
      this,
      languageSelection,
      entryRepo,
    );
    this.artistSearchViewModel = new ArtistSearchViewModel(
      this,
      languageSelection,
      artistRepo,
      loggedUserId,
      artistType,
    );

    this.albumSearchViewModel = new AlbumSearchViewModel(
      this,
      unknownPictureUrl,
      languageSelection,
      albumRepo,
      artistRepo,
      resourceRepo,
      cultureCode,
      isAlbum ? sort : null,
      isAlbum ? artistId : null,
      isAlbum ? childVoicebanks : null,
      albumType,
      isAlbum ? viewMode : null,
    );

    this.eventSearchViewModel = new EventSearchViewModel(
      this,
      ContentLanguagePreference[languageSelection],
      eventRepo,
      artistRepo,
      loggedUserId,
      sort,
      artistId,
      eventCategory,
    );

    this.songSearchViewModel = new SongSearchViewModel(
      this,
      urlMapper,
      languageSelection,
      songRepo,
      artistRepo,
      userRepo,
      eventRepo,
      resourceRepo,
      cultureCode,
      loggedUserId,
      isSong ? sort : null,
      isSong ? artistId : null,
      isSong ? childVoicebanks : null,
      songType,
      eventId,
      onlyWithPVs,
      onlyRatedSongs,
      since,
      isSong ? minScore : null,
      isSong ? viewMode : null,
      autoplay,
      shuffle,
      pvPlayersFactory,
    );

    this.tagSearchViewModel = new TagSearchViewModel(
      this,
      ContentLanguagePreference[languageSelection],
      tagRepo,
    );

    if (
      tagIds != null ||
      !!artistId ||
      !!eventId ||
      artistType ||
      albumType ||
      songType ||
      eventCategory ||
      onlyWithPVs != null ||
      since ||
      minScore
    )
      this.showAdvancedFilters(true);

    if (searchType) this.searchType(searchType);

    if (tagIds) this.tagFilters.addTags(tagIds);

    if (tagIds && childTags) this.tagFilters.childTags(childTags);

    if (pageSize) this.pageSize(pageSize);

    this.pageSize.subscribe(this.updateResults);
    this.searchTerm.subscribe(this.updateResults);
    this.tagFilters.filters.subscribe(this.updateResults);
    this.draftsOnly.subscribe(this.updateResults);
    this.showTags.subscribe(this.updateResults);

    this.showAnythingSearch = ko.computed(
      () => this.searchType() === SearchType.Anything,
    );
    this.showArtistSearch = ko.computed(
      () => this.searchType() === SearchType.Artist,
    );
    this.showAlbumSearch = ko.computed(
      () => this.searchType() === SearchType.Album,
    );
    this.showSongSearch = ko.computed(
      () => this.searchType() === SearchType.Song,
    );

    this.searchType.subscribe((val) => {
      this.updateResults();
      this.currentSearchType(val);
    });

    resourceRepo
      .getList(cultureCode, [
        'albumSortRuleNames',
        'artistSortRuleNames',
        'artistTypeNames',
        'discTypeNames',
        'eventCategoryNames',
        'eventSortRuleNames',
        'entryTypeNames',
        'songSortRuleNames',
        'songTypeNames',
      ])
      .then((resources) => {
        this.resources(resources);
        this.updateResults();
      });

    tagRepo
      .getTopTags(languageSelection, Tag.commonCategory_Genres, null)
      .then((result) => {
        this.genreTags(result);
      });
  }

  public albumSearchViewModel: AlbumSearchViewModel;
  public anythingSearchViewModel: AnythingSearchViewModel;
  public artistSearchViewModel: ArtistSearchViewModel;
  public eventSearchViewModel: EventSearchViewModel;
  public songSearchViewModel: SongSearchViewModel;
  public tagSearchViewModel: TagSearchViewModel;

  private currentSearchType = ko.observable(SearchType.Anything);
  public draftsOnly = ko.observable(false);
  public genreTags = ko.observableArray<TagBaseContract>();
  public pageSize = ko.observable(10);
  public resourcesManager: ResourcesManager;
  public resources: KnockoutObservable<ResourcesContract>;
  public showAdvancedFilters = ko.observable(false);
  public searchTerm = ko
    .observable('')
    .extend({ rateLimit: { timeout: 300, method: 'notifyWhenChangesStop' } });
  public searchType = ko.observable(SearchType.Anything);
  public tagFilters: TagFilters;

  public showAnythingSearch: KnockoutComputed<boolean>;
  public showArtistSearch: KnockoutComputed<boolean>;
  public showAlbumSearch: KnockoutComputed<boolean>;
  public showEventSearch = ko.computed(
    () => this.searchType() === SearchType.ReleaseEvent,
  );
  public showSongSearch: KnockoutComputed<boolean>;
  public showTagSearch = ko.computed(
    () => this.searchType() === SearchType.Tag,
  );
  public showTagFilter = ko.computed(() => !this.showTagSearch());
  public showTags = ko.observable(false);
  public showDraftsFilter = ko.computed(
    () => this.searchType() !== SearchType.Tag,
  );

  public isUniversalSearch = ko.computed(
    () => this.searchType() === SearchType.Anything,
  );

  public currentCategoryViewModel = (): ISearchCategoryBaseViewModel => {
    switch (this.searchType()) {
      case SearchType.Anything:
        return this.anythingSearchViewModel;
      case SearchType.Artist:
        return this.artistSearchViewModel;
      case SearchType.Album:
        return this.albumSearchViewModel;
      case SearchType.ReleaseEvent:
        return this.eventSearchViewModel;
      case SearchType.Song:
        return this.songSearchViewModel;
      case SearchType.Tag:
        return this.tagSearchViewModel;
      default:
        return null;
    }
  };

  public updateResults = (): void => {
    var vm = this.currentCategoryViewModel();

    if (vm != null) vm.updateResultsWithTotalCount();
  };
}

class SearchType {
  public static Anything = 'Anything';
  public static Artist = 'Artist';
  public static Album = 'Album';
  public static ReleaseEvent = 'ReleaseEvent';
  public static Song = 'Song';
  public static Tag = 'Tag';
}
