import RepositoryFactory from '@Repositories/RepositoryFactory';
import vdb from '@Shared/VdbStatic';
import { container } from '@Shared/inversify.config';
import AlbumCollectionViewModel from '@ViewModels/User/AlbumCollectionViewModel';
import $ from 'jquery';
import ko from 'knockout';

const UserAlbumCollection = (
	model: {
		user: {
			id: number;
		};
	},
	publicCollection: boolean,
): void => {
	$(document).ready(function () {
		var cultureCode = vdb.values.uiCulture;
		var lang = vdb.values.languagePreference;
		var loggedUserId = model.user.id;

		var repoFactory = container.get(RepositoryFactory);
		var userRepo = repoFactory.userRepository();
		var artistRepo = repoFactory.artistRepository();
		var resourceRepo = repoFactory.resourceRepository();

		var vm = new AlbumCollectionViewModel(
			userRepo,
			artistRepo,
			resourceRepo,
			lang,
			loggedUserId,
			cultureCode,
			publicCollection,
		);
		ko.applyBindings(vm);
	});
};

export default UserAlbumCollection;
