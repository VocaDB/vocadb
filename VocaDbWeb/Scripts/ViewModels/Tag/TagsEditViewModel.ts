import EntryType from '../../Models/EntryType';
import TagBaseContract from '../../DataContracts/Tag/TagBaseContract';
import TagSelectionContract from '../../DataContracts/Tag/TagSelectionContract';
import TagUsageForApiContract from '../../DataContracts/Tag/TagUsageForApiContract';

//module vdb.viewModels.tags {

	export default class TagsEditViewModel {

		constructor(private readonly repo: ITagSelectionsRepository, 
			public readonly target?: EntryType, 
			private readonly getSuggestions?: (callback: (result: TagUsageForApiContract[]) => void) => void) { }
		
		public addTag = () => {
			
			var tagName = this.newTagName();

			if (!tagName)
				return;

			this.newTagName("");

			tagName = _.trim(tagName);

			// If tag is already added, select it
			var selection = _.find(this.selections(), sel => sel.tag.name.toLowerCase() === tagName.toLowerCase());

			if (selection) {
				selection.selected(true);
			} else {
				this.selections.push(new TagSelectionViewModel({ tag: { name: tagName, id: null }, selected: true }));
			}

		}

		public autoCompletedTag = (tag: TagBaseContract) => {

			var selection = _.find(this.selections(), sel => sel.tag.id === tag.id);

			if (selection) {
				selection.selected(true);
			} else {
				this.selections.push(new TagSelectionViewModel({ tag: tag, selected: true }));
			}

		}

        public dialogVisible = ko.observable(false);

		public getSuggestionText = (suggestion: TagUsageForApiContract, countText: string) => {

			var text = "";

			if (suggestion.tag.additionalNames) {
				text += suggestion.tag.additionalNames + '\n';
			}

			if (suggestion.count > 0) {
				text += countText.replace('{0}', suggestion.count.toString());
			}

			return text;

		}

		public newTagName = ko.observable("");

		public selections = ko.observableArray<TagSelectionViewModel>();
		
		public save = () => {

			var tags = _
				.chain(this.selections())
				.filter(sel => sel.selected())
				.map(sel => sel.tag)
				.value();

			this.repo.saveTagSelections(tags);
			this.dialogVisible(false);

		}

		public show = () => {
			
			this.repo.getTagSelections(selections => {
				this.selections(_.map(selections, selection => new TagSelectionViewModel(selection)));
				this.dialogVisible(true);
			});

			if (this.getSuggestions) {
				this.suggestionsLoaded(false);
				this.getSuggestions(result => {
					this.suggestions(result);
					this.suggestionsLoaded(true);
				});
			}

		}

		public suggestions = ko.observableArray<TagUsageForApiContract>();
		public suggestionsLoaded = ko.observable(false);
		
	}

	export class TagSelectionViewModel {
		
		constructor(contract: TagSelectionContract) {
		
			this.tag = contract.tag;
			this.selected = ko.observable(contract.selected || false);

		}

		selected: KnockoutObservable<boolean>;

		tag: TagBaseContract;

	}

	export interface ITagSelectionsRepository {
		
		getTagSelections(callback: (selections: TagSelectionContract[]) => void): void;

		saveTagSelections(tags: TagBaseContract[]): void;

	}

//} 