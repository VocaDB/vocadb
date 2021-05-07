import AlbumRepository from '../Repositories/AlbumRepository';
import HttpClient from '../Shared/HttpClient';
import DeletedAlbumsViewModel from '../ViewModels/Album/DeletedAlbumsViewModel';

const AlbumDeleted = (): void => {
  $(function () {
    const httpClient = new HttpClient();
    var repo = new AlbumRepository(
      httpClient,
      vdb.values.baseAddress,
      vdb.values.languagePreference,
    );
    var viewModel = new DeletedAlbumsViewModel(repo);
    ko.applyBindings(viewModel);
  });
};

export default AlbumDeleted;
