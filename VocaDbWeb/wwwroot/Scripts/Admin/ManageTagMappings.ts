import TagRepository from '../Repositories/TagRepository';
import UrlMapper from '../Shared/UrlMapper';
import ManageTagMappingsViewModel from '../ViewModels/Admin/ManageTagMappingsViewModel';

const AdminManageTagMappings = () => {
  $(function () {
    ko.punches.enableAll();

    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var tagRepo = new TagRepository(vdb.values.baseAddress);

    var viewModel = new ManageTagMappingsViewModel(tagRepo);
    ko.applyBindings(viewModel);
  });
};

export default AdminManageTagMappings;
