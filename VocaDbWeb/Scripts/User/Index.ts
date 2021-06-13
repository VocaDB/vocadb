import RepositoryFactory from '@Repositories/RepositoryFactory';
import VocaDbContext from '@Shared/VocaDbContext';
import { container } from '@Shared/inversify.config';
import ListUsersViewModel from '@ViewModels/User/ListUsersViewModel';
import $ from 'jquery';
import ko from 'knockout';
import moment from 'moment';

const vocaDbContext = container.get(VocaDbContext);
const repoFactory = container.get(RepositoryFactory);

const UserIndex = (model: { filter: string; groupId: string }): void => {
	$(function () {
		moment.locale(vocaDbContext.culture);

		var filter = model.filter;
		var groupId = model.groupId;
		var repo = repoFactory.userRepository();
		var resourceRepo = repoFactory.resourceRepository();
		var viewModel = new ListUsersViewModel(
			vocaDbContext,
			repo,
			resourceRepo,
			filter,
			groupId,
		);
		ko.applyBindings(viewModel);
	});
};

export default UserIndex;
