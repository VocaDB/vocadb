
module vdb.viewModels.tags {
	
	export class TagsEditViewModel {

		constructor(private repo: ITagSelectionsRepository) { }
		
		public addTag = () => {
			
			var tagName = this.newTagName();

			if (!tagName)
				return;

			if (!this.isValidTagName(tagName)) {
				this.invalidTagError();
				return;
			}

			this.newTagName("");

			tagName = _.trim(tagName).replace(/ /g, "_"); // Simple text replace replaces only the first occurrence

			// If tag is already added, select it
			var selection = _.find(this.selections(), sel => sel.tagName === tagName);

			if (selection) {
				selection.selected(true);
			} else {
				this.selections.push(new TagSelectionViewModel({ tagName: tagName, selected: true, tagId: null }));
			}

		}

		public autoCompletedTag = (tag: dc.TagBaseContract) => {

			var selection = _.find(this.selections(), sel => sel.tagId === tag.id);

			if (selection) {
				selection.selected(true);
			} else {
				this.selections.push(new TagSelectionViewModel({ tagName: tag.name, selected: true, tagId: tag.id }));
			}

		}

        public dialogVisible = ko.observable(false);

		public invalidTagError = () => {
			// TODO: localize
			alert("Tag name may contain only word characters and cannot be empty.");			
		}

		private isValidTagName = (tagName: string) => {
			
			var regex = /^[a-z0-9_\- ]+$/i;
			return regex.test(tagName);

		}

		public newTagName = ko.observable("");

		public selections = ko.observableArray<TagSelectionViewModel>();
		
		public save = () => {

			var tags = _.chain(this.selections()).filter(sel => sel.selected()).map(sel => {
				return { id: sel.tagId, name: sel.tagName }
			}).value();
			this.repo.saveTagSelections(tags);
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
		
			this.tagId = contract.tagId;
			this.tagName = contract.tagName;
			this.selected = ko.observable(contract.selected || false);

		}

		selected: KnockoutObservable<boolean>;

		tagId: number;

		tagName: string;

	}

	export interface ITagSelectionsRepository {
		
		getTagSelections(callback: (selections: dataContracts.tags.TagSelectionContract[]) => void): void;

		saveTagSelections(tags: dc.TagBaseContract[]): void;

	}

} 