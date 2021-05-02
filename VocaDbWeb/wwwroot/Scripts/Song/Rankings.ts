import RepositoryFactory from '../Repositories/RepositoryFactory';
import UrlMapper from '../Shared/UrlMapper';
import RankingsViewModel from '../ViewModels/Song/RankingsViewModel';

const SongRankings = () => {
  moment.locale(vdb.values.culture);
  ko.punches.enableAll();

  $(function () {
    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var repoFactory = new RepositoryFactory(
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
