import IStoreWithUpdateResults from '@Stores/IStoreWithUpdateResults';
import _ from 'lodash';
import { reaction } from 'mobx';
import React from 'react';

const useStoreWithUpdateResults = <T extends Object>(
	store: IStoreWithUpdateResults<T>,
): void => {
	React.useEffect(() => {
		store.updateResults(true);

		// Returns the disposer.
		return reaction(
			() => store.routeParams,
			(routeParams, previousRouteParams) => {
				const clearResults = _.chain(routeParams)
					.omitBy((v, k) =>
						_.isEqual(
							previousRouteParams[k as keyof typeof previousRouteParams],
							v,
						),
					)
					.keys()
					.some((k) => store.clearResultsByQueryKeys.includes(k))
					.value();

				store.updateResults(clearResults);
			},
		);
	}, [store]);
};

export default useStoreWithUpdateResults;
