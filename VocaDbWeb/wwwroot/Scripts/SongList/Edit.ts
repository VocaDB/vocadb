import SongListEditViewModel from '../ViewModels/SongList/SongListEditViewModel';

export function initPage(repoFactory, urlMapper, listId) {
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
