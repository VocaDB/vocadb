
//module vdb.viewModels.artists {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class ArtistMergeViewModel {

		constructor(repo: rep.ArtistRepository, id: number) {

			this.target = new BasicEntryLinkViewModel(null, repo.getOne);

			this.targetSearchParams = {
				acceptSelection: this.target.id,
				ignoreId: id
			};

			repo.getOne(id, base => {

				ko.computed(() => {

					var result = helpers.EntryMergeValidationHelper.validateEntry(base, this.target.entry());
					this.validationError_targetIsLessComplete(result.validationError_targetIsLessComplete);
					this.validationError_targetIsNewer(result.validationError_targetIsNewer);

				});

			});

		}

		public target: BasicEntryLinkViewModel<dc.ArtistContract>;
		public targetSearchParams: vdb.knockoutExtensions.ArtistAutoCompleteParams;

		public validationError_targetIsLessComplete = ko.observable(false);
		public validationError_targetIsNewer = ko.observable(false);

	}

//} 