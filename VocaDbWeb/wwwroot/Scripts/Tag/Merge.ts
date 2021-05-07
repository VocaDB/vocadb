import TagBaseContract from '../DataContracts/Tag/TagBaseContract';
import TagRepository from '../Repositories/TagRepository';
import HttpClient from '../Shared/HttpClient';
import TagMergeViewModel from '../ViewModels/Tag/TagMergeViewModel';

const TagMerge = (model: TagBaseContract): void => {
  $(function () {
    const httpClient = new HttpClient();
    var repo = new TagRepository(httpClient, vdb.values.baseAddress);
    var data = model;
    var vm = new TagMergeViewModel(repo, data);
    ko.applyBindings(vm);

    $('#mergeBtn').click(function () {
      return window.confirm('Are you sure you want to merge the tags?');
    });
  });
};

export default TagMerge;
