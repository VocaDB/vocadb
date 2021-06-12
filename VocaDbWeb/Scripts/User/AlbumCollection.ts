import RepositoryFactory from '@Repositories/RepositoryFactory';
import VocaDbContext from '@Shared/VocaDbContext';
import { container } from '@Shared/inversify.config';
import AlbumCollectionViewModel from '@ViewModels/User/AlbumCollectionViewModel';
import $ from 'jquery';
import ko from 'knockout';

const vocaDbContext = container.get(VocaDbContext);
const repoFactory = container.get(RepositoryFactory);

const UserAlbumCollection = (
	model: {
		user: {
			id: number;
		};
	},
	publicCollection: boolean,
): void => {
	$(document).ready(function () {
		var cultureCode = vocaDbContext.uiCulture;
		var lang = vocaDbContext.languagePreference;
		var loggedUserId = model.user.id;

		var userRepo = repoFactory.userRepository();
		var artistRepo = repoFactory.artistRepository();
		var resourceRepo = repoFactory.resourceRepository();

		var vm = new AlbumCollectionViewModel(
			vocaDbContext,
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
