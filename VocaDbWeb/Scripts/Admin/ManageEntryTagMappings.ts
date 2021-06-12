import RepositoryFactory from '@Repositories/RepositoryFactory';
import { container } from '@Shared/inversify.config';
import ManageEntryTagMappingsViewModel from '@ViewModels/Admin/ManageEntryTagMappingsViewModel';
import $ from 'jquery';
import ko from 'knockout';

const AdminManageEntryTagMappings = (): void => {
	$(function () {
		ko.punches.enableAll();

		const repoFactory = container.get(RepositoryFactory);
		var tagRepo = repoFactory.tagRepository();

		var viewModel = new ManageEntryTagMappingsViewModel(tagRepo);
		ko.applyBindings(viewModel);
	});
};

export default AdminManageEntryTagMappings;
