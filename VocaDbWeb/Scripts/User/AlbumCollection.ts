import RepositoryFactory from '@Repositories/RepositoryFactory';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
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
		const httpClient = new HttpClient();
		var rootPath = vdb.values.baseAddress;
		var urlMapper = new UrlMapper(rootPath);
		var repoFactory = new RepositoryFactory(httpClient, urlMapper);
		var userRepo = repoFactory.userRepository();
		var artistRepo = repoFactory.artistRepository();
		var resourceRepo = repoFactory.resourceRepository();

		var vm = new AlbumCollectionViewModel(
			userRepo,
			artistRepo,
			resourceRepo,
			model.user.id,
			publicCollection,
		);
		ko.applyBindings(vm);
	});
};

export default UserAlbumCollection;
