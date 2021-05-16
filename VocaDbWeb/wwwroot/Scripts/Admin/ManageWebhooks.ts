import ManageWebhooksViewModel from '@ViewModels/Admin/ManageWebhooksViewModel';

const AdminManageWebhooks = (webhookEventNames: {
  [key: string]: string;
}): void => {
  $(function () {
    var viewModel = new ManageWebhooksViewModel(webhookEventNames);
    ko.applyBindings(viewModel);
  });
};

export default AdminManageWebhooks;
