import TagRepository from '../Repositories/TagRepository';
import HttpClient from '../Shared/HttpClient';
import UrlMapper from '../Shared/UrlMapper';
import ManageEntryTagMappingsViewModel from '../ViewModels/Admin/ManageEntryTagMappingsViewModel';

const AdminManageEntryTagMappings = () => {
  $(function () {
    ko.punches.enableAll();

    const httpClient = new HttpClient();
    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var tagRepo = new TagRepository(httpClient, vdb.values.baseAddress);

    var viewModel = new ManageEntryTagMappingsViewModel(tagRepo);
    ko.applyBindings(viewModel);
  });
};

export default AdminManageEntryTagMappings;
