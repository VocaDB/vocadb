import RepositoryFactory from '../Repositories/RepositoryFactory';
import UrlMapper from '../Shared/UrlMapper';
import ArtistCreateViewModel from '../ViewModels/ArtistCreateViewModel';

const ArtistCreate = (model: ArtistCreateViewModel) => {
  $(document).ready(function () {
    ko.punches.enableAll();
    var repoFactory = new RepositoryFactory(
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
