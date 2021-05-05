import TagRepository from '../Repositories/TagRepository';
import HttpClient from '../Shared/HttpClient';
import UrlMapper from '../Shared/UrlMapper';
import ManageTagMappingsViewModel from '../ViewModels/Admin/ManageTagMappingsViewModel';

const AdminManageTagMappings = () => {
  $(function () {
    ko.punches.enableAll();

    const httpClient = new HttpClient();
    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var tagRepo = new TagRepository(httpClient, vdb.values.baseAddress);

    var viewModel = new ManageTagMappingsViewModel(tagRepo);
    ko.applyBindings(viewModel);
  });
};

export default AdminManageTagMappings;
