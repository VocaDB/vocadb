import { ValidateFunction } from 'ajv';
import qs from 'qs';
import React from 'react';
import { useLocation, useNavigate } from 'react-router-dom';

export interface IStoreWithRouteParams<T> {
	routeParams: T;

	shouldClearResults: (value: T) => boolean;
	updateResults: (clearResults: boolean) => void;
}

export const useSwitchboard = <T>(
	validate: ValidateFunction<T>,
	store: IStoreWithRouteParams<T>,
	onValidate?: (routeParams: T) => void,
): void => {
	const { search } = useLocation();

	React.useEffect(() => {
		const routeParams: any = qs.parse(search.slice(1));
		if (validate(routeParams)) {
			onValidate?.(routeParams);

			const oldRouteParams = store.routeParams;
			store.routeParams = routeParams;
			store.updateResults(store.shouldClearResults(oldRouteParams));
		}
	}, [search, validate, onValidate, store]);
};

const createPath = <T>(pathname: string, from: T, to: T): string =>
	`${pathname}?${qs.stringify({ ...from, ...to })}`;

export const useRedial = <T>(from: T): ((to: T) => void) => {
	const navigate = useNavigate();
	const { pathname } = useLocation();

	return React.useCallback(
		(to: T) => navigate(createPath(pathname, from, to)),
		[navigate, pathname, from],
	);
};
