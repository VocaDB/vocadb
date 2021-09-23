export default interface IStoreWithRouteParams<T> {
	// Whether currently processing popstate. This is to prevent adding the previous state to history.
	popState: boolean;
	routeParams: T;

	validateRouteParams: (data: any) => data is T;
}
