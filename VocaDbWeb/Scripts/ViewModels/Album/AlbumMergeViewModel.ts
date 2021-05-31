import AlbumContract from '@DataContracts/Album/AlbumContract';
import EntryMergeValidationHelper from '@Helpers/EntryMergeValidationHelper';
import { ArtistAutoCompleteParams } from '@KnockoutExtensions/AutoCompleteParams';
import AlbumRepository from '@Repositories/AlbumRepository';
import vdb from '@Shared/VdbStatic';
import ko from 'knockout';

import BasicEntryLinkViewModel from '../BasicEntryLinkViewModel';

export default class AlbumMergeViewModel {
	constructor(repo: AlbumRepository, id: number) {
		this.target = new BasicEntryLinkViewModel<AlbumContract>(
			null!,
			(entryId, callback) =>
				repo.getOne(entryId, vdb.values.languagePreference).then(callback),
		);

		this.targetSearchParams = {
			acceptSelection: this.target.id,
			ignoreId: id,
		};

		repo.getOne(id, vdb.values.languagePreference).then((base) => {
			ko.computed(() => {
				var result = EntryMergeValidationHelper.validateEntry(
					base,
					this.target.entry(),
				);
				this.validationError_targetIsLessComplete(
					result.validationError_targetIsLessComplete,
				);
				this.validationError_targetIsNewer(
					result.validationError_targetIsNewer,
				);
			});
		});
	}

	public target: BasicEntryLinkViewModel<AlbumContract>;
	public targetSearchParams: ArtistAutoCompleteParams;

	public validationError_targetIsLessComplete = ko.observable(false);
	public validationError_targetIsNewer = ko.observable(false);
}
