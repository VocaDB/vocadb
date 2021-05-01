import CommentContract from '../DataContracts/CommentContract';
import ContentLanguagePreference from '../Models/Globalization/ContentLanguagePreference';
import RepositoryFactory from '../Repositories/RepositoryFactory';
import UrlMapper from '../Shared/UrlMapper';
import PVPlayersFactory from '../ViewModels/PVs/PVPlayersFactory';
import AlbumCollectionViewModel from '../ViewModels/User/AlbumCollectionViewModel';
import FollowedArtistsViewModel from '../ViewModels/User/FollowedArtistsViewModel';
import RatedSongsSearchViewModel from '../ViewModels/User/RatedSongsSearchViewModel';
import UserDetailsViewModel from '../ViewModels/User/UserDetailsViewModel';

function initPage(confirmDisableStr: string) {
  $('#mySettingsLink').button({ icons: { primary: 'ui-icon-wrench' } });
  $('#messagesLink').button({ icons: { primary: 'ui-icon-mail-closed' } });
  $('#composeMessageLink').button({
    icons: { primary: 'ui-icon-mail-closed' },
  });
  $('#editUserLink').button({ icons: { primary: 'ui-icon-wrench' } });
  $('#disableUserLink').button({ icons: { primary: 'ui-icon-close' } });
  $('#reportUserLink').button({ icons: { primary: 'ui-icon-alert' } });
  $('#setToLimitedLink').button({ icons: { primary: 'ui-icon-close' } });
  $('#avatar').tooltip(<any>{ placement: 'bottom' });

  $('#disableUserLink').click(function () {
    return confirm(confirmDisableStr);
  });

  $('#sfsCheckDialog').dialog(<any>{ autoOpen: false, model: true });
  $('#favoriteAlbums img').vdbAlbumToolTip();
}

const UserDetails = (
  artistId: number,
  canDeleteComments: boolean,
  childVoicebanks: boolean,
  confirmDisableStr: string,
  lastLoginAddress: string,
  model: {
    id: number;
    latestComments: CommentContract[];
  },
  publicCollection: boolean,
) => {
  ko.punches.enableAll();

  $(function () {
    var cultureCode = vdb.values.uiCulture;

    moment.locale(vdb.values.culture);

    var languageSelection =
      ContentLanguagePreference[vdb.values.languagePreference];
    var userId = model.id;
    var loggedUserId = vdb.values.loggedUserId;
    var rootPath = vdb.values.baseAddress;
    var urlMapper = new UrlMapper(rootPath);
    var repoFactory = new RepositoryFactory(
      urlMapper,
      vdb.values.languagePreference,
      vdb.values.loggedUserId,
    );
    var adminRepo = repoFactory.adminRepository();
    var userRepo = repoFactory.userRepository();
    var artistRepo = repoFactory.artistRepository();
    var resourceRepo = repoFactory.resourceRepository();
    var songRepo = repoFactory.songRepository();
    var tagRepo = repoFactory.tagRepository();
    var pvPlayersFactory = new PVPlayersFactory($('#pv-player-wrapper')[0]);
    var latestComments = model.latestComments;

    var sort = 'RatingDate';
    var groupByRating = true;

    var followedArtistsViewModel = new FollowedArtistsViewModel(
      userRepo,
      resourceRepo,
      tagRepo,
      languageSelection,
      userId,
      cultureCode,
    );

    var albumCollectionViewModel = new AlbumCollectionViewModel(
      userRepo,
      artistRepo,
      resourceRepo,
      languageSelection,
      userId,
      cultureCode,
      publicCollection,
      false,
    );

    var ratedSongsViewModel = new RatedSongsSearchViewModel(
      urlMapper,
      userRepo,
      artistRepo,
      songRepo,
      resourceRepo,
      tagRepo,
      languageSelection,
      userId,
      cultureCode,
      sort,
      groupByRating,
      pvPlayersFactory,
      false,
      artistId,
      childVoicebanks,
    );

    var viewModel = new UserDetailsViewModel(
      userId,
      cultureCode,
      loggedUserId,
      lastLoginAddress,
      canDeleteComments,
      urlMapper,
      userRepo,
      adminRepo,
      resourceRepo,
      tagRepo,
      languageSelection,
      followedArtistsViewModel,
      albumCollectionViewModel,
      ratedSongsViewModel,
      latestComments,
    );
    ko.applyBindings(viewModel);

    if (window.location.hash && window.location.hash.length >= 1) {
      viewModel.setView(window.location.hash.substr(1));
    }

    initPage(confirmDisableStr);
  });
};

export default UserDetails;
