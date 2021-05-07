import RepositoryFactory from '../Repositories/RepositoryFactory';
import HttpClient from '../Shared/HttpClient';
import UrlMapper from '../Shared/UrlMapper';
import SongCreateViewModel from '../ViewModels/SongCreateViewModel';

const SongCreate = (model: any): void => {
  $(document).ready(function () {
    ko.punches.enableAll();
    const httpClient = new HttpClient();
    var repoFactory = new RepositoryFactory(
      httpClient,
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
