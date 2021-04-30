import RepositoryFactory from '../Repositories/RepositoryFactory';
import UrlMapper from '../Shared/UrlMapper';
import AlbumCreateViewModel from '../ViewModels/Album/AlbumCreateViewModel';

const AlbumCreate = () => {
  $(function () {
    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var repoFactory = new RepositoryFactory(
      urlMapper,
      vdb.values.languagePreference,
    );
    var albumRepo = repoFactory.albumRepository();
    var artistRepo = repoFactory.artistRepository();

    ko.applyBindings(new AlbumCreateViewModel(albumRepo, artistRepo));
  });
};

export default AlbumCreate;
