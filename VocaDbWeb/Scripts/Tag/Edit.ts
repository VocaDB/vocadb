import RepositoryFactory from '@Repositories/RepositoryFactory';
import UrlMapper from '@Shared/UrlMapper';
import VocaDbContext from '@Shared/VocaDbContext';
import { container } from '@Shared/inversify.config';
import TagEditViewModel from '@ViewModels/TagEditViewModel';
import $ from 'jquery';
import ko from 'knockout';

const vocaDbContext = container.get(VocaDbContext);
const repoFactory = container.get(RepositoryFactory);

function initPage(): void {
	$('#trashLink').button({ icons: { primary: 'ui-icon-trash' } });
	$('#deleteLink').button({ icons: { primary: 'ui-icon-trash' } });
	$('#restoreLink').button({ icons: { primary: 'ui-icon-trash' } });
}

const TagEdit = (model: { id: number }): void => {
	$(document).ready(function () {
		initPage();

		var urlMapper = new UrlMapper(vocaDbContext.baseAddress);
		var tagRepo = repoFactory.tagRepository();
		var userRepo = repoFactory.userRepository();

		tagRepo
			.getById({
				id: model.id,
				fields:
					'AliasedTo,TranslatedDescription,Names,Parent,RelatedTags,WebLinks',
				lang: vocaDbContext.languagePreference,
			})
			.then(function (contract) {
				var viewModel = new TagEditViewModel(urlMapper, userRepo, contract);
				ko.applyBindings(viewModel);
			});
	});
};

export default TagEdit;
