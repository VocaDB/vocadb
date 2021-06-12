import RepositoryFactory from '@Repositories/RepositoryFactory';
import { container } from '@Shared/inversify.config';
import ManageWebhooksViewModel from '@ViewModels/Admin/ManageWebhooksViewModel';
import $ from 'jquery';
import ko from 'knockout';

const AdminManageWebhooks = (webhookEventNames: {
	[key: string]: string;
}): void => {
	$(function () {
		const repoFactory = container.get(RepositoryFactory);
		var adminRepo = repoFactory.adminRepository();

		var viewModel = new ManageWebhooksViewModel(webhookEventNames, adminRepo);
		ko.applyBindings(viewModel);
	});
};

export default AdminManageWebhooks;
