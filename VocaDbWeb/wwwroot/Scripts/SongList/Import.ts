import UrlMapper from '../Shared/UrlMapper';
import ImportSongListViewModel from '../ViewModels/SongList/ImportSongListViewModel';

const SongListImport = () => {
  ko.punches.enableAll();

  $(function () {
    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var viewModel = new ImportSongListViewModel(urlMapper);
    ko.applyBindings(viewModel);
  });
};

export default SongListImport;
