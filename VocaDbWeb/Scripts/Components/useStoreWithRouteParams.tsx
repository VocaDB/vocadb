import IStoreWithRouteParams from '@Stores/IStoreWithRouteParams';
import { reaction } from 'mobx';
import qs from 'qs';
import React from 'react';
import { useLocation, useNavigate } from 'react-router-dom';

// Updates a store that implements the `IStoreWithRouteParams` interface when a route changes, and vice versa.
const useStoreWithRouteParams = <T,>(store: IStoreWithRouteParams<T>): void => {
	const location = useLocation();
	const navigate = useNavigate();

	// Pass `location` as deps instead of `location.search`.
	React.useEffect(() => {
		const routeParams: any = qs.parse(location.search.slice(1));

		if (store.validateRouteParams(routeParams)) {
			store.popState = true;

			store.routeParams = routeParams;

			store.popState = false;
		}
	}, [location, store]);

	React.useEffect(() => {
		// Returns the disposer.
		return reaction(
			() => store.routeParams,
			(routeParams) => {
				if (!store.popState) {
					const newUrl = `?${qs.stringify(routeParams)}`;
					navigate(newUrl);
				}
			},
		);
	}, [store, navigate]);
};

export default useStoreWithRouteParams;
