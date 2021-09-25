import IStoreWithRouteParams from '@Stores/IStoreWithRouteParams';
import { reaction } from 'mobx';
import React from 'react';
import ReactGA from 'react-ga';

const useRouteParamsTracking = <T>(store: IStoreWithRouteParams<T>): void => {
	React.useEffect(() => {
		// Returns the disposer.
		return reaction(
			() => store.routeParams,
			() => {
				if (!store.popState) {
					ReactGA.pageview(
						`${window.location.pathname}${window.location.search}`,
					);
				}
			},
		);
	}, [store]);
};

export default useRouteParamsTracking;
