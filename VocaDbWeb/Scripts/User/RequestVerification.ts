import RepositoryFactory from '@Repositories/RepositoryFactory';
import { container } from '@Shared/inversify.config';
import RequestVerificationViewModel from '@ViewModels/User/RequestVerificationViewModel';
import $ from 'jquery';
import ko from 'knockout';

const UserRequestVerification = (): void => {
	$(document).ready(function () {
		const repoFactory = container.get(RepositoryFactory);
		var artistRepo = repoFactory.artistRepository();
		ko.applyBindings(new RequestVerificationViewModel(artistRepo));
	});
};

export default UserRequestVerification;
