import { ReleaseEventSeriesForEditContract } from '@/DataContracts/ReleaseEvents/ReleaseEventSeriesForEditContract';
import { EntryStatus } from '@/Models/EntryStatus';
import { EventCategory } from '@/Models/Events/EventCategory';
import { ContentLanguageSelection } from '@/Models/Globalization/ContentLanguageSelection';
import { NameMatchMode } from '@/Models/NameMatchMode';
import { ReleaseEventRepository } from '@/Repositories/ReleaseEventRepository';
import { DeleteEntryStore } from '@/Stores/DeleteEntryStore';
import { NamesEditStore } from '@/Stores/Globalization/NamesEditStore';
import { WebLinksEditStore } from '@/Stores/WebLinksEditStore';
import { action, makeObservable, observable, runInAction } from 'mobx';

export class ReleaseEventSeriesEditStore {
	@observable category: EventCategory;
	@observable defaultNameLanguage: ContentLanguageSelection;
	readonly deleteStore: DeleteEntryStore;
	@observable description: string;
	@observable duplicateName?: string;
	@observable errors?: Record<string, string[]>;
	readonly names: NamesEditStore;
	@observable status: EntryStatus;
	@observable submitting = false;
	readonly trashStore: DeleteEntryStore;
	readonly webLinks: WebLinksEditStore;

	constructor(
		private readonly eventRepo: ReleaseEventRepository,
		readonly contract: ReleaseEventSeriesForEditContract,
	) {
		makeObservable(this);

		this.deleteStore = new DeleteEntryStore((notes) =>
			this.eventRepo.deleteSeries({
				id: this.contract.id,
				notes: notes,
				hardDelete: false,
			}),
		);

		this.trashStore = new DeleteEntryStore((notes) =>
			this.eventRepo.deleteSeries({
				id: this.contract.id,
				notes: notes,
				hardDelete: true,
			}),
		);

		this.category = contract.category;
		this.defaultNameLanguage = contract.defaultNameLanguage;
		this.description = contract.description;
		this.names = NamesEditStore.fromContracts(contract.names);
		this.status = contract.status;
		this.webLinks = new WebLinksEditStore(contract.webLinks);
	}

	@action checkName = async (value: string): Promise<void> => {
		if (!value) {
			this.duplicateName = undefined;
			return;
		}

		const result = await this.eventRepo.getSeriesList({
			query: value,
			nameMatchMode: NameMatchMode.Exact,
			maxResults: 1,
		});

		runInAction(() => {
			this.duplicateName = result.items.length ? value : undefined;
		});
	};

	@action submit = async (pictureUpload: File | undefined): Promise<number> => {
		this.submitting = true;

		try {
			const id = await this.eventRepo.editSeries(
				{
					category: this.category,
					defaultNameLanguage: this.defaultNameLanguage,
					deleted: false,
					description: this.description,
					id: this.contract.id,
					name: '',
					names: this.names.toContracts(),
					status: this.status,
					webLinks: this.webLinks.items,
				},
				pictureUpload,
			);

			return id;
		} catch (error: any) {
			if (error.response) {
				runInAction(() => {
					this.errors = undefined;

					if (error.response.status === 400)
						this.errors = error.response.data.errors;
				});
			}

			throw error;
		} finally {
			runInAction(() => {
				this.submitting = false;
			});
		}
	};
}
