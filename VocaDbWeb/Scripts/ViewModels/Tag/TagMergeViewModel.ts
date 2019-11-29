
import BasicEntryLinkViewModel from '../BasicEntryLinkViewModel';
import EntryMergeValidationHelper from '../../Helpers/EntryMergeValidationHelper';
import TagApiContract from '../../DataContracts/Tag/TagApiContract';
import TagBaseContract from '../../DataContracts/Tag/TagBaseContract';
import TagRepository from '../../Repositories/TagRepository';

//module vdb.viewModels.tags {

	export class TagMergeViewModel {

		constructor(tagRepo: TagRepository, private base: TagBaseContract) {

			this.target = new BasicEntryLinkViewModel(null, (id, callback) => tagRepo.getById(id, null, null, callback));

			ko.computed(() => {

				var result = EntryMergeValidationHelper.validateEntry(this.base, this.target.entry());
				this.validationError_targetIsLessComplete(result.validationError_targetIsLessComplete);
				this.validationError_targetIsNewer(result.validationError_targetIsNewer);

			});

		}

		public tagFilter = (tag: TagApiContract) => {
			return tag.id !== this.base.id;
		}

		public target: BasicEntryLinkViewModel<TagBaseContract>;

		public validationError_targetIsLessComplete = ko.observable(false);
		public validationError_targetIsNewer = ko.observable(false);

	}

//}