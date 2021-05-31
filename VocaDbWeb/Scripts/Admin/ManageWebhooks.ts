import AdminRepository from '@Repositories/AdminRepository';
import HttpClient from '@Shared/HttpClient';
import ManageWebhooksViewModel from '@ViewModels/Admin/ManageWebhooksViewModel';
import $ from 'jquery';
import ko from 'knockout';

const AdminManageWebhooks = (webhookEventNames: {
  [key: string]: string;
}): void => {
  $(function () {
    const httpClient = new HttpClient();

    var adminRepo = new AdminRepository(httpClient);

    var viewModel = new ManageWebhooksViewModel(webhookEventNames, adminRepo);
    ko.applyBindings(viewModel);
  });
};

export default AdminManageWebhooks;
