
module vdb.viewModels.tags {
	
	export class TagsEditViewModel {

		constructor(private repo: ITagSelectionsRepository) { }
		
		public addTag = () => {
			
			var tagName = this.newTagName();

			if (!tagName)
				return;

			this.newTagName("");

			tagName = _.trim(tagName);

			// If tag is already added, select it
			var selection = _.find(this.selections(), sel => sel.tagName.toLowerCase() === tagName.toLowerCase());

			if (selection) {
				selection.selected(true);
			} else {
				this.selections.push(new TagSelectionViewModel({ tag: { name: tagName, id: null }, selected: true }));
			}

		}

		public autoCompletedTag = (tag: dc.TagBaseContract) => {

			var selection = _.find(this.selections(), sel => sel.tagId === tag.id);

			if (selection) {
				selection.selected(true);
			} else {
				this.selections.push(new TagSelectionViewModel({ tag: tag, selected: true }));
			}

		}

        public dialogVisible = ko.observable(false);

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
		
			this.tagId = contract.tag.id;
			this.tagName = contract.tag.name;
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