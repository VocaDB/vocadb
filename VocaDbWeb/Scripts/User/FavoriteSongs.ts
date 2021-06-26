import RepositoryFactory from '@Repositories/RepositoryFactory';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import vdb from '@Shared/VdbStatic';
import PVPlayersFactory from '@ViewModels/PVs/PVPlayersFactory';
import RatedSongsSearchViewModel from '@ViewModels/User/RatedSongsSearchViewModel';
import $ from 'jquery';
import ko from 'knockout';
import moment from 'moment';

const UserFavoriteSongs = (model: {
	groupByRating: boolean;
	sort: string;
	user: {
		id: number;
	};
}): void => {
	$(function () {
		moment.locale(vdb.values.culture);
		var sort = model.sort;
		var groupByRating = model.groupByRating;

		const httpClient = new HttpClient();
		var rootPath = vdb.values.baseAddress;
		var urlMapper = new UrlMapper(rootPath);
		var repoFactory = new RepositoryFactory(httpClient, urlMapper);
		var userRepo = repoFactory.userRepository();
		var artistRepo = repoFactory.artistRepository();
		var songRepo = repoFactory.songRepository();
		var resourceRepo = repoFactory.resourceRepository();
		var tagRepo = repoFactory.tagRepository();
		var pvPlayersFactory = new PVPlayersFactory($('#pv-player-wrapper')[0]);

		var vm = new RatedSongsSearchViewModel(
			urlMapper,
			userRepo,
			artistRepo,
			songRepo,
			resourceRepo,
			tagRepo,
			model.user.id,
			sort,
			groupByRating,
			pvPlayersFactory,
			true,
		);
		ko.applyBindings(vm);
	});
};

export default UserFavoriteSongs;
