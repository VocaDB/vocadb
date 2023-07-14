import { PVContract } from '@/DataContracts/PVs/PVContract';
import { DateTimeHelper } from '@/Helpers/DateTimeHelper';
import { PVServiceIcons } from '@/Models/PVServiceIcons';
import { PVType } from '@/Models/PVs/PVType';
import { PVRepository } from '@/Repositories/PVRepository';
import { HttpClientError } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import $ from 'jquery';
import { pull } from 'lodash-es';
import { action, makeObservable, observable, runInAction } from 'mobx';

export class PVEditStore {
	// eslint-disable-next-line prettier/prettier
	@observable disabled: boolean;
	readonly lengthFormatted: string;
	@observable name: string;
	readonly pvType: PVType;

	constructor(readonly contract: PVContract, pvType?: PVType) {
		makeObservable(this);

		this.disabled = contract.disabled!;
		this.pvType = pvType || contract.pvType;

		this.name = contract.name!;
		this.lengthFormatted = DateTimeHelper.formatFromSeconds(contract.length!);
	}
}

export class PVListEditStore {
	@observable isPossibleInstrumental = false;
	@observable newPvType = PVType.Original;
	@observable newPvUrl = '';
	@observable pvs: PVEditStore[];
	readonly pvServiceIcons: PVServiceIcons;

	constructor(
		private readonly pvRepo: PVRepository,
		urlMapper: UrlMapper,
		pvs: PVContract[],
		readonly canBulkDeletePVs: boolean,
		readonly showPublishDates: boolean,
		readonly allowDisabled: boolean,
	) {
		makeObservable(this);

		this.pvServiceIcons = new PVServiceIcons(urlMapper);
		this.pvs = pvs.map((pv) => new PVEditStore(pv));
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

	@action add = (): void => {
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

	@action remove = (pv: PVEditStore): void => {
		pull(this.pvs, pv);
	};

	formatLength = (seconds: number): string => {
		return DateTimeHelper.formatFromSeconds(seconds);
	};

	getPvServiceIcon = (service: string): string => {
		return this.pvServiceIcons.getIconUrl(service);
	};

	toContracts = (): PVContract[] => {
		return this.pvs.map((pv) => ({
			...pv.contract,
			disabled: pv.disabled,
			name: pv.name,
			pvType: pv.pvType,
		}));
	};

	uploadMedia = async (uploadMedia: File): Promise<void> => {
		const fd = new FormData();

		fd.append('file', uploadMedia);
		await $.ajax({
			url: '/Song/PostMedia/',
			data: fd,
			processData: false,
			contentType: false,
			type: 'POST',
			success: (result) =>
				runInAction(() => {
					this.pvs.push(new PVEditStore(result, PVType.Original));
				}),
			error: (result) => {
				const text =
					result.status === 404 ? 'File too large' : result.statusText;
				alert(`Unable to post file: ${text}` /* LOC */);
			},
		});
	};
}
