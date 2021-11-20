import IStoreWithRouteParams from '@Stores/IStoreWithRouteParams';
import { reaction } from 'mobx';
import qs from 'qs';
import React from 'react';
import { useLocation } from 'react-router-dom';

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

	React.useEffect(() => {
		// Returns the disposer.
		return reaction(
			() => store.routeParams,
			(routeParams) => {
				if (!store.popState) {
					// Use `window.history.pushState` instead of `useNavigate` so that we can push changes to URL without re-rendering.
					// Code from: https://github.com/vercel/next.js/discussions/18072#discussioncomment-109059
					const newUrl = `?${qs.stringify(routeParams)}`;
					window.history.pushState(
						{ ...window.history.state, as: newUrl, url: newUrl },
						'',
						newUrl,
					);
				}
			},
		);
	}, [store]);
};

export default useStoreWithRouteParams;
