import IStoreWithRouteParams from '@Stores/IStoreWithRouteParams';
import { ValidateFunction } from 'ajv';
import { reaction } from 'mobx';
import qs from 'qs';
import React from 'react';
import { useLocation, useNavigate } from 'react-router';

const useStoreWithRouteParams = <T>(
	validate: ValidateFunction<T>,
	store: IStoreWithRouteParams<T>,
): { popState: React.MutableRefObject<boolean> } => {
	// Whether currently processing popstate. This is to prevent adding the previous state to history.
	const popState = React.useRef(false);

	const location = useLocation();

	// Pass `location` as deps instead of `location.search`.
	React.useEffect(() => {
		const routeParams: any = qs.parse(location.search.slice(1));

		if (validate(routeParams)) {
			popState.current = true;

			store.routeParams = routeParams;

			popState.current = false;
		}
	}, [location, validate, store]);

	const navigate = useNavigate();

	React.useEffect(() => {
		// Returns the disposer.
		return reaction(
			() => store.routeParams,
			(routeParams) => {
				if (!popState.current) {
					// TODO: is there any way to push changes to url without re-rendering?
					navigate(`${location.pathname}?${qs.stringify(routeParams)}`);
				}
			},
		);
	}, [location.pathname, store, navigate]);

	return { popState: popState };
};

export default useStoreWithRouteParams;
