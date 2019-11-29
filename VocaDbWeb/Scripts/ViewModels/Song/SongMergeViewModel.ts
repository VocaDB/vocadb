
//module vdb.viewModels.songs {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class SongMergeViewModel {

		constructor(songRepo: rep.SongRepository, private base: dc.SongContract) {

			this.target = new BasicEntryLinkViewModel(null, songRepo.getOne);

			this.targetSearchParams = {
				acceptSelection: this.target.id,
				ignoreId: base.id
			};

			ko.computed(() => {

				var result = helpers.EntryMergeValidationHelper.validateEntry(this.base, this.target.entry());
				this.validationError_targetIsLessComplete(result.validationError_targetIsLessComplete);
				this.validationError_targetIsNewer(result.validationError_targetIsNewer);

			});

			/*this.validationError_targetIsLessComplete = ko.computed(helpers.EntryMergeValidationHelper.() => this.target.entry() &&
				models.EntryStatus[this.target.entry().status] < models.EntryStatus[this.base.status]);

			this.validationError_targetIsNewer = ko.computed(() => this.target.entry() &&
				moment(this.target.entry().createDate) > moment(this.base.createDate));*/

		}

		public target: BasicEntryLinkViewModel<dc.SongContract>;
		public targetSearchParams: vdb.knockoutExtensions.SongAutoCompleteParams;

		public validationError_targetIsLessComplete = ko.observable(false);
		public validationError_targetIsNewer = ko.observable(false);

		//public validationError_targetIsLessComplete: KnockoutObservable<boolean>;
		//public validationError_targetIsNewer: KnockoutObservable<boolean>;

	}

//}