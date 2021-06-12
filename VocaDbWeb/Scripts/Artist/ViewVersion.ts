import RepositoryFactory from '@Repositories/RepositoryFactory';
import { container } from '@Shared/inversify.config';
import ArchivedArtistViewModel from '@ViewModels/Artist/ArchivedArtistViewModel';
import $ from 'jquery';
import ko from 'knockout';

const repoFactory = container.get(RepositoryFactory);

const ArtistViewVersion = (model: {
	archivedVersion: {
		version: number;
	};
	artist: {
		id: number;
	};
}): void => {
	$(function () {
		$('#revertLink').button({ icons: { primary: 'ui-icon-arrowrefresh-1-w' } });
		$('#downloadXmlLink').button({
			icons: { primary: 'ui-icon-arrowthickstop-1-s' },
		});
		$('#reportEntryLink').button({ icons: { primary: 'ui-icon-alert' } });
		$('#showLink').button({ icons: { primary: 'ui-icon-unlocked' } });
		$('#hideLink').button({ icons: { primary: 'ui-icon-locked' } });

		var rep = repoFactory.artistRepository();

		var viewModel = new ArchivedArtistViewModel(
			model.artist.id,
			model.archivedVersion.version,
			rep,
		);
		ko.applyBindings(viewModel);
	});
};

export default ArtistViewVersion;
