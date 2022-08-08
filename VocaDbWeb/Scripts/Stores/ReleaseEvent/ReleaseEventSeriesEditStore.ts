import ReleaseEventSeriesForEditContract from '@/DataContracts/ReleaseEvents/ReleaseEventSeriesForEditContract';
import NameMatchMode from '@/Models/NameMatchMode';
import ReleaseEventRepository from '@/Repositories/ReleaseEventRepository';
import { action, makeObservable, observable, runInAction } from 'mobx';

import DeleteEntryStore from '../DeleteEntryStore';
import NamesEditStore from '../Globalization/NamesEditStore';
import WebLinksEditStore from '../WebLinksEditStore';

export default class ReleaseEventSeriesEditStore {
	@observable public category: string /* TODO: enum */;
	@observable public defaultNameLanguage: string;
	public readonly deleteStore = new DeleteEntryStore((notes) =>
		this.eventRepo.deleteSeries({
			id: this.contract.id,
			notes: notes,
			hardDelete: false,
		}),
	);
	@observable public description: string;
	@observable public duplicateName?: string;
	@observable public errors?: Record<string, string[]>;
	public readonly names: NamesEditStore;
	@observable public status: string /* TODO: enum */;
	@observable public submitting = false;
	public readonly trashStore = new DeleteEntryStore((notes) =>
		this.eventRepo.deleteSeries({
			id: this.contract.id,
			notes: notes,
			hardDelete: true,
		}),
	);
	public readonly webLinks: WebLinksEditStore;

	public constructor(
		private readonly eventRepo: ReleaseEventRepository,
		public readonly contract: ReleaseEventSeriesForEditContract,
	) {
		makeObservable(this);

		this.category = contract.category;
		this.defaultNameLanguage = contract.defaultNameLanguage;
		this.description = contract.description;
		this.names = NamesEditStore.fromContracts(contract.names);
		this.status = contract.status;
		this.webLinks = new WebLinksEditStore(contract.webLinks);
	}

	@action public checkName = async (value: string): Promise<void> => {
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

	@action public submit = async (
		pictureUpload: File | undefined,
	): Promise<number> => {
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
