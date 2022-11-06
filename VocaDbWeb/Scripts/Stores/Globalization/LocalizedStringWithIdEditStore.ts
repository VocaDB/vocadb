import { LocalizedStringWithIdContract } from '@/DataContracts/Globalization/LocalizedStringWithIdContract';
import { ContentLanguageSelection } from '@/Models/Globalization/ContentLanguageSelection';
import { makeObservable, observable } from 'mobx';

export class LocalizedStringWithIdEditStore {
	readonly id: number;
	@observable language = ContentLanguageSelection.Unspecified;
	@observable value: string;

	constructor(
		language: ContentLanguageSelection = ContentLanguageSelection.Unspecified,
		value: string = '',
		id: number = 0,
	) {
		makeObservable(this);

		this.language = language;
		this.value = value;
		this.id = id;
	}

	static fromContract(
		contract: LocalizedStringWithIdContract,
	): LocalizedStringWithIdEditStore {
		return new LocalizedStringWithIdEditStore(
			contract.language,
			contract.value,
			contract.id,
		);
	}
}
