import TagBaseContract from '@DataContracts/Tag/TagBaseContract';
import TagSelectionContract from '@DataContracts/Tag/TagSelectionContract';
import TagUsageForApiContract from '@DataContracts/Tag/TagUsageForApiContract';
import EntryType from '@Models/EntryType';
import _ from 'lodash';
import { action, makeObservable, observable, runInAction } from 'mobx';

interface ITagSelectionsRepository {
	getTagSelections(): Promise<TagSelectionContract[]>;

	saveTagSelections(tags: TagBaseContract[]): void;
}

class TagSelectionStore {
	@observable public selected: boolean;
	public readonly tag: TagBaseContract;

	public constructor(contract: TagSelectionContract) {
		makeObservable(this);

		this.tag = contract.tag;
		this.selected = contract.selected || false;
	}
}

export default class TagsEditStore {
	@observable public dialogVisible = false;
	@observable public selections: TagSelectionStore[] = [];
	@observable public suggestions: TagUsageForApiContract[] = [];
	@observable public suggestionsLoaded = false;

	public constructor(
		private readonly repo: ITagSelectionsRepository,
		public readonly target?: EntryType,
		public readonly getSuggestions?: () => Promise<TagUsageForApiContract[]>,
	) {
		makeObservable(this);
	}

	@action public addTag = (tagName: string): void => {
		if (!tagName) return;

		tagName = _.trim(tagName);

		// If tag is already added, select it
		const selection = _.find(
			this.selections,
			(sel) => sel.tag.name.toLowerCase() === tagName.toLowerCase(),
		);

		if (selection) {
			selection.selected = true;
		} else {
			this.selections.push(
				new TagSelectionStore({
					tag: { name: tagName, id: undefined! },
					selected: true,
				}),
			);
		}
	};

	@action public autoCompletedTag = (tag: TagBaseContract): void => {
		const selection = _.find(this.selections, (sel) => sel.tag.id === tag.id);

		if (selection) {
			selection.selected = true;
		} else {
			this.selections.push(new TagSelectionStore({ tag: tag, selected: true }));
		}
	};

	public getSuggestionText = (
		suggestion: TagUsageForApiContract,
		countText: string,
	): string => {
		var text = '';

		if (suggestion.tag.additionalNames) {
			text += `${suggestion.tag.additionalNames}\n`;
		}

		if (suggestion.count > 0) {
			text += countText.replace('{0}', suggestion.count.toString());
		}

		return text;
	};

	@action public save = (): void => {
		const tags = _.chain(this.selections)
			.filter((sel) => sel.selected)
			.map((sel) => sel.tag)
			.value();

		this.repo.saveTagSelections(tags);
		this.dialogVisible = false;
	};

	@action public show = (): void => {
		this.repo.getTagSelections().then((selections) => {
			runInAction(() => {
				this.selections = _.map(
					selections,
					(selection) => new TagSelectionStore(selection),
				);
				this.dialogVisible = true;
			});
		});

		if (this.getSuggestions) {
			this.suggestionsLoaded = false;
			this.getSuggestions().then((result) => {
				runInAction(() => {
					this.suggestions = result;
					this.suggestionsLoaded = true;
				});
			});
		}
	};
}
