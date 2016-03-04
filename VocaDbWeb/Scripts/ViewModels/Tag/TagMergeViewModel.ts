
module vdb.viewModels.tags {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class TagMergeViewModel {

		constructor(tagRepo: rep.TagRepository, private base: dc.TagBaseContract) {

			this.target = new BasicEntryLinkViewModel(null, (id, callback) => tagRepo.getById(id, null, null, callback));

			ko.computed(() => {

				var result = helpers.EntryMergeValidationHelper.validateEntry(this.base, this.target.entry());
				this.validationError_targetIsLessComplete(result.validationError_targetIsLessComplete);
				this.validationError_targetIsNewer(result.validationError_targetIsNewer);

			});

		}

		public tagFilter = (tag: dc.TagApiContract) => {
			return tag.id !== this.base.id;
		}

		public target: BasicEntryLinkViewModel<dc.TagBaseContract>;

		public validationError_targetIsLessComplete = ko.observable(false);
		public validationError_targetIsNewer = ko.observable(false);

	}

}