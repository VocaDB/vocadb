import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import RepositoryFactory from '@Repositories/RepositoryFactory';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import PVPlayersFactory from '@ViewModels/PVs/PVPlayersFactory';
import RatedSongsSearchViewModel from '@ViewModels/User/RatedSongsSearchViewModel';
import $ from 'jquery';
import moment from 'moment';

const UserFavoriteSongs = (model: {
  groupByRating: boolean;
  sort: string;
  user: {
    id: number;
  };
}): void => {
  $(function () {
    moment.locale(vdb.values.culture);
    var cultureCode = vdb.values.uiCulture;
    var languageSelection =
      ContentLanguagePreference[vdb.values.languagePreference];
    var loggedUserId = model.user.id;
    var sort = model.sort;
    var groupByRating = model.groupByRating;

    const httpClient = new HttpClient();
    var rootPath = vdb.values.baseAddress;
    var urlMapper = new UrlMapper(rootPath);
    var repoFactory = new RepositoryFactory(
      httpClient,
      urlMapper,
      vdb.values.languagePreference,
    );
    var userRepo = repoFactory.userRepository();
    var artistRepo = repoFactory.artistRepository();
    var songRepo = repoFactory.songRepository();
    var resourceRepo = repoFactory.resourceRepository();
    var tagRepo = repoFactory.tagRepository();
    var pvPlayersFactory = new PVPlayersFactory($('#pv-player-wrapper')[0]);

    var vm = new RatedSongsSearchViewModel(
      urlMapper,
      userRepo,
      artistRepo,
      songRepo,
      resourceRepo,
      tagRepo,
      languageSelection,
      loggedUserId,
      cultureCode,
      sort,
      groupByRating,
      pvPlayersFactory,
      true,
    );
    ko.applyBindings(vm);
  });
};

export default UserFavoriteSongs;
