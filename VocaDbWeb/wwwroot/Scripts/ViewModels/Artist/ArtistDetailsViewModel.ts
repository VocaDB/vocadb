import CountPerDayContract from '@DataContracts/Aggregate/CountPerDayContract';
import CommentContract from '@DataContracts/CommentContract';
import TagUsageForApiContract from '@DataContracts/Tag/TagUsageForApiContract';
import HighchartsHelper from '@Helpers/HighchartsHelper';
import TimeUnit from '@Models/Aggregate/TimeUnit';
import EntryType from '@Models/EntryType';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import AlbumRepository from '@Repositories/AlbumRepository';
import ArtistRepository from '@Repositories/ArtistRepository';
import ResourceRepository from '@Repositories/ResourceRepository';
import SongRepository from '@Repositories/SongRepository';
import UserRepository from '@Repositories/UserRepository';
import ui from '@Shared/MessagesTyped';
import UrlMapper from '@Shared/UrlMapper';

import EditableCommentsViewModel from '../EditableCommentsViewModel';
import EnglishTranslatedStringViewModel from '../Globalization/EnglishTranslatedStringViewModel';
import PVPlayersFactory from '../PVs/PVPlayersFactory';
import { IEntryReportType } from '../ReportEntryViewModel';
import ReportEntryViewModel from '../ReportEntryViewModel';
import AlbumSearchViewModel from '../Search/AlbumSearchViewModel';
import SongSearchViewModel from '../Search/SongSearchViewModel';
import TagListViewModel from '../Tag/TagListViewModel';
import TagsEditViewModel from '../Tag/TagsEditViewModel';

export default class ArtistDetailsViewModel {
  constructor(
    repo: ArtistRepository,
    private artistId: number,
    tagUsages: TagUsageForApiContract[],
    hasSubscription: boolean,
    emailNotifications: boolean,
    siteNotifications: boolean,
    hasEnglishDescription: boolean,
    private unknownPictureUrl: string,
    languagePreference: ContentLanguagePreference,
    private urlMapper: UrlMapper,
    private albumRepo: AlbumRepository,
    private songRepo: SongRepository,
    private resourceRepo: ResourceRepository,
    private userRepository: UserRepository,
    private cultureCode: string,
    reportTypes: IEntryReportType[],
    private loggedUserId: number,
    canDeleteAllComments: boolean,
    private pvPlayersFactory: PVPlayersFactory,
    latestComments: CommentContract[],
  ) {
    this.lang = ContentLanguagePreference[languagePreference];
    this.hasArtistSubscription = ko.observable(hasSubscription);
    this.customizeSubscriptionDialog = new CustomizeArtistSubscriptionViewModel(
      artistId,
      emailNotifications,
      siteNotifications,
      userRepository,
    );
    this.description = new EnglishTranslatedStringViewModel(
      hasEnglishDescription &&
        (languagePreference === ContentLanguagePreference.English ||
          languagePreference === ContentLanguagePreference.Romaji),
    );

    this.comments = new EditableCommentsViewModel(
      repo,
      artistId,
      loggedUserId,
      canDeleteAllComments,
      canDeleteAllComments,
      false,
      latestComments,
      true,
    );

    this.tagsEditViewModel = new TagsEditViewModel(
      {
        getTagSelections: (callback): Promise<void> =>
          userRepository.getArtistTagSelections(artistId).then(callback),
        saveTagSelections: (tags): void =>
          userRepository.updateArtistTags(
            artistId,
            tags,
            this.tagUsages.updateTagUsages,
          ),
      },
      EntryType.Artist,
      (callback) => repo.getTagSuggestions(this.artistId).then(callback),
    );

    this.tagUsages = new TagListViewModel(tagUsages);

    this.reportViewModel = new ReportEntryViewModel(
      reportTypes,
      (reportType, notes) => {
        repo.createReport(this.artistId, reportType, notes, null!);

        ui.showSuccessMessage(vdb.resources.shared.reportSent);
      },
    );

    this.loadHighcharts();
  }

  public addFollowedArtist = (): void => {
    this.userRepository.createArtistSubscription(this.artistId, () => {
      this.hasArtistSubscription(true);
      this.customizeSubscriptionDialog.notificationsMethod('Site');
    });
  };

  public comments: EditableCommentsViewModel;

  customizeSubscriptionDialog: CustomizeArtistSubscriptionViewModel;

