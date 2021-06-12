import PVContract from '@DataContracts/PVs/PVContract';
import DateTimeHelper from '@Helpers/DateTimeHelper';
import ko, { Observable } from 'knockout';

export default class PVEditViewModel {
	public constructor(contract: PVContract, pvType?: string) {
		this.author = contract.author!;
		this.createdBy = contract.createdBy!;
		this.disabled = ko.observable(contract.disabled!);
		this.extendedMetadata = contract.extendedMetadata;
		this.id = contract.id!;
		this.length = contract.length!;
		this.pvId = contract.pvId;
		this.service = contract.service;
		this.publishDate = contract.publishDate!;
		this.pvType = pvType || contract.pvType;
		this.thumbUrl = contract.thumbUrl!;
		this.url = contract.url!;

		this.name = ko.observable(contract.name!);
		this.lengthFormatted = DateTimeHelper.formatFromSeconds(this.length);
	}

	public author: string;

	public createdBy: number;

	public disabled: Observable<boolean>;

	public extendedMetadata: string;

	public id: number;

	public length: number;

	public lengthFormatted: string;

	public name: Observable<string>;

	public pvId: string;

	public service: string;

	public publishDate: string;

	public pvType: string;

	public thumbUrl: string;

	public url: string;
}
