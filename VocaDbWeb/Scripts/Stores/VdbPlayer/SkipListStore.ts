import { action, makeObservable, observable } from 'mobx';

export class SkipListStore {
	@observable public dialogVisible = false;

	public constructor() {
		makeObservable(this);
	}

	@action public showDialog = (): void => {
		this.dialogVisible = true;
	};

	@action public hideDialog = (): void => {
		this.dialogVisible = false;
	};
}
