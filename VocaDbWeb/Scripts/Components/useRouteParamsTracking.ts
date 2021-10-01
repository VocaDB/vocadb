import IStoreWithRouteParams from '@Stores/IStoreWithRouteParams';
import { reaction } from 'mobx';
import React from 'react';
import ReactGA from 'react-ga';

// Captures pageviews as the query string changes.
// The call to useRouteParamsTracking must go after useStoreWithRouteParams, useStoreWithUpdateResults or useStoreWithPaging.
// Set ready to false while translations are not yet loaded.
// This prevents Google Analytics from using an incomplete page title (e.g. `Index.Discussions - Vocaloid Database`).
const useRouteParamsTracking = <T>(
	store: IStoreWithRouteParams<T>,
	ready: boolean,
): void => {
	React.useEffect(() => {
		// Returns the disposer.
		return reaction(
			() => store.routeParams,
			() => {
				if (ready && !store.popState) {
					ReactGA.pageview(
						`${window.location.pathname}${window.location.search}`,
					);
				}
			},
		);
	}, [store, ready]);
};

export default useRouteParamsTracking;
