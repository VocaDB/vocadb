import IStoreWithRouteParams from '@Stores/IStoreWithRouteParams';
import { reaction } from 'mobx';
import qs from 'qs';
import React from 'react';
import { useLocation, useNavigate } from 'react-router';

// Updates a store that implements the `IStoreWithRouteParams` interface when a route changes, and vice versa.
const useStoreWithRouteParams = <T>(store: IStoreWithRouteParams<T>): void => {
	const location = useLocation();

	// Pass `location` as deps instead of `location.search`.
	React.useEffect(() => {
		const routeParams: any = qs.parse(location.search.slice(1));

		if (store.validateRouteParams(routeParams)) {
			store.popState = true;

			store.routeParams = routeParams;

			store.popState = false;
		}
	}, [location, store]);

	const navigate = useNavigate();

	React.useEffect(() => {
		// Returns the disposer.
		return reaction(
			() => store.routeParams,
			(routeParams) => {
				if (!store.popState) {
					// TODO: is there any way to push changes to url without re-rendering?
					navigate(`${location.pathname}?${qs.stringify(routeParams)}`);
				}
			},
		);
	}, [location.pathname, store, navigate]);
};

export default useStoreWithRouteParams;
