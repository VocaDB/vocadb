import AdminRepository from '@Repositories/AdminRepository';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import vdb from '@Shared/VdbStatic';
import ManageWebhooksViewModel from '@ViewModels/Admin/ManageWebhooksViewModel';
import $ from 'jquery';
import ko from 'knockout';

const AdminManageWebhooks = (webhookEventNames: {
	[key: string]: string;
}): void => {
	$(function () {
		const httpClient = new HttpClient();
		var rootPath = vdb.values.baseAddress;
		var urlMapper = new UrlMapper(rootPath);

		var adminRepo = new AdminRepository(httpClient, urlMapper);

		var viewModel = new ManageWebhooksViewModel(webhookEventNames, adminRepo);
		ko.applyBindings(viewModel);
	});
};

export default AdminManageWebhooks;
