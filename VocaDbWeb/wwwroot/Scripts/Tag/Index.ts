import TagRepository from '../Repositories/TagRepository';
import TagCreateViewModel from '../ViewModels/Tag/TagCreateViewModel';

const TagIndex = () => {
  $(function () {
    var tagRepo = new TagRepository(vdb.values.baseAddress);
    var viewModel = new TagCreateViewModel(tagRepo);
    ko.applyBindings(viewModel);
  });
};

export default TagIndex;
