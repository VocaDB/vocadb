import TagRepository from '@Repositories/TagRepository';
import HttpClient from '@Shared/HttpClient';
import ManageEntryTagMappingsViewModel from '@ViewModels/Admin/ManageEntryTagMappingsViewModel';
import $ from 'jquery';
import ko from 'knockout';

const AdminManageEntryTagMappings = (): void => {
	$(function () {
		ko.punches.enableAll();

		const httpClient = new HttpClient();
		var tagRepo = new TagRepository(httpClient, vdb.values.baseAddress);

		var viewModel = new ManageEntryTagMappingsViewModel(tagRepo);
		ko.applyBindings(viewModel);
	});
};

export default AdminManageEntryTagMappings;
