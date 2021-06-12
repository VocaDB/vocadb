import ko, { ObservableArray } from 'knockout';
import _ from 'lodash';

// Basic list view model implementation where items are constructed from data contracts.
// Item type can be constructed from a contract, or with default parameters (for new items).
export default class BasicListEditViewModel<TItem, TContract> {
	// type: item constructor, optionally receiving a data contract parameter.
	// contracts: list of data contracts for current items.
	public constructor(
		private type: { new (contract?: TContract): TItem },
		contracts: TContract[],
	) {
		this.items = ko.observableArray(
			_.map(contracts, (contract) => new type(contract)),
		);
	}

	// add new item by instansiating the item type with default parameters
	public add = (): void => {
		this.items.push(new this.type());
	};

	public items: ObservableArray<TItem>;

	public remove = (item: TItem): void => {
		this.items.remove(item);
	};

	public toContracts: () => TContract[] = () => {
		return (ko.toJS(this.items) as unknown) as TContract[];
	};
}
