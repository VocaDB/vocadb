import TagRepository from '../Repositories/TagRepository';
import UrlMapper from '../Shared/UrlMapper';
import ManageEntryTagMappingsViewModel from '../ViewModels/Admin/ManageEntryTagMappingsViewModel';

const AdminManageEntryTagMappings = () => {
  $(function () {
    ko.punches.enableAll();

    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var tagRepo = new TagRepository(vdb.values.baseAddress);

    var viewModel = new ManageEntryTagMappingsViewModel(tagRepo);
    ko.applyBindings(viewModel);
  });
};

export default AdminManageEntryTagMappings;
