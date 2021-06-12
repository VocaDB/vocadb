import RepositoryFactory from '@Repositories/RepositoryFactory';
import vdb from '@Shared/VdbStatic';
import { container } from '@Shared/inversify.config';
import ListUsersViewModel from '@ViewModels/User/ListUsersViewModel';
import $ from 'jquery';
import ko from 'knockout';
import moment from 'moment';

const repoFactory = container.get(RepositoryFactory);

const UserIndex = (model: { filter: string; groupId: string }): void => {
	$(function () {
		var cultureCode = vdb.values.culture;
		var uiCultureCode = vdb.values.uiCulture;
		moment.locale(cultureCode);

		var filter = model.filter;
		var groupId = model.groupId;
		var repo = repoFactory.userRepository();
		var resourceRepo = repoFactory.resourceRepository();
		var viewModel = new ListUsersViewModel(
			repo,
			resourceRepo,
			uiCultureCode,
			filter,
			groupId,
		);
		ko.applyBindings(viewModel);
	});
};

export default UserIndex;
