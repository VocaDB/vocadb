import RepositoryFactory from '../Repositories/RepositoryFactory';
import UrlMapper from '../Shared/UrlMapper';
import SongCreateViewModel from '../ViewModels/SongCreateViewModel';

const SongCreate = (model: SongCreateViewModel) => {
  $(document).ready(function () {
    ko.punches.enableAll();
    var repoFactory = new RepositoryFactory(
      new UrlMapper(vdb.values.baseAddress),
      vdb.values.languagePreference,
    );
    var repo = repoFactory.songRepository();
    var artistRepo = repoFactory.artistRepository();
    var tagRepo = repoFactory.tagRepository();
    var json = model;
    ko.applyBindings(new SongCreateViewModel(repo, artistRepo, tagRepo, json));

    $('#pvLoader')
      .ajaxStart(function () {
        $(this).show();
      })
      .ajaxStop(function () {
        $(this).hide();
      });
  });
};

export default SongCreate;
