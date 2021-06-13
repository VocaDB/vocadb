import RepositoryFactory from '@Repositories/RepositoryFactory';
import VocaDbContext from '@Shared/VocaDbContext';
import { container } from '@Shared/inversify.config';
import FeaturedSongListsViewModel from '@ViewModels/SongList/FeaturedSongListsViewModel';
import $ from 'jquery';
import ko from 'knockout';
import moment from 'moment';

const vocaDbContext = container.get(VocaDbContext);
const repoFactory = container.get(RepositoryFactory);

const SongListFeatured = (
	categories: string[],
	model: {
		tagId: number[];
	},
): void => {
	moment.locale(vocaDbContext.culture);
	ko.punches.enableAll();

	$(function () {
		$('#createLink').button({ icons: { primary: 'ui-icon-plusthick' } });
		$('#importLink').button({ icons: { primary: 'ui-icon-plusthick' } });

		var cultureCode = vocaDbContext.uiCulture;
		var tagIds = model.tagId;

		var songListRepo = repoFactory.songListRepository();
		var resourceRepo = repoFactory.resourceRepository();
		var tagRepo = repoFactory.tagRepository();
		var viewModel = new FeaturedSongListsViewModel(
			vocaDbContext,
			songListRepo,
			resourceRepo,
			tagRepo,
			cultureCode,
			tagIds,
			categories,
		);
		ko.applyBindings(viewModel);

		if (window.location.hash && window.location.hash.length >= 1) {
			viewModel.setCategory(window.location.hash.substr(1));
		}
	});
};

export default SongListFeatured;
