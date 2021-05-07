import SongContract from '../DataContracts/Song/SongContract';
import SongRepository from '../Repositories/SongRepository';
import HttpClient from '../Shared/HttpClient';
import SongMergeViewModel from '../ViewModels/Song/SongMergeViewModel';

const SongMerge = (model: SongContract): void => {
  $(function () {
    const httpClient = new HttpClient();
    var repo = new SongRepository(
      httpClient,
      vdb.values.baseAddress,
      vdb.values.languagePreference,
    );
    var data = model;
    var vm = new SongMergeViewModel(repo, data);
    ko.applyBindings(vm);

    $('#mergeBtn').click(function () {
      return window.confirm('Are you sure you want to merge the songs?');
    });
  });
};

export default SongMerge;
