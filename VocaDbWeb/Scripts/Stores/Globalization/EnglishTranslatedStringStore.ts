import { action, makeObservable, observable } from 'mobx';

export class EnglishTranslatedStringStore {
	@observable isFullDescriptionShown = false;
	@observable showTranslatedDescription: boolean;

	constructor(showTranslatedDescription: boolean) {
		makeObservable(this);

		this.showTranslatedDescription = showTranslatedDescription;
	}

	@action showFullDescription = (): void => {
		this.isFullDescriptionShown = true;
	};
}
