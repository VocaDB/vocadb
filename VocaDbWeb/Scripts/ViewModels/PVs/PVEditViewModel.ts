import PVContract from '@DataContracts/PVs/PVContract';
import DateTimeHelper from '@Helpers/DateTimeHelper';
import ko, { Observable } from 'knockout';

export default class PVEditViewModel {
	constructor(contract: PVContract, pvType?: string) {
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

	author: string;

	createdBy: number;

	disabled: Observable<boolean>;

	extendedMetadata: string;

	id: number;

	length: number;

	lengthFormatted: string;

	name: Observable<string>;

	pvId: string;

	service: string;

	publishDate: string;

	pvType: string;

	thumbUrl: string;

	url: string;
}