  public hasArtistSubscription: KnockoutObservable<boolean>;

  private lang: string;

  private loadHighcharts = (): void => {
    // Delayed load highcharts stuff
    var highchartsPromise = $.getScript(
      this.urlMapper.mapRelative('scripts/highcharts/4.2.0/highcharts.js'),
    );
    var songsPerMonthDataPromise = this.songRepo.getOverTime(
      TimeUnit.month,
      this.artistId,
    );

    $.when(songsPerMonthDataPromise, highchartsPromise).done(
      (songsPerMonthData: JQueryPromiseCallback<CountPerDayContract[]>) => {
        var points: CountPerDayContract[] =
          songsPerMonthData[
            0 as keyof JQueryPromiseCallback<CountPerDayContract[]>
          ];

        // Need at least 2 points because lone point looks weird
        if (points && points.length >= 2) {
          this.songsOverTimeChart(
            HighchartsHelper.dateLineChartWithAverage(
              'Songs per month',
              null!,
              'Songs',
              points,
            ),
          );
        }
      },
    );
  };

  public removeFollowedArtist = (): void => {
    this.userRepository.deleteArtistSubscription(this.artistId, () => {
      this.hasArtistSubscription(false);
    });
  };

  public showAllMembers = ko.observable(false);
  public description: EnglishTranslatedStringViewModel;
  public songsViewModel: KnockoutObservable<SongSearchViewModel> = ko.observable(
    null!,
  );

  public songsOverTimeChart = ko.observable<HighchartsOptions>(null!);

  public collaborationAlbumsViewModel: KnockoutObservable<AlbumSearchViewModel> = ko.observable(
    null!,
  );
  public mainAlbumsViewModel: KnockoutObservable<AlbumSearchViewModel> = ko.observable(
    null!,
  );

  public reportViewModel: ReportEntryViewModel;

  public tagsEditViewModel: TagsEditViewModel;

  public tagUsages: TagListViewModel;

  public initMainAlbums = (): void => {
    if (this.mainAlbumsViewModel()) return;

    this.mainAlbumsViewModel(
      new AlbumSearchViewModel(
        null!,
        this.unknownPictureUrl,
        this.lang,
        this.albumRepo,
        null!,
        this.resourceRepo,
        this.cultureCode,
        null!,
        [this.artistId],
        null!,
        'Unknown',
        null!,
      ),
    );
    this.mainAlbumsViewModel().artistFilters.artistParticipationStatus(
      'OnlyMainAlbums',
    );
  };

  public initCollaborationAlbums = (): void => {
    if (this.collaborationAlbumsViewModel()) return;

    this.collaborationAlbumsViewModel(
      new AlbumSearchViewModel(
        null!,
        this.unknownPictureUrl,
        this.lang,
        this.albumRepo,
        null!,
        this.resourceRepo,
        this.cultureCode,
        null!,
        [this.artistId],
        null!,
        'Unknown',
        null!,
      ),
    );
    this.collaborationAlbumsViewModel().artistFilters.artistParticipationStatus(
      'OnlyCollaborations',
    );
  };

  public initSongs = (): void => {
    if (this.songsViewModel()) return;

    this.songsViewModel(
      new SongSearchViewModel(
        null!,
        this.urlMapper,
        this.lang,
        this.songRepo,
        null!,
        this.userRepository,
        null!,
        this.resourceRepo,
        this.cultureCode,
        this.loggedUserId,
        null!,
        [this.artistId],
        null!,
        null!,
        null!,
        false,
        false,
        null!,
        null!,
        null!,
        null!,
        null!,
        this.pvPlayersFactory,
      ),
    );
    this.songsViewModel().updateResults(true);
  };
}

export class CustomizeArtistSubscriptionViewModel {
  public dialogVisible = ko.observable(false);

  public notificationsMethod: KnockoutObservable<string>;

  constructor(
    artistId: number,
    emailNotifications: boolean,
    siteNotifications: boolean,
    userRepository: UserRepository,
  ) {
    this.notificationsMethod = ko.observable(
      !siteNotifications ? 'Nothing' : !emailNotifications ? 'Site' : 'Email',
    );

    this.notificationsMethod.subscribe((method) => {
      userRepository.updateArtistSubscription(
        artistId,
        method == 'Email',
        method == 'Site' || method == 'Email',
      );
    });
  }

  public show = (): void => {
    this.dialogVisible(true);
  };
}
