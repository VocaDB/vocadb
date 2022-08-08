import LocalizedStringWithIdContract from '@/DataContracts/Globalization/LocalizedStringWithIdContract';
import ContentLanguageSelection from '@/Models/Globalization/ContentLanguageSelection';
import { computed, makeObservable, observable } from 'mobx';

export default class LocalizedStringWithIdEditStore {
	public readonly id: number;
	@observable public language = ContentLanguageSelection.Unspecified;
	@observable public value: string;

	public constructor(
		language: ContentLanguageSelection = ContentLanguageSelection.Unspecified,
		value: string = '',
		id: number = 0,
	) {
		makeObservable(this);

		this.language = language;
		this.value = value;
		this.id = id;
	}

	public static fromContract(
		contract: LocalizedStringWithIdContract,
	): LocalizedStringWithIdEditStore {
		return new LocalizedStringWithIdEditStore(
			ContentLanguageSelection[
				contract.language as keyof typeof ContentLanguageSelection
			],
			contract.value,
			contract.id,
		);
	}

	@computed public get languageStr(): string {
		return ContentLanguageSelection[this.language];
	}
	public set languageStr(value: string) {
		this.language =
			ContentLanguageSelection[value as keyof typeof ContentLanguageSelection];
	}
}
