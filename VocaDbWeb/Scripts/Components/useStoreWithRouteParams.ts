import IStoreWithRouteParams from '@Stores/IStoreWithRouteParams';
import { ValidateFunction } from 'ajv';
import qs from 'qs';
import React from 'react';
import { useLocation } from 'react-router';

const useStoreWithRouteParams = <T>(
	validate: ValidateFunction<T>,
	store: IStoreWithRouteParams<T>,
): void => {
	const location = useLocation();

	React.useEffect(() => {
		const routeParams: any = qs.parse(location.search.slice(1));

		if (validate(routeParams)) {
			store.routeParams = routeParams;
		}
	}, [location, validate, store]);
};

export default useStoreWithRouteParams;
