
module vdb.viewModels.tags {
	
	export class TagCreateViewModel {

		constructor(private tagRepo: repositories.TagRepository) {

			this.newTagName.subscribe(val => {

				if (!val) {
					this.duplicateName(false);
					return;
				}

				tagRepo.getList({ start: 0, maxResults: 1, getTotalCount: false, query: val, nameMatchMode: models.NameMatchMode.Exact, allowAliases: true }, result => {
					this.duplicateName(result.items.length > 0);
				});
			});

		}

		public createTag = () => {
			this.tagRepo.create(this.newTagName(), t => window.location.href = utils.EntryUrlMapper.details_tag_contract(t));
		}

		public dialogVisible = ko.observable(false);

		public duplicateName = ko.observable(false);

		public newTagName = ko.observable("").extend({ rateLimit: { timeout: 100, method: "notifyWhenChangesStop" } });

		public isValid = ko.computed(() => this.newTagName() && !this.duplicateName());

	}

}