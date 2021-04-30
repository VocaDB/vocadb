import TagBaseContract from '../DataContracts/Tag/TagBaseContract';
import TagRepository from '../Repositories/TagRepository';
import TagMergeViewModel from '../ViewModels/Tag/TagMergeViewModel';

const TagMerge = (model: TagBaseContract) => {
  $(function () {
    var repo = new TagRepository(vdb.values.baseAddress);
    var data = model;
    var vm = new TagMergeViewModel(repo, data);
    ko.applyBindings(vm);

    $('#mergeBtn').click(function () {
      return confirm('Are you sure you want to merge the tags?');
    });
  });
};

export default TagMerge;
