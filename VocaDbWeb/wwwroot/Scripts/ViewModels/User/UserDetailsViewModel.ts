import CommentContract from '@DataContracts/CommentContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import ReleaseEventContract from '@DataContracts/ReleaseEvents/ReleaseEventContract';
import SongListContract from '@DataContracts/Song/SongListContract';
import HighchartsHelper from '@Helpers/HighchartsHelper';
import UserEventRelationshipType from '@Models/Users/UserEventRelationshipType';
import AdminRepository from '@Repositories/AdminRepository';
import ResourceRepository from '@Repositories/ResourceRepository';
import TagRepository from '@Repositories/TagRepository';
import UserRepository from '@Repositories/UserRepository';
import ui from '@Shared/MessagesTyped';
import UrlMapper from '@Shared/UrlMapper';
import { Options } from 'highcharts';
import $ from 'jquery';

import DeleteEntryViewModel from '../DeleteEntryViewModel';
import EditableCommentsViewModel from '../EditableCommentsViewModel';
import SongListsBaseViewModel from '../SongList/SongListsBaseViewModel';
import AlbumCollectionViewModel from './AlbumCollectionViewModel';
import FollowedArtistsViewModel from './FollowedArtistsViewModel';
import RatedSongsSearchViewModel from './RatedSongsSearchViewModel';

export default class UserDetailsViewModel {
  private static overview = 'Overview';

  public addBan = (): void => {
    this.adminRepo
      .addIpToBanList({ address: this.lastLoginAddress, notes: this.name })
      .then((result) => {
        if (result) {
          ui.showSuccessMessage('Added to ban list');
        } else {
          ui.showErrorMessage('Already in the ban list');
        }
      });
  };

  public checkSFS = (): void => {
    this.adminRepo.checkSFS(this.lastLoginAddress, (html) => {
      $('#sfsCheckDialog').html(html);
      $('#sfsCheckDialog').dialog('open');
    });
  };

  public comments: EditableCommentsViewModel;
  private eventsLoaded = false;
  public events = ko.observableArray<ReleaseEventContract>([]);
  public eventsType = ko.observable(
    UserEventRelationshipType[UserEventRelationshipType.Attending],
  );

  public limitedUserViewModel = new DeleteEntryViewModel((notes) => {
    $.postJSON(
      this.urlMapper.mapRelative(
        'api/users/' + this.userId + '/status-limited',
      ),
      { reason: notes, createReport: true },
      () => {
        window.location.reload();
      },
      'json',
    );
  });

  public reportUserViewModel = new DeleteEntryViewModel((notes) => {
    $.postJSON(
      this.urlMapper.mapRelative('api/users/' + this.userId + '/reports'),
      { reason: notes, reportType: 'Spamming' },
      () => {
        ui.showSuccessMessage(vdb.resources.shared.reportSent);
        this.reportUserViewModel.notes('');
      },
      'json',
    );
  }, true);

  public initComments = (): void => {
    this.comments.initComments();
  };

  private initEvents = (): void => {
    if (this.eventsLoaded) {
      return;
    }

    this.loadEvents();
    this.eventsLoaded = true;
  };

  private loadEvents = (): void => {
    this.userRepo
      .getEvents(
        this.userId,
        UserEventRelationshipType[
          this.eventsType() as keyof typeof UserEventRelationshipType
        ],
      )
      .then((events) => {
        this.events(events);
      });
  };

  private name!: string;
  public ratingsByGenreChart = ko.observable<Options>(null!);

  public view = ko.observable(UserDetailsViewModel.overview);

  private initializeView = (viewName: string): void => {
    switch (viewName) {
      case 'Albums':
        this.albumCollectionViewModel.init();
        break;
      case 'Artists':
        this.followedArtistsViewModel.init();
        break;
      case 'Comments':
        this.initComments();
        break;
      case 'CustomLists':
        this.songLists.init();
        break;
      case 'Songs':
        this.ratedSongsViewModel.init();
        break;
      case 'Events':
        this.initEvents();
        break;
    }
  };

  public setView = (viewName: string): void => {
    if (!viewName) viewName = UserDetailsViewModel.overview;

    this.initializeView(viewName);

    window.scrollTo(0, 0);
    window.location.hash =
      viewName !== UserDetailsViewModel.overview ? viewName : '';
    this.view(viewName);
  };

  public setOverview = (): void => this.setView('Overview');
  public setViewAlbums = (): void => this.setView('Albums');
  public setViewArtists = (): void => this.setView('Artists');
  public setComments = (): void => this.setView('Comments');
  public setCustomLists = (): void => this.setView('CustomLists');
  public setViewSongs = (): void => this.setView('Songs');
  public setViewEvents = (): void => this.setView('Events');

  public songLists: UserSongListsViewModel;

  constructor(
    private readonly userId: number,
    cultureCode: string,
    private loggedUserId: number,
    private lastLoginAddress: string,
    private canEditAllComments: boolean,
    private urlMapper: UrlMapper,
    private userRepo: UserRepository,
    private adminRepo: AdminRepository,
    resourceRepo: ResourceRepository,
    tagRepo: TagRepository,
    languageSelection: string,
    public followedArtistsViewModel: FollowedArtistsViewModel,
    public albumCollectionViewModel: AlbumCollectionViewModel,
    public ratedSongsViewModel: RatedSongsSearchViewModel,
    latestComments: CommentContract[],
  ) {
    var canDeleteAllComments = userId === loggedUserId;

    this.comments = new EditableCommentsViewModel(
      userRepo,
      userId,
      loggedUserId,
      canDeleteAllComments,
      canEditAllComments,
      false,
      latestComments,
      true,
    );
    this.songLists = new UserSongListsViewModel(
      userId,
      userRepo,
      resourceRepo,
      tagRepo,
      languageSelection,
      cultureCode,
    );

    window.onhashchange = (): void => {
      if (window.location.hash && window.location.hash.length >= 1)
        this.setView(window.location.hash.substr(1));
    };

    userRepo.getRatingsByGenre(userId).then((data) => {
      this.ratingsByGenreChart(
        HighchartsHelper.simplePieChart(null!, 'Songs', data),
      );
    });

    userRepo.getOne(userId, null!).then((data) => {
      this.name = data.name!;
    });

    this.eventsType.subscribe(this.loadEvents);
  }
}

export class UserSongListsViewModel extends SongListsBaseViewModel {
  constructor(
    private readonly userId: number,
    private readonly userRepo: UserRepository,
    resourceRepo: ResourceRepository,
    tagRepo: TagRepository,
    languageSelection: string,
    cultureCode: string,
  ) {
    super(resourceRepo, tagRepo, languageSelection, cultureCode, [], true);
  }

  public loadMoreItems = (
    callback: (result: PartialFindResultContract<SongListContract>) => void,
  ): void => {
    this.userRepo
      .getSongLists(
        this.userId,
        this.query(),
        { start: this.start, maxEntries: 50, getTotalCount: true },
        this.tagFilters.tagIds(),
        this.sort(),
        this.fields(),
      )
      .then(callback);
  };
}
