
module vdb.viewModels.tags {
	
	export class TagsEditViewModel {

		constructor(private repo: ITagSelectionsRepository) { }
		
		public addTag = () => {
			
			var tagName = this.newTagName();

			if (!tagName)
				return;

			if (!this.isValidTagName(tagName)) {
				// TODO: localize
				alert("Tag name may contain only word characters and cannot be empty.");
				return;
			}

			tagName = _.trim(tagName).replace(" ", "_");

			// If tag is already added, select it
			var selection = _.find(this.selections(), sel => sel.tagName === tagName);

			if (selection) {
				selection.selected(true);
			} else {
				this.selections.push(new TagSelectionViewModel({ tagName: tagName, selected: true }));
			}

		}

		public autoCompletedTag = (tagName: string) => {
			
			this.newTagName(tagName);
			this.addTag();

		}

        public dialogVisible = ko.observable(false);

		private isValidTagName = (tagName: string) => {
			
			var regex = /^[a-z0-9_\- ]+$/i;
			return regex.test(tagName);

		}

		public newTagName = ko.observable("");

		private selections = ko.observableArray<TagSelectionViewModel>();
		
		public save = () => {

			var selectedTags = _.chain(this.selections()).filter(sel => sel.selected()).map(sel => sel.tagName).value();
			this.repo.saveTagSelections(selectedTags);
			this.dialogVisible(false);

		}

		public show = () => {
			
			this.repo.getTagSelections(selections => {
				this.selections(_.map(selections, selection => new TagSelectionViewModel(selection)));
				this.dialogVisible(true);
			});

		}
		
	}

	export class TagSelectionViewModel {
		
		constructor(contract: dataContracts.tags.TagSelectionContract) {
		
			this.tagName = contract.tagName;
			this.selected = ko.observable(contract.selected || false);

		}

		selected: KnockoutObservable<boolean>;

		tagName: string;

	}

	export interface ITagSelectionsRepository {
		
		getTagSelections(callback: (selections: dataContracts.tags.TagSelectionContract[]) => void): void;

		saveTagSelections(tags: string[]): void;

	}

} 