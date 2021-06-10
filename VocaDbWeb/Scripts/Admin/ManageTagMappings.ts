import RepositoryFactory from '@Repositories/RepositoryFactory';
import { container } from '@Shared/inversify.config';
import ManageTagMappingsViewModel from '@ViewModels/Admin/ManageTagMappingsViewModel';
import $ from 'jquery';
import ko from 'knockout';

const AdminManageTagMappings = (): void => {
	$(function () {
		ko.punches.enableAll();

		var repoFactory = container.get(RepositoryFactory);
		var tagRepo = repoFactory.tagRepository();

		var viewModel = new ManageTagMappingsViewModel(tagRepo);
		ko.applyBindings(viewModel);
	});
};

export default AdminManageTagMappings;
