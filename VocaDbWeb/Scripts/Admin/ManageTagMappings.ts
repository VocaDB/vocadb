import TagRepository from '@Repositories/TagRepository';
import HttpClient from '@Shared/HttpClient';
import ManageTagMappingsViewModel from '@ViewModels/Admin/ManageTagMappingsViewModel';
import $ from 'jquery';
import ko from 'knockout';

const AdminManageTagMappings = (): void => {
	$(function () {
		ko.punches.enableAll();

		const httpClient = new HttpClient();
		var tagRepo = new TagRepository(httpClient);

		var viewModel = new ManageTagMappingsViewModel(tagRepo);
		ko.applyBindings(viewModel);
	});
};

export default AdminManageTagMappings;
