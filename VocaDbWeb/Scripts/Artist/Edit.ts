import ArtistForEditContract from '@DataContracts/Artist/ArtistForEditContract';
import TranslatedEnumField from '@DataContracts/TranslatedEnumField';
import RepositoryFactory from '@Repositories/RepositoryFactory';
import DialogService from '@Shared/DialogService';
import UrlMapper from '@Shared/UrlMapper';
import vdb from '@Shared/VdbStatic';
import { container } from '@Shared/inversify.config';
import ArtistEditViewModel from '@ViewModels/Artist/ArtistEditViewModel';
import $ from 'jquery';
import ko from 'knockout';

function initPage(): void {
	$('#tabs').tabs();
	$('#deleteLink').button({ icons: { primary: 'ui-icon-trash' } });
	$('#restoreLink').button({ icons: { primary: 'ui-icon-trash' } });
	$('#mergeLink').button();
}

const ArtistEdit = (
	model: {
		artist: {
			id: number;
		};
		editedArtist: ArtistForEditContract;
	},
	saveWarning: any,
	webLinkCategoryJson: TranslatedEnumField[],
): void => {
	$(document).ready(function () {
		vdb.resources.entryEdit = {
			saveWarning: saveWarning,
		};

		var urlMapper = new UrlMapper(vdb.values.baseAddress);
		const repoFactory = container.get(RepositoryFactory);
		var artistRepo = repoFactory.artistRepository();
		var userRepo = repoFactory.userRepository();
		var editedModel = model.editedArtist;

		if (editedModel) {
			ko.applyBindings(
				new ArtistEditViewModel(
					artistRepo,
					userRepo,
					urlMapper,
					webLinkCategoryJson,
					editedModel,
					new DialogService(),
				),
			);
		} else {
			artistRepo.getForEdit(model.artist.id).then(function (model) {
				ko.applyBindings(
					new ArtistEditViewModel(
						artistRepo,
						userRepo,
						urlMapper,
						webLinkCategoryJson,
						model,
						new DialogService(),
					),
				);
			});
		}

		initPage();
	});
};

export default ArtistEdit;
