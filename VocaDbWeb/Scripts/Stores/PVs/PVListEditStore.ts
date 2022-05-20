import _ from 'lodash';
import { action, makeObservable, observable, runInAction } from 'mobx';

import PVContract from '../../DataContracts/PVs/PVContract';
import DateTimeHelper from '../../Helpers/DateTimeHelper';
import PVServiceIcons from '../../Models/PVServiceIcons';
import PVType from '../../Models/PVs/PVType';
import PVRepository from '../../Repositories/PVRepository';
import { HttpClientError } from '../../Shared/HttpClient';

export class PVEditStore {
	public readonly author: string;
	public readonly createdBy: number;
	@observable public disabled: boolean;
	public readonly extendedMetadata: string;
	public readonly id: number;
	public readonly length: number;
	public readonly lengthFormatted: string;
	@observable public name: string;
	public readonly pvId: number;
	public readonly service: string;
	public readonly publishDate: string;
	public readonly pvType: string;
	public readonly thumbUrl: string;
	public readonly url: string;

	public constructor(contract: PVContract, pvType?: string) {
		makeObservable(this);

		this.author = contract.author;
		this.createdBy = contract.createdBy;
		this.disabled = contract.disabled;
		this.extendedMetadata = contract.extendedMetadata;
		this.id = contract.id;
		this.length = contract.length;
		this.pvId = contract.pvId;
		this.service = contract.service;
		this.publishDate = contract.publishDate;
		this.pvType = pvType || contract.pvType;
		this.thumbUrl = contract.thumbUrl;
		this.url = contract.url;

		this.name = contract.name;
		this.lengthFormatted = DateTimeHelper.formatFromSeconds(this.length);
	}
}

export default class PVListEditStore {
	@observable public isPossibleInstrumental = false;
	@observable public newPvType = PVType[PVType.Original];
	@observable public newPvUrl = '';
	@observable public pvs: PVEditStore[];
	public readonly pvServiceIcons: PVServiceIcons;

	public constructor(private readonly pvRepo: PVRepository) {
		makeObservable(this);
	}

	// Attempts to identify whether the PV could be instrumental
	private isPossibleInstrumentalPv = (pv: PVContract): boolean => {
		return (
			!!pv &&
			!!pv.name &&
			(pv.name.toLowerCase().indexOf('inst.') >= 0 ||
				pv.name.toLowerCase().indexOf('instrumental') >= 0 ||
				pv.name.indexOf('カラオケ') >= 0 ||
				pv.name.indexOf('オフボーカル') >= 0)
		);
	};

	@action public add = (): void => {
		const newPvUrl = this.newPvUrl;

		if (!newPvUrl) return;

		const pvType = this.newPvType;

		this.pvRepo
			.getPVByUrl({ pvUrl: newPvUrl, type: this.newPvType })
			.then((pv) => {
				runInAction(() => {
					this.newPvUrl = '';
					this.isPossibleInstrumental = this.isPossibleInstrumentalPv(pv);
					this.pvs.push(new PVEditStore(pv, pvType));
				});
			})
			.catch(({ response }: HttpClientError) => {
				const error = response?.data || response?.statusText;

				if (error) alert(error);
			});
	};

	@action public remove = (pv: PVEditStore): void => {
		_.pull(this.pvs, pv);
	};

	public formatLength = (seconds: number): string => {
		return DateTimeHelper.formatFromSeconds(seconds);
	};

	public getPvServiceIcon = (service: string): string => {
		return this.pvServiceIcons.getIconUrl(service);
	};

	public toContracts = (): PVContract[] => {
		return this.pvs;
	};

	public uploadMedia = (): void => {
		// TODO
	};
}
