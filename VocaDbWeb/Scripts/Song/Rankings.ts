import RepositoryFactory from '@Repositories/RepositoryFactory';
import UrlMapper from '@Shared/UrlMapper';
import vdb from '@Shared/VdbStatic';
import { container } from '@Shared/inversify.config';
import RankingsViewModel from '@ViewModels/Song/RankingsViewModel';
import $ from 'jquery';
import ko from 'knockout';
import moment from 'moment';

const repoFactory = container.get(RepositoryFactory);

const SongRankings = (): void => {
	moment.locale(vdb.values.culture);
	ko.punches.enableAll();

	$(function () {
		var urlMapper = new UrlMapper(vdb.values.baseAddress);
		var songRepo = repoFactory.songRepository();
		var userRepo = repoFactory.userRepository();
		var viewModel = new RankingsViewModel(
			urlMapper,
			songRepo,
			userRepo,
			vdb.values.languagePreference,
		);
		ko.applyBindings(viewModel);
	});
};

export default SongRankings;
