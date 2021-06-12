import RepositoryFactory from '@Repositories/RepositoryFactory';
import UrlMapper from '@Shared/UrlMapper';
import VocaDbContext from '@Shared/VocaDbContext';
import { container } from '@Shared/inversify.config';
import PVPlayersFactory from '@ViewModels/PVs/PVPlayersFactory';
import RatedSongsSearchViewModel from '@ViewModels/User/RatedSongsSearchViewModel';
import $ from 'jquery';
import ko from 'knockout';
import moment from 'moment';

const vocaDbContext = container.get(VocaDbContext);
const repoFactory = container.get(RepositoryFactory);

const UserFavoriteSongs = (model: {
	groupByRating: boolean;
	sort: string;
	user: {
		id: number;
	};
}): void => {
	$(function () {
		moment.locale(vocaDbContext.culture);
		var cultureCode = vocaDbContext.uiCulture;
		var lang = vocaDbContext.languagePreference;
		var loggedUserId = model.user.id;
		var sort = model.sort;
		var groupByRating = model.groupByRating;

		var rootPath = vocaDbContext.baseAddress;
		var urlMapper = new UrlMapper(rootPath);
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
