import RepositoryFactory from '../Repositories/RepositoryFactory';
import UrlMapper from '../Shared/UrlMapper';
import SongListEditViewModel from '../ViewModels/SongList/SongListEditViewModel';

function initPage(repoFactory, urlMapper, listId) {
  $('#tabs').tabs();
  $('#deleteLink').button({ icons: { primary: 'ui-icon-trash' } });
  $('#trashLink').button({ icons: { primary: 'ui-icon-trash' } });

  var songListRepo = repoFactory.songListRepository();
  var songRepo = repoFactory.songRepository();

  var viewModel = new SongListEditViewModel(
    songListRepo,
    songRepo,
    urlMapper,
    listId,
  );
  viewModel.init(function () {
    ko.applyBindings(viewModel);
  });
}

const SongListEdit = (model: { id: number }) => {
  $(document).ready(function () {
    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var repoFactory = new RepositoryFactory(
      urlMapper,
      vdb.values.languagePreference,
    );
    initPage(repoFactory, urlMapper, model.id);
  });
};

export default SongListEdit;
