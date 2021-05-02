import RepositoryFactory from '../Repositories/RepositoryFactory';
import UrlMapper from '../Shared/UrlMapper';
import ArtistMergeViewModel from '../ViewModels/Artist/ArtistMergeViewModel';

const ArtistMerge = (model: { id: number }) => {
  $(function () {
    var repoFactory = new RepositoryFactory(
      new UrlMapper(vdb.values.baseAddress),
      vdb.values.languagePreference,
    );
    var repo = repoFactory.artistRepository();
    var vm = new ArtistMergeViewModel(repo, model.id);
    ko.applyBindings(vm);

    $('#mergeBtn').click(function () {
      return confirm('Are you sure you want to merge the artists?');
    });
  });
};

export default ArtistMerge;
