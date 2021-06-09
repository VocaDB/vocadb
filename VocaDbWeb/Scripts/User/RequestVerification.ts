import RepositoryFactory from '@Repositories/RepositoryFactory';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import vdb from '@Shared/VdbStatic';
import RequestVerificationViewModel from '@ViewModels/User/RequestVerificationViewModel';
import $ from 'jquery';
import ko from 'knockout';

const UserRequestVerification = (): void => {
	$(document).ready(function () {
		const httpClient = new HttpClient();
		var repoFactory = new RepositoryFactory(
			httpClient,
			new UrlMapper(vdb.values.baseAddress),
		);
		var artistRepo = repoFactory.artistRepository();
		ko.applyBindings(new RequestVerificationViewModel(artistRepo));
	});
};

export default UserRequestVerification;
