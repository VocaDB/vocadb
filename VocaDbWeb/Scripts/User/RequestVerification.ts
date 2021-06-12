import RepositoryFactory from '@Repositories/RepositoryFactory';
import { container } from '@Shared/inversify.config';
import RequestVerificationViewModel from '@ViewModels/User/RequestVerificationViewModel';
import $ from 'jquery';
import ko from 'knockout';

const repoFactory = container.get(RepositoryFactory);

const UserRequestVerification = (): void => {
	$(document).ready(function () {
		var artistRepo = repoFactory.artistRepository();
		ko.applyBindings(new RequestVerificationViewModel(artistRepo));
	});
};

export default UserRequestVerification;
