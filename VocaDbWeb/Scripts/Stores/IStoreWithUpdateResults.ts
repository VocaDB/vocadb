import IStoreWithRouteParams from './IStoreWithRouteParams';

export default interface IStoreWithUpdateResults<T>
	extends IStoreWithRouteParams<T> {
	clearResultsByQueryKeys: string[];

	updateResults: (clearResults: boolean) => void;
}
