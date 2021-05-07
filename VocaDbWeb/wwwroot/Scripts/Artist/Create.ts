import RepositoryFactory from '../Repositories/RepositoryFactory';
import HttpClient from '../Shared/HttpClient';
import UrlMapper from '../Shared/UrlMapper';
import ArtistCreateViewModel from '../ViewModels/ArtistCreateViewModel';

const ArtistCreate = (model: any): void => {
  $(document).ready(function () {
    ko.punches.enableAll();
    const httpClient = new HttpClient();
    var repoFactory = new RepositoryFactory(
      httpClient,
      new UrlMapper(vdb.values.baseAddress),
      vdb.values.languagePreference,
    );
    var repo = repoFactory.artistRepository();
    var tagRepo = repoFactory.tagRepository();
    var json = model;
    ko.applyBindings(new ArtistCreateViewModel(repo, tagRepo, json));
  });
};

export default ArtistCreate;
