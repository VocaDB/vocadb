import IStoreWithPaging from '@Stores/IStoreWithPaging';
import React from 'react';

import useStoreWithRouteParams from './useStoreWithRouteParams';
import useStoreWithUpdateResults from './useStoreWithUpdateResults';

const useStoreWithPaging = <T>(store: IStoreWithPaging<T>): void => {
	const handleClearResults = React.useCallback(
		(popState) => {
			// Do not go to the first page when the back/forward buttons are clicked.
			if (!popState) store.paging.goToFirstPage();
		},
		[store],
	);

	// `useStoreWithUpdateResults` must be called before `useStoreWithRouteParams` because the former may change `routeParams` based on `clearResultsByQueryKeys`.
	useStoreWithUpdateResults(store, handleClearResults);

	useStoreWithRouteParams(store);

	React.useEffect(() => {
		// This is called when the page is first loaded.
		store.updateResults(true);
	}, [store]);
};

export default useStoreWithPaging;
