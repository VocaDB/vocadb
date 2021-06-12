import UserKnownLanguageContract from '@DataContracts/User/UserKnownLanguageContract';
import WebLinkContract from '@DataContracts/WebLinkContract';
import RepositoryFactory from '@Repositories/RepositoryFactory';
import { container } from '@Shared/inversify.config';
import MySettingsViewModel from '@ViewModels/User/MySettingsViewModel';
import $ from 'jquery';
import ko from 'knockout';

const repoFactory = container.get(RepositoryFactory);

const UserMySettings = (model: {
	aboutMe: string;
	email: string;
	emailVerified: boolean;
	knownLanguages: UserKnownLanguageContract[];
	webLinks: WebLinkContract[];
}): void => {
	$(document).ready(function () {
		$('#tabs').tabs();

		var repository = repoFactory.userRepository();

		var viewModel = new MySettingsViewModel(
			repository,
			model.aboutMe,
			model.email,
			model.emailVerified,
			model.webLinks,
			model.knownLanguages,
		);
		ko.applyBindings(viewModel);
	});
};

export default UserMySettings;
