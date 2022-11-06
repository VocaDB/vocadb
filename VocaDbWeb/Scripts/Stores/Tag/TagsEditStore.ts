import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { TagSelectionContract } from '@/DataContracts/Tag/TagSelectionContract';
import { TagUsageForApiContract } from '@/DataContracts/Tag/TagUsageForApiContract';
import { EntryType } from '@/Models/EntryType';
import { trim } from 'lodash-es';
import { action, makeObservable, observable, runInAction } from 'mobx';

interface ITagSelectionsRepository {
	getTagSelections(): Promise<TagSelectionContract[]>;

	saveTagSelections(tags: TagBaseContract[]): void;
}

class TagSelectionStore {
	@observable selected: boolean;
	readonly tag: TagBaseContract;

	constructor(contract: TagSelectionContract) {
		makeObservable(this);

		this.tag = contract.tag;
		this.selected = contract.selected || false;
	}
}

export class TagsEditStore {
	@observable dialogVisible = false;
	@observable selections: TagSelectionStore[] = [];
	@observable suggestions: TagUsageForApiContract[] = [];
	@observable suggestionsLoaded = false;

	constructor(
		private readonly repo: ITagSelectionsRepository,
		readonly target?: EntryType,
		readonly getSuggestions?: () => Promise<TagUsageForApiContract[]>,
	) {
		makeObservable(this);
	}

	@action addTag = (tagName: string): void => {
		if (!tagName) return;

		tagName = trim(tagName);

		// If tag is already added, select it
		const selection = this.selections.find(
			(sel) => sel.tag.name.toLowerCase() === tagName.toLowerCase(),
		);

		if (selection) {
			selection.selected = true;
		} else {
			this.selections.push(
				new TagSelectionStore({
					tag: { name: tagName, id: undefined!, status: undefined! },
					selected: true,
				}),
			);
		}
	};

	@action autoCompletedTag = (tag: TagBaseContract): void => {
		const selection = this.selections.find((sel) => sel.tag.id === tag.id);

		if (selection) {
			selection.selected = true;
		} else {
			this.selections.push(new TagSelectionStore({ tag: tag, selected: true }));
		}
	};

	getSuggestionText = (
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

	@action save = (): void => {
		const tags = this.selections
			.filter((sel) => sel.selected)
			.map((sel) => sel.tag);

		this.repo.saveTagSelections(tags);
		this.dialogVisible = false;
	};

	@action show = (): void => {
		this.repo.getTagSelections().then((selections) => {
			runInAction(() => {
				this.selections = selections.map(
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
