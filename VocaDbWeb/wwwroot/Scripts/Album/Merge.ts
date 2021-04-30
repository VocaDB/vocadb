import AlbumRepository from '../Repositories/AlbumRepository';
import AlbumMergeViewModel from '../ViewModels/Album/AlbumMergeViewModel';

const AlbumMerge = (model: { id: number }) => {
  $(function () {
    var repo = new AlbumRepository(
      vdb.values.baseAddress,
      vdb.values.languagePreference,
    );
    var vm = new AlbumMergeViewModel(repo, model.id);
    ko.applyBindings(vm);

    $('#mergeBtn').click(function () {
      return confirm('Are you sure you want to merge the albums?');
    });
  });
};

export default AlbumMerge;
