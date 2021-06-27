import RepositoryFactory from '@Repositories/RepositoryFactory';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import FeaturedSongListsViewModel from '@ViewModels/SongList/FeaturedSongListsViewModel';
import $ from 'jquery';
import ko from 'knockout';
import moment from 'moment';

const SongListFeatured = (
	categories: string[],
	model: {
		tagId: number[];
	},
): void => {
	moment.locale(vdb.values.culture);
	ko.punches.enableAll();

	$(function () {
		$('#createLink').button({ icons: { primary: 'ui-icon-plusthick' } });
		$('#importLink').button({ icons: { primary: 'ui-icon-plusthick' } });

		var tagIds = model.tagId;

		const httpClient = new HttpClient();
		var rootPath = vdb.values.baseAddress;
		var urlMapper = new UrlMapper(rootPath);
		var repoFactory = new RepositoryFactory(httpClient, urlMapper);
		var songListRepo = repoFactory.songListRepository();
		var resourceRepo = repoFactory.resourceRepository();
		var tagRepo = repoFactory.tagRepository();
		var viewModel = new FeaturedSongListsViewModel(
			vdb.values,
			songListRepo,
			resourceRepo,
			tagRepo,
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
