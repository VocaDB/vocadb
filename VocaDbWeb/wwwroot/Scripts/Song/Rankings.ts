import RepositoryFactory from '../Repositories/RepositoryFactory';
import HttpClient from '../Shared/HttpClient';
import UrlMapper from '../Shared/UrlMapper';
import RankingsViewModel from '../ViewModels/Song/RankingsViewModel';

const SongRankings = (): void => {
  moment.locale(vdb.values.culture);
  ko.punches.enableAll();

  $(function () {
    const httpClient = new HttpClient();
    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var repoFactory = new RepositoryFactory(
      httpClient,
      urlMapper,
      vdb.values.languagePreference,
      vdb.values.loggedUserId,
    );
    var songRepo = repoFactory.songRepository();
    var userRepo = repoFactory.userRepository();
    var viewModel = new RankingsViewModel(
      urlMapper,
      songRepo,
      userRepo,
      vdb.values.languagePreference,
    );
    ko.applyBindings(viewModel);
  });
};

export default SongRankings;
