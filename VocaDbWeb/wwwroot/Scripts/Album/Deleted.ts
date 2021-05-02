import AlbumRepository from '../Repositories/AlbumRepository';
import DeletedAlbumsViewModel from '../ViewModels/Album/DeletedAlbumsViewModel';

const AlbumDeleted = () => {
  $(function () {
    var repo = new AlbumRepository(
      vdb.values.baseAddress,
      vdb.values.languagePreference,
    );
    var viewModel = new DeletedAlbumsViewModel(repo);
    ko.applyBindings(viewModel);
  });
};

export default AlbumDeleted;
