import TagRepository from '@Repositories/TagRepository';
import HttpClient from '@Shared/HttpClient';
import ManageTagMappingsViewModel from '@ViewModels/Admin/ManageTagMappingsViewModel';
import $ from 'jquery';

const AdminManageTagMappings = (): void => {
  $(function () {
    ko.punches.enableAll();

    const httpClient = new HttpClient();
    var tagRepo = new TagRepository(httpClient, vdb.values.baseAddress);

    var viewModel = new ManageTagMappingsViewModel(tagRepo);
    ko.applyBindings(viewModel);
  });
};

export default AdminManageTagMappings;
