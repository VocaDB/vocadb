import { PVContract } from '@/DataContracts/PVs/PVContract';
import { DateTimeHelper } from '@/Helpers/DateTimeHelper';
import { PVType } from '@/Models/PVs/PVType';
import ko, { Observable } from 'knockout';

export class PVEditViewModel {
	public constructor(public readonly contract: PVContract, pvType?: PVType) {
		this.disabled = ko.observable(contract.disabled!);
		this.pvType = pvType || contract.pvType;

		this.name = ko.observable(contract.name!);
		this.lengthFormatted = DateTimeHelper.formatFromSeconds(contract.length!);
	}

	public disabled: Observable<boolean>;

	public lengthFormatted: string;

	public name: Observable<string>;

	public pvType: PVType;
}
