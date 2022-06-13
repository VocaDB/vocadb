import LocalizedStringWithIdContract from '@DataContracts/Globalization/LocalizedStringWithIdContract';
import ContentLanguageSelection from '@Models/Globalization/ContentLanguageSelection';
import _ from 'lodash';
import { action, makeObservable, observable } from 'mobx';

import LocalizedStringWithIdEditStore from './LocalizedStringWithIdEditStore';

export default class NamesEditStore {
	@observable public aliases: LocalizedStringWithIdEditStore[];
	public englishName: LocalizedStringWithIdEditStore;
	public originalName: LocalizedStringWithIdEditStore;
	public romajiName: LocalizedStringWithIdEditStore;

	public constructor(names: LocalizedStringWithIdEditStore[] = []) {
		makeObservable(this);

		this.englishName = NamesEditStore.nameOrEmpty(
			names,
			ContentLanguageSelection.English,
		);
		this.originalName = NamesEditStore.nameOrEmpty(
			names,
			ContentLanguageSelection.Japanese,
		);
		this.romajiName = NamesEditStore.nameOrEmpty(
			names,
			ContentLanguageSelection.Romaji,
		);

		this.aliases = _.filter(
			names,
			(n) =>
				n.id !== this.englishName.id &&
				n.id !== this.originalName.id &&
				n.id !== this.romajiName.id,
		);
	}

	private static nameOrEmpty(
		names: LocalizedStringWithIdEditStore[],
		lang: ContentLanguageSelection,
	): LocalizedStringWithIdEditStore {
		const name = _.find(names, (n) => n.language === lang);
		return name || new LocalizedStringWithIdEditStore(lang, '');
	}

	@action public createAlias = (): void => {
		this.aliases.push(new LocalizedStringWithIdEditStore());
	};

	@action public deleteAlias = (
		alias: LocalizedStringWithIdEditStore,
	): void => {
		_.pull(this.aliases, alias);
	};

	private getAllPrimaryNames: () => LocalizedStringWithIdEditStore[] = () => {
		return [this.originalName, this.romajiName, this.englishName];
	};

	public getAllNames = (): LocalizedStringWithIdEditStore[] => {
		return _.filter(
			this.getAllPrimaryNames().concat(this.aliases),
			(name) => !!name && !!name.value,
		);
	};

	public getPrimaryNames = (): LocalizedStringWithIdEditStore[] =>
		_.filter(this.getAllPrimaryNames(), (n) => !!n && !!n.value);

	// Whether the primary name is specified (in any language). This excludes aliases.
	public hasPrimaryName = (): boolean => {
		return _.some(this.getPrimaryNames(), (name) => name && name.value);
	};

	public toContracts = (): LocalizedStringWithIdContract[] => {
		return this.getAllNames().map((name) => {
			const contract: LocalizedStringWithIdContract = {
				id: name.id,
				language: name.languageStr,
				value: name.value!,
			};

			return contract;
		});
	};

	public static fromContracts(
		contracts: LocalizedStringWithIdContract[],
	): NamesEditStore {
		return new NamesEditStore(
			contracts.map((contract) =>
				LocalizedStringWithIdEditStore.fromContract(contract),
			),
		);
	}
}
