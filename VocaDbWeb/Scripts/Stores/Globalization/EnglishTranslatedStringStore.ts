import { action, makeObservable, observable } from 'mobx';

export default class EnglishTranslatedStringStore {
	@observable public isFullDescriptionShown = false;
	@observable public showTranslatedDescription: boolean;

	public constructor(showTranslatedDescription: boolean) {
		makeObservable(this);

		this.showTranslatedDescription = showTranslatedDescription;
	}

	@action public showFullDescription = (): void => {
		this.isFullDescriptionShown = true;
	};
}
