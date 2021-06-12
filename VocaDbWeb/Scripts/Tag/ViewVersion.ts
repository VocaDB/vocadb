import RepositoryFactory from '@Repositories/RepositoryFactory';
import { container } from '@Shared/inversify.config';
import ArchivedEntryViewModel from '@ViewModels/ArchivedEntryViewModel';
import $ from 'jquery';
import ko from 'knockout';

const repoFactory = container.get(RepositoryFactory);

const TagViewVersion = (model: {
	entry: {
		archivedVersion: {
			version: number;
		};
		tag: {
			id: number;
		};
	};
}): void => {
	$(function () {
		$('#downloadXmlLink').button({
			icons: { primary: 'ui-icon-arrowthickstop-1-s' },
		});
		$('#reportEntryLink').button({ icons: { primary: 'ui-icon-alert' } });
		$('#showLink').button({ icons: { primary: 'ui-icon-unlocked' } });
		$('#hideLink').button({ icons: { primary: 'ui-icon-locked' } });

		var rep = repoFactory.tagRepository();
		var viewModel = new ArchivedEntryViewModel(
			model.entry.tag.id,
			model.entry.archivedVersion.version,
			rep,
		);
		ko.applyBindings(viewModel);
	});
};

export default TagViewVersion;
