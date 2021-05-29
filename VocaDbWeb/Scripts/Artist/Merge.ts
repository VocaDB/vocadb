import RepositoryFactory from '@Repositories/RepositoryFactory';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import vdb from '@Shared/VdbStatic';
import ArtistMergeViewModel from '@ViewModels/Artist/ArtistMergeViewModel';
import $ from 'jquery';
import ko from 'knockout';

const ArtistMerge = (model: { id: number }): void => {
  $(function () {
    const httpClient = new HttpClient();
    var repoFactory = new RepositoryFactory(
      httpClient,
      new UrlMapper(vdb.values.baseAddress),
      vdb.values.languagePreference,
    );
    var repo = repoFactory.artistRepository();
    var vm = new ArtistMergeViewModel(repo, model.id);
    ko.applyBindings(vm);

    $('#mergeBtn').click(function () {
      return window.confirm('Are you sure you want to merge the artists?');
    });
  });
};

export default ArtistMerge;
