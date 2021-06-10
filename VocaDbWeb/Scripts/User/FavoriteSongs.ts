import RepositoryFactory from '@Repositories/RepositoryFactory';
import UrlMapper from '@Shared/UrlMapper';
import vdb from '@Shared/VdbStatic';
import { container } from '@Shared/inversify.config';
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
		var cultureCode = vdb.values.uiCulture;
		var lang = vdb.values.languagePreference;
		var loggedUserId = model.user.id;
		var sort = model.sort;
		var groupByRating = model.groupByRating;

		var rootPath = vdb.values.baseAddress;
		var urlMapper = new UrlMapper(rootPath);
		var repoFactory = container.get(RepositoryFactory);
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
			lang,
			loggedUserId,
			cultureCode,
			sort,
			groupByRating,
			pvPlayersFactory,
			true,
		);
		ko.applyBindings(vm);
	});
};

export default UserFavoriteSongs;
