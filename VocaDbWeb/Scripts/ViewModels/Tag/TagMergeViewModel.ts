import TagApiContract from '@DataContracts/Tag/TagApiContract';
import TagBaseContract from '@DataContracts/Tag/TagBaseContract';
import EntryMergeValidationHelper from '@Helpers/EntryMergeValidationHelper';
import TagRepository from '@Repositories/TagRepository';
import ko from 'knockout';

import BasicEntryLinkViewModel from '../BasicEntryLinkViewModel';

export default class TagMergeViewModel {
	public constructor(tagRepo: TagRepository, private base: TagBaseContract) {
		this.target = new BasicEntryLinkViewModel<TagBaseContract>(null!, (id) =>
			tagRepo.getById({ id: id, fields: undefined, lang: undefined }),
		);

		ko.computed(() => {
			var result = EntryMergeValidationHelper.validateEntry(
				this.base,
				this.target.entry(),
			);
			this.validationError_targetIsLessComplete(
				result.validationError_targetIsLessComplete,
			);
			this.validationError_targetIsNewer(result.validationError_targetIsNewer);
		});
	}

	public tagFilter = (tag: TagApiContract): boolean => {
		return tag.id !== this.base.id;
	};

	public target: BasicEntryLinkViewModel<TagBaseContract>;

	public validationError_targetIsLessComplete = ko.observable(false);
	public validationError_targetIsNewer = ko.observable(false);
}
