import RepositoryFactory from '../Repositories/RepositoryFactory';
import HttpClient from '../Shared/HttpClient';
import UrlMapper from '../Shared/UrlMapper';
import SongListEditViewModel from '../ViewModels/SongList/SongListEditViewModel';

function initPage(repoFactory, urlMapper, listId): void {
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

const SongListEdit = (model: { id: number }): void => {
  $(document).ready(function () {
    const httpClient = new HttpClient();
    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var repoFactory = new RepositoryFactory(
      httpClient,
      urlMapper,
      vdb.values.languagePreference,
    );
    initPage(repoFactory, urlMapper, model.id);
  });
};

export default SongListEdit;
