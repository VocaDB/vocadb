import CommentContract from '@DataContracts/CommentContract';
import TagUsageForApiContract from '@DataContracts/Tag/TagUsageForApiContract';
import RepositoryFactory from '@Repositories/RepositoryFactory';
import UrlMapper from '@Shared/UrlMapper';
import VocaDbContext from '@Shared/VocaDbContext';
import { container } from '@Shared/inversify.config';
import PVPlayersFactory from '@ViewModels/PVs/PVPlayersFactory';
import SongListViewModel from '@ViewModels/SongList/SongListViewModel';
import $ from 'jquery';
import ko from 'knockout';

const vocaDbContext = container.get(VocaDbContext);
const repoFactory = container.get(RepositoryFactory);

const SongListDetails = (
	canDeleteAllComments: boolean,
	defaultSortRuleName: string,
	model: {
		songList: {
			id: number;
			latestComments: CommentContract[];
			tags: TagUsageForApiContract[];
		};
	},
): void => {
	$(function () {
		$('#editListLink').button({ icons: { primary: 'ui-icon-wrench' } });
		$('#viewVersions').button({ icons: { primary: 'ui-icon-clock' } });
		$('#export').button({ icons: { primary: 'ui-icon-arrowthickstop-1-s' } });

		var languageSelection = vocaDbContext.languagePreference;
		var listId = model.songList.id;

		var rootPath = vocaDbContext.baseAddress;
		var urlMapper = new UrlMapper(rootPath);
		var userRepo = repoFactory.userRepository();
		var songRepo = repoFactory.songRepository();
		var artistRepo = repoFactory.artistRepository();
		var songListRepo = repoFactory.songListRepository();
		var resourceRepo = repoFactory.resourceRepository();
		var cultureCode = vocaDbContext.uiCulture;
		var pvPlayerElem = $('#pv-player-wrapper')[0];
		var pvPlayersFactory = new PVPlayersFactory(pvPlayerElem);
		var latestComments = model.songList.latestComments;
		var tagUsages = model.songList.tags;

		var vm = new SongListViewModel(
			vocaDbContext,
			urlMapper,
			songListRepo,
			songRepo,
			userRepo,
			artistRepo,
			resourceRepo,
			defaultSortRuleName,
			latestComments,
			vocaDbContext.loggedUserId,
			languageSelection,
			cultureCode,
			listId,
			tagUsages,
			pvPlayersFactory,
			canDeleteAllComments,
		);
		ko.applyBindings(vm);
	});
};

export default SongListDetails;
