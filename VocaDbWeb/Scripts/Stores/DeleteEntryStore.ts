import { AntiforgeryRepository } from '@/Repositories/AntiforgeryRepository';
import { action, computed, makeObservable, observable } from 'mobx';

export class DeleteEntryStore {
	@observable dialogVisible = false;
	@observable notes = '';

	constructor(
		private readonly antiforgeryRepo: AntiforgeryRepository,
		private readonly deleteCallback: (
			requestToken: string,
			notes: string,
		) => Promise<void>,
		readonly notesRequired = false,
	) {
		makeObservable(this);
	}

	@computed get isValid(): boolean {
		return !this.notesRequired || !!this.notes;
	}

	@action deleteEntry = async (): Promise<void> => {
		this.dialogVisible = false;

		const requestToken = await this.antiforgeryRepo.getToken();

		return this.deleteCallback(requestToken, this.notes);
	};

	@action show = (): void => {
		this.dialogVisible = true;
	};
}
